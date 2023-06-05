using BooksBot.API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BooksBot.API.Services
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly BooksBotContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DatabaseSeeder(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            BooksBotContext db,
            ILogger<DatabaseSeeder> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _logger = logger;
        }

        public void Initialize()
        {
            CreateRoles();
            AddAdministrator("ilqa.nawaz@arpatech.com", "ilqa");
            AddAdministrator("louis@booktech.co.uk", "louis");
            AddAdministrator("tom@booktech.co.uk", "tom");
            AddAdministrator("syd@booktech.co.uk", "syd");
            AddAdministrator("system@booktech.co.uk", "system");
          
            //_db.SaveChanges();
        }

        private void CreateRoles()
        {
            Task.Run(async () =>
            {
                var adminRoleInDb = await _roleManager.FindByNameAsync("Administrator");
                if (adminRoleInDb == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole() { Name = "Administrator" });
                    _logger.LogInformation("Seeded Administrator Role.");
                }
                var basicRoleInDb = await _roleManager.FindByNameAsync("Basic");
                if (basicRoleInDb == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole() { Name = "Basic" });
                    _logger.LogInformation("Seeded Client Role.");
                }

            }).GetAwaiter().GetResult();
        }

        private void AddAdministrator( string email, string userName)
        {
            Task.Run(async () =>
            {
                var adminRoleInDb = await _roleManager.FindByNameAsync("Administrator");

                //Check if User Exists
                var superUser = new IdentityUser()
                {
                    Email = email,
                    UserName = userName,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };
                var superUserInDb = await _userManager.FindByEmailAsync(superUser.Email);
                if (superUserInDb == null)
                {
                    await _userManager.CreateAsync(superUser, "Password@123");
                    var result = await _userManager.AddToRoleAsync(superUser, "Administrator");
                }
            }).GetAwaiter().GetResult();
        }

        //private void AddBasicUser()
        //{
        //    Task.Run(async () =>
        //    {
        //        //Check if Role Exists
        //        var basicRole = new ApplicationRole(RoleConstants.BasicRole, "Basic role with default permissions");
        //        var basicRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.BasicRole);
        //        if (basicRoleInDb == null)
        //        {
        //            await _roleManager.CreateAsync(basicRole);
        //            _logger.LogInformation("Seeded Basic Role.");
        //        }
        //        //Check if User Exists
        //        var basicUser = new ApplicationUser
        //        {
        //            FirstName = "John",
        //            LastName = "Doe",
        //            Email = "john@blazorhero.com",
        //            UserName = "johndoe",
        //            EmailConfirmed = true,
        //            PhoneNumberConfirmed = true,
        //            CreatedOn = DateTime.Now,
        //            IsActive = true
        //        };
        //        var basicUserInDb = await _userManager.FindByEmailAsync(basicUser.Email);
        //        if (basicUserInDb == null)
        //        {
        //            await _userManager.CreateAsync(basicUser, UserConstants.DefaultPassword);
        //            await _userManager.AddToRoleAsync(basicUser, RoleConstants.BasicRole);
        //            _logger.LogInformation("Seeded User with Basic Role.");
        //        }
        //    }).GetAwaiter().GetResult();
        //}
    }
}