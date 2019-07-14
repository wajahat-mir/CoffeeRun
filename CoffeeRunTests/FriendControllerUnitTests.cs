using AutoMapper;
using CoffeeRun.Controllers;
using CoffeeRun.Models;
using CoffeeRun.Profiles;
using CoffeeRun.Repositories;
using CoffeeRun.ViewModels;
using CoffeeRunTests.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Xunit;

namespace CoffeeRunTests
{
    public class FriendControllerUnitTests
    {
        public static List<Friend> GetFriends()
        {
            var friends = new List<Friend>();
            friends.Add(new Friend()
            {
                FriendId = 1,
                UserId = "123",
                FriendUserId = "321",
                FriendUniqueName = "MyFriend",
                isConfirmed = true
            });
            friends.Add(new Friend()
            {
                FriendId = 1,
                UserId = "123",
                FriendUserId = "456",
                FriendUniqueName = "MyFriend2",
                isConfirmed = true
            });
            return friends;
        }

        private Friend GetFriend()
        {
            var friend = new Friend();
            friend.FriendId = 1;
            friend.UserId = "123";
            friend.FriendUserId = "321";
            friend.FriendUniqueName = "MyFriend";
            friend.isConfirmed = true;

            return friend;
        }

        private Friend NullFriend()
        {
            Friend friend = null;
            return friend;
        }

        [Fact]
        public void Index_ReturnsAViewResult_WithAllFriends()
        {
            var mockUserManager = UserMockService.BaseUser();
            var mapper = MapperService.DefaultMapper();

            var mockRepo = new Mock<IFriendRepository>();
            mockRepo.Setup(repo => repo.GetAllFriendsAsync(It.IsAny<string>()))
                .ReturnsAsync(GetFriends());

            var controller = new FriendsController(mapper, mockUserManager.Object, mockRepo.Object);

            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<FriendViewModel>>(
                viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void Index_ReturnsAViewResult_WithNoFriends()
        {
            var mockUserManager = UserMockService.BaseUser();
            var mapper = MapperService.DefaultMapper();

            List<Friend> noFriends = new List<Friend>();
            var mockRepo = new Mock<IFriendRepository>();
            mockRepo.Setup(repo => repo.GetAllFriendsAsync(It.IsAny<string>()))
                .ReturnsAsync(noFriends);

            var controller = new FriendsController(mapper, mockUserManager.Object, mockRepo.Object);

            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<FriendViewModel>>(
                viewResult.ViewData.Model);
            Assert.Empty(model);
        }

        [Fact]
        public void VerifyUserName_ReturnTrue()
        {
            var mockUserManager = UserMockService.BaseUser();
            var mapper = MapperService.DefaultMapper();

            ApplicationUser friend = new ApplicationUser();
            var mockRepo = new Mock<IFriendRepository>();
            mockRepo.Setup(repo => repo.FindUserAsync(It.IsAny<string>()))
                .ReturnsAsync(friend);

            var controller = new FriendsController(mapper, mockUserManager.Object, mockRepo.Object);
            var result = controller.VerifyUserName(It.IsAny<string>());
            var jsonResult = Assert.IsType<JsonResult>(result.Result);
            Assert.Equal(true, jsonResult.Value);
        }

        [Fact]
        public void VerifyUserName_ReturnFalse()
        {
            var mockUserManager = UserMockService.BaseUser();
            var mapper = MapperService.DefaultMapper();

            ApplicationUser friend = null;
            var mockRepo = new Mock<IFriendRepository>();
            mockRepo.Setup(repo => repo.FindUserAsync(It.IsAny<string>()))
                .ReturnsAsync(friend);

            var controller = new FriendsController(mapper, mockUserManager.Object, mockRepo.Object);
            var result = controller.VerifyUserName(It.IsAny<string>());
            var jsonResult = Assert.IsType<JsonResult>(result.Result);
            Assert.Equal(false, jsonResult.Value);
        }

        [Fact]
        public void Create_ReturnsBadRequest_FailCreate()
        {
            var mockUserManager = UserMockService.BaseUser();
            var mapper = MapperService.DefaultMapper();

            var mockRepo = new Mock<IFriendRepository>();
            mockRepo.Setup(repo => repo.AddFriendAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            FriendViewModel friendViewModel = new FriendViewModel();
            friendViewModel.FriendUniqueName = "Test_User";

            var controller = new FriendsController(mapper, mockUserManager.Object, mockRepo.Object);
            var result = controller.Create(friendViewModel);
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public void Create_ReturnsRedirect_SuccessfulCreate()
        {
            var mockUserManager = UserMockService.BaseUser();
            var mapper = MapperService.DefaultMapper();

            var mockRepo = new Mock<IFriendRepository>();
            mockRepo.Setup(repo => repo.AddFriendAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            FriendViewModel friendViewModel = new FriendViewModel();
            friendViewModel.FriendUniqueName = "Test_User";

            var controller = new FriendsController(mapper, mockUserManager.Object, mockRepo.Object);
            var result = controller.Create(friendViewModel);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result.Result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public void Delete_ReturnsNotFound_IdIsNull()
        {
            var mockUserManager = UserMockService.BaseUser();
            var mapper = MapperService.DefaultMapper();
            var mockRepo = new Mock<IFriendRepository>();

            var controller = new FriendsController(mapper, mockUserManager.Object, mockRepo.Object);
            var result = controller.Delete(null);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void Delete_ReturnsNotFound_FriendIsNull()
        {
            var mockUserManager = UserMockService.BaseUser();
            var mapper = MapperService.DefaultMapper();

            var mockRepo = new Mock<IFriendRepository>();
            mockRepo.Setup(r => r.GetFriendAsync(It.IsAny<int>()))
                .ReturnsAsync(NullFriend());

            var controller = new FriendsController(mapper, mockUserManager.Object, mockRepo.Object);
            var result = controller.Delete(null);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void Delete_ReturnsNotFound_WithFriend()
        {
            var mockUserManager = UserMockService.BaseUser();
            var mapper = MapperService.DefaultMapper();

            var mockRepo = new Mock<IFriendRepository>();
            mockRepo.Setup(r => r.GetFriendAsync(It.IsAny<int>()))
                .ReturnsAsync(GetFriend());

            var controller = new FriendsController(mapper, mockUserManager.Object, mockRepo.Object);
            var result = controller.Delete(It.IsAny<int>());
            var viewResult = Assert.IsType<ViewResult>(result.Result);
            var model = Assert.IsAssignableFrom<FriendViewModel>(
                viewResult.ViewData.Model);
        }

        [Fact]
        public void DeleteConfirmed_ReturnBadRequest_BadDelete()
        {
            var mockUserManager = UserMockService.BaseUser();
            var mapper = MapperService.DefaultMapper();

            var mockRepo = new Mock<IFriendRepository>();
            mockRepo.Setup(r => r.RemoveFriendAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            var controller = new FriendsController(mapper, mockUserManager.Object, mockRepo.Object);
            var result = controller.DeleteConfirmed(It.IsAny<int>());
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public void DeleteConfirmed_ReturnRedirect_GoodDelete()
        {
            var mockUserManager = UserMockService.BaseUser();
            var mapper = MapperService.DefaultMapper();

            var mockRepo = new Mock<IFriendRepository>();
            mockRepo.Setup(r => r.RemoveFriendAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var controller = new FriendsController(mapper, mockUserManager.Object, mockRepo.Object);
            var result = controller.DeleteConfirmed(It.IsAny<int>());
            var redirectResult = Assert.IsType<RedirectToActionResult>(result.Result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }
}
