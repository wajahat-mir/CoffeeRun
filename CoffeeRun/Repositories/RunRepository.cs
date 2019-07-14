using CoffeeRun.Data;
using CoffeeRun.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeRun.Repositories
{
    public interface IRunRepository
    {
        Task<IEnumerable<Run>> AllActiveRunsForUserAndTheirFriendsAsync(string UserId);
        Task<IEnumerable<Run>> FindAllActiveRunsForUserAsync(string UserId);
        Task SetRunner(int id, string userName);
        Task SetRunStatus(int id, RunStatus status);
        Task<IEnumerable<Run>> FindAllRunsForUserAsync(string userId);
        Task<Run> FindRunAsync(int id);
        Task CreateRunAsync(Run run);
        Task EditRunAsync(Run run);
        Task RemoveRunAsync(int id);
    }

    public class RunRepository : IRunRepository
    {
        private readonly ApplicationDbContext _db;

        public RunRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Run>> AllActiveRunsForUserAndTheirFriendsAsync(string UserId)
        {
            var friendsOfUser = await _db.Friends.Where(f => f.UserId == UserId && f.isConfirmed == true).ToListAsync();
            List<Run> ActiveRuns = new List<Run>();
            foreach(Friend friend in friendsOfUser)
            {
                var runs = await FindAllActiveRunsForUserAsync(friend.FriendUserId);
                ActiveRuns.AddRange(runs);
            }
            var myRuns = await FindAllActiveRunsForUserAsync(UserId);
            ActiveRuns.AddRange(myRuns);

            return ActiveRuns;
        }

        public async Task<IEnumerable<Run>> FindAllRunsForUserAsync(string UserId)
        {
            var runs = await _db.Runs.Where(r => r.OwnerUserId == UserId).ToListAsync();
            return runs;
        }

        public async Task SetRunner(int id, string userName)
        {
            var run = await FindRunAsync(id);
            run.Runner = userName;
            run.runStatus = RunStatus.Prepped;

            _db.Entry(run).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task SetRunStatus(int id, RunStatus status)
        {
            var run = await FindRunAsync(id);
            run.runStatus = status;

            _db.Entry(run).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Run>> FindAllActiveRunsForUserAsync(string UserId)
        {
            var runs = await _db.Runs.Where(r => r.OwnerUserId == UserId && r.TimeToRun > DateTime.Now).ToListAsync();
            return runs;
        }

        public async Task<Run> FindRunAsync(int id)
        {
            var run = await _db.Runs.FindAsync(id);
            return run;
        }

        public async Task CreateRunAsync(Run run)
        {
            _db.Runs.Add(run);
            await _db.SaveChangesAsync();
        }

        public async Task EditRunAsync(Run run)
        {
            _db.Entry(run).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task RemoveRunAsync(int id)
        {
            var run = await FindRunAsync(id);
            _db.Runs.Remove(run);
            await _db.SaveChangesAsync();
        }
    }
}
