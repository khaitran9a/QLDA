using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Xml.Linq;
using QLDA.Models;

namespace QLDA.Areas.Admin.Controllers
{
    public class ThamGiaDuAnController : Controller
    {
        private QLDAEntities db = new QLDAEntities();
        private DuAnController da = new DuAnController();

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

        [HttpGet]
        public ActionResult dsNhanVien()
        {
            var items = db.tbl_NhanVien
                 .Join(db.tbl_ThamGiaDuAn, nv => nv.MaNV, tg => tg.MaNV, (nv, tg) => nv)
                 .GroupBy(nv => nv.MaNV)
                 .Select(g => g.FirstOrDefault()); 

            return PartialView("_dsNhanVien", items);
        }

        [HttpGet]
        public ActionResult dsDuAn()
        {
            var items = db.tbl_DuAn 
                 .Join(db.tbl_ThamGiaDuAn, nv => nv.MaDuAn, tg => tg.MaDuAn, (nv, tg) => nv)
                 .GroupBy(nv => nv.MaDuAn)
                 .Select(g => g.FirstOrDefault()); ;

            return PartialView("_dsDuAn", items);
        }

        [HttpGet]
        public ActionResult dsDuAnByNV(int? id)
        {
            id = Convert.ToInt32(id);
            try
            {
                var dsThamGia = (from tg in db.tbl_ThamGiaDuAn
                              .Where(x => x.MaNV == id)
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

        [HttpGet]
        public ActionResult dsNhanVienByDuAn(int? id)
        {
            id = Convert.ToInt32(id);
            try
            {
                var dsThamGia = (from tg in db.tbl_ThamGiaDuAn
                              .Where(x => x.MaDuAn == id)
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

        [HttpPost]
        public ActionResult addNhanVien(int idNv, int idDa)
        {
            try
            {
                tbl_ThamGiaDuAn myItem = new tbl_ThamGiaDuAn();
                myItem.MaDuAn = idDa;
                myItem.MaNV = idNv;
                myItem.NgayThamGia = DateTime.Now;
                db.tbl_ThamGiaDuAn.Add(myItem);
                db.SaveChanges();
                da.dsNhanVien(myItem.MaDuAn);
                da.dsNhanVienInDuAn(myItem.MaDuAn);
                return Json(new { message = true });

            }
            catch
            {
                return Json(new { message = false });
            }
        }


        [HttpPost, ActionName("deleteNhanVien")]
        public ActionResult deleteNhanVien(int maDa, int? maNV)
        {
            if(maNV == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            maDa = Convert.ToInt32(maDa);
            maNV = Convert.ToInt32(maNV);
            try
            {
                tbl_ThamGiaDuAn tg = (from c in db.tbl_ThamGiaDuAn
                                      where c.MaDuAn == maDa && c.MaNV == maNV
                                      select c).FirstOrDefault();
                db.tbl_ThamGiaDuAn.Remove(tg);
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


        [HttpPost]
        public ActionResult setManager(int maNV, int maDa)
        {
            if (maNV == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                tbl_ThamGiaDuAn tg = (from c in db.tbl_ThamGiaDuAn
                                                  where c.MaNV == maNV && c.MaDuAn == maDa
                                                  select c).FirstOrDefault();
                tg.isManager = true;
                db.Entry(tg).State = EntityState.Modified;
                db.SaveChanges();

                List<tbl_ThamGiaDuAn> nvConlai = db.tbl_ThamGiaDuAn.Where(x => x.MaNV != maNV && x.MaDuAn == maDa).ToList();
                int dem = nvConlai.Count();
                foreach (var item in nvConlai)
                {
                    item.isManager = false;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
                db.SaveChanges();
                return Json(new { message = true });

            }
            catch
            {
                return Json(new { message = false });
            }
        }





        //[HttpPost]
        //public ActionResult DeleteById(int maDa, int maNv)
        //{

        //        tbl_ThamGiaDuAn tg = (from c in db.tbl_ThamGiaDuAn
        //                             where c.MaDuAn == maDa && c.MaNV == maNv
        //                             select c).FirstOrDefault();
        //        if (tg != null)
        //        {
        //            db.tbl_ThamGiaDuAn.Remove(tg);
        //            db.SaveChanges();
        //            return Json(tg);
        //        }       
        //    return new EmptyResult();
        //}

        // GET: Admin/ThamGiaDuAn/Edit/5
        public ActionResult Edit(int mada, int manv)
        {
            
            tbl_ThamGiaDuAn tg = db.tbl_ThamGiaDuAn.FirstOrDefault(a => a.MaDuAn == mada && a.MaNV == manv);
            if (tg == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaDuAn = new SelectList(db.tbl_DuAn, "MaDuAn", "TenDuAn", tg.MaDuAn);
            ViewBag.MaNV = new SelectList(db.tbl_NhanVien, "MaNV", "Ho", tg.MaNV);
            return View(tg);
        }

        // POST: Admin/ThamGiaDuAn/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNV,MaDuAn,NgayThamGia,GhiChu")] tbl_ThamGiaDuAn tbl_ThamGiaDuAn)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_ThamGiaDuAn).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaDuAn = new SelectList(db.tbl_DuAn, "MaDuAn", "TenDuAn", tbl_ThamGiaDuAn.MaDuAn);
            ViewBag.MaNV = new SelectList(db.tbl_NhanVien, "MaNV", "Ho", tbl_ThamGiaDuAn.MaNV);
            return View(tbl_ThamGiaDuAn);
        }

        // GET: Admin/ThamGiaDuAn/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_ThamGiaDuAn tbl_ThamGiaDuAn = db.tbl_ThamGiaDuAn.Find(id);
            if (tbl_ThamGiaDuAn == null)
            {
                return HttpNotFound();
            }
            return View(tbl_ThamGiaDuAn);
        }

        //// POST: Admin/ThamGiaDuAn/Delete/5
        //[HttpPostc]
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
