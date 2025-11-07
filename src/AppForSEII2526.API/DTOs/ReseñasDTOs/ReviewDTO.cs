namespace AppForSEII2526.API.DTOs.ReseñasDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

    public class ReviewDetailsDTO
    {
    

    public string Username { get; set; } 
        public string CustomerCountry { get; set; }
        public string ReviewTitle { get; set; } 
        public DateTime DateOfReview { get; set; } 
        public IList<ReviewItemDetailsDTO> ReviewItems { get; set; } = new List<ReviewItemDetailsDTO>();

    public override bool Equals(object? obj)
    {
        return obj is ReviewDetailsDTO dTO &&
               Username == dTO.Username &&
               CustomerCountry == dTO.CustomerCountry &&
               ReviewTitle == dTO.ReviewTitle &&
               DateOfReview == dTO.DateOfReview &&
               EqualityComparer<IList<ReviewItemDetailsDTO>>.Default.Equals(ReviewItems, dTO.ReviewItems);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Username, CustomerCountry, ReviewTitle, DateOfReview, ReviewItems);
    }
}



