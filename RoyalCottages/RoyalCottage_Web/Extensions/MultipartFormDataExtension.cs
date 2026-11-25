using System.Net.Http.Headers;

namespace RoyalCottage_Web.Extensions
{
    public static class MultipartFormDataExtension
    {
        // This extension method converts an object to MultipartFormDataContent for sending multipart/form-data requests.
        public static MultipartFormDataContent ToMultipartFormDataContent(this object obj)
        {
            // Create a new instance of MultipartFormDataContent to hold the form data
            var formContent = new MultipartFormDataContent();

            // Get the properties of the object using reflection
            var properties = obj.GetType().GetProperties();

            // Loop through each property of the object
            foreach (var property in properties)
            {
                // Get the value of the property for the current object
                var value = property.GetValue(obj);

                // Skip the property if its value is null
                if (value==null)
                {
                    continue;
                }

                // Get the name of the property to use as the form field name
                var propertyName = property.Name;

                // Check if the property value is of type IFormFile (for file uploads)
                if (value is IFormFile file && file.Length > 0)
                {
                    // Create a StreamContent for the file and set its content type
                    var streamContent = new StreamContent(file.OpenReadStream());

                    // Set the content type of the file based on its MIME type
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                    // Add the file content to the MultipartFormDataContent with the property name and file name
                    formContent.Add(streamContent, propertyName, file.FileName);


                }
                else if(value is not IFormFile)
                {
                    // If the property value is not a file, add it as a string content to the form data
                    formContent.Add(new StringContent(value.ToString()!),propertyName);
                }
            }
            // Return the constructed MultipartFormDataContent containing all the form fields and files
            return formContent;
        }
    }
}
