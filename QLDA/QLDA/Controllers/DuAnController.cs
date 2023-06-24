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
    public class DuAnController : Controller
    {
        private QLDAEntities db = new QLDAEntities();

        // GET: DuAn
        public ActionResult Index()
        {
            return View(db.tbl_DuAn.ToList());
        }

        // GET: DuAn/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_DuAn tbl_DuAn = db.tbl_DuAn.Find(id);
            if (tbl_DuAn == null)
            {
                return HttpNotFound();
            }
            return View(tbl_DuAn);
        }

        // GET: DuAn/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DuAn/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaDuAn,TenDuAn,Thumbnail,KinhPhi,ThoiGianDuKien,NgayKhoiCong,TrangThaiDuAn")] tbl_DuAn tbl_DuAn)
        {
            if (ModelState.IsValid)
            {
                db.tbl_DuAn.Add(tbl_DuAn);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_DuAn);
        }

        // GET: DuAn/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_DuAn tbl_DuAn = db.tbl_DuAn.Find(id);
            if (tbl_DuAn == null)
            {
                return HttpNotFound();
            }
            return View(tbl_DuAn);
        }

        // POST: DuAn/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaDuAn,TenDuAn,Thumbnail,KinhPhi,ThoiGianDuKien,NgayKhoiCong,TrangThaiDuAn")] tbl_DuAn tbl_DuAn)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_DuAn).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_DuAn);
        }

        // GET: DuAn/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_DuAn tbl_DuAn = db.tbl_DuAn.Find(id);
            if (tbl_DuAn == null)
            {
                return HttpNotFound();
            }
            return View(tbl_DuAn);
        }

        // POST: DuAn/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_DuAn tbl_DuAn = db.tbl_DuAn.Find(id);
            db.tbl_DuAn.Remove(tbl_DuAn);
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
