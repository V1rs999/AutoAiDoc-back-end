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
using WebApi.Models;

namespace WebApi.Test.Controllers
{
    public class RegistrationControllerTests
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RegistrationController _registrationController;
        public RegistrationControllerTests()
        {
            //Dependencies
            _userManager = A.Fake<UserManager<AppUser>>();
            _signInManager = A.Fake<SignInManager<AppUser>>();

            //SUT
            _registrationController = new RegistrationController(_userManager, _signInManager);
        }

        [Fact]
        public void RegistrationController_Index_ReturnSuccess()
        {
            //Arrange
            var registerVM = A.Fake<RegisterDto>();
            var user = A.Fake<AppUser>();

            A.CallTo(() => _userManager.FindByEmailAsync(registerVM.Email)).Returns(user);

            var newUser = A.Fake<AppUser>();
            var newUserResponse = A.Fake<IdentityResult>();

            A.CallTo(() => _userManager.CreateAsync(newUser, registerVM.Password)).Returns(newUserResponse);

            var result = A.Fake<Microsoft.AspNetCore.Identity.SignInResult>();

            A.CallTo(() => _signInManager.PasswordSignInAsync(newUser, registerVM.Password, false, false)).Returns(result);

            //Act
            var testResult = _registrationController.Index(registerVM);

            //Assert
            testResult.Should().BeOfType<Task<IActionResult>>();
        }
    }
}
