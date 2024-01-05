using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Test.Repository
{
    public class DropFileRepositoryTests
    {
        private async Task<AppDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new AppDbContext(options);
            databaseContext.Database.EnsureCreated();

            var passHasher = new PasswordHasher<AppUser>();
            if (await databaseContext.VinCodes.CountAsync() < 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.VinCodes.Add(
                        new VinCodes
                        {
                            Vin = i.ToString(),
                            AppUser = new AppUser
                            {
                                UserName = "Sokil",
                                Email = "Sokil@mail.com",
                                PasswordHash = passHasher.HashPassword(new AppUser(), "1Sokil@mail.com"),
                                PhoneNumber = "+380123456789",
                                ImageUrl = "http://res.cloudinary.com/dihwzdmiw/image/upload/v1703859755/howv8qpolsrk03jzsfnl.png",
                                FirstName = "Taras",
                                LastName = "Karas",
                            },
                            Errors = new List<Errors>() 
                            { 
                                new Errors
                                {
                                    Code = i.ToString(),
                                    Description = i.ToString(),
                                    DateTime = DateTime.Now,
                                }
                            }
                        });
                    await databaseContext.SaveChangesAsync();
                }
            }
            return databaseContext;
        }

        [Fact]
        public async void DropFileRepository_AddVin_RetursnBool()
        {
            //Arrange
            var passHasher = new PasswordHasher<AppUser>();

            var vin = new VinCodes
            {
                Vin = "11",
                AppUser = new AppUser
                {
                    UserName = "Sokil",
                    Email = "Sokil@mail.com",
                    PasswordHash = passHasher.HashPassword(new AppUser(), "1Sokil@mail.com"),
                    PhoneNumber = "+380123456789",
                    ImageUrl = "http://res.cloudinary.com/dihwzdmiw/image/upload/v1703859755/howv8qpolsrk03jzsfnl.png",
                    FirstName = "Taras",
                    LastName = "Karas",
                },
                Errors = new List<Errors>()
                {
                    new Errors
                    {
                        Code = "11",
                        Description = "11",
                        DateTime = DateTime.Now,
                    }
                }
            };

            var dbContext = await GetDbContext();
            var dropFileRepository = new DropFileRepository(dbContext);

            //Act
            var result = dropFileRepository.AddVin(vin);

            //Assert
            result.Should().BeTrue();
        }
    }
}
