using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.AspNetCore.Http;
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
    public class DropFileControllerTests
    {
        private readonly IDropFileRepository _dropFileRepository;
        private readonly IFileReader _fileReader;
        private readonly DropFileController _dropFileController;
        public DropFileControllerTests()
        {
            //Dependencies
            _dropFileRepository = A.Fake<IDropFileRepository>();
            _fileReader = A.Fake<IFileReader>();

            //SUT
            _dropFileController = new DropFileController(_dropFileRepository, _fileReader);
        }

        [Fact]
        public async void DropFileController_Index_ReturnSuccess()
        {
            // Arrange
            string userId = "fakeUserId";
            var formFile = A.Fake<IFormFile>();
            string Vin = "fakeVin";
            var errorsDto = A.Fake<List<ErrorsDto>>();
            var user = A.Fake<AppUser>();
            var vinCode = A.Fake<VinCodes>();

            A.CallTo(() => _dropFileRepository.GetUserById(userId)).Returns(user);
            A.CallTo(() => _fileReader.ReadFile(formFile)).Returns(vinCode);
            A.CallTo(() => _dropFileRepository.AddVin(vinCode)).Returns(true);

            // Act
            var result = _dropFileController.Index(userId, formFile, Vin);

            // Assert
            result.Should().BeOfType<Task<IActionResult>>();
        }
    }
}
