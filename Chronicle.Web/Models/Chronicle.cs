using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chronicle.Web.Models
{
    public class Chronicle
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? TimeDeleted { get; set; }
        public List<Post> Posts { get; set; }
        public bool IsPrivate { get; set; }
        public User User { get; set; }
        public long UserId { get; set; }
    }
}
