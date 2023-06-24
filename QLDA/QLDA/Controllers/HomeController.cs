using QLDA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLDA.Controllers
{
    public class HomeController : Controller
    {
        private QLDAEntities db = new QLDAEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult HomeDuAN()
        {
        
            if (Session["UserID"] != null)
            {
                string chuoi = Session["UserID"].ToString();
                int maNv = Convert.ToInt32(chuoi);
                var query1 = db.tbl_ThamGiaDuAn.Where(x => x.MaNV == maNv)
                                                .Select(x => x.MaDuAn);
                var items = db.tbl_DuAn.Where(x=> query1.Contains(x.MaDuAn));
                return PartialView("_HomeDuAn", items);

            }
            return View("_Home");

        }
        //public ActionResult dsCongViec(int id)
        //{
        //    var query1 = db.tbl_ThamGiaDuAn.Where(y => y.MaDuAn == id)
        //                  .Select(x => x.MaNV);
        //    var items = db.tbl_NhanVien.Where(x => !query1.Contains(x.MaNV));
        //    return PartialView("_dsCongViec", items);
        //}

    }
}