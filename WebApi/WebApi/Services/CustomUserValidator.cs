using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace WebApi.Services
{
    public class CustomUserValidator<TUser> : UserValidator<TUser> where TUser : class
    {
        public override async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            var identityResult = await base.ValidateAsync(manager, user);

            var errors = identityResult.Errors.ToList();

            // Add custom validation for Ukrainian letters in usernames
            var username = await manager.GetUserNameAsync(user);
            if (!string.IsNullOrEmpty(username))
            {
                var isValid = IsValidUkrainianUsername(username);
                if (!isValid)
                {
                    errors.Add(new IdentityError
                    {
                        Code = "InvalidUsername",
                        Description = "Usernames must contain only alphanumeric characters and Ukrainian letters."
                    });
                }
            }

            return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }

        private bool IsValidUkrainianUsername(string username)
        {
            // Customize this regular expression to fit your specific requirements
            var regex = new Regex(@"^[a-zA-Z0-9а-яА-Я]+$");
            return regex.IsMatch(username);
        }
    }
}
