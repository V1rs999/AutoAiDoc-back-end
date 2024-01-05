using CloudinaryDotNet;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using WebApi.Controllers;
using WebApi.Dto;
using WebApi.Interface;
using WebApi.Middleware;
using WebApi.Models;

namespace WebApi.Test.Controllers
{
    public class AuthorizationControllerTests
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IToken _token;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthorizationController _authorizationController;
        public AuthorizationControllerTests()
        {
            //Dependencies
            _userManager = A.Fake<UserManager<AppUser>>();
            _signInManager = A.Fake<SignInManager<AppUser>>();
            _token = A.Fake<IToken>();
            _httpContextAccessor = A.Fake<IHttpContextAccessor>();

            //SUT
            _authorizationController = new AuthorizationController(_userManager, _signInManager, _token, _httpContextAccessor);
        }

        [Fact]
        public void AuthorizationController_Index_ReturnSuccess()
        {
            //Arrange
            var model = A.Fake<LoginDto>();
            var curUser = A.Fake<AppUser>();

            A.CallTo(() => _userManager.FindByEmailAsync(model.Email)).Returns(curUser);
            A.CallTo(() => _userManager.CheckPasswordAsync(curUser, model.Password)).Returns(true);

            var authClaims = A.Fake<List<Claim>>();

            A.CallTo(() => _token.GetClaimsForJwt()).Returns(authClaims);

            var autrhResult = A.Fake<Microsoft.AspNetCore.Identity.SignInResult>();
            autrhResult = Microsoft.AspNetCore.Identity.SignInResult.Success;

            var myToken = A.Fake<JwtSecurityToken>();

            A.CallTo(() => _token.GetToken(authClaims)).Returns(myToken);

            A.CallTo(() => _signInManager.PasswordSignInAsync(curUser, model.Password, false, false))
            .Returns(autrhResult);

            //Act
            var result = _authorizationController.Index(model);

            //Assert
            result.Should().BeOfType<Task<IActionResult>>();
        }
    }

}
