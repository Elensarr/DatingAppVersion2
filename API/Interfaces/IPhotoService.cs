using CloudinaryDotNet.Actions;

namespace API.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        //IFormFile - file sent with http request
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
