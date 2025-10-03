using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII2526.API.Models
{
    [Index(nameof(NameModel), IsUnique = true)]
    public class Model
    {
        public Model()
        {
        }

        public Model(int id, string nameModel) : this(nameModel)
        {
            Id = id;
        }

        public Model(string nameModel)
        {
            NameModel = nameModel;
        }

        [Key]
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "Model name cannot be longer than 50 characters.")]
        [Required]
        public string NameModel { get; set; }

        // Relación uno a muchos: un Model puede tener muchos Devices
        public IList<Device> Devices { get; set; }
    }
}
