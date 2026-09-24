using Evaluacion_SanchezCori.Validation;
using System.ComponentModel.DataAnnotations;

namespace Evaluacion_SanchezCori.Models
{
    public class Cita
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de la cita")]
        [FechaNoPasada]
        public DateTime FechaCita { get; set; } = DateTime.Today;

        public EstadoCita Estado { get; set; }
            = EstadoCita.Pendiente;

        [Required(ErrorMessage = "Seleccione una mascota")]
        [Display(Name = "Mascota")]
        public int MascotaId { get; set; }

        public Mascota? Mascota { get; set; }

        [Required(ErrorMessage = "Seleccione un servicio")]
        [Display(Name = "Servicio veterinario")]
        public int ServicioVeterinarioId { get; set; }

        public ServicioVeterinario? ServicioVeterinario { get; set; }
    }
}