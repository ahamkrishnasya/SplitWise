using Microsoft.AspNetCore.Mvc;
using SplitWise.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace Splitwise.API.Controllers
{
    public class UsersController : Controller
    {
       private readonly IUserService _userService;
        public UsersController(IUserService UserService)
        {
            _userService = UserService;
        }

    }
}
