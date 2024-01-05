using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Controllers;
using WebApi.Dto;
using WebApi.Interface;
using WebApi.Repository;

namespace WebApi.Test.Controllers
{
    public class ListOFErrorControllerTests
    {
        private readonly IListOFErrorRepository _listOFErrorRepository;
        private readonly ListOFErrorController _listOFErrorController;
        public ListOFErrorControllerTests()
        {
            //Dependencies
            _listOFErrorRepository = A.Fake<IListOFErrorRepository>();

            //SUT
            _listOFErrorController = new ListOFErrorController(_listOFErrorRepository);
        }

        [Fact]
        public async Task ListOFErrorController_Index_ReturnSuccess()
        {
            //Arrange
            string Vin = "fakeVin";
            var models = A.Fake<IEnumerable<ListOfErrorsOutputDto>>();

            A.CallTo(() => _listOFErrorRepository.GetErrorsByVinAsync(Vin)).Returns(models);

            //Act
            var result = await _listOFErrorController.Index(Vin);

            //Assert
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
