using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SplitWise.Application.DTOs.Groups;
using SplitWise.Application.Interfaces.Services;
using SplitWise.Infrastructure.Identity;
using SplitWise.MVC.Models;

namespace SplitWise.MVC.Controllers
{
    [Authorize]
    public class GroupController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly UserManager<ApplicationUser> _userManager;

        public GroupController(IGroupService  groupService, UserManager<ApplicationUser> userManager)
        {
            _groupService = groupService;   
            _userManager = userManager; 
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Create(GroupViewModel model)
        {
            if (model != null)
            {
               var data = new CreateGroupDto
               {                    
                   GroupName = model.GroupName,
                   Description = model.Description,
                   CreatedByUserId = _userManager.GetUserId(User)
               };
               await _groupService.CreateGroupAsync(data);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MyGroups()
        {
            var userId = _userManager.GetUserId(User);
            var groups = await _groupService.GetGroupsByUserIdAsync(userId);
            return View(groups);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var groups =  _groupService.EditGroup(_userManager.GetUserId(User));
            var data = new GroupViewModel();

            return View();
        }

    }

}
