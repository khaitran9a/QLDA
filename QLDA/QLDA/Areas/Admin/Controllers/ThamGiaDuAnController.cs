using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLDA.Models;

namespace QLDA.Areas.Admin.Controllers
{
    public class ThamGiaDuAnController : Controller
    {
        private QLDAEntities db = new QLDAEntities();

        // GET: Admin/ThamGiaDuAn
        public ActionResult Index()
        {
            var tbl_ThamGiaDuAn = db.tbl_ThamGiaDuAn.Include(t => t.tbl_DuAn).Include(t => t.tbl_NhanVien);
            return View(tbl_ThamGiaDuAn.ToList());
        }




        [HttpGet]
        public JsonResult DsThamGia(string tuKhoa)
        {
            try
            {
                var dsThamGia = (from tg in db.tbl_ThamGiaDuAn
                              .Where(x => x.tbl_DuAn.TenDuAn.Contains(tuKhoa) || x.tbl_NhanVien.Ten.Contains(tuKhoa))
                              select new
                              {
                                  MaDuAn = tg.tbl_DuAn.MaDuAn,
                                  MaNhanVien = tg.tbl_NhanVien.MaNV,
                                  TenDuAN = tg.tbl_DuAn.TenDuAn,
                                  TenNhanVien = tg.tbl_NhanVien.Ho + " " + tg.tbl_NhanVien.Ten,
                                 
                                  NgayThamGia = tg.NgayThamGia.ToString(),
                                 
                GhiChu = tg.GhiChu
                              }).ToList();
                return Json(new { code = 200, dsThamGia = dsThamGia, msg = "Lấy danh sách thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy danh sách thất bại" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        //// GET: Admin/ThamGiaDuAn/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    tbl_ThamGiaDuAn tbl_ThamGiaDuAn = db.tbl_ThamGiaDuAn.Find(id);
        //    if (tbl_ThamGiaDuAn == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(tbl_ThamGiaDuAn);
        //}

        // GET: Admin/ThamGiaDuAn/Create
        [HttpGet]
        public ActionResult Create(int id)
        {
            tbl_DuAn da = db.tbl_DuAn.SingleOrDefault(a => a.MaDuAn == id);
            ViewBag.TenDuAn = da.TenDuAn;
            GlobalData.Ma = Convert.ToInt32(id); 
            ViewBag.MaNV = new SelectList(db.tbl_NhanVien, "MaNV", "Ho");
            return View();
        }

        // POST: Admin/ThamGiaDuAn/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tbl_ThamGiaDuAn thamgia, int? MaNV)
        {
            thamgia.NgayThamGia = DateTime.Now;
            thamgia.MaNV = Convert.ToInt32(MaNV);


            thamgia.MaDuAn = GlobalData.Ma;
            if (ModelState.IsValid)
            {
                
                db.tbl_ThamGiaDuAn.Add(thamgia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaDuAn = new SelectList(db.tbl_DuAn, "MaDuAn", "TenDuAn", thamgia.MaDuAn);
            ViewBag.MaNV = new SelectList(db.tbl_NhanVien, "MaNV", "Ho", thamgia.MaNV);
            return View(thamgia);
        }

        //// GET: Admin/ThamGiaDuAn/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    tbl_ThamGiaDuAn tbl_ThamGiaDuAn = db.tbl_ThamGiaDuAn.Find(id);
        //    if (tbl_ThamGiaDuAn == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.MaDuAn = new SelectList(db.tbl_DuAn, "MaDuAn", "TenDuAn", tbl_ThamGiaDuAn.MaDuAn);
        //    ViewBag.MaNV = new SelectList(db.tbl_NhanVien, "MaNV", "Ho", tbl_ThamGiaDuAn.MaNV);
        //    return View(tbl_ThamGiaDuAn);
        //}

        //// POST: Admin/ThamGiaDuAn/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "MaNV,MaDuAn,NgayThamGia,GhiChu")] tbl_ThamGiaDuAn tbl_ThamGiaDuAn)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(tbl_ThamGiaDuAn).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.MaDuAn = new SelectList(db.tbl_DuAn, "MaDuAn", "TenDuAn", tbl_ThamGiaDuAn.MaDuAn);
        //    ViewBag.MaNV = new SelectList(db.tbl_NhanVien, "MaNV", "Ho", tbl_ThamGiaDuAn.MaNV);
        //    return View(tbl_ThamGiaDuAn);
        //}

        //// GET: Admin/ThamGiaDuAn/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    tbl_ThamGiaDuAn tbl_ThamGiaDuAn = db.tbl_ThamGiaDuAn.Find(id);
        //    if (tbl_ThamGiaDuAn == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(tbl_ThamGiaDuAn);
        //}

        //// POST: Admin/ThamGiaDuAn/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    tbl_ThamGiaDuAn tbl_ThamGiaDuAn = db.tbl_ThamGiaDuAn.Find(id);
        //    db.tbl_ThamGiaDuAn.Remove(tbl_ThamGiaDuAn);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
