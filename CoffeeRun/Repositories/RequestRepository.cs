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
    public interface IRequestRepository
    {
        Task<IEnumerable<Friend>> GetAllFriendRequests(string userId);
        Task SetToConfirmedAndAddFriend(int id);
        Task RemoveRequest(int id);
        Task<Friend> GetFriendAsync(int id);
    }
    public class RequestRepository : IRequestRepository
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        public RequestRepository(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<IEnumerable<Friend>> GetAllFriendRequests(string userId)
        {
            var friends = await _db.Friends.Where(f => f.FriendUserId == userId && f.isConfirmed == false).ToListAsync();
            return friends;
        }

        public async Task SetToConfirmedAndAddFriend(int id)
        {
            var friend = await GetFriendAsync(id);
            if (friend != null)
            {
                friend.isConfirmed = true;
                _db.Entry(friend).State = EntityState.Modified;
                if(await _db.SaveChangesAsync() > 0)
                {
                    Friend reverseFriend = new Friend();
                    reverseFriend.UserId = friend.FriendUserId;
                    reverseFriend.FriendUserId = friend.UserId;

                    var reverseUser = await _userManager.FindByIdAsync(friend.UserId);
                    reverseFriend.FriendUniqueName = reverseUser.Email;
                    reverseFriend.isConfirmed = true;

                    _db.Friends.Add(reverseFriend);
                    await _db.SaveChangesAsync();
                }
            }
        }

        public async Task RemoveRequest(int id)
        {
            Friend friend = await GetFriendAsync(id);
            _db.Friends.Remove(friend);
            await _db.SaveChangesAsync();
        }

        public async Task<Friend> GetFriendAsync(int id)
        {
            var friend = await _db.Friends.FindAsync(id);
            return friend;
        }
    }
}
