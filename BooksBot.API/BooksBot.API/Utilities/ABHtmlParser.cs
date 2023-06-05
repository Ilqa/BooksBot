using BooksBot.API.Constants;
using BooksBot.API.Extensions;
using BooksBot.API.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BooksBot.API.Utilities
{
    public  class ABHtmlParser : IHtmlParser ,IDisposable
    {
        public string EanSearchURL => Constants.StringConstants.ABSearchURL;

        private ParseResponseModel ParseBookDetailPageHtml(HtmlNode bookDiv, string url)
        {
            WobBookModel wobBookModel = new WobBookModel();
            ParseResponseModel ReponseModel = new ParseResponseModel();
            var titleElement = bookDiv.Descendants().FirstOrDefault(n => n.Attributes.Any(a => a.Value.Contains("product-name")));
            wobBookModel.Title = titleElement != null ? titleElement.InnerText.RemoveNewLinesAndWhiteSpaces() : string.Empty;
            var priceElement = bookDiv.SelectNodes("//*[@data-price]")?.FirstOrDefault(); // Sometimes price isnr mentioned in case of out of stock
            wobBookModel.Price = priceElement != null ? Convert.ToDouble(priceElement.InnerHtml.GetDecimalOrIntPartFromString()) : 0;
            wobBookModel.ProductUrl = url;
            wobBookModel.EAN = GetEANNumber(url); 
            var quanitityTag = bookDiv.SelectNodes("//span[@id='availability']")?.FirstOrDefault();
            var quantity = quanitityTag == null ? null : Regex.Match(quanitityTag.InnerText, @"\d+").Value;

            if (!string.IsNullOrEmpty(quantity) && int.TryParse(quantity, out _))
                wobBookModel.Quantity = Convert.ToInt32(quantity);

            ReponseModel.IsUsed = false;
            ReponseModel.UsedURL = "";
            ReponseModel.bookData.Add(wobBookModel);
            return ReponseModel;
        }
        public  string ParseProductUrl(string html)
        {
            string productUrl = string.Empty;
           // var pageNotFound = !string.IsNullOrEmpty(html) ? AppContants.NotFoundMessagesArray.Any(item => responseHtml.Contains(item)) : false;

            
            if (!string.IsNullOrEmpty(html) && !AppContants.NotFoundMessagesArray.Any(item => html.Contains(item)))
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                try
                {
                    var BookDetail = htmlDoc.DocumentNode.SelectNodes("//h5[@class='mb-1 product-item__title book_title']").ElementAt(0);
                    if (BookDetail != null)
                    {
                        
                        var url = BookDetail.ChildNodes["a"].Attributes["href"];
                        if (url != null)
                        {
                            productUrl = url.Value;
                        }
                    }
                }
                catch (Exception ex) { }
            }
            return productUrl;
        }
        private string GetEANNumber(string Url) 
        {
            return Url.Split('/')[4].ToString(); 
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public ParseResponseModel IdentifyExactParser(HtmlResponseModel htmlResponse)
        {
            ParseResponseModel ReponseModel = new ParseResponseModel();
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlResponse.Html);
            
            var bookDiv = htmlDoc.DocumentNode.SelectNodes("//div[@class='container']").ElementAt(3);
            if (bookDiv != null)
            {
                var UsedTag = bookDiv.SelectNodes("//div[@class='condition-options-item differentCondition col']")?.LastOrDefault();
                if (UsedTag != null)
                {
                    var UsedBookURL = UsedTag.ChildNodes["a"].Attributes["href"].Value;
                    //var Ischecked = UsedTag.ChildNodes["a"].ChildNodes["input"].Attributes["checked"];
                    if (!(UsedBookURL.Equals(htmlResponse.CrawlSource.Url)) && string.IsNullOrEmpty(htmlResponse.CrawlSource.UsedUrl))
                    {
                        ReponseModel.IsUsed = true;
                        ReponseModel.UsedURL = UsedBookURL;
                        ReponseModel.bookData = null;
                        return ReponseModel;
                    }
                    
                }
                ReponseModel = ParseBookDetailPageHtml(bookDiv, htmlResponse.CrawlSource.Url);
            }
            return ReponseModel;
        }
    }
}
