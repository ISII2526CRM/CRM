using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.ReseñasDTOs
{
    // DTO usado tanto en POST (entrada) como en GET (salida).
    public class ReviewItemDTO
    {
        // Constructor sin parámetros requerido por System.Text.Json para deserializar.
        public ReviewItemDTO() { }

        // Constructor utilizado en la proyección GET (información completa del dispositivo).
        public ReviewItemDTO(string deviceName, string deviceModel, int deviceYear, int rating, string? comment)
        {
            DeviceName = deviceName ?? string.Empty;
            DeviceModel = deviceModel ?? string.Empty;
            DeviceYear = deviceYear;
            Rating = rating;
            Comment = comment;
        }

        // Constructor usado para recibir los campos mínimos en POST (DeviceId, Rating, Comment).
        public ReviewItemDTO(int deviceId, int rating, string comment)
        {
            DeviceId = deviceId;
            Rating = rating;
            Comment = comment;
        }

        // Campos obligatorios para la creación
        [Range(1, int.MaxValue, ErrorMessage = "DeviceId debe ser mayor que 0.")]
        public int DeviceId { get; set; }

        [Range(1, 5, ErrorMessage = "La puntuación debe estar entre 1 y 5.")]
        public int Rating { get; set; }
        public string? Comment { get; set; }

        // Campos informativos para la respuesta GET
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceModel { get; set; } = string.Empty;
        public int DeviceYear { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ReviewItemDTO dTO &&
                   DeviceId == dTO.DeviceId &&
                   Rating == dTO.Rating &&
                   Comment == dTO.Comment &&
                   DeviceName == dTO.DeviceName &&
                   DeviceModel == dTO.DeviceModel &&
                   DeviceYear == dTO.DeviceYear;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DeviceId, Rating, Comment, DeviceName, DeviceModel, DeviceYear);
        }
    }
}
