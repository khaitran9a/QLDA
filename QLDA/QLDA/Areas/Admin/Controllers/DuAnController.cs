using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using QLDA.Models;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace QLDA.Areas.Admin.Controllers
{
    public class DuAnController : Controller
    {
        private QLDAEntities db = new QLDAEntities();

        // GET: Admin/DuAn
        public ActionResult Index(string searchString)
        {
            //if(!String.IsNullOrEmpty(searchString))
            //{
            //    var findByName = db.tbl_DuAn.Where(da => da.TenDuAn.ToLower().Contains(searchString.ToLower())).ToList();
            //    return View(findByName);

            //}
            //else
            //{
            return View(db.tbl_DuAn.ToList());
            //}    

        }

        [HttpGet]
        public JsonResult DsDuAn(string tuKhoa)
        {
            try
            {
                var dsDuAn = (from da in db.tbl_DuAn
                              .Where(x => x.TenDuAn.Contains(tuKhoa))
                              select new
                              {
                                  MaDuAn = da.MaDuAn,
                                  TenDuAn = da.TenDuAn,
                                  Thumbnail = da.Thumbnail,
                                  KinhPhi = da.KinhPhi,
                                  tgDuKien = da.ThoiGianDuKien,
                                  tgbatDau = da.NgayKhoiCong.ToString(),
                                  status = da.TrangThaiDuAn
                              }).ToList();
                return Json(new { code = 200, dsDuAn = dsDuAn, msg = "Lấy danh sách thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy danh sách thất bại" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult dsNhanVien(int id)
        {
            var query1 = db.tbl_ThamGiaDuAn.Where(y => y.MaDuAn == id)
                          .Select(x => x.MaNV);
            var items = db.tbl_NhanVien.Where(x => !query1.Contains(x.MaNV));
            return PartialView("_ListNhanVien", items);
        }

        public ActionResult dsNhanVienInDuAn(int id)
        {
            var query1 = db.tbl_ThamGiaDuAn.Where(y => y.MaDuAn == id)
                          .Select(x => x.MaNV);
            var items = db.tbl_NhanVien.Where(x => query1.Contains(x.MaNV));
            return PartialView("_dsNhanVienThamGia", items);
        }

        [HttpGet]
        public JsonResult sortByKinhPhi()
        {
            try
            {
                var dsDuAn = (from da in db.tbl_DuAn
                              .OrderBy(x => x.KinhPhi)
                              select new
                              {
                                  MaDuAn = da.MaDuAn,
                                  TenDuAn = da.TenDuAn,
                                  Thumbnail = da.Thumbnail,
                                  KinhPhi = da.KinhPhi,
                                  tgDuKien = da.ThoiGianDuKien,
                                  tgbatDau = da.NgayKhoiCong/*.Value.ToString("yyyy-MM-dd HH:mm tt")*/,
                                  status = da.TrangThaiDuAn
                              }).ToList();    
                return Json(new { code = 200, dsDuAn = dsDuAn, msg = "Sắp xếp tăng dần" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy danh sách thất bại" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult kinhPhiGiamDan()
        {
            try
            {
                var dsDuAn = (from da in db.tbl_DuAn
                              .OrderByDescending(x => x.KinhPhi)
                              select new
                              {
                                  MaDuAn = da.MaDuAn,
                                  TenDuAn = da.TenDuAn,
                                  Thumbnail = da.Thumbnail,
                                  KinhPhi = da.KinhPhi,
                                  tgDuKien = da.ThoiGianDuKien,
                                  tgbatDau = da.NgayKhoiCong/*.Value.ToString("yyyy-MM-dd HH:mm tt")*/,
                                  status = da.TrangThaiDuAn
                              }).ToList();
                return Json(new { code = 200, dsDuAn = dsDuAn, msg = "Sắp xếp giảm dần" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy danh sách thất bại" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult NgayKhoiCongSomNhat()
        {
            var items = db.tbl_DuAn.OrderBy(a => a.NgayKhoiCong).ToList();
            return View(items);
        }

        // GET: Admin/DuAn/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_DuAn duan = db.tbl_DuAn.Find(id);
            if (duan == null)
            {
                return HttpNotFound();
            }
            return View(duan);
        }

        // GET: Admin/DuAn/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/DuAn/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(tbl_DuAn duan, HttpPostedFileBase uploadhinh)
        {
            if (ModelState.IsValid)
            {
                db.tbl_DuAn.Add(duan);
                db.SaveChanges();

                if (uploadhinh != null && uploadhinh.ContentLength > 0)
                {
                    int id = duan.MaDuAn;
                    string _FileName = "";
                    int index = uploadhinh.FileName.IndexOf('.');

                    _FileName = "Project " + id.ToString() + "." + uploadhinh.FileName.Substring(index + 1);
                    string _path = Path.Combine(Server.MapPath("~/Content/images/projects"), _FileName);
                    uploadhinh.SaveAs(_path);
                    duan.Thumbnail = _FileName;
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(duan);
        }


        // GET: Admin/DuAn/Edit/5
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

        // POST: Admin/DuAn/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tbl_DuAn duan, HttpPostedFileBase uploadhinh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(duan).State = EntityState.Modified;
                db.SaveChanges();

                if (uploadhinh != null && uploadhinh.ContentLength > 0)
                {
                    int id = duan.MaDuAn;
                    string _FileName = "";
                    int index = uploadhinh.FileName.IndexOf('.');

                    _FileName = "Project " + id.ToString() + "." + uploadhinh.FileName.Substring(index + 1);
                    string _path = Path.Combine(Server.MapPath("~/Content/images/projects"), _FileName);
                    uploadhinh.SaveAs(_path);
                    duan.Thumbnail = _FileName;
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(duan);
        }



        // GET: Admin/DuAn/Delete/5
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

        // POST: Admin/DuAn/Delete/5
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

        public string removeVietnameseTones(string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');
            return (sb.ToString().Normalize(NormalizationForm.FormD));
        }
    }
}
