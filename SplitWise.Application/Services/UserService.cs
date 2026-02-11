using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplitWise.Application.Interfaces.Repositories;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Domain.Entities;

namespace SplitWise.Application.Services
{
    public class SessionService: ISessionService
    {
        private readonly ISessionRepository _SessionRepository; 

        public SessionService(ISessionRepository SessionRepository)
        {
            _SessionRepository = SessionRepository;
        }   

        
    }
}
