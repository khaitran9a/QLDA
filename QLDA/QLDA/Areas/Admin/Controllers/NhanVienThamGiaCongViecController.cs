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
    public class NhanVienThamGiaCongViecController : Controller
    {
        private QLDAEntities db = new QLDAEntities();

        // GET: Admin/NhanVienThamGiaCongViec
        public ActionResult Index()
        {
            var tbl_NhanVienThamGiaCongViec = db.tbl_NhanVienThamGiaCongViec.Include(t => t.tbl_CongViec).Include(t => t.tbl_NhanVien);
            return View(tbl_NhanVienThamGiaCongViec.ToList());
        }

        [HttpGet]
        public JsonResult DsThamGia(string tuKhoa)
        {
            try
            {
                var dsThamGia = (from tg in db.tbl_NhanVienThamGiaCongViec
                              .Where(x => x.tbl_CongViec.MucTieu.Contains(tuKhoa) || x.tbl_NhanVien.Ten.Contains(tuKhoa))
                                 select new
                                 {
                                     MaCongViec = tg.tbl_CongViec.MaCongViec,
                                     MaNhanVien = tg.tbl_NhanVien.MaNV,
                                     MaDuAn = tg.tbl_CongViec.MaDuAn,
                                     TenDuAn = tg.tbl_CongViec.tbl_DuAn.TenDuAn,
                                     TenCongViec = tg.tbl_CongViec.MucTieu,

                                     TenNhanVien = tg.tbl_NhanVien.Ho + " " + tg.tbl_NhanVien.Ten,
                                     TrangThai = tg.TrangThai,
                                     NgayThamGia = tg.NgayThem.ToString(),
                                 }).ToList();
                return Json(new { code = 200, dsThamGia = dsThamGia, msg = "Lấy danh sách thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy danh sách thất bại" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult FindByDaNv(int idNV, int idDA)
        {
            try
            {
                var dsThamGia = (from tg in db.tbl_NhanVienThamGiaCongViec
                              .Where(x => x.MaNV == idNV && x.tbl_CongViec.tbl_DuAn.MaDuAn == idDA)
                                 select new
                                 {
                                     MaCongViec = tg.tbl_CongViec.MaCongViec,
                                     MaNhanVien = tg.tbl_NhanVien.MaNV,
                                     MaDuAn = tg.tbl_CongViec.MaDuAn,
                                     TenDuAn = tg.tbl_CongViec.tbl_DuAn.TenDuAn,
                                     TenCongViec = tg.tbl_CongViec.MucTieu,

                                     TenNhanVien = tg.tbl_NhanVien.Ho + " " + tg.tbl_NhanVien.Ten,
                                     TrangThai = tg.TrangThai,
                                     NgayThamGia = tg.NgayThem.ToString(),
                                 }).ToList();
                return Json(new { code = 200, dsThamGia = dsThamGia, msg = "Lấy danh sách thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy danh sách thất bại" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
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


        [HttpPost, ActionName("deleteNhanVien")]
        public ActionResult deleteNhanVien(int maCv, int? maNV)
        {
            if (maNV == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            maCv = Convert.ToInt32(maCv);
            maNV = Convert.ToInt32(maNV);
            try
            {
                tbl_NhanVienThamGiaCongViec tg = (from c in db.tbl_NhanVienThamGiaCongViec
                                      where c.MaCongViec == maCv && c.MaNV == maNV
                                      select c).FirstOrDefault();
                db.tbl_NhanVienThamGiaCongViec.Remove(tg);
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


        public ActionResult dsNhanVien()
        {
            var nv = db.tbl_NhanVien.ToList();
            return PartialView("_dsNhanVien", nv);
        }
        public ActionResult dsDuAn()
        {
            var da = db.tbl_DuAn.ToList();
            return PartialView("_dsDuAn", da);
        }





















































        // GET: Admin/NhanVienThamGiaCongViec/Details/5
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

        // GET: Admin/NhanVienThamGiaCongViec/Create
        public ActionResult Create()
        {
            ViewBag.MaCongViec = new SelectList(db.tbl_CongViec, "MaCongViec", "MucTieu");
            ViewBag.MaNV = new SelectList(db.tbl_NhanVien, "MaNV", "Ho");
            return View();
        }

        // POST: Admin/NhanVienThamGiaCongViec/Create
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

        // GET: Admin/NhanVienThamGiaCongViec/Edit/5
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

        // POST: Admin/NhanVienThamGiaCongViec/Edit/5
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

        // GET: Admin/NhanVienThamGiaCongViec/Delete/5
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

        // POST: Admin/NhanVienThamGiaCongViec/Delete/5
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
