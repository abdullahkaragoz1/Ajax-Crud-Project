using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AjaxExample.Models;

namespace AjaxExample.Controllers
{
    public class lolcController : Controller
    {

        private DboKursListesiEntities db = new DboKursListesiEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetCourses()
        {

            using (DboKursListesiEntities db = new DboKursListesiEntities())
            {

                var courses = (from d1 in db.Tbl_Records
                               join d2 in db.Tbl_RecordsInfo on d1.recordId equals d2.recordId
                               select new
                               {
                                   d1.recordId,
                                   d1.CourseName,
                                   d1.recordDate,
                                   d2.InfoId,
                                   d2.fee,
                                   d2.date,

                               }).OrderByDescending(x => x.date.Value).ToList();
                return Json(new { data = courses }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCurrentCourses()
        {
            using (DboKursListesiEntities _db = new DboKursListesiEntities())
            {
                var courses = (from d1 in _db.Tbl_Records
                               select new
                               {
                                   recordId = d1.recordId,
                                   CourseName = d1.CourseName,
                                   recordDate = d1.recordDate

                               }).OrderByDescending(x => x.recordDate.Value).ToList();

                return Json(new { data = courses }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Edit(int Id)
        {

            //TODO BURADA KALDI 
            var course = db.Tbl_Records.Where(a => a.recordId == Id).FirstOrDefault();
            return Json(new { data = course }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Remove(int InfoId)
        {

            bool status = false;

            using (DboKursListesiEntities _db = new DboKursListesiEntities())
            {

                var record = _db.Tbl_RecordsInfo.FirstOrDefault(a => a.InfoId == InfoId);


                if (record != null)
                {
                    _db.Tbl_RecordsInfo.Remove(record);
                }
                _db.SaveChanges();
                status = true;
            }

            return new JsonResult { Data = new { status = status } };
        }

        public ActionResult Save(Tbl_Records rec)
        {
            bool status = false;

            if (ModelState.IsValid)
            {
                using (DboKursListesiEntities db = new DboKursListesiEntities())
                {

                    if (rec.recordId > 0)
                    {
                        var v = db.Tbl_Records.FirstOrDefault(a => a.recordId == rec.recordId);
                        if (v != null)
                        {
                            v.CourseName = rec.CourseName;
                            db.Entry(v).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        db.Tbl_Records.Add(rec);
                    }
                    db.SaveChanges();
                    status = true;

                }
            }
            return new JsonResult { Data = new { status } };
        }

        public JsonResult GetChart()
        {
            using (DboKursListesiEntities db = new DboKursListesiEntities())
            {

                var courses = (from d1 in db.Tbl_Records
                               join d2 in db.Tbl_RecordsInfo on d1.recordId equals d2.recordId
                               select new
                               {
                                   CourseName = d1.CourseName,
                                   fee = d2.fee
                               }).ToArray();
                return Json(courses, JsonRequestBehavior.AllowGet);

            }


        }


    }
}