using System.Security.Cryptography;
using System.Text;

public static class PasswordUtil
{
    public static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {         
            byte[] bytes = Encoding.UTF8.GetBytes(password);     
            byte[] hashBytes = sha256.ComputeHash(bytes);
            StringBuilder builder = new StringBuilder();
            foreach (var b in hashBytes)
            {
                builder.Append(b.ToString("x2")); 
            }
           return builder.ToString();
        }
    }
}
