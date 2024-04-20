using Microsoft.AspNetCore.Components.Routing;
using System.ComponentModel.DataAnnotations.Schema;

namespace Report.Models
{
    public class Reports
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime DateTime { get; set; }
        public string location { get; set; }
        public byte[] image { get; set; }
    }
}
