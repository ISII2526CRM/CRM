namespace AppForSEII2526.API.DTOs.ReseñasDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

    public class ReviewDetailsDTO
    {
    public ReviewDetailsDTO(string username, string customerCountry, string reviewTitle, DateTime dateOfReview, IList<ReviewItemDetailsDTO> reviewItems)
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
        public IList<ReviewItemDetailsDTO> ReviewItems { get; set; } = new List<ReviewItemDetailsDTO>();
    }



