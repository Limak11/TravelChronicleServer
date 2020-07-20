using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chronicle.Web.Models
{
    public class HttpPhoto
    {
        public long Id { get; set; }
        public string ImageString { get; set; }
        public long PostId { get; set; }
    }
}
