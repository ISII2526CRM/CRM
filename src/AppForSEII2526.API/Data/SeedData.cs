using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models; 
using System;
using System.Collections.Generic;
using System.Linq;
using AppForSEII2526.API.Data;   
using AppForSEII2526.API.Models; 

namespace AppForSEII2526.API.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider, ILogger logger)
        {
            // Obtenemos el contexto de base de datos
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // 1. CREAR ROLES
            List<string> rolesNames = new List<string> { "Administrator", "Customer" };
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            try
            {
                SeedRoles(roleManager, rolesNames);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al crear los roles.");
            }

            // 2. CREAR USUARIOS (Alice)
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            try
            {
                SeedUsers(userManager, rolesNames);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al crear los usuarios.");
            }

            // 3. CREAR DISPOSITIVOS Y MODELOS
            try
            {
                SeedModelsAndDevices(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al crear Modelos y Dispositivos.");
            }
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager, List<string> roles)
        {
            foreach (string roleName in roles)
            {
                if (!roleManager.RoleExistsAsync(roleName).Result)
                {
                    IdentityRole role = new IdentityRole();
                    role.Name = roleName;
                    role.NormalizedName = roleName.ToUpper();
                    var result = roleManager.CreateAsync(role).Result;
                }
            }
        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles)
        {
            // Creamos a Alice (Customer)
            if (userManager.FindByNameAsync("alice@test.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = "alice@test.com",
                    Email = "alice@test.com",
                    Name = "Alice",
                    Surname = "Wonderland",
                    Address = "Calle Falsa 123",
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(user, "Password123!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Customer").Wait();
                }
            }

            // Creamos un Admin (Opcional, por si lo necesitas)
            if (userManager.FindByNameAsync("admin@test.com").Result == null)
            {
                ApplicationUser admin = new ApplicationUser
                {
                    UserName = "admin@test.com",
                    Email = "admin@test.com",
                    Name = "Admin",
                    Surname = "System",
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(admin, "Password123!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(admin, "Administrator").Wait();
                }
            }
        }

        public static void SeedModelsAndDevices(ApplicationDbContext dbcontext)
        {
            // 1. Crear un Modelo base (Tu tabla Device requiere un ModelId)
            Model standardModel;
            var existingModel = dbcontext.Model.FirstOrDefault(m => m.NameModel == "Standard");

            if (existingModel == null)
            {
                standardModel = new Model { NameModel = "Standard" };
                dbcontext.Model.Add(standardModel);
                // Guardamos para que genere el ID y poder usarlo en los dispositivos
                dbcontext.SaveChanges();
            }
            else
            {
                standardModel = existingModel;
            }

            dbcontext.SaveChanges();
        }
    }
}