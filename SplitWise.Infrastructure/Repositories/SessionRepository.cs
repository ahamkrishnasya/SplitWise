using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplitWise.Domain.Entities;
using SplitWise.Infrastructure.Data;
using SplitWise.Application.Interfaces.Repositories;

namespace SplitWise.Infrastructure.Services
{
    public class SessionRepository: ISessionRepository
    {
        private readonly ApplicationDbContext _context;
        public SessionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

    }
}
