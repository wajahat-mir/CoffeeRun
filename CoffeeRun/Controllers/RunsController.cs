using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoffeeRun.Data;
using CoffeeRun.Models;
using CoffeeRun.Repositories;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using CoffeeRun.ViewModels;
using Microsoft.AspNetCore.Http;

namespace CoffeeRun.Controllers
{
    public class RunsController : Controller
    {
        private readonly IRunRepository _runRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public RunsController(IRunRepository runRepository, UserManager<ApplicationUser> userManager, 
            IMapper mapper)
        {
            _runRepository = runRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            RunIndexViewModel runIndexViewModel = new RunIndexViewModel();
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("historical")))
            {
                HttpContext.Session.SetString("historical", "false");
            }

            runIndexViewModel.historicalFlag = Convert.ToBoolean(HttpContext.Session.GetString("historical"));

            if (runIndexViewModel.historicalFlag)
            {
                runIndexViewModel.historicalLabel = "View Active Bills";
            }
            else
            {
                runIndexViewModel.historicalLabel = "View Historical Bills";
            }
            return View(runIndexViewModel);
        }

        public IActionResult UpdateHistorical()
        {
            var historical = Convert.ToBoolean(HttpContext.Session.GetString("historical"));
            historical = (historical == false) ? true : false;
            HttpContext.Session.SetString("historical", historical.ToString());
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,TimeToRun")] RunViewModel runViewModel)
        {
            if (ModelState.IsValid)
            {
                var run = _mapper.Map<Run>(runViewModel);
                run.OwnerUserId = _userManager.GetUserId(User);
                run.runStatus = RunStatus.Prepped;
                await _runRepository.CreateRunAsync(run);

                return RedirectToAction(nameof(Index));
            }
            return View(runViewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var run = await _runRepository.FindRunAsync(Convert.ToInt32(id));
            if (run == null)
            {
                return NotFound();
            }
            var runViewModel = _mapper.Map<RunViewModel>(run);
            return View(runViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RunId,Name,TimeToRun")] RunViewModel runViewModel)
        {
            if (id != runViewModel.RunId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var run = await _runRepository.FindRunAsync(runViewModel.RunId);
                    run.Name = runViewModel.Name;
                    run.TimeToRun = runViewModel.TimeToRun;
                    await _runRepository.EditRunAsync(run);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(runViewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var run = await _runRepository.FindRunAsync(Convert.ToInt32(id));
            if (run == null)
            {
                return NotFound();
            }

            var runViewModel = _mapper.Map<RunViewModel>(run);
            return View(runViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _runRepository.RemoveRunAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MakeMeRunner(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            await _runRepository.SetRunner(id, user.UserName);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MarkComplete(int id)
        {
            await _runRepository.SetRunStatus(id, RunStatus.Complete);
            return RedirectToAction(nameof(Index));
        }
    }
}
