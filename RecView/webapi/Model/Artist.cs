﻿namespace webapi.Model
{
    public class Artist
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Followers { get; set; }
        public List<Album> Albums { get; set; }
        public List<Genre> Genres { get; set; }
    }
}
