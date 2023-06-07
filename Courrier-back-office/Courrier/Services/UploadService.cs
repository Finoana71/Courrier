namespace Courrier.Services
{
    public class UploadService
    {
        public string UploadFile(IFormFile file)
        {
            if (file == null)
                return null;
            // Generate a unique file name
            string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;

            // Get the uploads directory path
            string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            // Create the uploads directory if it doesn't exist
            Directory.CreateDirectory(uploadsPath);

            // Save the file to the uploads directory
            string filePath = Path.Combine(uploadsPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return fileName;
        }

    }
}
