using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chronicle.Web.Models
{
    public class Post
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string Name { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public DateTime? TimeDeleted { get; set; }
        public Photo Photo { get; set; }

        public long ChronicleId { get; set; }
        public Chronicle Chronicle { get; set; }
    }
}
