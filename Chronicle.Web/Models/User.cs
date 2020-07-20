using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chronicle.Web.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public String Description { get; set; }
        public List<Chronicle> OwnedChronicles { get; set; }

        public string Key { get; set; }
    }
}
