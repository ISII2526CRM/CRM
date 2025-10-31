namespace AppForSEII2526.API.DTOs.ReseñasDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

    public class ReviewDTO
    {
    public ReviewDTO(string username, string customerCountry, string reviewTitle, DateTime dateOfReview, IList<ReviewItemDTO> reviewItems)
    {
        Username = username;
        CustomerCountry = customerCountry;
        ReviewTitle = reviewTitle;
        DateOfReview = dateOfReview;
        ReviewItems = reviewItems;
    }

    public string Username { get; set; } 
        public string CustomerCountry { get; set; }
        public string ReviewTitle { get; set; } 
        public DateTime DateOfReview { get; set; } 
        public IList<ReviewItemDTO> ReviewItems { get; set; } = new List<ReviewItemDTO>();

    public override bool Equals(object? obj)
    {
        return obj is ReviewDTO dTO &&
               Username == dTO.Username &&
               CustomerCountry == dTO.CustomerCountry &&
               ReviewTitle == dTO.ReviewTitle &&
               DateOfReview == dTO.DateOfReview &&
               EqualityComparer<IList<ReviewItemDTO>>.Default.Equals(ReviewItems, dTO.ReviewItems);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Username, CustomerCountry, ReviewTitle, DateOfReview, ReviewItems);
    }
}



