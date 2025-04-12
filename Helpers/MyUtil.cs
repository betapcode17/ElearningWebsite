using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;

public static class MyUtil
{
    public static string UploadImg(IFormFile img, string folderName)
    {
        if (img == null || img.Length == 0)
        {
            return null;
        }

        try
        {
            Console.WriteLine(img);
            // Lấy đường dẫn wwwroot mặc định
            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // Tạo đường dẫn thư mục lưu ảnh
            string folderPath = Path.Combine(webRootPath, "images", folderName);

            // Tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Xử lý tên file
            string fileName = Path.GetFileNameWithoutExtension(img.FileName);
            fileName = RemoveInvalidChars(fileName);
            fileName += Path.GetExtension(img.FileName);

            // Đường dẫn đầy đủ
            string filePath = Path.Combine(folderPath, fileName);

            // Ghi file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                img.CopyTo(stream); // dùng đồng bộ cho đơn giản
            }

            // Trả về đường dẫn tương đối
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
