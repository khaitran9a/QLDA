using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLDA.Models;

namespace QLDA.Controllers
{
    public class NhanVienThamGiaCongViecController : Controller
    {
        private QLDAEntities db = new QLDAEntities();

        // GET: NhanVienThamGiaCongViec
        public ActionResult Index()
        {
            var tbl_NhanVienThamGiaCongViec = db.tbl_NhanVienThamGiaCongViec.Include(t => t.tbl_CongViec).Include(t => t.tbl_NhanVien);
            return View(tbl_NhanVienThamGiaCongViec.ToList());
        }

        
        [HttpPost]
        public ActionResult addNhanVien(int idNv, int maCongViec)
        {
            try
            {
                tbl_NhanVienThamGiaCongViec myItem = new tbl_NhanVienThamGiaCongViec();
                myItem.MaCongViec = maCongViec;
                myItem.MaNV = idNv;
                myItem.NgayThem = DateTime.Now;
                db.tbl_NhanVienThamGiaCongViec.Add(myItem);
                db.SaveChanges();
                return Json(new { message = true });
            }
            catch
            {
                return Json(new { message = false });
            }
        }

        public ActionResult dsNhanVienTrongCongViec(int id = 0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var query1 = db.tbl_CongViec.Where(y => y.MaCongViec == id)
                          .Select(x => x.MaDuAn);
            var query2 = db.tbl_ThamGiaDuAn.Where(y => query1.Contains(y.MaDuAn))
                          .Select(x => x.MaNV);
            //var query3 = db.tbl_NhanVien.Where(y => query2.Contains(y.MaNV))
            //               .Select(x => x.MaNV);
            var query4 = db.tbl_NhanVienThamGiaCongViec.Where(y => y.MaCongViec == id)
                            .Select(x => x.MaNV);
            var items = db.tbl_NhanVien.Where(x => query2.Contains(x.MaNV) && query4.Contains(x.MaNV));

            int ip = items.Count();

            return PartialView("_dsNhanVienCongViec", items);
        }





        // GET: NhanVienThamGiaCongViec/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_NhanVienThamGiaCongViec tbl_NhanVienThamGiaCongViec = db.tbl_NhanVienThamGiaCongViec.Find(id);
            if (tbl_NhanVienThamGiaCongViec == null)
            {
                return HttpNotFound();
            }
            return View(tbl_NhanVienThamGiaCongViec);
        }

        // GET: NhanVienThamGiaCongViec/Create
        public ActionResult Create()
        {
            ViewBag.MaCongViec = new SelectList(db.tbl_CongViec, "MaCongViec", "MucTieu");
            ViewBag.MaNV = new SelectList(db.tbl_NhanVien, "MaNV", "Ho");
            return View();
        }

        // POST: NhanVienThamGiaCongViec/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaCongViec,MaNV,NgayThem,TrangThai,isManager")] tbl_NhanVienThamGiaCongViec tbl_NhanVienThamGiaCongViec)
        {
            if (ModelState.IsValid)
            {
                db.tbl_NhanVienThamGiaCongViec.Add(tbl_NhanVienThamGiaCongViec);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaCongViec = new SelectList(db.tbl_CongViec, "MaCongViec", "MucTieu", tbl_NhanVienThamGiaCongViec.MaCongViec);
            ViewBag.MaNV = new SelectList(db.tbl_NhanVien, "MaNV", "Ho", tbl_NhanVienThamGiaCongViec.MaNV);
            return View(tbl_NhanVienThamGiaCongViec);
        }

        // GET: NhanVienThamGiaCongViec/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_NhanVienThamGiaCongViec tbl_NhanVienThamGiaCongViec = db.tbl_NhanVienThamGiaCongViec.Find(id);
            if (tbl_NhanVienThamGiaCongViec == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaCongViec = new SelectList(db.tbl_CongViec, "MaCongViec", "MucTieu", tbl_NhanVienThamGiaCongViec.MaCongViec);
            ViewBag.MaNV = new SelectList(db.tbl_NhanVien, "MaNV", "Ho", tbl_NhanVienThamGiaCongViec.MaNV);
            return View(tbl_NhanVienThamGiaCongViec);
        }

        // POST: NhanVienThamGiaCongViec/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaCongViec,MaNV,NgayThem,TrangThai,isManager")] tbl_NhanVienThamGiaCongViec tbl_NhanVienThamGiaCongViec)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_NhanVienThamGiaCongViec).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaCongViec = new SelectList(db.tbl_CongViec, "MaCongViec", "MucTieu", tbl_NhanVienThamGiaCongViec.MaCongViec);
            ViewBag.MaNV = new SelectList(db.tbl_NhanVien, "MaNV", "Ho", tbl_NhanVienThamGiaCongViec.MaNV);
            return View(tbl_NhanVienThamGiaCongViec);
        }

        // GET: NhanVienThamGiaCongViec/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_NhanVienThamGiaCongViec tbl_NhanVienThamGiaCongViec = db.tbl_NhanVienThamGiaCongViec.Find(id);
            if (tbl_NhanVienThamGiaCongViec == null)
            {
                return HttpNotFound();
            }
            return View(tbl_NhanVienThamGiaCongViec);
        }

        // POST: NhanVienThamGiaCongViec/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_NhanVienThamGiaCongViec tbl_NhanVienThamGiaCongViec = db.tbl_NhanVienThamGiaCongViec.Find(id);
            db.tbl_NhanVienThamGiaCongViec.Remove(tbl_NhanVienThamGiaCongViec);
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
