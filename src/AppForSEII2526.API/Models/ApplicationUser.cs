using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
    }

    public ApplicationUser(string id, string name, string surname, string userName, IList<Purchase> purchases, IList<Rental> rentals, IList<Review> reviews)
    {
        Id = id;
        Name = name;
        Surname = surname;
        UserName = userName;
        Email = userName;
        Purchases = purchases;
        Rentals = rentals;
        Reviews = reviews;
    }

    [Display(Name = "Name")]
    public string? Name { get; set; }

    [Display(Name = "Surname")]
    public string? Surname { get; set; }

    public IList<Purchase> Purchases { get; set; } = new List<Purchase>();
    public IList<Rental> Rentals { get; set; } = new List<Rental>();
    public IList<Review> Reviews { get; set; } = new List<Review>();
}