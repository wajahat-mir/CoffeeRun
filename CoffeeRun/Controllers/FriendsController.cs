using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoffeeRun.Data;
using CoffeeRun.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using CoffeeRun.Repositories;
using CoffeeRun.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace CoffeeRun.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFriendRepository _friendRepository;

        public FriendsController(IMapper mapper, 
            UserManager<ApplicationUser> userManager, IFriendRepository friendRepository)
        {
            _mapper = mapper;
            _userManager = userManager;
            _friendRepository = friendRepository;
        }

        public async Task<IActionResult> Index()
        {
            string userId = _userManager.GetUserId(User);
            var friends = await _friendRepository.GetAllFriendsAsync(userId);

            var friendsViewModel = _mapper.Map<IEnumerable<FriendViewModel>>(friends);
            return View(friendsViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyUserName(string FriendUniqueName)
        {
            var friend = await _friendRepository.FindUserAsync(FriendUniqueName);
            if (friend ==  null)
                return Json(false);
            return Json(true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FriendUniqueName")] FriendViewModel friendViewModel)
        {
            if (ModelState.IsValid)
            {
                string userId = _userManager.GetUserId(User);
                bool result = await _friendRepository
                    .AddFriendAsync(friendViewModel.FriendUniqueName, userId);
                if (!result)
                    return BadRequest();
                return RedirectToAction(nameof(Index));
            }
            return View(friendViewModel);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friend = await _friendRepository.GetFriendAsync(Convert.ToInt32(id));
            
            if (friend == null)
            {
                return NotFound();
            }

            var friendViewModel = _mapper.Map<FriendViewModel>(friend);
            return View(friendViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await _friendRepository.RemoveFriendAsync(id))
                return RedirectToAction(nameof(Index));
            else
                return BadRequest();
        }
    }
}
