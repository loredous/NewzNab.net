namespace NewzNab.net
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using System.Web;

    public class NewzNabCapabilities
    {
        //Public Properties
        public int MaxResults;
        public int DefaultResults;
        public int Retention;

        public bool SearchAvail;
        public bool TVSearchAvail;
        public bool MovieSearchAvail;
        public bool AudioSearchAvail;

        public Dictionary<int, string> Categories = new Dictionary<int, string>();
        public List<UsenetGroup> Groups = new List<UsenetGroup>();
        public List<NewzNabGenre> Genres = new List<NewzNabGenre>();
        
        public static NewzNabCapabilities ParseXMLResponse(string XMLResponse)
        {
            XmlDocument Capabilities = new XmlDocument();
            Capabilities.LoadXml(XMLResponse);
            return ParseXMLResponse(Capabilities);            
        }

        public static NewzNabCapabilities ParseXMLResponse(XmlDocument XMLResponse)
        {
            NewzNabCapabilities Result = new NewzNabCapabilities();

            //Basic Capabilities
            Result.MaxResults = Convert.ToInt32(XMLResponse.SelectSingleNode("/caps/limits").Attributes["max"].Value);
            Result.DefaultResults = Convert.ToInt32(XMLResponse.SelectSingleNode("/caps/limits").Attributes["default"].Value);
            Result.Retention = Convert.ToInt32(XMLResponse.SelectSingleNode("/caps/retention").Attributes["days"].Value);

            //Search Capabilities
            Result.SearchAvail = YesNoToBool(XMLResponse.SelectSingleNode("/caps/searching/search").Attributes["available"].Value);
            Result.TVSearchAvail = YesNoToBool(XMLResponse.SelectSingleNode("/caps/searching/tv-search").Attributes["available"].Value);
            Result.MovieSearchAvail = YesNoToBool(XMLResponse.SelectSingleNode("/caps/searching/movie-search").Attributes["available"].Value);
            Result.AudioSearchAvail = YesNoToBool(XMLResponse.SelectSingleNode("/caps/searching/audio-search").Attributes["available"].Value);

            //Categories
            foreach (XmlNode Cat in XMLResponse.SelectNodes("caps/categories/category"))
            {
                Result.Categories.Add(Convert.ToInt32(Cat.Attributes["id"].Value), HttpUtility.HtmlDecode(Cat.Attributes["name"].Value));
                foreach (XmlNode SubCat in Cat.ChildNodes)
                {
                    Result.Categories.Add(Convert.ToInt32(SubCat.Attributes["id"].Value), HttpUtility.HtmlDecode(Cat.Attributes["name"].Value + "\\" + SubCat.Attributes["name"].Value));
                }
            }

            //Groups
            UsenetGroup CurrentGroup;
            foreach (XmlNode Group in XMLResponse.SelectNodes("caps/groups/group"))
            {
                CurrentGroup = new UsenetGroup();
                CurrentGroup.ID = Convert.ToInt32(Group.Attributes["id"].Value);
                CurrentGroup.Name = HttpUtility.HtmlDecode(Group.Attributes["name"].Value);
                CurrentGroup.Description = Group.Attributes["description"].Value;
                DateTime.TryParse(Group.Attributes["lastupdate"].Value, out CurrentGroup.LastUpdate);
                Result.Groups.Add(CurrentGroup);
            }

            //Genres
            NewzNabGenre CurrentGenre;
            foreach (XmlNode Genre in XMLResponse.SelectNodes("caps/genres/genres"))
            {
                CurrentGenre = new NewzNabGenre();
                CurrentGenre.ID = Convert.ToInt32(Genre.Attributes["id"].Value);
                CurrentGenre.CategoryID = Convert.ToInt32(Genre.Attributes["categoryid"].Value);
                CurrentGenre.Name = HttpUtility.HtmlDecode(Result.Categories[CurrentGenre.CategoryID] + "\\" + Genre.Attributes["name"].Value);
                Result.Genres.Add(CurrentGenre);
            }

            return Result;
        }

        private static bool YesNoToBool(string YesNo)
        {
            if (YesNo.ToLower() == "yes") { return true; }
            else if (YesNo.ToLower() == "no") { return false; }
            return false;
        }


    }

    public struct UsenetGroup
    {
        public int ID;
        public string Name;
        public string Description;
        public DateTime LastUpdate;

        public override string ToString()
        {
            return Name;
        }
    }

    public struct NewzNabGenre
    {
        public int ID;
        public int CategoryID;
        public string Name;

        public override string ToString()
        {
            return Name;
        }
    }
}
