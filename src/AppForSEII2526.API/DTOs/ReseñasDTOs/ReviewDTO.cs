namespace AppForSEII2526.API.DTOs.ReseñasDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

    public class ReviewItemDetailsDTO
    {
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public int DeviceYear { get; set; }
        public int Rating { get; set; } 
        public string? Comment { get; set; } 
    }

    
    public class ReviewDetailsDTO
    {
        public string Username { get; set; } 
        public string CustomerCountry { get; set; }
        public string ReviewTitle { get; set; } 
        public DateTime DateOfReview { get; set; } 
        public IList<ReviewItemDetailsDTO> ReviewItems { get; set; } = new List<ReviewItemDetailsDTO>();
    }



