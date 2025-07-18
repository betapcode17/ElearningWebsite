using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public static class MyUtil
{
    public static async Task<string> UploadImg(IFormFile imageFile, string folderName)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return null;
        }

        try
        {
            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string folderPath = Path.Combine(webRootPath, "images", folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
            fileName = RemoveInvalidChars(fileName);
            fileName += Path.GetExtension(imageFile.FileName);

            string filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return Path.Combine("images", folderName, fileName).Replace("\\", "/");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi: " + ex.Message);
            return null;
        }
    }

    private static string RemoveInvalidChars(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c.ToString(), "");
        }

        byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(name);
        name = Encoding.ASCII.GetString(bytes);

        return name;
    }
}