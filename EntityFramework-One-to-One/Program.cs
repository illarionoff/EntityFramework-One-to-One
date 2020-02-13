using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_One_to_One
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public UserProfile Profile { get; set; }
    }

    public class UserProfile
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int Age { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public ApplicationContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=onetoonedb;Trusted_Connection=True;");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                User user1 = new User { Login = "login1", Password = "pass1234" };
                User user2 = new User { Login = "login2", Password = "5678word2" };
                db.Users.AddRange(new List<User> { user1, user2 });
                db.SaveChanges();

                UserProfile profile1 = new UserProfile { Age = 22, Name = "Tom", UserId = user1.Id };
                UserProfile profile2 = new UserProfile { Age = 27, Name = "Alice", UserId = user2.Id };
                db.UserProfiles.AddRange(new List<UserProfile> { profile1, profile2 });
                db.SaveChanges();

                foreach (User user in db.Users.Include(u => u.Profile).ToList())
                    Console.WriteLine("Name: {0}  Age: {1}  Login: {2}  Password: {3}",
                        user?.Profile.Name, user?.Profile.Age, user.Login, user.Password);

                // Рекдактирование
                User FirstUser = db.Users.FirstOrDefault();
                if (FirstUser != null)
                {
                    FirstUser.Password = "dsfvbggg";
                    db.SaveChanges();
                }

                UserProfile profile = db.UserProfiles.FirstOrDefault(p => p.User.Login == "login2");
                if (profile != null)
                {
                    profile.Name = "Alice II";
                    db.SaveChanges();
                }

                foreach (User user in db.Users.Include(u => u.Profile).ToList())
                    Console.WriteLine("Name: {0}  Age: {1}  Login: {2}  Password: {3}",
                        user?.Profile.Name, user?.Profile.Age, user.Login, user.Password);

                //Удаление
                User DeleteUser = db.Users.FirstOrDefault();
                if (DeleteUser != null)
                {
                    db.Users.Remove(DeleteUser);
                    db.SaveChanges();
                }

                Console.WriteLine("After User Delete");
                foreach (User user in db.Users.ToList())
                    Console.WriteLine("Name: {0}",
                        user?.Login);
                foreach (UserProfile profileE in db.UserProfiles.ToList())
                    Console.WriteLine("Login: {0}",
                        profileE?.Name);


                UserProfile DeleteProfile = db.UserProfiles.FirstOrDefault(p => p.User.Login == "login2");
                if (DeleteProfile != null)
                {
                    db.UserProfiles.Remove(DeleteProfile);
                    db.SaveChanges();
                }

                Console.WriteLine("After Profile Delete");
                foreach (User user in db.Users.ToList())
                    Console.WriteLine("Name: {0}",
                        user?.Login);
                foreach (UserProfile profileE in db.UserProfiles.ToList())
                    Console.WriteLine("Login: {0}",
                        profileE?.Name);

            }
        }
    }
}
