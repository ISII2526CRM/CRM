
namespace AppForSEII2526.API.DTOs.ReseñasDTOs
{
    public class DevicesReseñaDTO
    {
        public DevicesReseñaDTO(int id, string name, string brand, string color, int year, string model)
        {
            Id = id;
            Name = name;
            Brand = brand;
            Color = color;
            Year = year;
            Model = model;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Color { get; set; }
        public int Year { get; set; }
        public string Model { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is DevicesReseñaDTO dTO &&
                   Id == dTO.Id &&
                   Name == dTO.Name &&
                   Brand == dTO.Brand &&
                   Color == dTO.Color &&
                   Year == dTO.Year &&
                   Model == dTO.Model;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Brand, Color, Year, Model);
        }
    }


}
