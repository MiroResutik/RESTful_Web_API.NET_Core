namespace RoyalCottages_API.Services.IServices
{
    public interface IImageService
    {
        // Endpoint to upload an image and return its URL
        Task<string> UploadImageAsync(IFormFile file);

        // Endpoint to delete an image by its URL
        Task<bool> DeleteImageAsync(string imageUrl);

        // Endpoint to validate the uploaded image (e.g., check file type, size, etc.)
        bool ValidateImage(IFormFile file);
    }
}
