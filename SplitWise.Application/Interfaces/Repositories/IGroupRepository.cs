using SplitWise.Application.DTOs.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SplitWise.Domain.Entities;

namespace SplitWise.Application.Interfaces.Repositories
{
    public interface IGroupRepository
    {
        Task AddAsync(Groups group);
        Task<IEnumerable<Groups>> GetGroupsByUserIdAsync(int userId);    
    }
}
