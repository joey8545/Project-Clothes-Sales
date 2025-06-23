using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using Project.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Project.Controllers
{
    public class OrderController : Controller
    {
        private readonly DbuniPayContext _db;

        public OrderController(DbuniPayContext db) 
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List(CKeywordViewModel vm, string status, int page = 1)
        {
            string keyword = vm.txtKeyword;
            status = string.IsNullOrEmpty(status) ? "All" : status;

            IQueryable<Torder> orders = _db.Torders.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                orders = orders.Where(p => p.Oname.Contains(keyword));
            }

            if (status != "All")
            {
                if (status == "付款完成")
                {
                    orders = orders.Where(p => p.Opayment == true);
                }
                else if (status == "尚未付款")
                {
                    orders = orders.Where(p => p.Opayment == false);
                }
                else
                {
                    orders = orders.Where(p => p.Ostatus == status);
                }
            }
             
            orders = orders.OrderByDescending(p => p.Odate);
             
            int pageSize = 20;
            int totalItems = orders.Count();  
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var paginatedOrders = orders
                .Skip((page - 1) * pageSize) 
                .Take(pageSize)  
                .ToList();
             
            List<OrderWrap> orderWrapList = paginatedOrders.Select(order => new OrderWrap
            {
                Oid = order.Oid,
                Oname = order.Oname,
                Oprice = order.Oprice,
                Odiscountedprice = order.Odiscountedprice,
                OtotalPrice = order.OtotalPrice,
                Odate = order.Odate,
                Mid = order.Mid,
                Oaddress = order.Oaddress,
                Ophone = order.Ophone,
                Oemail = order.Oemail,
                Ostatus = order.Ostatus,
                Opayment = order.Opayment,
                OcancelStatus = order.OcancelStatus,
                OreturnStatus = order.OreturnStatus
            }).ToList();
             
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Status = status;
            ViewBag.Keyword = keyword;

            return View(orderWrapList);
        }


        public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("List");

            Torder x = _db.Torders.FirstOrDefault(c => c.Oid == id);
            if (x == null)
                return RedirectToAction("List");

            return View(new OrderWrap() { order = x });
        }
        [HttpPost]
        public IActionResult Edit(OrderWrap p) {

			Torder x = _db.Torders.FirstOrDefault(c => c.Oid == p.Oid);
			if (x != null)
			{
				x.Oname = p.Oname;
                x.Oprice = p.Oprice;
                x.Odiscountedprice = p.Odiscountedprice;
				x.OtotalPrice = p.OtotalPrice;
                x.Odate = new DateTime(p.Odate.Year, p.Odate.Month, p.Odate.Day, p.Odate.Hour, p.Odate.Minute, p.Odate.Second);
                x.Mid = p.Mid;
				x.Oaddress = p.Oaddress;
				x.Ophone = p.Ophone;
                x.Oemail = p.Oemail;
				x.Ostatus = p.Ostatus;
				x.Opayment = p.Opayment;
                x.OcancelStatus = p.OcancelStatus; 
                x.OreturnDate = p.OreturnDate;
                x.OreturnStatus = p.OreturnStatus;
                x.OreturnNo = p.OreturnNo;
				_db.SaveChanges();
			}
			return RedirectToAction("List");
		}
        public IActionResult OrderDetail(int id)
        {
			var OrderDetails = _db.TorderDetails 
                               .Where (t=>t.Oid == id )
                               .ToList();
            List<COrderDetailWarp> list = new List<COrderDetailWarp>();
            foreach (var t in OrderDetails)
                list.Add(new COrderDetailWarp() { orderdetail = t });
            return View(list);
        }

        public IActionResult OrderAdvice()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult>  getAdvice()
        {
            var advice = await _db.Tadvices.OrderByDescending(c=>c.DateTime).ToListAsync();
            if (advice==null) {
                advice = new List<Tadvice>();
            }
            return Json(advice);
        }
    }
}
