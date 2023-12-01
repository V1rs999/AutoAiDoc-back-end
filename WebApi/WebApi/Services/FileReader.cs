﻿
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using WebApi.Models;

namespace WebApi.Services
{
    public class FileReader
    {
        readonly string carScannerRegex = @"^[PBCU]\d{4}(?=\s)";
        readonly string carScannerDescriptionRegex = @"(?<=Статус:\s)(.{4,})(?=\s*=\s*|\s*$)";

        public IEnumerable<Errors> ReadAsList(IFormFile file)
        {
            List<Errors> errors = new List<Errors>();
            var result = new StringBuilder();
            string line;
            using (var reader = new StreamReader(file.OpenReadStream()))
            { 
                
                while (reader.Peek() >= 0)
                    result.AppendLine(reader.ReadLine());

                line = result.ToString();
                MatchCollection matchesCode = Regex.Matches(line, carScannerRegex, RegexOptions.Multiline);
                MatchCollection matchesDescription = Regex.Matches(line, carScannerDescriptionRegex, RegexOptions.Multiline);
                
                int matchesCount = matchesCode.Count;
                if (matchesCode.Count > 0)
                {
                    for (int i = 0; i < matchesCount; i++) {

                        string description = matchesDescription
                            .Where(d => d.Index >= matchesCode[i].Index)
                            .Select(m => m.Value)
                            .FirstOrDefault();

                        errors.Add(new Errors {
                            Code = matchesCode[i].Value,
                            Description = description.Substring(0, description.Length - 1),
                        });
                    }
                }
            }
            return errors;
        }
    }
}


