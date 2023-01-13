using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_Speed.DTO
{
    internal class TemplateDTO
    {
        public List<Template> BrandnameTemplates;
    }
    public class Template{
        public string NetworkID;
        public string TempContent;
        public string TempId;
    }
}
