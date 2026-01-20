using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWise.Application.Interfaces.Repositories
{
    public interface IGroupService
    {
        Task<string> GetGroupById(int id);
    }
}
