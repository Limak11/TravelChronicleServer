using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chronicle.Web.Models
{
    public class Photo
    {
        public long Id { get; set; }
        public byte[] ImageData { get; set; }

        public Post Post { get; set; }
        public long PostId { get; set; }
    }
}
