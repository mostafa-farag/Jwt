namespace Report.Models
{
    public class Reportdto
    {
        public string name { get; set; }
        public string location { get; set; }
        public IFormFile? image { get; set; }
    }
}
