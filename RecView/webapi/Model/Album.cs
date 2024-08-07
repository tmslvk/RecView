﻿namespace webapi.Model
{
    public class Album
    {
        public string Id { get; set; }
        public Artist Artist { get; set; }
        public string ArtistId { get; set; }
        public string Title { get; set; }
        public string AlbumType { get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<Song> Songs { get; set; }
        public List<Genre> Genres { get; set; }
        public List<UserOverview>? UserOverviews { get; set; }
    }
}
