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
    public class GhiChuDuAnController : Controller
    {
        private QLDAEntities db = new QLDAEntities();

        // GET: Admin/GhiChuDuAn
        public ActionResult Index()
        {
            var tbl_GhiChuDuAn = db.tbl_GhiChuDuAn.Include(t => t.tbl_DuAn);
            return View(tbl_GhiChuDuAn.ToList());
        }

        public ActionResult dsGhiChu(int id)
        {
            var items = db.tbl_GhiChuDuAn.Where(x => x.MaDuAn == id);
            return PartialView("_dsGhiChu", items);
        }

        [HttpPost]
        public ActionResult addGhiChu(int idDa, string chiTiet)
        {
            try
            {
                tbl_GhiChuDuAn myItem = new tbl_GhiChuDuAn();
                myItem.MaDuAn = idDa;
                myItem.ChiTiet = chiTiet;
                myItem.NgayGhiChu = DateTime.Now;
                db.tbl_GhiChuDuAn.Add(myItem);
                db.SaveChanges();
                return Json(new { message = true });

            }
            catch
            {
                return Json(new { message = false });
            }
        }

        [HttpPost, ActionName("deleteGhiChu")]
        public ActionResult deleteGhiChu(int maGhiChu)
        {
            if (maGhiChu == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            try
            {
                tbl_GhiChuDuAn ghichu = (from c in db.tbl_GhiChuDuAn
                                      where c.MaGhiChu == maGhiChu
                                         select c).FirstOrDefault();
                db.tbl_GhiChuDuAn.Remove(ghichu);
                db.SaveChanges();
                //da.dsNhanVien(myItem.MaDuAn);
                //da.dsNhanVienInDuAn(myItem.MaDuAn);
                return Json(new { message = true });

            }
            catch
            {
                return Json(new { message = false });
            }
        }


























        // GET: Admin/GhiChuDuAn/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_GhiChuDuAn tbl_GhiChuDuAn = db.tbl_GhiChuDuAn.Find(id);
            if (tbl_GhiChuDuAn == null)
            {
                return HttpNotFound();
            }
            return View(tbl_GhiChuDuAn);
        }

        // GET: Admin/GhiChuDuAn/Create
        public ActionResult Create()
        {
            ViewBag.MaDuAn = new SelectList(db.tbl_DuAn, "MaDuAn", "TenDuAn");
            return View();
        }

        // POST: Admin/GhiChuDuAn/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaGhiChu,ChiTiet,NgayGhiChu,MaDuAn")] tbl_GhiChuDuAn tbl_GhiChuDuAn)
        {
            if (ModelState.IsValid)
            {
                db.tbl_GhiChuDuAn.Add(tbl_GhiChuDuAn);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaDuAn = new SelectList(db.tbl_DuAn, "MaDuAn", "TenDuAn", tbl_GhiChuDuAn.MaDuAn);
            return View(tbl_GhiChuDuAn);
        }

        // GET: Admin/GhiChuDuAn/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_GhiChuDuAn tbl_GhiChuDuAn = db.tbl_GhiChuDuAn.Find(id);
            if (tbl_GhiChuDuAn == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaDuAn = new SelectList(db.tbl_DuAn, "MaDuAn", "TenDuAn", tbl_GhiChuDuAn.MaDuAn);
            return View(tbl_GhiChuDuAn);
        }

        // POST: Admin/GhiChuDuAn/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaGhiChu,ChiTiet,NgayGhiChu,MaDuAn")] tbl_GhiChuDuAn tbl_GhiChuDuAn)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_GhiChuDuAn).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaDuAn = new SelectList(db.tbl_DuAn, "MaDuAn", "TenDuAn", tbl_GhiChuDuAn.MaDuAn);
            return View(tbl_GhiChuDuAn);
        }

        // GET: Admin/GhiChuDuAn/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_GhiChuDuAn tbl_GhiChuDuAn = db.tbl_GhiChuDuAn.Find(id);
            if (tbl_GhiChuDuAn == null)
            {
                return HttpNotFound();
            }
            return View(tbl_GhiChuDuAn);
        }

        // POST: Admin/GhiChuDuAn/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_GhiChuDuAn tbl_GhiChuDuAn = db.tbl_GhiChuDuAn.Find(id);
            db.tbl_GhiChuDuAn.Remove(tbl_GhiChuDuAn);
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
