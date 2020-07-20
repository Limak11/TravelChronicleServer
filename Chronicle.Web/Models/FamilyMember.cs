using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chronicle.Web.Models
{
    public class FamilyMember
    {
        public long Id { get; set; }
        public long FamilyId { get; set; }
        public long UserId { get; set; }
    }
}
