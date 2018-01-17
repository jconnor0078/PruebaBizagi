using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaBizagi.BLL.Common
{
    public class PadronResponse
    {
        public string response { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string estado_civil { get; set; }
        public string sexo { get; set; }
        public string cod_nacionalidad { get; set; }
        public string cod_ocupacion { get; set; }
        public string fecha_nacimiento { get; set; }
        public string foto
        {
            get;
            set;
        }

        public byte[] fotobyte
        {
            get;
            set;
        }

    }
}
