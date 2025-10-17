namespace AppForSEII2526.API.DTOs.ReseñasDTOs
{
    public class DevicesReseñaDTO
    {
        public DevicesReseñaDTO(int id, string brand, string color, int year, string model)
        {
            Id = id;
            Brand = brand;
            Color = color;
            Year = year;
            Model = model;
        }

        public int Id { get; set; }
        public string Brand { get; set; }
        public string Color { get; set; }
        public int Year { get; set; }
        public string Model { get; set; }
    }


}
