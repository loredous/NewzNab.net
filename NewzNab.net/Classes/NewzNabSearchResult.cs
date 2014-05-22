namespace NewzNab.net
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Text.RegularExpressions;

    public class NewzNabSearchResult
    {
        public string Title { get; set; }
        public string Guid { get; set; }
        public string Link { get; set; }
        public string CommentLink { get; set; }
        public DateTime PublishDate { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string NZBUrl { get; set; }
        public List<KeyValuePair<string, string>> Attributes = new List<KeyValuePair<string, string>>();

        public static NewzNabSearchResult ParseItemBlock(XmlNode Item)
        {
            NewzNabSearchResult Result = new NewzNabSearchResult();
            Result.Title = Item["title"].InnerText;
            Result.Guid = Item["guid"].InnerText;
            Result.Link = Item["link"].InnerText;
            Result.CommentLink = Item["comments"].InnerText;
            Result.PublishDate = DateTime.Parse(Item["pubDate"].InnerText);
            Result.Category = Item["category"].InnerText;
            Result.Description = Regex.Replace(Item["description"].InnerText.Replace(@"<br />",Environment.NewLine), "<.*?>", string.Empty).Trim();
            Result.NZBUrl = Item["enclosure"].GetAttribute("url");
            foreach (XmlNode Attr in Item.ChildNodes)
            {
                if (Attr.Name == "newznab:attr")
                {
                    Result.Attributes.Add(new KeyValuePair<string,string>(Attr.Attributes["name"].Value, Attr.Attributes["value"].Value));
                }
            }
            return Result;
        }
    }
}
