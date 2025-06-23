using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Controllers
{
	public class PaymentController : Controller
	{
		private readonly DbuniPayContext _db;
		public PaymentController(DbuniPayContext db) 
		{
			_db = db;
		}
		public IActionResult Payment()
		{
			return View();
		}

		public async Task<IActionResult> GetWeek()
		{
			var today = DateTime.Today;
			int diff = today.DayOfWeek - DayOfWeek.Monday;
			if (diff < 0)
			{
				diff += 7;
			}
			var monday = today.AddDays(-diff);
			var sunday = monday.AddDays(6);
			var WeekDataDate = new List<string>();
			var WeekDataPrice = new List<int>();
			for (DateTime date = monday; date <= sunday; date = date.AddDays(1))
			{
				WeekDataDate.Add(date.ToString("MM/dd"));
				var price = await _db.Torders.Where(c => c.Opayment == true && c.Odate.Date == date).SumAsync(c => c.OtotalPrice);
				WeekDataPrice.Add(price);
			}
			return Json(new { price = WeekDataPrice, date = WeekDataDate });
		}

		public async Task<IActionResult> GetMonth()
		{
			var today = DateTime.Today;
			var currentMonth = DateTime.Today.Month;
			var currentYear = DateTime.Today.Year;
			var firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);
			var lastDayOfMonth = new DateTime(currentYear, currentMonth, DateTime.DaysInMonth(currentYear, currentMonth));
			var WeekDataDate = new List<string>();
			var WeekDataPrice = new List<int>();
			for (DateTime date = firstDayOfMonth; date <= lastDayOfMonth; date = date.AddDays(1))
			{
				WeekDataDate.Add(date.ToString("MM/dd"));
				var price = await _db.Torders.Where(c => c.Opayment == true && c.Odate.Date == date).SumAsync(c => c.OtotalPrice);
				WeekDataPrice.Add(price);
			}
			return Json(new { price = WeekDataPrice, date = WeekDataDate });
		}

		public async Task<IActionResult> GetYear()
		{
			var currentYear = DateTime.Today.Year;


			var WeekDataDate = new List<string>();
			var WeekDataPrice = new List<int>();
			for (int month = 1; month <= 12; month++)
			{
				var price = await _db.Torders.Where(c => c.Opayment == true && c.Odate.Month == month && c.Odate.Year == currentYear).SumAsync(c => c.OtotalPrice);
				WeekDataPrice.Add(price);
			}
			return Json(new { price = WeekDataPrice });
		}

		public async Task<IActionResult> DateSearch([FromQuery] string start, [FromQuery] string end)
		{
			var dateStart = Convert.ToDateTime(start);
			var dateEnd = Convert.ToDateTime(end);
			var WeekDataDate = new List<string>();
			var WeekDataPrice = new List<int>();
			for (var date = dateStart; date <= dateEnd; date = date.AddDays(1))
			{
				WeekDataDate.Add(date.ToString("MM-dd"));
				var price = await _db.Torders
									.Where(c => c.Opayment == true && c.Odate.Date == date.Date) 
									.Select(c => (int?)c.OtotalPrice) 
									.SumAsync() ?? 0;
				WeekDataPrice.Add(price);
			}
			return Json(new { price = WeekDataPrice, date = WeekDataDate });
		}
	}
}
