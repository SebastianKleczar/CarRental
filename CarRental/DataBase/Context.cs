using Azure;
using CarRental.DataBase.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using CarRental.DTO;
using System.Reflection.Emit;
using System.ComponentModel;
using System.Xml;
using Microsoft.Build.Execution;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

namespace CarRental.DataBase
{
    public class Context : IdentityDbContext<Client>
    {
        
           
           public DbSet<Car> Cars { get; set; }
           public DbSet<Client> Clients { get; set; }
           public DbSet<Rental> Rentals { get; set; }
           public DbSet<Review> Reviews { get; set; }

       

        public Context(DbContextOptions<Context> options) : base(options)
        {
            
          
        }

        public void Seed(UserManager<Client> userManager)
        {
            var AdminName = "Admin@gmail.com";
            if (this.Clients.FirstOrDefault(c => c.Email == AdminName) == null)
            {


                this.Roles.Add(new Microsoft.AspNetCore.Identity.IdentityRole() { Id = "Admin", Name = "Admin", NormalizedName = "ADMIN" });
                //this.Clients.Add(new Client() { Email ="admin", })

                var user = new Client() { Email = "Admin@gmail.com" };
                userManager.CreateAsync(user, "Admin123!");
                //var user = new Client() { Email = AdminName };
                //var password = new PasswordHasher<Client>();
                //var hashed = password.HashPassword(user, "Admin123!");
                //user.PasswordHash = hashed;
                //var addedUser = this.Clients.Add(user.Result);
                userManager.AddToRoleAsync(user, "Admin");


                this.SaveChanges();
            }
            }


        protected override void OnModelCreating(ModelBuilder builder)
        {
                base.OnModelCreating(builder);




            builder.Entity<Rental>()
                .HasKey(r => r.Id);

            builder.Entity<Rental>()
                .HasOne<Car>(r => r.Car)
                .WithMany(c => c.Rentals)
                .HasForeignKey(r => r.CarId);

            builder.Entity<Rental>()
                .HasOne<Client>(r => r.Client)
                .WithMany(c => c.Rentals)
                .HasForeignKey(r => r.ClientId);




            builder.Entity<Review>()
                .HasKey(r => r.Id);

            builder.Entity<Review>()
                .HasOne<Car>(r => r.Car)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CarId);

            builder.Entity<Review>()
                .HasOne<Client>(r => r.Client)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.ClientId);


            // Cascade deleting
            builder.Entity<Car>()
                .HasMany(c => c.Rentals)
                 .WithOne(r => r.Car)
                 .OnDelete(DeleteBehavior.Cascade);

           builder.Entity<Car>()
                .HasMany(c => c.Reviews)
                .WithOne(r => r.Car)
                .OnDelete(DeleteBehavior.Cascade);



            //var AdminName = "Admin@gmail.com";
            //if (this.Clients.FirstOrDefault(c => c.Email == AdminName) != null)
            //{


            //    this.Roles.Add(new Microsoft.AspNetCore.Identity.IdentityRole() { Id = "Admin", Name = "Admin", NormalizedName = "ADMIN" });
            //    //this.Clients.Add(new Client() { Email ="admin", })



            //    var user = new Client() { Email = AdminName };
            //    var password = new PasswordHasher<Client>();
            //    var hashed = password.HashPassword(user, "Admin123!");
            //    user.PasswordHash = hashed;
            //    var addedUser = this.Clients.Add(user);

            //    this.UserRoles.Add(new IdentityUserRole<string>() { UserId = addedUser.Entity.Id, RoleId = "Admin" });

            //}
            //builder.Entity<Application>()
            //    .HasKey(ap => new { ap.JobId, ap.UserId });


            //builder.Entity<Application>()
            //    .HasOne<Job>(ap => ap.Job)
            //    .WithMany(j => j.Applications)
            //    .HasForeignKey(ap => ap.JobId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //builder.Entity<Application>()
            //    .HasOne<User>(ap => ap.User)
            //    .WithMany(u => u.Applications)
            //    .HasForeignKey(ap => ap.UserId)
            //    .OnDelete(DeleteBehavior.NoAction);





        }


            public DbSet<CarRental.DTO.AvailableCarViewModel> AvailableCarViewModel { get; set; } = default!;

    }

    }

