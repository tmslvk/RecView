﻿using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace webapi.Model
{
    public class SpotifyUser
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string SpotifyId { get; set; }
    }
}
