using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PruebaBizagi.ViewModels
{
    public class FormViewModel
    {
        public string name { get; set; }
        public string nombres { get; set; }

        public string apellidos { get; set; }

        public string cedula { get; set; }

        public string direccion1 { get; set; }

        public string direccion2 { get; set; }

        public string direccion3 { get; set; }

        public string telefonoResidencial { get; set; }

        public string telefonoCelular { get; set; }

        public string telefonoOficina { get; set; }

        public string correoElectronico { get; set; }

        public List<SelectListItem> fuenteIngreso { get; set; }
        public int fuenteIngresoId { get; set; }

        public decimal ingresosMensuales { get; set; }

        public List<SelectListItem> tipoVivienda { get; set; }
        public int tipoViviendaId { get; set; }

        public string lugarTrabajo { get; set; }

        public string posicionActual { get; set; }

        public int anoServicios { get; set; }

        public List<SelectListItem> tipoVehiculo { get; set; }
        public int tipoVehiculoId { get; set; }

        public List<SelectListItem> marca { get; set; }
        public int marcaId { get; set; }

        public List<SelectListItem> modelo { get; set; }
        public int modeloId { get; set; }

        public List<SelectListItem> condicion { get; set; }
        public int condicionId { get; set; }

        public int anoVehiculo { get; set; }

        public List<SelectListItem> periodoTasaFija { get; set; }
        public int periodoTasaFijaId { get; set; }

        public List<SelectListItem> plazo { get; set; }
        public int plazoId { get; set; }

        public decimal montoSolicitado { get; set; }

        public string valorVehiculo { get; set; }

        public string isOkRequest { get; set; }
        public string msjRequest { get; set; }

    }
}