using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using WebApplication4.Application.Common.Interfaces;

namespace WebApplication4.Infrastructure.Common
{
    public class CloudinaryFileUploadService : IFileUploadService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public CloudinaryFileUploadService(CloudinaryDotNet.Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }


        public async Task<string> UploadAsync(IFormFile file)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "prescriptions"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.AbsoluteUri;
        }
    }
}