namespace Evaluacion_SanchezCori.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalServicios { get; set; }

        public int TotalMascotas { get; set; }

        public int TotalUsuarios { get; set; }

        public int TotalCitas { get; set; }

        public List<string> Meses { get; set; }
            = new List<string>();

        public List<int> Cantidades { get; set; }
            = new List<int>();
    }
}