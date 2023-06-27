﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using QLDA.Models;

namespace QLDA.Areas.Admin.Controllers
{
    public class CongViecController : Controller
    {
        private QLDAEntities db = new QLDAEntities();

        // GET: Admin/CongViec
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

        public ActionResult dsCongViecCap2(int id)
        {
            var items = db.tbl_CongViec.Where(x => x.ParentID == id && x.TrangThai == true);
            return PartialView("_dsCongViecCap2", items);
        }

        [HttpGet]
        public ActionResult ListDuAn()
        {

            var items = db.tbl_CongViec.Select(x => x.tbl_DuAn);
            var query = from duAn in db.tbl_DuAn
                        where (from congViec in db.tbl_CongViec
                               group congViec by congViec.MaDuAn into g
                               select g.Key).Contains(duAn.MaDuAn)
                        select duAn;


            return PartialView("_listDuAn", query);
        }

        [HttpGet]
        public JsonResult ListCongViec(string tuKhoa)
        {
            try
            {
                var listCongViec = (from cv in db.tbl_CongViec
                              .Where(x => x.MucTieu.Contains(tuKhoa))
                              select new
                              {
                                  MaCongViec = cv.MaCongViec,
                                  TenCongViec = cv.MucTieu,
                                  TenDuAn = cv.tbl_DuAn.TenDuAn,
                                  Deadline = cv.ThoiGianHoanThanh,
                                  TienDo = cv.TienDo,
                                  TrangThai = cv.TrangThai
                              }).ToList();
                return Json(new { code = 200, dsDuAn = listCongViec, msg = "Lấy danh sách thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy danh sách thất bại" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult dsCongViecByDuAn(int? id)
        {
            id = Convert.ToInt32(id);
            try
            {
                var listCongViec = (from cv in db.tbl_CongViec
                              .Where(x => x.MaDuAn == id)
                                    select new
                                    {
                                        MaCongViec = cv.MaCongViec,
                                        TenCongViec = cv.MucTieu,
                                        TenDuAn = cv.tbl_DuAn.TenDuAn,
                                        Deadline = cv.ThoiGianHoanThanh,
                                        TienDo = cv.TienDo,
                                        TrangThai = cv.TrangThai
                                    }).ToList();
                return Json(new { code = 200, listCongViec = listCongViec, msg = "Lấy danh sách thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy danh sách thất bại" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }







        [HttpPost]
        public ActionResult addCongViec(int maDa, string mucTieu, string chiTiet, float tg)
        {
            try
            {
                tbl_CongViec myItem = new tbl_CongViec();
                myItem.MaDuAn = maDa;
                myItem.MucTieu = mucTieu;
                myItem.ChiThietCongViec = chiTiet;
                myItem.ThoiGianHoanThanh = tg;
                myItem.NgayTao = DateTime.Now;
                myItem.TrangThai = true;
                db.tbl_CongViec.Add(myItem);
                db.SaveChanges();
                return Json(new { message = true });

            }
            catch
            {
                return Json(new { message = false });
            }
        }


        [HttpPost]
        public ActionResult editCongViec(int MaCongViec, string mucTieu, string chiTiet, float tg)
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
                congViec.MucTieu = mucTieu;
                congViec.ChiThietCongViec = chiTiet;
                congViec.ThoiGianHoanThanh = tg;
                db.Entry(congViec).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { message = true });

            }
            catch
            {
                return Json(new { message = false });
            }
        }

        //[HttpPost, ActionName("deleteCongViec")]
        public async Task deleteCongViec(int maCongViec)
        {
            if (maCongViec == 0)
            {
                throw new ArgumentException("Invalid job ID", nameof(maCongViec));
            }

            var jobChild = db.tbl_CongViec.Where(x => x.ParentID == maCongViec).ToList();

            foreach(var item in jobChild)
            {
                await deleteCongViec(item.MaCongViec);
            }

            var job = await db.tbl_CongViec.FindAsync(maCongViec);

            if(job != null)
            {
                job.TrangThai = false;
                db.Entry(job).State = EntityState.Modified;
                await db.SaveChangesAsync();
              
            }
            

            //try
            //{
            //    tbl_CongViec congViec = (from c in db.tbl_CongViec
            //                             where c.MaCongViec == maCongViec
            //                             select c).FirstOrDefault();
            //    congViec.TrangThai = false;
            //    db.Entry(congViec).State = EntityState.Modified;
            //    db.SaveChanges();
            //    //da.dsNhanVien(myItem.MaDuAn);
            //    //da.dsNhanVienInDuAn(myItem.MaDuAn);
            //    return Json(new { message = true });

            //}
            //catch
            //{
            //    return Json(new { message = false });
            //}
        }




        [HttpPost]
        public ActionResult addCongViecCon(int maDa, int maCv, string tenCv, string chiTiet, float deadLine)
        {
      
            try
            {
                tbl_CongViec myItem = new tbl_CongViec();
                myItem.MaDuAn = maDa;
                myItem.ParentID = maCv;
                myItem.MucTieu = tenCv;
                myItem.ChiThietCongViec = chiTiet;
                myItem.ThoiGianHoanThanh = deadLine;
                myItem.NgayTao = DateTime.Now;
                myItem.TrangThai = true;
                db.tbl_CongViec.Add(myItem);
                db.SaveChanges();
                return Json(new { message = true });

            }
            catch
            {
                return Json(new { message = false });
            }
        }


        // GET: Admin/CongViec/Details/5
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

        public ActionResult dsNhanVienInDuAn(int id)
        {
            var query1 = db.tbl_CongViec.Where(y => y.MaCongViec == id)
                          .Select(x => x.MaDuAn);

            var parentId = db.tbl_CongViec.Where(y => y.MaCongViec == id && y.ParentID != null)
                          .Select(x => x.ParentID);
            var query2 = db.tbl_ThamGiaDuAn.Where(y => query1.Contains(y.MaDuAn))
                          .Select(x => x.MaNV);
            //var query3 = db.tbl_NhanVien.Where(y => query2.Contains(y.MaNV))
            //               .Select(x => x.MaNV);
            var query4 = db.tbl_NhanVienThamGiaCongViec.Where(y => y.MaCongViec == id)
                            .Select(x => x.MaNV);

            int parentIdValue = 0;
            if (parentId.Any())
            {
                parentIdValue = (int)parentId.FirstOrDefault();
            }

            var items = db.tbl_NhanVien.Where(x => query2.Contains(x.MaNV) && !query4.Contains(x.MaNV));


            if (parentIdValue != 0)
            {
                var cv = db.tbl_CongViec.Where(y => y.MaCongViec == parentIdValue)
                                        .Select(x => x.MaCongViec);
                query1 = db.tbl_CongViec.Where(y => cv.Contains(y.MaCongViec))
                                        .Select(x => x.MaDuAn);
                query2 = db.tbl_NhanVienThamGiaCongViec.Where(y => y.MaCongViec == parentIdValue)
                           .Select(x => x.MaNV);
                var query3 = db.tbl_ThamGiaDuAn.Where(y => query1.Contains(y.MaDuAn))
                         .Select(x => x.MaNV);
                int maCv = (int)cv.FirstOrDefault();
                query4 = db.tbl_NhanVienThamGiaCongViec.Where(y => y.MaCongViec == id)
                                .Select(x => x.MaNV);
                items = db.tbl_NhanVien.Where(x => query2.Contains(x.MaNV) && !query4.Contains(x.MaNV) && query3.Contains(x.MaNV));
            }

            
             
            return PartialView("_dsNhanVienThamGia", items);
        }

        public ActionResult dsNhanVienInCongViec(int id)
        {
            ViewBag.idCongViec = id;
            var query1 = db.tbl_CongViec.Where(y => y.MaCongViec == id)
                           .Select(x => x.MaDuAn);
            var query2 = db.tbl_ThamGiaDuAn.Where(y => query1.Contains(y.MaDuAn))
                          .Select(x => x.MaNV);
            //var query3 = db.tbl_NhanVien.Where(y => query2.Contains(y.MaNV))
            //               .Select(x => x.MaNV);
            var query4 = db.tbl_NhanVienThamGiaCongViec.Where(y => y.MaCongViec == id)
                            .Select(x => x.MaNV);
            var items = db.tbl_NhanVien.Where(x => query2.Contains(x.MaNV) && query4.Contains(x.MaNV));
            return PartialView("_dsNhanVienThamGiaCongViec", items);
        }




        //[HttpPost]
        //public ActionResult editCongViecCon(int MaCongViec, string mucTieu, string chiTiet, float tg)
        //{
        //    if (MaCongViec == 0)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    try
        //    {
        //        tbl_CongViec congViec = (from c in db.tbl_CongViec
        //                                 where c.MaCongViec == MaCongViec
        //                                 select c).FirstOrDefault();
        //        congViec.MucTieu = mucTieu;
        //        congViec.ChiThietCongViec = chiTiet;
        //        congViec.ThoiGianHoanThanh = tg;
        //        db.Entry(congViec).State = EntityState.Modified;
        //        db.SaveChanges();
        //        //da.dsNhanVien(myItem.MaDuAn);
        //        //da.dsNhanVienInDuAn(myItem.MaDuAn);
        //        return Json(new { message = true });

        //    }
        //    catch
        //    {
        //        return Json(new { message = false });
        //    }
        //}

        //[HttpPost, ActionName("deleteCongViec")]
        //public ActionResult deleteCongViecCon(int maCongViec)
        //{
        //    if (maCongViec == 0)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    try
        //    {
        //        tbl_CongViec congViec = (from c in db.tbl_CongViec
        //                                 where c.MaCongViec == maCongViec
        //                                 select c).FirstOrDefault();
        //        congViec.TrangThai = false;
        //        db.Entry(congViec).State = EntityState.Modified;
        //        db.SaveChanges();
        //        //da.dsNhanVien(myItem.MaDuAn);
        //        //da.dsNhanVienInDuAn(myItem.MaDuAn);
        //        return Json(new { message = true });

        //    }
        //    catch
        //    {
        //        return Json(new { message = false });
        //    }
        //}

















        //    // GET: Admin/CongViec/Create
        //    public ActionResult Create()
        //    {
        //        ViewBag.MaDuAn = new SelectList(db.tbl_DuAn, "MaDuAn", "TenDuAn");
        //        return View();
        //    }

        //    // POST: Admin/CongViec/Create
        //    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult Create([Bind(Include = "MaCongViec,MaDuAn,MucTieu,ThoiGianHoanThanh,ChiThietCongViec,TienDo,NgayTao,TrangThai")] tbl_CongViec tbl_CongViec)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            db.tbl_CongViec.Add(tbl_CongViec);
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }

        //        ViewBag.MaDuAn = new SelectList(db.tbl_DuAn, "MaDuAn", "TenDuAn", tbl_CongViec.MaDuAn);
        //        return View(tbl_CongViec);
        //    }

        //    // GET: Admin/CongViec/Edit/5
        //    public ActionResult Edit(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        tbl_CongViec tbl_CongViec = db.tbl_CongViec.Find(id);
        //        if (tbl_CongViec == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        ViewBag.MaDuAn = new SelectList(db.tbl_DuAn, "MaDuAn", "TenDuAn", tbl_CongViec.MaDuAn);
        //        return View(tbl_CongViec);
        //    }

        //    // POST: Admin/CongViec/Edit/5
        //    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult Edit([Bind(Include = "MaCongViec,MaDuAn,MucTieu,ThoiGianHoanThanh,ChiThietCongViec,TienDo,NgayTao,TrangThai")] tbl_CongViec tbl_CongViec)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            db.Entry(tbl_CongViec).State = EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //        ViewBag.MaDuAn = new SelectList(db.tbl_DuAn, "MaDuAn", "TenDuAn", tbl_CongViec.MaDuAn);
        //        return View(tbl_CongViec);
        //    }

        //    // GET: Admin/CongViec/Delete/5
        //    public ActionResult Delete(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        tbl_CongViec tbl_CongViec = db.tbl_CongViec.Find(id);
        //        if (tbl_CongViec == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        return View(tbl_CongViec);
        //    }

        //    // POST: Admin/CongViec/Delete/5
        //    [HttpPost, ActionName("Delete")]
        //    [ValidateAntiForgeryToken]
        //    public ActionResult DeleteConfirmed(int id)
        //    {
        //        tbl_CongViec tbl_CongViec = db.tbl_CongViec.Find(id);
        //        db.tbl_CongViec.Remove(tbl_CongViec);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    protected override void Dispose(bool disposing)
        //    {
        //        if (disposing)
        //        {
        //            db.Dispose();
        //        }
        //        base.Dispose(disposing);
        //    }
        //}
    }
}
