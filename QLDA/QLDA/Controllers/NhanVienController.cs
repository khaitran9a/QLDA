using System.Data;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using QLDA.Models;
using System.Web.UI.WebControls;

namespace QLDA.Controllers
{
    public class NhanVienController : Controller
    {
        private QLDAEntities db = new QLDAEntities();


        // GET: NhanVien/Create
        public ActionResult DangKy()
        {
            ViewBag.MaChucVu = new SelectList(db.tbl_ChucVu, "MaChucVu", "TenChucVu");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(tbl_NhanVien nv, HttpPostedFileBase uploadhinh)
        {
            // kiem tra va luu vao db
            if (ModelState.IsValid)
            {
                var _nV = db.tbl_NhanVien.SingleOrDefault(c => c.TenDangNhap == nv.TenDangNhap);
                if (_nV == null)
                {
                    var check = db.tbl_NhanVien.FirstOrDefault(c => c.Email == nv.Email);
                    //if (check != null)
                    //{   
                    //    ViewBag.error = "Email đã có người sử dụng";
                    //    return View();
                    //}

                    //if (IsEmail(nv.Email) == false)
                    //{
                    //    ViewBag.error = "Email không đúng định dạng vd: nv@gmail.com";
                    //    return View();
                    //}

                    var pass = new LoginModel
                    {
                        TenDangNhap = nv.TenDangNhap,
                        MatKhau = nv.MatKhau
                    };


                    var _nv = new tbl_NhanVien
                    {
                        TenDangNhap = nv.TenDangNhap,
                        MatKhau = PasswordEncryption(pass),
                       
                        DiaChi = nv.DiaChi,
                        NgaySinh = nv.NgaySinh

                    };
                    _nv.Ten = nv.Ten;
                    _nv.Ho = nv.Ho;

                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.tbl_NhanVien.Add(_nv);
                    db.SaveChanges();


                    if (uploadhinh != null && uploadhinh.ContentLength > 0)
                    {
                        int id = _nv.MaNV;
                        string _FileName = "";
                        int index = uploadhinh.FileName.IndexOf('.');

                        _FileName = "Avatar" + id.ToString() + "." + uploadhinh.FileName.Substring(index + 1);
                        string _path = Path.Combine(Server.MapPath("~/Content/images/avatars"), _FileName);
                        uploadhinh.SaveAs(_path);
                        _nv.AnhDaiDien = _FileName;
                    }

                    db.SaveChanges();
                    return RedirectToAction("DangNhap");
                }



                else
                {
                    ViewBag.error = "Username đã có người sử dụng";
                    return View();
                }
            }

            ViewBag.error = "Đăng kí thất bại";
            return View();

        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            if (Session["UserID"] != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DangNhap(LoginModel user)
        {
            if (ModelState.IsValid)
            {
                var pass = PasswordEncryption(user);
                var data = db.tbl_NhanVien.SingleOrDefault(s => s.TenDangNhap.Equals(user.TenDangNhap) && s.MatKhau.Equals(pass));

                if (data != null)
                {
                    //add session
                    Session["FullName"] = data.Ho + data.Ten;
                    Session["Email"] = data.Email;
                    Session["UserID"] = data.MaNV;
                    Session["TenDangNhap"] = data.TenDangNhap;
                    if (data.isAdmin == true)
                    {
                        Session["Admin"] = "Admin";
                    }
                    Session["ChucVu"] = data.MaChucVu;
                    Session["Avatar"] = data.AnhDaiDien;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.error = "Tài khoản hoặc mật khẩu không chính xác";
                    return View();
                }
            }
            else
            {
                ViewBag.error = "Đăng nhập thất bại, thử lại sau";
                return View();
            }

        }


        [AllowAnonymous]
        public ActionResult LogOut()
        {
            Response.AddHeader("Cache-Control", "no-cache, no-store,must-revalidate");
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("Expires", "0");
            Session.Abandon();

            Session.Clear();
            Response.Cookies.Clear();
            Session.RemoveAll();

            return RedirectToAction("Index", "Home");
        }




        // GET: NhanVien/Edit/5
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

        // POST: NhanVien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNV,Ho,Ten,TenDangNhap,MatKhau,DiaChi,SoDienThoai,Status,MaChucVu")] tbl_NhanVien tbl_NhanVien)
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

        // GET: NhanVien/Delete/5
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

        // POST: NhanVien/Delete/5
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

        public static bool IsEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;    

            string strRegex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            Regex regex = new Regex(strRegex);

            return regex.IsMatch(email);
        }
        public string convertToUnSign2(string s)
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
            sb = sb.Replace(' ', '-');
            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');
            return (sb.ToString().Normalize(NormalizationForm.FormD));
        }

        //cài đăt time
        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1979, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

            return dateTimeInterval;
        }


        //mã hóa pass

        private string PasswordEncryption(LoginModel nv)
        {
            MD5 mh = MD5.Create();
            //Chuyển kiểu chuổi thành kiểu byte
            byte[] _nv = System.Text.Encoding.ASCII.GetBytes(nv.MatKhau + nv.TenDangNhap);
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(nv.MatKhau + Encoding.UTF8.GetBytes("daylaconghoaxahoichunghiavietnam"));

            //mã hóa chuỗi đã chuyển
            byte[] hash = mh.ComputeHash(inputBytes);
            byte[] hash2 = mh.ComputeHash(_nv);
            //tạo đối tượng StringBuilder (làm việc với kiểu dữ liệu lớn)
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
                sb.Append(hash2[i].ToString("X2"));
            }
            return (sb.ToString());
        }

    }
}
