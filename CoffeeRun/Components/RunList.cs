using AutoMapper;
using CoffeeRun.Models;
using CoffeeRun.Repositories;
using CoffeeRun.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeRun.Components
{
    public class RunList : ViewComponent
    {
        private readonly IRunRepository _runRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public RunList(IRunRepository runRepository, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _runRepository = runRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool historical = false)
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            IEnumerable<Run> runs;

            if (!historical)
                runs = await _runRepository.AllActiveRunsForUserAndTheirFriendsAsync(userId);
            else
                runs = await _runRepository.FindAllRunsForUserAsync(userId);

            var runsViewModels = _mapper.Map<IEnumerable<RunViewModel>>(runs);
            foreach (var runViewModel in runsViewModels)
            {
                if (runViewModel.OwnerUserId == userId && runViewModel.TimeToRun > DateTime.Now)
                    runViewModel.ableToModify = true;
                else
                    runViewModel.ableToModify = false;
            }

            return View(runsViewModels);
        }
    }
}
