using System;
using static BooksBot.API.Constants.Enums;

namespace BooksBot.API.Utilities
{
    public class ParserFactory : IDisposable
    {
        public void Dispose()
        {
            //this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IHtmlParser GetParser(SourceWebSiteShortNameEnum Source)
        {

            return Source switch
            {
                SourceWebSiteShortNameEnum.WOB_US => new WOBHtmlParser(),
                SourceWebSiteShortNameEnum.WOB_GB => new WOBHtmlParser(),
                SourceWebSiteShortNameEnum.AB => new ABHtmlParser(),
                _ => throw new ApplicationException(string.Format("Parser '{0}' cannot be created", Source)),
            };
        }
    }
}
