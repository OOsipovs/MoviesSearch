using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoviesSearch.Core.Entities
{
    public class MovieDetails : Movie
    {
        public string Plot { get; set; }
        [JsonPropertyName("imdbRating")]
        public string ImdbRating { get; set; }
        public string Director { get; set; }
        public string Actors { get; set; }
        public string Response { get; set; }
        public string Error { get; set; }
    }
}
