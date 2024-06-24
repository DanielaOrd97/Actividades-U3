using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace U3Cliente.Models
{
    public class ActividadesDTO
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = "";

        public string? Descripcion { get; set; } = "";

        public DateOnly? FechaRealizacion { get; set; }

        public int IdDepartamento { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaActualizacion { get; set; }

        public int Estado { get; set; }

        public string EstadoString
        {
            get
            {
                switch (Estado)
                {
                    case 0: return "Borrador";
                    case 1: return "Publicado";
                    case 2: return "Eliminado";
                    default:
                        return "Error";

                }
            }
        }
        public string Evidencia { get; set; } = "";
    }

    public class ActividadesSubordinadas
    {
        public int IdDepartamento { get; set; }
        public string NombreDepartamento { get; set; } = "";
        public List<ActividadesDTO> Actividades { get; set; } = new();
    }

    public class MisActividadesYSubordinados
    {
        public List<ActividadesSubordinadas> ActividadesSubordinadas { get; set; } = new();
    }
}
