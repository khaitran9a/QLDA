using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using QLDA.Models;

namespace QLDA.Controllers
{
    public class CongViecController : Controller
    {
        private QLDAEntities db = new QLDAEntities();

        // GET: CongViec
        public ActionResult Index()
        {
            var tbl_CongViec = db.tbl_CongViec.Include(t => t.tbl_DuAn);
            return View(tbl_CongViec.ToList());
        }
        public ActionResult dsCongViec(int id)
        {
            var items = db.tbl_CongViec.Where(x => x.MaDuAn == id && x.TrangThai == true);
            return PartialView("_dsCongViec", items);
        }


        public ActionResult dsNhanVienTrongCongViec(int id = 0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var query1 = db.tbl_NhanVienThamGiaCongViec.Where(y => y.MaCongViec == id)
                          .Select(x => x.MaNV);
            int dem1 = query1.Count();

            var maDuAn = db.tbl_CongViec.Where(cv => cv.MaDuAn == id).Select(cv => cv.MaDuAn).FirstOrDefault();
        
            var query2 = db.tbl_ThamGiaDuAn.Where(y => query1.Contains(y.MaDuAn))
                           .Select(x => x.MaNV);
            int dem2 = query1.Count();
            var items = db.tbl_NhanVien.Where(x => query1.Contains(x.MaNV));
            int dem = items.Count();

            return PartialView("_dsNhanVienTrongCongViec", items);
        }



        [HttpPost]
        public ActionResult HoanThanh(int MaCongViec)
        {
            if (MaCongViec == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                tbl_CongViec congViec = (from c in db.tbl_CongViec
                                         where c.MaCongViec == MaCongViec
                                         select c).FirstOrDefault();
                congViec.TienDo = 100;
                db.Entry(congViec).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { message = true });

            }
            catch
            {
                return Json(new { message = false });
            }
        }



        [HttpPost]
        public ActionResult daLamDuoc(int MaCongViec, float daLam)
        {
            if (MaCongViec == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                tbl_CongViec congViec = (from c in db.tbl_CongViec
                                         where c.MaCongViec == MaCongViec
                                         select c).FirstOrDefault();
                congViec.TienDo = daLam/congViec.ThoiGianHoanThanh * 100;
                congViec.ThoiGianHoanThanh = congViec.ThoiGianHoanThanh - daLam;
                
                db.Entry(congViec).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { message = true });

            }
            catch
            {
                return Json(new { message = false });
            }
        }


        [HttpPost]
        public ActionResult HoanThanhMotPhan(int MaCongViec)
        {
            if (MaCongViec == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                tbl_CongViec congViec = (from c in db.tbl_CongViec
                                         where c.MaCongViec == MaCongViec
                                         select c).FirstOrDefault();
                congViec.TienDo = 100;
                db.Entry(congViec).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { message = true });

            }
            catch
            {
                return Json(new { message = false });
            }
        }


        [HttpPost]
        public ActionResult addCongViecCon(int maDa, int pID, string mucTieu, string chiTiet, float tg, int tienDo = 0)
        {

            try
            {
                tbl_CongViec myItem = new tbl_CongViec();
                myItem.MaDuAn = maDa;
                myItem.ParentID = pID;
                myItem.MucTieu = mucTieu;
                myItem.ChiThietCongViec = chiTiet;
                myItem.ThoiGianHoanThanh = tg;
                myItem.NgayTao = DateTime.Now;
                myItem.TrangThai = true;
                myItem.TienDo = tienDo;
                db.tbl_CongViec.Add(myItem);
                db.SaveChanges();
                return Json(new { message = true , maCongviec = myItem.MaCongViec});

            }
            catch
            {
                return Json(new { message = false });
            }
        }


        public ActionResult dsCongViecCon(int id)
        {
            var items = db.tbl_CongViec.Where(x => x.ParentID == id && x.TrangThai == true);
            return PartialView("_dsCongViecCon", items);
        }


        // GET: CongViec/Details/5
        public ActionResult ChiTiet(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_CongViec tbl_CongViec = db.tbl_CongViec.Find(id);
            if (tbl_CongViec == null)
            {
                return HttpNotFound();
            }
            return View(tbl_CongViec);
        }











        // GET: CongViec/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_CongViec tbl_CongViec = db.tbl_CongViec.Find(id);
            if (tbl_CongViec == null)
            {
                return HttpNotFound();
            }
            return View(tbl_CongViec);
        }

        // GET: CongViec/Create
        public ActionResult Create()
        {
            ViewBag.MaDuAn = new SelectList(db.tbl_DuAn, "MaDuAn", "TenDuAn");
            return View();
        }

        // POST: CongViec/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaCongViec,MaDuAn,MucTieu,ThoiGianHoanThanh,ChiThietCongViec,TienDo,NgayTao,TrangThai,ParentID")] tbl_CongViec tbl_CongViec)
        {
            if (ModelState.IsValid)
            {
                db.tbl_CongViec.Add(tbl_CongViec);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaDuAn = new SelectList(db.tbl_DuAn, "MaDuAn", "TenDuAn", tbl_CongViec.MaDuAn);
            return View(tbl_CongViec);
        }

        // GET: CongViec/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_CongViec tbl_CongViec = db.tbl_CongViec.Find(id);
            if (tbl_CongViec == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaDuAn = new SelectList(db.tbl_DuAn, "MaDuAn", "TenDuAn", tbl_CongViec.MaDuAn);
            return View(tbl_CongViec);
        }

        // POST: CongViec/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaCongViec,MaDuAn,MucTieu,ThoiGianHoanThanh,ChiThietCongViec,TienDo,NgayTao,TrangThai,ParentID")] tbl_CongViec tbl_CongViec)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_CongViec).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaDuAn = new SelectList(db.tbl_DuAn, "MaDuAn", "TenDuAn", tbl_CongViec.MaDuAn);
            return View(tbl_CongViec);
        }

        // GET: CongViec/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_CongViec tbl_CongViec = db.tbl_CongViec.Find(id);
            if (tbl_CongViec == null)
            {
                return HttpNotFound();
            }
            return View(tbl_CongViec);
        }

        // POST: CongViec/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_CongViec tbl_CongViec = db.tbl_CongViec.Find(id);
            db.tbl_CongViec.Remove(tbl_CongViec);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
