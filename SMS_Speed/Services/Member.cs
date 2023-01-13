using SMS_Speed.DTO;
using SMS_Speed.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_Speed.Services
{
    class Member
    {
        public List<MemberDTO>  getMemberByBirthday(string startDate, string endDate)
        {

            try
            {
                string query = @"Select FirstName, LastName,BirthDate,HomeTele from Member where birthdate BETWEEN '" + startDate + @"' and '" + endDate + @"'";
                DBConenection.getInstance().Open();


                OdbcCommand cmd = new OdbcCommand(query, DBConenection.getInstance());
                OdbcDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                DBConenection.getInstance().Close();

                List<MemberDTO> members = new List<MemberDTO>();
                members = (from DataRow dr in dt.Rows
                           select new MemberDTO()
                           {
                               FirstName = dr["FirstName"].ToString(),
                               LastName = dr["LastName"].ToString(),
                               BirthDate = dr["BirthDate"].ToString(),
                               HomeTele = dr["HomeTele"].ToString()
                           }).ToList();
                List<MemberDTO> customers = members.Where(w => w.HomeTele != "").Select(s => s).ToList();
                return customers;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
