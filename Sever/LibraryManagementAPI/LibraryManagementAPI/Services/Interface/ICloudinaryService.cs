namespace LibraryManagementAPI.Services.Interface
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file, string uniqueFileName);
        Task<bool> DeleteImageAsync(string publicId);

    }
}
