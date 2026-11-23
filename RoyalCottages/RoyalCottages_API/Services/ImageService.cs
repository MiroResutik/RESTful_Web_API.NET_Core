using RoyalCottages_API.Services.IServices;

namespace RoyalCottages_API.Services
{
    public class ImageService : IImageService
    {
        // Define the maximum file size for uploaded images (5 MB)
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        // Define the allowed file extensions for uploaded images
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };

        // Inject the IWebHostEnvironment to access the web root path
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Constructor to initialize the ImageService with the IWebHostEnvironment
        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        // Method to delete an image asynchronously based on its URL
        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            try
            {
                // Check if the image URL is null or empty
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return false;
                }

                // Get the file name from the image URL
                var fileName = Path.GetFileName(imageUrl);

                // Combine the web root path with the image folder and file name to get the full file path
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "cottages", fileName);

                // Check if the file exists and delete it asynchronously
                if (File.Exists(filePath))
                {
                    await Task.Run(()=> File.Delete(filePath));
                }

                return false;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // Method to upload an image asynchronously and return its URL
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            try
            {
                // Validate the uploaded image file
                if (!ValidateImage(file))
                {
                    throw new InvalidOperationException("Invalid image file");
                }

                // Combine the web root path with the image folder to get the upload folder path
                var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images","cottages");

                // Create the upload folder if it doesn't exist
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                // Get the file extension of the uploaded image
                var fileExtension = Path.GetExtension(file.FileName);

                // Generate a unique file name using a GUID and the original file extension
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

                // Combine the upload folder path with the unique file name to get the full file path
                var filePath = Path.Combine(uploadFolder, uniqueFileName);

                // Save the uploaded image file to the specified file path asynchronously
                using (var fileStream = new FileStream(filePath,FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Return the relative URL of the uploaded image
                var imageUrl = $"/images/cottages/{uniqueFileName}";

                return imageUrl;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public bool ValidateImage(IFormFile file)
        {
            // Check if the file is null or empty
            if (file == null || file.Length == 0)
            {
                return false;
            }

            // Check if the file size exceeds the maximum allowed size
            if (file.Length > MaxFileSize) 
            {
                return false;
            }

            // Check if the file extension is allowed
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            // Check if the file extension is in the list of allowed extensions
            if (!AllowedExtensions.Contains(extension))
            {
                return false;
            }

            return true;
        }
    }
}
