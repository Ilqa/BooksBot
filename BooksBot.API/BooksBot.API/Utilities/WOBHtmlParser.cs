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
using System.Text.RegularExpressions;

namespace BooksBot.API.Utilities
{
    public  class WOBHtmlParser  : IHtmlParser ,IDisposable
    {
        public string EanSearchURL => Constants.StringConstants.WOBSearchURL;

        public ParseResponseModel ParseWobListPageHtml(string html)
        {
            ParseResponseModel ReponseModel = new ParseResponseModel();
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var booksList = htmlDoc.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "").Contains("gridItem")).ToList();

            List<WobBookModel> wobBooks = new List<WobBookModel>();

            foreach (var bookItem in booksList)
            {
                if (bookItem.FirstChild.Attributes.Count > 0)
                {
                    var bookData = new WobBookModel();
                    bookData.Title = bookItem.ChildNodes.Where(node => node.GetAttributeValue("class", "").Contains("itemTitle")).FirstOrDefault()?.FirstChild.InnerHtml;
                    var priceDiv = bookItem.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("itemPrice")).FirstOrDefault();
                    bookData.Price = !string.IsNullOrEmpty(priceDiv.InnerHtml) ? Convert.ToDouble(priceDiv.InnerHtml.GetDecimalOrIntPartFromString()) : 0;
                    bookData.ProductUrl = $"{StringConstants.WobWebsiteBaseUrl}{bookItem.FirstChild.Attributes[0].Value}";
                    bookData.EAN = Path.GetFileName(bookData.ProductUrl);
                    wobBooks.Add(bookData);
                }
            }
            ReponseModel.bookData = wobBooks;
            //ReponseModel.UsedURL = "";
            //ReponseModel.IsUsed = false;
            return ReponseModel;
            //return wobBooks;
        }
        public ParseResponseModel ParseWobBookDetailPageHtml(string html, string url)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            WobBookModel wobBookModel = new WobBookModel();
            htmlDoc.LoadHtml(html);

            var bookDiv = htmlDoc.DocumentNode.SelectNodes("//div[@class='product']").FirstOrDefault();
            //WobBookModel bookData = null;
            ParseResponseModel ReponseModel = new ParseResponseModel();

            if (bookDiv != null)
            {
                //bookData = new WobBookModel();
                var titleElement = bookDiv.Descendants().FirstOrDefault(n => n.Attributes.Any(a => a.Value.Contains("title")));
                wobBookModel.Title = titleElement != null ? titleElement.InnerText.RemoveNewLinesAndWhiteSpaces() : string.Empty;
                var priceElement = bookDiv.SelectNodes("//div[@class='price']")?.FirstOrDefault(); // Sometimes price isnr mentioned in case of out of stock
                wobBookModel.Price = priceElement != null ? Convert.ToDouble(priceElement.InnerHtml.GetDecimalOrIntPartFromString()) : 0;
                wobBookModel.ProductUrl = url;
                wobBookModel.EAN = Path.GetFileName(wobBookModel.ProductUrl);
               
                var quanitityTag = bookDiv.Descendants().FirstOrDefault(n => n.Attributes.Any(a => a.Value.Contains("stockStatus")));
                var quantity = quanitityTag == null ? null : Regex.Match(quanitityTag.InnerText, @"\d+").Value;
                
                if (!string.IsNullOrEmpty(quantity) && int.TryParse(quantity, out _))
                    wobBookModel.Quantity = Convert.ToInt32(quantity);
                
            }
            //ReponseModel.IsUsed = false;
            //ReponseModel.UsedURL = "";
            ReponseModel.bookData.Add(wobBookModel);
            return ReponseModel;
            //return bookData;
        }
        
        ////was added for testing the parsing
        //public  List<string> ParseHtml(string html)
        //{
        //    HtmlDocument htmlDoc = new HtmlDocument();
        //    htmlDoc.LoadHtml(html);
        //    var programmerLinks = htmlDoc.DocumentNode.Descendants("li")
        //            .Where(node => !node.GetAttributeValue("class", "").Contains("tocsection")).ToList();

        //    List<string> wikiLink = new List<string>();

        //    foreach (var link in programmerLinks)
        //    {
        //        if (link.FirstChild.Attributes.Count > 0)
        //            wikiLink.Add("https://en.wikipedia.org/" + link.FirstChild.Attributes[0].Value);
        //    }

        //    return wikiLink;
        //}
        public  string ParseProductUrl(string html)
        {
            string productUrl = string.Empty;
            var htmlDoc = new HtmlDocument();
            if (!string.IsNullOrEmpty(html))
            {
                htmlDoc.LoadHtml(html);
                try
                {
                    var scripts = htmlDoc.DocumentNode.Descendants()
                                     .Where(n => n.Name == "script");
                    var script = scripts.Skip(1).FirstOrDefault().InnerHtml;
                    var data = (JObject)JsonConvert.DeserializeObject(script);
                    productUrl = data["url"].Value<string>();
                }
                catch (Exception ex) { }
            }
            return productUrl;
        }

        public void Dispose()
        {
            //this.Dispose();
            GC.SuppressFinalize(this);
        }

        public ParseResponseModel IdentifyExactParser(HtmlResponseModel htmlResponse)
        {
            ParseResponseModel response;
            if (htmlResponse.CrawlSource.PageType == Enums.PageTypeEnum.Listing)
                response = ParseWobListPageHtml(htmlResponse.Html);
            else
                response = ParseWobBookDetailPageHtml(htmlResponse.Html, htmlResponse.CrawlSource.Url);
            return response;
        }
    }
}
