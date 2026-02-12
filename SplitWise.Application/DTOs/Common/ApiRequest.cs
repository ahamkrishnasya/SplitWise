using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Application.DTOs.Common
{
    public class ApiRequest<T>
    {
        public T Data { get; set; }    
        public MetaData Meta { get; set; }
    }
}
