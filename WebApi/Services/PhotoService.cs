using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using WebApi.Helpers;
using WebApi.Interface;

namespace WebApi.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloundinary;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
                );
            _cloundinary = new Cloudinary(acc);
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face").Radius("max")
                };
                uploadResult = await _cloundinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string imageUrl)
        {
            var publicId = GetPublicIdFromUrl(imageUrl);
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloundinary.DestroyAsync(deleteParams);

            return result;
        }

        private string GetPublicIdFromUrl(string cloudinaryUrl)
        {
            // Define a regular expression pattern to match the public ID in a Cloudinary URL
            string pattern = @"\/v\d+\/([^\./]+)";

            // Create a regex object with the pattern
            Regex regex = new Regex(pattern);

            // Match the pattern in the URL
            Match match = regex.Match(cloudinaryUrl);

            // Check if there is a match and get the captured group value
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            // Return null or an empty string if no match is found
            return null;
        }
    }
}
