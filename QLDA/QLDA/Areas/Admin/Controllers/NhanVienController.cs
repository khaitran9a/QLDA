using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLDA.Models;

namespace QLDA.Areas.Admin.Controllers
{
    public class NhanVienController : Controller
    {
        private QLDAEntities db = new QLDAEntities();

        // GET: Admin/NhanVien
        public ActionResult Index()
        {
            var tbl_NhanVien = db.tbl_NhanVien.Include(t => t.tbl_ChucVu);
            return View(tbl_NhanVien.ToList());
        }

        // GET: Admin/NhanVien/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_NhanVien tbl_NhanVien = db.tbl_NhanVien.Find(id);
            if (tbl_NhanVien == null)
            {
                return HttpNotFound();
            }
            return View(tbl_NhanVien);
        }

        // GET: Admin/NhanVien/Create
        public ActionResult Create()
        {
            ViewBag.MaChucVu = new SelectList(db.tbl_ChucVu, "MaChucVu", "TenChucVu");
            return View();
        }

        // POST: Admin/NhanVien/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tbl_NhanVien nhanvien, HttpPostedFileBase uploadhinh)
        {
            if (ModelState.IsValid)
            {
                db.tbl_NhanVien.Add(nhanvien);
                db.SaveChanges();

                if (uploadhinh != null && uploadhinh.ContentLength > 0)
                {
                    int id = nhanvien.MaNV;
                    string _FileName = "";
                    int index = uploadhinh.FileName.IndexOf('.');
                    _FileName = "Avartar " + id.ToString() + "." + uploadhinh.FileName.Substring(index + 1);
                    string _path = Path.Combine(Server.MapPath("~/Content/images/avatars"), _FileName);
                    uploadhinh.SaveAs(_path);
                    nhanvien.AnhDaiDien = _FileName;
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(nhanvien);
        }

        // GET: Admin/NhanVien/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_NhanVien tbl_NhanVien = db.tbl_NhanVien.Find(id);
            if (tbl_NhanVien == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaChucVu = new SelectList(db.tbl_ChucVu, "MaChucVu", "TenChucVu", tbl_NhanVien.MaChucVu);
            return View(tbl_NhanVien);
        }

        // POST: Admin/NhanVien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNV,Ho,Ten,TenDangNhap,MatKhau,DiaChi,SoDienThoai,Status,MaChucVu,Email,NgaySinh,AnhDaiDien")] tbl_NhanVien tbl_NhanVien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_NhanVien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaChucVu = new SelectList(db.tbl_ChucVu, "MaChucVu", "TenChucVu", tbl_NhanVien.MaChucVu);
            return View(tbl_NhanVien);
        }

        // GET: Admin/NhanVien/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_NhanVien tbl_NhanVien = db.tbl_NhanVien.Find(id);
            if (tbl_NhanVien == null)
            {
                return HttpNotFound();
            }
            return View(tbl_NhanVien);
        }

        // POST: Admin/NhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_NhanVien tbl_NhanVien = db.tbl_NhanVien.Find(id);
            db.tbl_NhanVien.Remove(tbl_NhanVien);
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
