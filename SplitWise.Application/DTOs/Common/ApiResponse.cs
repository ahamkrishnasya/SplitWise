using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Application.DTOs.Common
{
    public class ApiResponse<T>
    {
        public int statusCode { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public T data { get; set; }
        public List<ApiError> errors { get; set; }
        public MetaData meta { get; set; }
    }
}
