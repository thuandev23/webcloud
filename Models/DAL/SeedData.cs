namespace QLKhachSanAPI.Models.DAL
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using QLKhachSanAPI.Models.Domains;

    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            // https://stackoverflow.com/questions/52487989/cannot-resolve-scoped-service-microsoft-aspnetcore-identity-usermanager1ident
            using (var scope = app.ApplicationServices.CreateScope())
            {
                // Resolve ASP .NET Core Identity with DI help
                var userManager = (UserManager<ApplicationUser>)scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>));
                var roleManager = (RoleManager<IdentityRole>)scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>));

                //var roleManager = app.ApplicationServices.GetRequiredService<RoleManager<IdentityRole>>();

                var dbContext = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();

                if (dbContext.Database.GetPendingMigrations().Any())
                {
                    dbContext.Database.Migrate();
                }

                if (!dbContext.Users.Any())
                {
                    // Create the default admin user                    

                    // Check if the "Admin" role exists, and create it if not
                    if (!roleManager.RoleExistsAsync("Admin").Result)
                    {
                        var adminRole = new IdentityRole("Admin");
                        roleManager.CreateAsync(adminRole).Wait();
                    }

                    // Create the default admin user
                    var adminUser = new ApplicationUser
                    {
                        UserName = "admin",
                        Email = "admin@example.com",
                        FullName = "Quản lý",
                        DateJoined = DateTimeOffset.Now,
                 
                        // Add other properties as needed
                    };

                    // Create the admin user with the default password "Abc@123"
                    var result = userManager.CreateAsync(adminUser, "Abc@123").Result;

                    if (result.Succeeded)
                    {
                        // Assign the "Admin" role to the admin user
                        userManager.AddToRoleAsync(adminUser, "Admin").Wait();
                    }

                    // Save changes to the database
                    dbContext.SaveChanges();
                }

                // seed RoomType & Room
                if (!dbContext.RoomTypes.Any())
                {           

                    dbContext.RoomTypes.AddRange(
                        new RoomType
                        {
                            RoomTypeID = "6d3fdce6-8d34-43af-aee2-1f007d870d73",
                            Name = "Single",
                            // Capacity = 1,
                            Description = "Không quá 1 người, 24 m² (1 giường đơn)",
                            AreaInSquareMeters = 24,
                        
                            DailyPrice = 150,
                           
                        },
                        new RoomType
                        {
                            RoomTypeID = "3ec3a6dd-4f5d-415d-be92-9f113d50f755",
                            Name = "Couple",
                            // Capacity = 2,
                            Description = "Loại phòng dành cho couple (1 giường đôi, bonus cầu tuột)",
                            AreaInSquareMeters = 37,
                          
                            DailyPrice = 180,
                            
                        },
                        new RoomType
                        {
                            RoomTypeID = "f08fae14-144c-4226-bbee-7ebaac02c63e",
                            Name = "Deluxe Triple",
                            // Capacity = 3,
                            Description = "Dành cho 3 người lớn (2 giường đôi, 1 giường đơn)",
                            AreaInSquareMeters = 46,
                          
                            DailyPrice = 220,
                           
                        }
                     );
                    dbContext.Rooms.AddRange(
                        new Room
                        {
                            RoomNumber = "A101",
                            RoomTypeID = "6d3fdce6-8d34-43af-aee2-1f007d870d73", // Single Room
                            IsAvaiable = true
                        },
                        new Room
                        {
                            RoomNumber = "A201",
                            RoomTypeID = "3ec3a6dd-4f5d-415d-be92-9f113d50f755", // Couple Room
                            IsAvaiable = true
                        },
                        new Room
                        {
                            RoomNumber = "A301",
                            RoomTypeID = "f08fae14-144c-4226-bbee-7ebaac02c63e", // Deluxe Triple Room
                            IsAvaiable = true
                        }
                    );

                    // Save changes to the database
                    dbContext.SaveChanges();
                }
            }
            
        }
    }
}
