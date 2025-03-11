using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using Project.ViewModel;

namespace Project.Controllers
{
    public class StyleController : Controller
    {
        private readonly DbuniPayContext _context;
        IWebHostEnvironment _enviro = null;
        public StyleController(IWebHostEnvironment p,DbuniPayContext context)
        {
            _context = context;
            _enviro = p;
        }

        //前台StyleList
        public async Task<IActionResult> StyleList(string category)
        {
            var query = from tsi in _context.TstyleImgs
                        join ts in _context.Tstyles on tsi.Sid equals ts.Sid
                        join tp in _context.Tproducts on ts.Pid equals tp.Pid
                        select new { tsi.Simg, tsi.SimgCategory, tp.Pid, tp.Pname, tp.Pphoto, tp.Pprice };

            // 如果有篩選條件，則進行篩選
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(x => x.SimgCategory == category);
            }

            var styles = query
                         .ToList()
                         .GroupBy(x => x.Simg)
                         .Select(g => new StyleViewModel
                         {
                             SImg = g.Key,
                             Products = g.Select(p => new ProductViewModel
                             {
                                 PID = p.Pid,
                                 PName = p.Pname,
                                 PPhoto = p.Pphoto,
                                 PPrice = p.Pprice
                             }).ToList()
                         }).ToList();

            return View(styles);
        }


        //後台StyleList
        public async Task<IActionResult> List()
        {
            DbuniPayContext db = new DbuniPayContext();
            var styles = (from tsi in _context.TstyleImgs
                          join ts in _context.Tstyles on tsi.Sid equals ts.Sid
                          join tp in _context.Tproducts on ts.Pid equals tp.Pid
                          select new { tsi.Sid, tsi.Simg, tsi.SimgCategory, tp.Pid, tp.Pname, tp.Pphoto, tp.Pprice })
                      .ToList()
                      .GroupBy(x => x.Sid)
                      .Select(g => new StyleViewModel
                      {
                          Sid = g.Key,
                          SImg = g.Select(x => x.Simg).FirstOrDefault(),
                          SimgCategory = g.Select(x => x.SimgCategory).FirstOrDefault() ?? string.Empty,
                          Products = g.Select(p => new ProductViewModel
                          {
                              PID = p.Pid,
                              PName = p.Pname,
                              PPhoto = p.Pphoto,
                              PPrice = p.Pprice
                          }).ToList()
                      }).ToList();

            return View(styles);
        }
    
    }
}
