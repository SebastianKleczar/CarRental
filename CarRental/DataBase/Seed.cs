using CarRental.DataBase.Models;
using Microsoft.AspNetCore.Identity;

namespace CarRental.DataBase
{
    public class Seed : ISeed
    {
        private readonly UserManager<Client> userManager;
        private readonly Context context;

        public Seed(UserManager<Client> userManager, Context context)
        {   
            this.userManager = userManager;
            this.context = context;
        }

        public void SeedToDb()
        {
            var AdminName = "Admin@gmail.com";
            if (context.Clients.FirstOrDefault(c => c.Email == AdminName) == null)
            {


                context.Roles.Add(new Microsoft.AspNetCore.Identity.IdentityRole() { Id = "Admin", Name = "Admin", NormalizedName = "ADMIN" });
                //this.Clients.Add(new Client() { Email ="admin", })

                var user = new Client() { Email = "Admin@gmail.com" };
                userManager.CreateAsync(user, "Admin123!");
                //var user = new Client() { Email = AdminName };
                //var password = new PasswordHasher<Client>();
                //var hashed = password.HashPassword(user, "Admin123!");
                //user.PasswordHash = hashed;
                //var addedUser = this.Clients.Add(user.Result);
                userManager.AddToRoleAsync(user, "Admin");


                context.SaveChanges();

            }

        }
    }
}
