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
    public interface IFriendRepository
    {
        Task<IEnumerable<Friend>> GetAllFriendsAsync(string UserId);
        Task<Friend> GetFriendAsync(int id);
        Task<ApplicationUser> FindUserAsync(string userName);
        Task<bool> AddFriendAsync(string userName, string userId);
        Task<bool> RemoveFriendAsync(int id);
        Task<IEnumerable<Friend>> FindFriendRequestsAsync(string userId);
    }

    public class FriendRepository : IFriendRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public FriendRepository(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Friend>> GetAllFriendsAsync(string UserId)
        {
            var friends = await _db.Friends.Where(f => f.UserId == UserId).ToListAsync();
            return friends;
        }

        public async Task<Friend> GetFriendAsync(int id)
        {
            var friend = await _db.Friends.FindAsync(id);
            return friend;
        }

        public async Task<ApplicationUser> FindUserAsync(string userName)
        {
            var friend = await _userManager.FindByNameAsync(userName);
            return friend;          
        }

        public async Task<bool> AddFriendAsync(string userName, string userId)
        {
            var user = await FindUserAsync(userName);
            if(user != null)
            {
                Friend friend = new Friend();
                friend.FriendUniqueName = userName;
                friend.UserId = userId;
                friend.FriendUserId = user.Id;
                friend.isConfirmed = false;

                _db.Friends.Add(friend);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveFriendAsync(int id)
        {
            Friend friend = await GetFriendAsync(id);
            _db.Friends.Remove(friend);
            bool success = (await _db.SaveChangesAsync() > 0) ? true : false;

            return success;
        }

        public async Task<IEnumerable<Friend>> FindFriendRequestsAsync(string userId)
        {
            var friends = await _db.Friends.Where(f => f.FriendUserId == userId && f.isConfirmed == false).ToListAsync();
            return friends;
        }
    }
}
