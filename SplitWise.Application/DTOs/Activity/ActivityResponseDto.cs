using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Application.DTOs.Activities
{
    public class ActivityResponseDto
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public DateTime ActivityDate { get; set; }
    }
}
