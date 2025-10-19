using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoviesSearch.Core.Entities
{
    public class Movie
    {
        public Guid Id { get; set; }
        [JsonPropertyName("imdbID")]
        public string ImdbID { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public string Poster { get; set; }
    }
}
