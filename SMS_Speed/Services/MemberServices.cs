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
    class MemberServices
    {
        public MemberServices() { }
        public List<MemberDTO>  GetMemberByBirthday(string month)
        {

            try
            {
                string query = @"Select FirstName, LastName,BirthDate,HomeTele from Member where substr(BirthDate,6,2) = '"+month+"' and HomeTele is not null";
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
                               FirstName = (dr["FIRSTNAME"] != null && dr["FIRSTNAME"].ToString() != "null") ? dr["FIRSTNAME"].ToString() : "",
                               LastName = (dr["LASTNAME"] != null && dr["LASTNAME"].ToString() != "null") ? dr["LASTNAME"].ToString() : "",
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
        public List<MemberDTO> GetMemberLastVisit(string startDate, string endDate)
        {
            try
            {
                string query = @"Select FIRSTNAME, LASTNAME, ANNIVER, HOMETELE, CurPoints, LastTrans, LastTrans2, LastTrans3
                            From Member
                            Where (LASTVISIT BETWEEN CONVERT(DATE, '" + startDate + @"',365) AND CONVERT(DATE, '" + endDate + @"')) 
                                  and (LastTrans is not null or LastTrans2 is not null or LastTrans3 is not null)
                                    and HOMETELE IS NOT NULL
                                    and ISACTIVE = 1;";
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
                               FirstName = (dr["FIRSTNAME"] != null && dr["FIRSTNAME"].ToString() != "null") ? dr["FIRSTNAME"].ToString() : "",
                               LastName = (dr["LASTNAME"] != null && dr["LASTNAME"].ToString() != "null") ? dr["LASTNAME"].ToString() : "",
                               BirthDate = dr["BirthDate"].ToString(),
                               HomeTele = dr["HomeTele"].ToString(),
                               Anniver = DateTime.Parse(dr["ANNIVER"].ToString()),
                               CurPoints = Convert.ToInt16(dr["CurPoints"]),
                               LastTrans = Convert.ToInt32(dr["LastTrans"]),
                               LastTrans2 = Convert.ToInt32(dr["LastTrans2"]),
                               LastTrans3 = Convert.ToInt32(dr["LastTrans2"])
                           }).ToList();
                return members;
            }
            catch
            {
                return null;
            }
        }
    }

    
}
