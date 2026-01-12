using SplitWise.Domain.Comman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Domain.Entities
{
    public class Group: BaseEntity
    {
        public string GroupName { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
    }s
}
