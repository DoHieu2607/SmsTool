using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_Speed.DTO
{
    internal class TemplateDTO
    {
        public string NetworkID;
        public string TempContent;
        public string TempId;
        
    }
    internal class TemplateResponseDTO{
        public List<TemplateDTO> BrandnameTemplates { get; set; } = new List<TemplateDTO>();
        public int CodeResult { get; set; }
        public string ErrorMessage { get; set; }
    }
}
