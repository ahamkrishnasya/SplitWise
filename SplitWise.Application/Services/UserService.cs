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
    public class UserService: IUserService
    {
        private readonly IUserRepository _UserRepository; 

        public UserService(IUserRepository UserRepository)
        {
            _UserRepository = UserRepository;
        }   

        
    }
}
