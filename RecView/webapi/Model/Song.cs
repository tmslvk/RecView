﻿namespace webapi.Model
{
    public class Song
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Duration { get; set; }
        public Album Album { get; set; }
        public string AlbumId { get; set; }
    }
}
