using System.Collections.Generic;

namespace MvvmFramework.Models
{
    public class ListviewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageIcon { get; set; }
        public string Category { get; set; }
        public int CurrentRating { get; set; }
        public List<string> StarRatings { get; set; }
        public string Postcode { get; set; }
        public double Distance { get; set; }
        public bool HasR { get; set; }
        public bool HasW { get; set; }
        public bool HasH { get; set; }
        public string Url { get; set; }
    }
}
