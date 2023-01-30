using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_Speed.DTO
{
    internal class BrandnameDTO
    {
        public string BrandName { get; set; }
        public int Type { get; set; }
    }

    internal class BrandnameResponseDTO
    {
        public int CodeResponse { get; set; }

        public List<BrandnameDTO> ListBrandname { get; set; } = new List<BrandnameDTO>();

    }
}
