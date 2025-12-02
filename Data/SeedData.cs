using MindEase.Models;

namespace MindEase.Data
{
    public static class SeedData
    {
        public static void Initialize(MindEaseContext context)
        {
            // Zaten kullanıcı varsa tekrar ekleme
            if (context.Users.Any())
            {
                return;
            }

            var users = new List<User>
            {
                new User { Username = "hatice_admin",  Password = "admin123",   UserType = "admin" },
                new User { Username = "melisa_user",   Password = "user2024",   UserType = "user" },
                new User { Username = "john_therapist",Password = "healme",     UserType = "therapist" },
                new User { Username = "ayse_user",     Password = "123456",     UserType = "user" },
                new User { Username = "mehmet_user",   Password = "qwerty",     UserType = "user" }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}
