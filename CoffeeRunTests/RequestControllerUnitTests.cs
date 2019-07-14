using CoffeeRun.Controllers;
using CoffeeRun.Models;
using CoffeeRun.Repositories;
using CoffeeRun.ViewModels;
using CoffeeRunTests.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CoffeeRunTests
{
    public class RequestControllerUnitTests
    {
        [Fact]
        public void Index_ReturnsAViewResult_WithAllFriends()
        {
            var mockUserManager = UserMockService.BaseUser();
            var mapper = MapperService.DefaultMapper();

            var mockRepo = new Mock<IRequestRepository>();
            mockRepo.Setup(repo => repo.GetAllFriendRequests(It.IsAny<string>()))
                .ReturnsAsync(FriendControllerUnitTests.GetFriends());

            var controller = new RequestsController(mockRepo.Object, mockUserManager.Object, mapper);

            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<FriendViewModel>>(
                viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void Index_ReturnsAViewResult_NoFriends()
        {
            var mockUserManager = UserMockService.BaseUser();
            var mapper = MapperService.DefaultMapper();

            List<Friend> noFriends = new List<Friend>();
            var mockRepo = new Mock<IRequestRepository>();
            mockRepo.Setup(repo => repo.GetAllFriendRequests(It.IsAny<string>()))
                .ReturnsAsync(noFriends);

            var controller = new RequestsController(mockRepo.Object, mockUserManager.Object, mapper);

            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<FriendViewModel>>(
                viewResult.ViewData.Model);
            Assert.Empty(model);
        }

        [Fact]
        public void Confirm_ReturnsRedirect()
        {
            var mockUserManager = UserMockService.BaseUser();
            var mapper = MapperService.DefaultMapper();
            var mockRepo = new Mock<IRequestRepository>();
            mockRepo.Setup(repo => repo.SetToConfirmedAndAddFriend(It.IsAny<int>()));

            var controller = new RequestsController(mockRepo.Object, mockUserManager.Object, mapper);

            var result = controller.Confirm(It.IsAny<int>());
            var redirectResult = Assert.IsType<RedirectToActionResult>(result.Result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public void Remove_ReturnsRedirect()
        {
            var mockUserManager = UserMockService.BaseUser();
            var mapper = MapperService.DefaultMapper();
            var mockRepo = new Mock<IRequestRepository>();
            mockRepo.Setup(repo => repo.RemoveRequest(It.IsAny<int>()));

            var controller = new RequestsController(mockRepo.Object, mockUserManager.Object, mapper);

            var result = controller.Revoke(It.IsAny<int>());
            var redirectResult = Assert.IsType<RedirectToActionResult>(result.Result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }
}
