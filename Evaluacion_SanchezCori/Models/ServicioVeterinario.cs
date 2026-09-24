using System.ComponentModel.DataAnnotations;

namespace Evaluacion_SanchezCori.Models
{
    public class ServicioVeterinario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(80)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(300)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        [Range(1, 100000, ErrorMessage = "Ingrese un precio válido")]
        public decimal Precio { get; set; }

        public ICollection<Cita> Citas { get; set; }
            = new List<Cita>();
    }
}