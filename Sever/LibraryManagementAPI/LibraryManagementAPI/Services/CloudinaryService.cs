using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using LibraryManagementAPI.Models;
using LibraryManagementAPI.Services.Interface;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace LibraryManagementAPI.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly long MaxSizeFile = 5 * 1024 * 1024;
        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }


        public async Task<string> UploadImageAsync(IFormFile file, string uniqueFileName)
        {
            if (file.Length > 0 && file.Length < MaxSizeFile)
            {
                using var stream = file.OpenReadStream();

                // Load the image using ImageSharp
                using var image = await Image.LoadAsync<Rgba32>(stream);

                // Resize the image to the desired size (e.g., 800x600)
                int targetWidth = 800;
                int targetHeight = 600;
                image.Mutate(x => x.Resize(targetWidth, targetHeight));

                // Save the resized image to a MemoryStream
                using var memoryStream = new MemoryStream();
                var encoder = new JpegEncoder(); // Choose JPEG as the output format
                await image.SaveAsync(memoryStream, encoder);

                // Reset the memory stream position to the beginning before upload
                memoryStream.Seek(0, SeekOrigin.Begin);

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(uniqueFileName, memoryStream),
                    PublicId = uniqueFileName
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            }

            return null;
        }



        public async Task<bool> DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result == "Delete successfull in Cloudinary";
        }
    }
}
