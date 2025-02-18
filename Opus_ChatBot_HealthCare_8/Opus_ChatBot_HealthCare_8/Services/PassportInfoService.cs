using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Models.SupportModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using System.Net;
using System.IO;
using Opus_ChatBot_HealthCare_8.Models.MasterData;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class PassportInfoService : IPassportInfoService
    {
        private readonly ApplicationDbContext _contex;

        public PassportInfoService(ApplicationDbContext contex)
        {
            _contex = contex;
        }

        public async Task<int> SavePassportInfo(PassportInfo passportInfo)
        {
            try
            {
                if (passportInfo.Id != 0)
                {
                    _contex.passportInfos.Update(passportInfo);
                }
                else
                {
                    _contex.passportInfos.Add(passportInfo);
                }

                await _contex.SaveChangesAsync();
                return passportInfo.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<PassportInfo> GetPassportInfoByPasspoertIds(string id)
        {
            return await _contex.passportInfos.Where(x => x.passportNo == id || x.refNo == id).LastOrDefaultAsync();
        }
        public async Task<PassportInfo> GetPassportInfoByPasspoertid(int id)
        {
            return await _contex.passportInfos.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PassportInfo>> GetPassportInfo()
        {
            return await _contex.passportInfos.AsNoTracking().ToListAsync();
        }

        public async Task<bool> DeletePassportInfoById(int id)
        {
            _contex.passportInfos.Remove(_contex.passportInfos.Find(id));
            return 1 == await _contex.SaveChangesAsync();
        }
        public async Task<IEnumerable<ColumnHeading>> GetAllColumnBySp()
        {
            var data = _contex.columnHeadings.FromSql("sp_GetColumnName");
            return await data.ToListAsync();
        }

        #region PoliceClearenceLog

        public async Task<int> SavePoliceClearenceLog(PoliceClearenceLog passportInfo)
        {
            try
            {
                if (passportInfo.Id != 0)
                {
                    _contex.policeClearenceLogs.Update(passportInfo);
                }
                else
                {
                    _contex.policeClearenceLogs.Add(passportInfo);
                }

                await _contex.SaveChangesAsync();
                return passportInfo.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<PoliceClearenceLog>> GetPoliceClearenceLog()
        {
            return await _contex.policeClearenceLogs.AsNoTracking().ToListAsync();
        }



        public async Task<bool> GETApi(string data)
        {
            try
            {
                String strurl = String.Format("http://pcc.police.gov.bd:8080/ords/pcc2/hr/getapplicantinfo/" + data);
                WebRequest webRequest = WebRequest.Create(strurl);
                webRequest.Method = "GET";
                HttpWebResponse httpWebResponse = null;
                httpWebResponse = (HttpWebResponse)webRequest.GetResponse();

                string response = "";
                var pasportdata = new pasportdata();
                using (Stream stream = httpWebResponse.GetResponseStream())
                {
                    StreamReader streamReader = new StreamReader(stream);
                    response = streamReader.ReadToEnd();
                    streamReader.Close();
                }
                string[] values = new string[] { "a", "b", "c", "d" };
                string[] fchild = new string[] { "a", "b", "c", "d" };
                if (response != "")
                {

                    var datas = JObject.Parse(response);
                    var datass = datas["items"];
                    int datasss = datass.Count();
                    if (datasss <= 0)
                    {
                        return false;
                    }
                    values = datass.ToString().Split(",");
                    int i = 0;

                    string token_id = "";
                    string person = "";
                    string passport_no = "";
                    string issue_date = "";
                    DateTime? issue_dated = null;
                    string expire_date = "";
                    DateTime? expire_dated = null;
                    string apply_date = "";
                    DateTime? apply_dated = null;
                    string father_name = "";
                    string date_of_birth = "";
                    DateTime? date_of_birthd = null;
                    string mobile_no = "";
                    string current_status = "";
                    string current_contact = "";
                    string reason = "";
                    string remarks = "";
                    string expected_delivery_date = "";
                    DateTime? expected_delivery_dated = null;



                    foreach (string t in values)
                    {
                        fchild = t.Split(":");
                        if (fchild.Count()>1)
                        {


                            if (i == 0)
                            {
                                token_id = fchild[1];
                            }
                            if (i == 1)
                            {
                                person = fchild[1];
                            }
                            if (i == 2)
                            {
                                passport_no = fchild[1];
                            }
                            if (i == 3)
                            {
                                issue_date = fchild[1];
                                if (issue_date == "" || issue_date.Contains("null"))
                                {

                                }
                                else
                                {
                                    issue_dated = Convert.ToDateTime(issue_date.Replace("\"", "").Replace(" ", "").Replace("}", "").Replace("]", ""));
                                }
                            }
                            if (i == 4)
                            {
                                expire_date = fchild[1];
                                if (expire_date == "" || expire_date.Contains("null"))
                                {

                                }
                                else
                                {
                                    expire_dated = Convert.ToDateTime(expire_date.Replace("\"", "").Replace(" ", "").Replace("}", "").Replace("]", ""));
                                }
                            }
                            if (i == 5)
                            {
                                apply_date = fchild[1];
                                if (apply_date == "" || apply_date.Contains("null"))
                                {

                                }
                                else
                                {
                                    apply_dated = Convert.ToDateTime(apply_date.Replace("\"", "").Replace(" ", "").Replace("}", "").Replace("]", ""));
                                }
                            }
                            if (i == 6)
                            {
                                father_name = fchild[1];
                            }
                            if (i == 7)
                            {
                                date_of_birth = fchild[1];
                                if (date_of_birth == "" || date_of_birth.Contains("null"))
                                {

                                }
                                else
                                {
                                    date_of_birthd = Convert.ToDateTime(date_of_birth.Replace("\"", "").Replace(" ", "").Replace("}", "").Replace("]", ""));
                                }
                            }
                            if (i == 8)
                            {
                                mobile_no = fchild[1];
                            }
                            if (i == 9)
                            {
                                current_status = fchild[1];
                            }
                            if (i == 10)
                            {
                                current_contact = fchild[1];
                            }
                            if (i == 11)
                            {
                                reason = fchild[1];
                            }
                            if (i == 12)
                            {
                                remarks = fchild[1];
                            }
                            if (i == 13)
                            {
                                expected_delivery_date = fchild[1];
                                if (expected_delivery_date.Contains("null") || expected_delivery_date == "")
                                {

                                }
                                else
                                {
                                    expected_delivery_dated = Convert.ToDateTime(expected_delivery_date.Replace("\"", "").Replace(" ", "").Replace("}", "").Replace("]", ""));
                                }
                            }
                            i++;
                        }

                    }


                    PassportInfo passportInfo = new PassportInfo
                    {
                        refNo = token_id.Replace("\"", "").Replace("}", "").Replace("]", "").Replace(" ", ""),
                        passportNo = passport_no.Replace("\"", "").Replace("}", "").Replace("]", "").Replace(" ", "").Replace("No","Number Pending").Replace("no", "Number Pending"),
                        name = person.Replace("\"", "").Replace("}", "").Replace("]", ""),
                        fname = father_name.Replace("\"", "").Replace("}", "").Replace("]", ""),
                        dob =date_of_birthd, //Convert.ToDateTime(date_of_birth.Replace("\"", "").Replace(" ", "").Replace("}", "").Replace("]", "")),
                        expireDate =expire_dated, //Convert.ToDateTime(expire_date.Replace("\"", "").Replace(" ", "").Replace("}", "").Replace("]", "")),
                        issueDate =issue_dated, //Convert.ToDateTime(issue_date.Replace("\"", "").Replace(" ", "").Replace("}", "").Replace("]", "")),
                        mobile = mobile_no.Replace("\"", "").Replace("}", "").Replace("]", ""),
                        currentContact = current_contact.Replace("\"", "").Replace("}", "").Replace("]", ""),
                        expectedDeliveryDate =expected_delivery_dated ,//Convert.ToDateTime(expected_delivery_date.Replace("\"", "").Replace(" ", "").Replace("}", "").Replace("]", "")),
                        status = current_status.Replace("\"", "").Replace("}", "").Replace("]", ""),
                        applyDate=apply_dated,
                        remarks= remarks.Replace("\"", "").Replace("}", "").Replace("]", ""),
                        reason = reason.Replace("\"", "").Replace("}", "").Replace("]", ""),



                    };
                    _contex.passportInfos.Add(passportInfo);
                    await _contex.SaveChangesAsync();



                }

            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}
