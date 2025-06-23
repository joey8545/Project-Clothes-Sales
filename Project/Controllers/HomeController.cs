using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using Project.ViewModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly DbuniPayContext _db;

        public HomeController(ILogger<HomeController> logger,DbuniPayContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var currentMonth = DateTime.Today.Month;
            var currentYear = DateTime.Today.Year;
            var lastDayOfMonth = DateTime.DaysInMonth(currentYear, currentMonth);

            List<Torder> order =await _db.Torders.ToListAsync();
            List<TorderDetail> orderDetail =await _db.TorderDetails.ToListAsync();
            List<Tproduct> product_none =await _db.Tproducts.Where(t => t.Pinventory == 0).ToListAsync();
            List<Torder> order_none =await _db.Torders.Where(t => t.Ostatus == "待出貨" && t.Odate <= today.AddDays(-3)).ToListAsync();
            List<int> month_Days = new List<int>();
            List<int?> members_count_Month = new List<int?>();
            List<int?> members_count_Year = new List<int?>();
            List<int> product_inventory = new List<int>();
            List<string> product_name = new List<string>();
            List<int> type_sales_count =await (from tod in _db.TorderDetails
                                          join tp in _db.Tproducts on tod.Pid equals tp.Pid
                                          group tod by tp.Pcategory into grouped
                                          select grouped.Sum(t => t.Pcount)
                                            )
                                            .ToListAsync();

            int sumday = 0;
            int summonth = 0;

            for (int day = 1; day <= lastDayOfMonth; day++)
            {
                if (day <= today.Day)
                {
                    sumday += _db.Tmembers.Count(t => t.McreatedDate.Month == currentMonth && t.McreatedDate.Year == currentYear && t.McreatedDate.Day == day);
                    members_count_Month.Add(sumday);
                }
                else
                {
                    members_count_Month.Add(null);
                }

                month_Days.Add(day);

            }

            for (int month = 1; month <= 12; month++)
            {
                if (month <= currentMonth)
                {
                    summonth += _db.Tmembers.Count(t => t.McreatedDate.Month == month && t.McreatedDate.Year == currentYear);
                    members_count_Year.Add(summonth);
                }
                else
                {
                    members_count_Year.Add(0);
                }

            }

            product_inventory.AddRange(_db.Tproducts.Where(t => t.Pinventory <= 30).Select(t => t.Pinventory).ToList());
            product_name.AddRange(_db.Tproducts.Where(t => t.Pinventory <= 30).Select(t => t.Pname).ToList());

            ViewBag.SalesCount_TOP5_Today =await (from tod in _db.TorderDetails
                                             join td in _db.Torders on tod.Oid equals td.Oid
                                             where td.Odate.Date == today && tod.Pid != 0
                                             group tod by tod.Pid into grouped
                                             select new
                                             {
                                                 pid = grouped.Key,
                                                 count = grouped.Sum(t => t.Pcount),
                                                 image = grouped.FirstOrDefault().Photo0,
                                                 name = grouped.FirstOrDefault().Pname,
                                                 totalprice = grouped.Sum(t => t.Pprice * t.Pcount),
                                                 price = grouped.FirstOrDefault().Pprice
                                             })
                                             .OrderByDescending(x => x.count)
                                             .Take(5)
                                             .ToListAsync();

            ViewBag.SalesCount_TOP5_Month = await (from tod in _db.TorderDetails
                                             join td in _db.Torders on tod.Oid equals td.Oid
                                             where td.Odate.Month == currentMonth && td.Odate.Year == currentYear && tod.Pid != 0
											 group tod by tod.Pid into grouped
                                             select new
                                             {
                                                 pid = grouped.Key,
                                                 count = grouped.Sum(t => t.Pcount),
                                                 image = grouped.FirstOrDefault().Photo0,
                                                 name = grouped.FirstOrDefault().Pname,
                                                 totalprice = grouped.Sum(t => t.Pprice * t.Pcount),
                                                 price = grouped.FirstOrDefault().Pprice
                                             })
                                             .OrderByDescending(x => x.count)
                                             .Take(5)
                                             .ToListAsync();

            ViewBag.SalesCount_TOP5_Year =await (from tod in _db.TorderDetails
                                            join td in _db.Torders on tod.Oid equals td.Oid
                                            where td.Odate >= new DateTime(2025, 1, 1) && td.Odate <= new DateTime(2025, 12, 31) && tod.Pid != 0
											group tod by tod.Pid into grouped
                                            select new
                                            {
                                                pid = grouped.Key,
                                                count = grouped.Sum(t => t.Pcount),
                                                image = grouped.FirstOrDefault().Photo0,
                                                name = grouped.FirstOrDefault().Pname,
                                                totalprice = grouped.Sum(t => t.Pprice * t.Pcount),
                                                price = grouped.FirstOrDefault().Pprice
                                            })
                                            .OrderByDescending(x => x.count)
                                            .Take(5)
                                            .ToListAsync();

            ViewBag.typesales_today =await (from tod in _db.TorderDetails
                                       join tp in _db.Tproducts on tod.Pid equals tp.Pid
                                       join td in _db.Torders on tod.Oid equals td.Oid
                                       where td.Odate.Date == today
                                       group new { tod, tp } by tp.Pcategory into grouped
                                       select new
                                       {
                                           value = grouped.Sum(g => g.tod.Pcount),
                                           name = grouped.Key // 分組鍵即為 Pcategory
                                       }
                                        )
                                        .ToListAsync();

            ViewBag.typesales_month =await (from tod in _db.TorderDetails
                                       join tp in _db.Tproducts on tod.Pid equals tp.Pid
                                       join td in _db.Torders on tod.Oid equals td.Oid
                                       where td.Odate.Month == currentMonth && td.Odate.Year == currentYear
                                       group new { tod, tp } by tp.Pcategory into grouped
                                       select new
                                       {
                                           value = grouped.Sum(g => g.tod.Pcount),
                                           name = grouped.Key // 分組鍵即為 Pcategory
                                       }
                                        )
                                        .ToListAsync();

            ViewBag.typesales_year =await (from tod in _db.TorderDetails
                                      join tp in _db.Tproducts on tod.Pid equals tp.Pid
                                      join td in _db.Torders on tod.Oid equals td.Oid
                                      where td.Odate.Year == currentYear
                                      group new { tod, tp } by tp.Pcategory into grouped
                                      select new
                                      {
                                          value = grouped.Sum(g => g.tod.Pcount),
                                          name = grouped.Key // 分組鍵即為 Pcategory
                                      }
                                        )
                                        .ToListAsync();

            int totalprice_today = _db.Torders.Where(t => t.Odate.Date == today).Sum(t => t.OtotalPrice);
            int totalprice_month = _db.Torders.Where(t => t.Odate.Month == currentMonth && t.Odate.Year == currentYear).Sum(t => t.OtotalPrice);
            int totalprice_year = _db.Torders.Where(t => t.Odate.Year == currentYear).Sum(t => t.OtotalPrice);
            int totalprice_yesterday = _db.Torders.Where(t => t.Odate.Date == today.AddDays(-1)).Sum(t => t.OtotalPrice);
            int totalprice_lastmonth = _db.Torders.Where(t => t.Odate.Month == currentMonth - 1 && t.Odate.Year == currentYear).Sum(t => t.OtotalPrice);
            int totalprice_lastyear = _db.Torders.Where(t => t.Odate.Year == currentYear - 1).Sum(t => t.OtotalPrice);

            int totalsales_today = _db.Torders.Where(t => t.Odate.Date == today).Count();
            int totalsales_month = _db.Torders.Where(t => t.Odate.Month == currentMonth && t.Odate.Year == currentYear).Count();
            int totalsales_year = _db.Torders.Where(t => t.Odate.Year == currentYear).Count();
            int totalsales_yesterday = _db.Torders.Where(t => t.Odate.Date == today.AddDays(-1)).Count();
            int totalsales_lastmonth = _db.Torders.Where(t => t.Odate.Month == currentMonth - 1 && t.Odate.Year == currentYear).Count();
            int totalsales_lastyear = _db.Torders.Where(t => t.Odate.Year == currentYear - 1).Count();


            int Advice_today = _db.Tadvices.Where(t => t.DateTime.Date == today).Count();
            int Advice_month = _db.Tadvices.Where(t => t.DateTime.Month == currentMonth && t.DateTime.Year == currentYear).Count();
            int Advice_year = _db.Tadvices.Where(t => t.DateTime.Year == currentYear).Count();
            int CustService_yesterday = _db.Tchats.Where(t => t.ChatCreateTime.Date == today.AddDays(-1)).Count();
            int CustSercive_lastmonth = _db.Tchats.Where(t => t.ChatCreateTime.Month == currentMonth - 1 && t.ChatCreateTime.Year == currentYear).Count();
            int CustSercive_lastyear = _db.Tchats.Where(t => t.ChatCreateTime.Year == currentYear - 1).Count();


            var viewModel = new IndexViewModel
            {
                Torder = order,
                TorderDetail = orderDetail,
                TotalPrice_Today = totalprice_today,
                TotalPrice_Month = totalprice_month,
                TotalPrice_Year = totalprice_year,
                TotalPrice_Yesterday = totalprice_yesterday,
                TotalPrice_LastMonth = totalprice_lastmonth,
                TotalPrice_LastYear = totalprice_lastyear,
                TotalSales_Today = totalsales_today,
                TotalSales_Month = totalsales_month,
                TotalSales_Year = totalsales_year,
                TotalSales_Yesterday = totalsales_yesterday,
                TotalSales_LastMonth = totalsales_lastmonth,
                TotalSales_LastYear = totalsales_lastyear,
                Torder_none = order_none,
                Tproduct_none = product_none,
                members_count_Month = members_count_Month,
                members_count_Year = members_count_Year,
                month_Days = month_Days,
                product_inventory = product_inventory,
                product_name = product_name,
				Advice_today = Advice_today,
				Advice_month = Advice_month,
				Advice_year = Advice_year,
				Advice_yesterday = CustService_yesterday,
				Advice_lastmonth = CustSercive_lastmonth,
				Advice_lastyear = CustSercive_lastyear,
            };
            return View(viewModel);
        }

		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Login(MemberViewModel m)//存入快取
		{
			
			Tmember T = _db.Tmembers.FirstOrDefault(c => c.Maccount == m.Account && c.Mpassword == m.Password);
			string Error = "false";
			if (T != null && T.Mpassword == m.Password)
			{
				string json = JsonSerializer.Serialize(T);
				HttpContext.Session.SetString(CDictionary.SK_LOGEDIN_USER, json);
                //HttpContext.Session.SetInt32("MemberId", T.Mid);
                return RedirectToAction("Index","Home");
			}
			else
			{
				Error = "true";
				ViewBag.Error = Error;
				return View();
			}

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SingOut()
        {
            HttpContext.Session.Clear();
            bool isSessionCleared = HttpContext.Session.Keys.Count() == 0;
            return Json(new { success = isSessionCleared });
        }

    }
}