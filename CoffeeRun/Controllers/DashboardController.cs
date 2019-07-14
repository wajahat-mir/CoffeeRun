using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoffeeRun.Data;
using CoffeeRun.ViewModels;
using CoffeeRun.Repositories;
using CoffeeRun.Models;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace CoffeeRun.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IRunRepository _runRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public DashboardController(IRunRepository runRepository, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _runRepository = runRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: Dashboard
        public async Task<IActionResult> Index()
        {
            string userId = _userManager.GetUserId(User);
            IEnumerable<Run> runs;

            runs = await _runRepository.AllActiveRunsForUserAndTheirFriendsAsync(userId);

            var dashboardViewModels = _mapper.Map<IEnumerable<DashboardViewModel>>(runs);

            return View(dashboardViewModels);
        }
    }
}
