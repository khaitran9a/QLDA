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


        public ActionResult dsCongViecOfNhanVien(int id)
        {
            int userID = Convert.ToInt32(Session["UserID"]);
            var items = db.tbl_CongViec.Join(db.tbl_NhanVienThamGiaCongViec,
                        cv => cv.MaCongViec,
                        nvtc => nvtc.MaCongViec,
                        (cv, nvtc) => new { CongViec = cv, NhanVienThamGia = nvtc })
                  .Where(x => x.CongViec.MaDuAn == id
                              && x.CongViec.TrangThai == true
                              && x.NhanVienThamGia.MaNV == userID).Select(x => x.CongViec);
                  
            return PartialView("_dsCongViecOfNhanVien", items);
        }



        public ActionResult dsNhanVienInDuAn(int id)
        {
            var query1 = db.tbl_ThamGiaDuAn.Where(y => y.MaDuAn == id)
                          .Select(x => x.MaNV);
            var items = db.tbl_NhanVien.Where(x => query1.Contains(x.MaNV));
            return PartialView("_dsNhanVienInDuAn", items);
        }




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


            int userID = (int)Session["UserID"];

            var query = db.tbl_ThamGiaDuAn.FirstOrDefault(x => x.MaDuAn == id && x.MaNV == userID && x.isManager == true);
            //int count = query.Count();
            

            if (query != null)
            {
                tbl_DuAn = db.tbl_DuAn.Find(query.MaDuAn);
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
