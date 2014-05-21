namespace NewzNab.net
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NewzNabQuery
    {
        public Functions RequestedFunction;
        public SearchTypes SearchType;
        public string Query;
        public List<int> Groups = new List<int>();
        public List<int> Categories = new List<int>();
        public int Offset;
    }

    public enum Functions
    {
        CAPS,
        REGISTER,
        SEARCH,
        TV_SEARCH,
        MOVIE_SEARCH,
        MUSIC_SEARCH,
        BOOK_SEARCH,
        DETAILS,
        GETNFO,
        GET,
        CART_ADD,
        CART_DEL,
        COMMENTS,
        COMMENTS_ADD,
        USER
    }

    public enum SearchTypes
    {
        General,
        Music,
        TV,
        Movie,
        Audio
    }
}
