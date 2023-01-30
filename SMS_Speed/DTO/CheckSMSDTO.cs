using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_Speed.DTO
{
    internal class CheckSMSDTO
    {
        public int CodeResponse { get; set; }
        public string SMSID { get; set; }
        public int SendFailed { get; set; }
        public int SendStatus { get; set; }
        public int SendSuccess { get; set; }
        public double TotalPrice { get; set; }
        public int TotalReceiver { get; set; }
        public int TotalSent { get; set; }
    }
}
