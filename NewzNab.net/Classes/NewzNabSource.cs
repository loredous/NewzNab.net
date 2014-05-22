namespace NewzNab.net
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Net;
    using System.IO;
    using System.Xml;
    using System.Linq;
    using System.Web;

    public class NewzNabSource
    {
        //Configuration Variables
        public string APIKey { get; private set; }
        public Uri APIURL { get; private set; }
        public bool UseSSL { get; private set; }
        public EncodingType Encoding { get; private set; }

        //Storage Variables
        public NewzNabCapabilities Capabilities { get; private set; }

        //Enums
        public enum EncodingType
        {
            JSON,
            XML
        }

        //Constructors
        #region Constructors
        public NewzNabSource(string URL)
        {
            APIKey = "";
            Encoding = EncodingType.XML;
            UseSSL = false;
            APIURL = URLtoNewzNabURI(URL, UseSSL);
        }

        public NewzNabSource(string URL, bool SSL)
        {
            APIKey = "";
            Encoding = EncodingType.XML;
            UseSSL = SSL;
            APIURL = URLtoNewzNabURI(URL, UseSSL);
        }

        public NewzNabSource(string URL, bool SSL, string API_Key)
        {
            APIKey = API_Key;
            Encoding = EncodingType.XML;
            UseSSL = SSL;
            APIURL = URLtoNewzNabURI(URL, UseSSL);
        }

        public NewzNabSource(string URL, bool SSL, string API_Key, EncodingType UseEncodingType)
        {
            APIKey = API_Key;
            Encoding = UseEncodingType;
            UseSSL = SSL;
            APIURL = URLtoNewzNabURI(URL, UseSSL);
        }

        
        #endregion Constructors

        public bool GetCapabilities()
        {
            NewzNabQuery CapsQuery = new NewzNabQuery();
            CapsQuery.RequestedFunction = Functions.CAPS;
            XmlDocument Response = DoQuery(CapsQuery);
            Capabilities = NewzNabCapabilities.ParseXMLResponse(Response);
            return true;
        }

        public static Uri URLtoNewzNabURI(string URL, bool SSL)
        {
            UriBuilder builder = new UriBuilder(URL);
            if (SSL)
            {
                builder.Scheme = Uri.UriSchemeHttps;
                builder.Port = -1;
            }
            else
            {
                builder.Scheme = Uri.UriSchemeHttp;
                builder.Port = -1;
            }
            if (!builder.Path.EndsWith("api"))
            {
                if (!builder.Path.EndsWith("/"))
                {
                    builder.Path += "/";
                }
                builder.Path = string.Concat(builder.Path, "api");
            }
            builder.Query = "";
            return builder.Uri;
        }

        private XmlDocument DoQuery(NewzNabQuery Query)
        {
            UriBuilder QueryURI = new UriBuilder(APIURL);
            switch (Query.RequestedFunction)
            {
                case Functions.CAPS:
                    QueryURI.Query = "t=caps&o=xml";
                    break;
                case Functions.REGISTER:
                    break;
                case Functions.SEARCH:
                    QueryURI.Query = BuildGenericQueryString(Query);
                    break;
                case Functions.TV_SEARCH:
                    break;
                case Functions.MOVIE_SEARCH:
                    break;
                case Functions.MUSIC_SEARCH:
                    break;
                case Functions.BOOK_SEARCH:
                    break;
                case Functions.DETAILS:
                    break;
                case Functions.GETNFO:
                    break;
                case Functions.GET:
                    break;
                case Functions.CART_ADD:
                    break;
                case Functions.CART_DEL:
                    break;
                case Functions.COMMENTS:
                    break;
                case Functions.COMMENTS_ADD:
                    break;
                case Functions.USER:
                    break;
                default:
                    break;
            }
            XmlDocument XMLResponse = new XmlDocument();
            XMLResponse.Load(QueryURI.Uri.ToString());
            return XMLResponse;
        }

        private string BuildGenericQueryString(NewzNabQuery Query)
        {
            StringBuilder result = new StringBuilder();
            result.Append("t=search");
            if (APIKey != "") { result.Append("&apikey=" + APIKey); }
            if (Query.Query != "") { result.Append("&q=" + Query.Query); }
            if (Query.Offset != null) { result.Append("&offset=" + Query.Offset.ToString()); }
            if (Query.Groups.Count > 0)
            {
                result.Append("&group=");
                result.Append(string.Join(",", Query.Groups));
            }
            if (Query.Categories.Count > 0)
            {
                result.Append("&cat=");
                result.Append(string.Join(",", Query.Categories));
            }
            return result.ToString();
        }

        public List<NewzNabSearchResult> Search(NewzNabQuery Query)
        {
            List<NewzNabSearchResult> Results = new List<NewzNabSearchResult>();
            XmlDocument Result = DoQuery(Query);
            XmlNodeList Items = Result.SelectNodes("/rss/channel/item");
            foreach (XmlNode Item in Items)
            {
                Results.Add(NewzNabSearchResult.ParseItemBlock(Item));
            }
            return Results;
        }
    }

    
}
