using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Controllers;
using WebApi.Interface;
using WebApi.Middleware;

namespace WebApi.Test.Controllers
{
    public class TokenControllerTests
    {
        private readonly IToken _token;
        private readonly TokenController _tokenController;

        public TokenControllerTests()
        {
            //Dependencies
            _token = A.Fake<IToken>();

            //SUT
            _tokenController = new TokenController(_token);
        }

        [Fact]
        public void TokenController_Index_ReturnSuccess()
        {
            //Arrange
            string token = "fakeToken";

            A.CallTo(() => _token.isExpired(token)).Returns(false);

            //Act
            var result = _tokenController.Index(token);

            //Assert
            result.Should().BeOfType<OkResult>();
        }
    }
}
