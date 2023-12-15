using System.Text.RegularExpressions;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Services
{
    public class FileReader : IFileReader
    {
        const string VIN_CODE_REGEX = @"[A-HJ-NPR-Za-hj-npr-z\d]{8}[\dX][A-HJ-NPR-Za-hj-npr-z\d]{2}\d{6}";
        const string CAR_SCANNER_REGEX = @"^[PBCU]?\d{4,8}\s";
        const string CAR_SCANNER_DESCRIPTION_REGEX = @"(?<=Статус:\s)(.{4,})(?=\s*=\s*|\s*$)";
        public VinCodes ReadFile(IFormFile file)
        {
            var currentModule = new VinCodes() { };
            Errors errorInfo = new Errors();

            using (StreamReader sr = new StreamReader(file.OpenReadStream()))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (errorInfo.Code != null && errorInfo.Description != null)
                    {
                        errorInfo.DateTime = DateTime.Now;
                        currentModule.Errors.Add(errorInfo);
                        errorInfo = new Errors();
                    }

                    Match matchesCode = Regex.Match(line, CAR_SCANNER_REGEX, RegexOptions.Singleline);
                    if (matchesCode.Success)
                    {
                        errorInfo.Code = matchesCode.Value.Trim();
                        continue;
                    }

                    Match matchesDescription = Regex.Match(line, CAR_SCANNER_DESCRIPTION_REGEX, RegexOptions.Singleline);
                    if (matchesDescription.Success)
                    {
                        errorInfo.Description = matchesDescription.Value;
                        continue;
                    }

                    Match matchesVin = Regex.Match(line, VIN_CODE_REGEX, RegexOptions.Singleline);
                    if (matchesVin.Success)
                    {
                        currentModule = new VinCodes
                        {
                            Vin = matchesVin.Value
                        };
                        continue;
                    }
                }
                return currentModule;
            }
        }
    }
}