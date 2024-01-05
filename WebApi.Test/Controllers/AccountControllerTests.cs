using CloudinaryDotNet.Actions;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Controllers;
using WebApi.Dto;
using WebApi.Interface;
using WebApi.Models;
using WebApi.Repository;
using WebApi.Services;

namespace WebApi.Test.Controllers
{
    public class AccountControllerTests
    {
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IPhotoService _photoService;
        private readonly AccountController _accountController;
        public AccountControllerTests()
        {
            //Dependencies
            _accountRepository = A.Fake<IAccountRepository>();
            _userManager = A.Fake<UserManager<AppUser>>();
            _photoService = A.Fake<IPhotoService>();

            //SUT
            _accountController = new AccountController(_accountRepository, _userManager, _photoService);
        }

        [Fact]
        public async Task AccountController_GetIndex_ResurtSuccess()
        {
            //Arrange
            string userId = "fakeUserId";
            var user = A.Fake<AppUser>();

            A.CallTo(() => _accountRepository.GetUserByIdAsync(userId)).Returns(user);

            bool LoginProvider = false;

            A.CallTo(() => _accountRepository.isLoginProvider(userId)).Returns(LoginProvider);

            var model = A.Fake<AccountDto>();
            //Act
            var result = await _accountController.Index(userId);

            //Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AccountController_PostIndex_ResurtSuccess()
        {
            //Arrange
            var model = A.Fake<UpdateAccountDto>();
            var user = A.Fake<AppUser>();

            A.CallTo(() => _accountRepository.GetUserByIdAsync(model.userId)).Returns(user);

            bool LoginProvider = false;

            A.CallTo(() => _accountRepository.isLoginProvider(model.userId)).Returns(LoginProvider);

            bool validate = false;

            A.CallTo(() => _userManager.CheckPasswordAsync(user, model.passwordR ?? string.Empty)).Returns(validate);
            A.CallTo(() => _accountRepository.Update(user)).Returns(true);
            //Act
            var result = await _accountController.Index(model);

            //Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AccountController_ChangeImage_ResurtSuccess()
        {

            //Arrange
            var model = A.Fake<ChangeImageDto>();
            var user = A.Fake<AppUser>();

            A.CallTo(() => _accountRepository.GetUserByIdAsync(model.Id)).Returns(user);

            var photoResult = A.Fake<ImageUploadResult>();

            A.CallTo(() => _photoService.AddPhotoAsync(model.Image)).Returns(photoResult);
            A.CallTo(() => _accountRepository.Update(user)).Returns(true);

            //Act
            var result = await _accountController.ChangeImage(model);

            //Assert
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
