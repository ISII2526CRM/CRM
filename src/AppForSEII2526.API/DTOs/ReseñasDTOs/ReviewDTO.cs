using System;
using System.Collections.Generic;
using System.Linq; // <--- IMPRESCINDIBLE PARA SequenceEqual

namespace AppForSEII2526.API.DTOs.ReseñasDTOs
{
    public class ReviewDTO
    {
        public ReviewDTO(int reviewId, string username, string customerCountry, string reviewTitle, DateTime dateOfReview, IList<ReviewItemDTO> reviewItems)
        {
            ReviewId = reviewId;
            Username = username;
            CustomerCountry = customerCountry;
            ReviewTitle = reviewTitle;
            DateOfReview = dateOfReview;
            ReviewItems = reviewItems;
        }

        public ReviewDTO()
        {
            ReviewItems = new List<ReviewItemDTO>();
        }

        public int ReviewId { get; set; }
        public string Username { get; set; }
        public string CustomerCountry { get; set; }
        public string ReviewTitle { get; set; }
        public DateTime DateOfReview { get; set; }
        public IList<ReviewItemDTO> ReviewItems { get; set; } = new List<ReviewItemDTO>();

        public override bool Equals(object? obj)
        {
            return obj is ReviewDTO dTO &&
                   ReviewId == dTO.ReviewId && // <--- AÑADIDO: Faltaba comparar el ID
                   Username == dTO.Username &&
                   CustomerCountry == dTO.CustomerCountry &&
                   ReviewTitle == dTO.ReviewTitle &&
                   DateOfReview == dTO.DateOfReview &&
                   // CAMBIADO: Usamos SequenceEqual para comparar el CONTENIDO de la lista
                   (ReviewItems ?? new List<ReviewItemDTO>()).SequenceEqual(dTO.ReviewItems ?? new List<ReviewItemDTO>());
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ReviewId, Username, CustomerCountry, ReviewTitle, DateOfReview);
        }
    }
}


