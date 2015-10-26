using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using LinqToDB;
using System.Dynamic;
using Utilities;

namespace Services
{
    public class ManagerService
    {
        public int create(string name, string phone , DateTime birth)
        {            
            var db = new DrugDBDB();            
            int res = (int)db.BenhNhans.InsertWithIdentity(() => new BenhNhan()
            {
                Name = name,
                Phone = phone,
                BirthDay = birth
            });
            return res;
        }
        public List<BenhNhan> GetBenhNhanByName(string[] names)
        {
            List<BenhNhan> list = new List<BenhNhan>();
            var db = new DrugDBDB();
            foreach (string name in names)
            {
                var bns = db.BenhNhans.Where(p => p.Name.ToLower().Contains(name.ToLower())).ToList();
                list.AddRange(bns);
            }
            list = list.Distinct(new GenericCompare<BenhNhan>(p=>p.ID)).ToList();
            return list;
        }
        public BenhNhan GetBenhNhanByID(int id)
        {
            var db = new DrugDBDB();
            BenhNhan bn = db.BenhNhans.FirstOrDefault(p => p.ID == id);
            return bn;
        }

        public int Update(DrugHistory dh)
        {
            int res = 0;
            var db = new DrugDBDB();
            if (dh.ID != 0)
            {
                res = (int)db.DrugHistories.Where(p=>p.ID == dh.ID).Update(t => new DrugHistory()
                {
                    DrugName = dh.DrugName,
                    BenhNhanID = dh.BenhNhanID,
                    Count = dh.Count,
                    Unit = dh.Unit,
                    Price = dh.Price,
                    Date = dh.Date
                });
                if (res == 1)
                {
                    res = dh.ID;
                }
            }
            else
            {
                 res = (int)db.DrugHistories.InsertWithIdentity(() => new DrugHistory()
                            {
                                DrugName = dh.DrugName,
                                BenhNhanID = dh.BenhNhanID,
                                Count = dh.Count,
                                Unit = dh.Unit,
                                Price = dh.Price,
                                Date = dh.Date
                            });

            }
            
            return res;
        }

        public dynamic GetDrugHistoryDatetimeByBenhNhan(int benhnhanid)
        {
            var db = new DrugDBDB();
            var list = db.DrugHistories.Where(p => p.BenhNhanID == benhnhanid)
                                        //.Distinct(new GenericCompare<DrugHistory>(p=>p.DrugName.ToLower()))
                                        .OrderByDescending(p => p.Date).ToList();
            var group = list.GroupBy(p => new { p.Date.Value.Year, p.Date.Value.Month, p.Date.Value.Day})
                            .Select(g => new
                            {
                                BenhNhanID = benhnhanid,
                                DateTime = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),                                
                                Count = g.Count()
                            });
            dynamic dyn = new ExpandoObject();
            dyn.Group = group.ToList();

            return dyn;
        }

        public List<DrugHistory> GetDrugHistories(int benhnhanid, DateTime datetime)
        {
            var db = new DrugDBDB();
            List<DrugHistory> list = db.DrugHistories.Where(p => p.BenhNhanID == benhnhanid
                                                    && p.Date.Value.Year == datetime.Year
                                                    && p.Date.Value.Month == datetime.Month
                                                    && p.Date.Value.Day == datetime.Day
            ).ToList();
            return list;
        }

        public List<BenhNhan> GetBenhNhanById(string id)
        {
            var db = new DrugDBDB();
            List<BenhNhan> list = db.BenhNhans.Where(p => p.ID.ToString().Contains(id)).ToList();                
            return list;
        }

        public List<BenhNhan> GetBenhNhanByDate(int day, int month, int year)
        {
            
            var db = new DrugDBDB();
            List<int> ids = db.DrugHistories.Where(p => (p.Date.Value.Day == day || day == 0)
                                                       && p.Date.Value.Month == month
                                                       && p.Date.Value.Year == year)
                                            .OrderBy(p=>p.Date)
                                            .Select(p=>p.BenhNhanID).ToList();
            List<BenhNhan> list = db.BenhNhans.Where(p => ids.Contains(p.ID)).ToList();
            return list;
        }

        public void DeleteDrugHistory(int id)
        {
            var db = new DrugDBDB();
            db.DrugHistories.Where(p => p.ID == id).Delete();
        }

        public List<BenhNhan> GetBenhNhanByPhone(string phone)
        {
            
            var db = new DrugDBDB();
            List<BenhNhan> list = db.BenhNhans.Where(p => p.Phone.Contains(phone)).ToList();     
            return list;
        }
    }
}
