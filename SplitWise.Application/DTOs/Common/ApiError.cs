using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Application.DTOs.Common
{
    public class ApiError
    {
        public string type { get; set; }    
        public string message { get; set; }
        public string field { get; set; }
    }
}
