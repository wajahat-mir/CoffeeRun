using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoffeeRun.Models;
using CoffeeRun.Repositories;
using CoffeeRun.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeRun.Controllers
{
    public class RequestsController : Controller
    {
        private readonly IRequestRepository _requestRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public RequestsController(IRequestRepository requestRepository, 
            UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _requestRepository = requestRepository;
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            string userId = _userManager.GetUserId(User);
            var friends = await _requestRepository.GetAllFriendRequests(userId);

            var friendViewModel = _mapper.Map<IEnumerable<FriendViewModel>>(friends);

            return View(friendViewModel);
        }

        public async Task<IActionResult> Confirm(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            await _requestRepository.SetToConfirmedAndAddFriend(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Revoke(int id)
        {
            await _requestRepository.RemoveRequest(id);
            return RedirectToAction(nameof(Index));
        }
    }
}