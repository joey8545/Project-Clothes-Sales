using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.DTO;
using Project.Models;
using Project.ViewModel;
using System.Security.Cryptography;

namespace Project.Controllers
{
    public class ProductController : Controller
    {
        private readonly DbuniPayContext _context;

        IWebHostEnvironment _enviro = null;
        public ProductController(IWebHostEnvironment p, DbuniPayContext context)
        {
            _enviro = p;
            _context = context;
        }

        //前台ProductList
        public IActionResult ProductList(string keyword, int id, string sortby = "default")
        {
            DbuniPayContext db = new DbuniPayContext();
            var datas = db.Tproducts
                              .Where(t => !t.PisHided) // 過濾已下架產品
                              .AsQueryable();
            // 關鍵字篩選
            if (!string.IsNullOrEmpty(keyword))
            {
                datas = datas.Where(t => t.Pname.Contains(keyword) ||
                                         t.Ptype.Contains(keyword) ||
                                         t.Pcategory.Contains(keyword));
            }

            // 取得銷售數量 (熱銷產品) - 關聯 Torderdetail 表
            var salesData = db.TorderDetails
                              .GroupBy(o => o.Pid)
                              .Select(g => new { Pid = g.Key, TotalSales = g.Sum(o => o.Pcount) })
                              .ToDictionary(x => x.Pid, x => x.TotalSales);

            // 切換到客戶端評估
            var productList = datas.AsEnumerable();

            // 加入排序方式
            switch (sortby.ToLower())
            {
                case "price_asc":  // 價格升序 (低到高)
                    productList = productList.OrderBy(t => t.Pprice);
                    break;
                case "price_desc": // 價格降序 (高到低)
                    productList = productList.OrderByDescending(t => t.Pprice);
                    break;
                case "hot": // 熱銷排序
                    productList = productList.OrderByDescending(t => salesData.ContainsKey(t.Pid) ? salesData[t.Pid] : 0);
                    break;
                default:
                    productList = productList.OrderBy(t => t.Pname); // 預設按名稱排序
                    break;
            }

            // 封裝 CProductWrap
            List<CProductWrap> ProductList = productList
                .Select(t => new CProductWrap() { product = t })
                .ToList();

            ViewBag.Keyword = keyword;
            ViewBag.SortBy = sortby;

            return View(ProductList);
        }

        public IActionResult ProductDetail(int? id)
        {
            if (id == null)
                return RedirectToAction("ProductList");

            DbuniPayContext db = new DbuniPayContext();
            Tproduct x = db.Tproducts.FirstOrDefault(c => c.Pid == id);
            if (x == null)
                return RedirectToAction("ProductList");

            // 查詢對應的圖片列表
            List<string> images = db.Tpimages
                                     .Where(img => img.Pid == id)
                                     .Select(img => img.Piname)
                                     .ToList();

            // 查詢對應的顏色與尺寸（不顯示 Pstock = 0 的）
            var inventory = db.TproductInventories
                              .Where(inv => inv.Pid == id)
                              .ToList();

            List<string> colors = inventory.Select(inv => inv.Pcolor).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();
            List<string> sizes = inventory.Select(inv => inv.Psize).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();

            // 建立顏色+尺寸的庫存數據
            Dictionary<string, int> stockMap = inventory.ToDictionary(inv => $"{inv.Pcolor}-{inv.Psize}", inv => inv.Pstock);

            // 查詢該產品的評價列表
            List<Tcomment> comments = db.Tcomments
                                        .Where(c => c.Pid == id)
                                        .OrderByDescending(c => c.ComCreateDate) // 依創建時間排序
                                        .ToList();

            // 計算評價數量
            int commentCount = comments.Count;

            return View(new CProductWrap()
            {
                product = x,
                Images = images,
                Colors = colors,
                Sizes = sizes,
                StockMap = stockMap,
                Comments = comments, // 傳遞評價列表
                CommentCount = commentCount // 傳遞評價數量
            });
        }


        //後台List
        public IActionResult List(string keyword, int id)
        {
            DbuniPayContext db = new DbuniPayContext();

            // 取得符合條件的產品 (排除下架的)
            var datas = db.Tproducts
                          .Where(t => !t.PisHided);

            if (!string.IsNullOrEmpty(keyword))
            {
                datas = datas.Where(t => t.Pname.Contains(keyword) ||
                                         t.Ptype.Contains(keyword) ||
                                         t.Pcategory.Contains(keyword));
            }

            // 直接用 LINQ Join 取得產品與庫存
            var productList = datas
                .Select(t => new CProductWrap
                {
                    product = t,
                    TproductInventories = db.TproductInventories
                                   .Where(i => i.Pid == t.Pid)
                                   .ToList()
                })
                .ToList();

            return View(productList);
        }

        //後台商品下架
        public IActionResult Hide(int? id)
        {
            if (id != null)
            {
                DbuniPayContext db = new DbuniPayContext();
                Tproduct x = db.Tproducts.FirstOrDefault(c => c.Pid == id);
                if (x != null)
                {
                    x.PisHided = true;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("List");
        }

        // 後台重新上架頁面
        public IActionResult Renew(string keyword)
        {
            DbuniPayContext db = new DbuniPayContext();
            var datas = db.Tproducts
                          .Where(t => t.PisHided);

            if (!string.IsNullOrEmpty(keyword))
            {
                datas = datas.Where(t => t.Pname.Contains(keyword) ||
                                         t.Ptype.Contains(keyword) ||
                                         t.Pcategory.Contains(keyword));
            }
            List<CProductWrap> list = new List<CProductWrap>();
            foreach (var t in datas)
                list.Add(new CProductWrap() { product = t });
            return View(list);
        }

        // 後台重新上架
        public IActionResult Recovery(int id)
        {
            if (id != null)
            {
                DbuniPayContext db = new DbuniPayContext();
                Tproduct x = db.Tproducts.FirstOrDefault(c => c.Pid == id);
                if (x != null)
                {
                    x.PisHided = false;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("List");
        }

        // 後台刪除商品
        public IActionResult Delete(int? id)
        {
            DbuniPayContext db = new DbuniPayContext();
            if (id != null)
            {
                Tproduct x = db.Tproducts.FirstOrDefault(c => c.Pid == id);
                if (x != null)
                {
                    // 刪除對應的 Tpimages 及其圖片檔案
                    var images = db.Tpimages.Where(img => img.Pid == id).ToList();
                    foreach (var img in images)
                    {
                        string imagePath = Path.Combine(_enviro.WebRootPath, "images", img.Piname);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    db.Tpimages.RemoveRange(images);

                    // 刪除對應的 TproductInventories
                    db.TproductInventories.RemoveRange(db.TproductInventories.Where(i => i.Pid == id));

                    // 刪除主圖片
                    if (!string.IsNullOrEmpty(x.Pphoto))
                    {
                        string mainPhotoPath = Path.Combine(_enviro.WebRootPath, "images", x.Pphoto);
                        if (System.IO.File.Exists(mainPhotoPath))
                        {
                            System.IO.File.Delete(mainPhotoPath);
                        }
                    }

                    // 刪除 Tproduct
                    db.Tproducts.Remove(x);

                    db.SaveChanges();
                }
            }

            return RedirectToAction("Renew");
        }
        //後台新增商品
        public IActionResult Create()
        {
            DbuniPayContext db = new DbuniPayContext();
            CProductWrap model = new CProductWrap
            {
                PtypeList = db.Tproducts.Select(p => p.Ptype).Distinct().ToList(),
                PcategoryList = db.Tproducts.Select(p => p.Pcategory).Distinct().ToList(),
                Sizes = db.TproductInventories.Select(i => i.Psize).Distinct().ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CProductWrap p, List<IFormFile> photos)
        {
            DbuniPayContext db = new DbuniPayContext();
            var transaction = db.Database.BeginTransaction();
            try
            {
                if (p.photoPath == null)
                {
                    ModelState.AddModelError("photoPath", "必須上傳照片。");
                    return View(p);
                }

                // 處理主圖上傳
                string photoName = Guid.NewGuid().ToString() + ".jpg";
                p.Pphoto = photoName;
                using (var fileStream = new FileStream(
                    Path.Combine(_enviro.WebRootPath, "images", photoName),
                    FileMode.Create))
                {
                    p.photoPath.CopyTo(fileStream);
                }

                // 設置創建時間
                p.PcreatedDate = DateTime.Now;

                // 先新增產品，讓資料庫生成 Pid
                db.Tproducts.Add(p.product);
                db.SaveChanges();  // 確保 Pid 生成

                // 獲取剛剛插入的產品 ID
                int productId = p.product.Pid;

                // 新增 TProductInventory 記錄
                int totalStock = 0;
                foreach (var stock in p.TproductInventories)
                {
                    var inventory = new TproductInventory
                    {
                        Pid = productId,
                        Pcolor = stock.Pcolor,
                        Psize = stock.Psize,
                        Pstock = stock.Pstock,
                        PlastUpdated = DateTime.Now
                    };
                    db.TproductInventories.Add(inventory);
                    totalStock += stock.Pstock;
                }

                // 更新 Pinventory
                p.product.Pinventory = totalStock;
                db.Tproducts.Update(p.product);

                // 處理多張圖片上傳 (存入 Tpimages)
                if (photos != null && photos.Count > 0)
                {
                    foreach (var photo in photos)
                    {
                        if (photo.Length > 0)
                        {
                            string imageName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                            string imagePath = Path.Combine(_enviro.WebRootPath, "images", imageName);

                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                photo.CopyTo(stream);
                            }

                            // 存入 Tpimage 表
                            Tpimage img = new Tpimage()
                            {
                                Pid = productId,   // 關聯產品 ID
                                Piname = imageName // 存圖片名稱
                            };
                            db.Tpimages.Add(img);
                        }
                    }
                }

                db.SaveChanges(); // 儲存所有數據
                transaction.Commit(); // 交易提交
                                      // 回傳 JSON 給前端
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Json(new { success = false, message = "發生錯誤：" + ex.Message });
            }
        }

        //後台編輯商品
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("List");

            DbuniPayContext db = new DbuniPayContext();
            Tproduct x = db.Tproducts.FirstOrDefault(c => c.Pid == id);
            if (x == null)
                return RedirectToAction("List");

            var productWrap = new CProductWrap()
            {
                product = x,
                PtypeList = db.Tproducts.Select(p => p.Ptype).Distinct().ToList(),
                PcategoryList = db.Tproducts.Select(p => p.Pcategory).Distinct().ToList(),
                Sizes = db.TproductInventories.Select(i => i.Psize).Distinct().ToList(),
                TproductInventories = db.TproductInventories.Where(i => i.Pid == x.Pid).ToList(), // 加入庫存清單
                Images = db.Tpimages.Where(img => img.Pid == x.Pid).Select(img => img.Piname).ToList() // 加入圖片清單
            };
            return View(productWrap);
        }

        [HttpPost]
        public IActionResult Edit(CProductWrap p, List<IFormFile> photos)
        {
            try
            {   DbuniPayContext db = new DbuniPayContext();
                Tproduct x = db.Tproducts.FirstOrDefault(c => c.Pid == p.Pid);
                if (x == null)
                {
                    return Json(new { success = false, message = "找不到該商品" });
                }

                x.Pname = p.Pname;
                x.Pprice = p.Pprice;
                x.Ptype = p.Ptype;
                x.Pdescription = p.Pdescription;
                x.Pcategory = p.Pcategory;

                // **更新主圖片**
                if (p.photoPath != null && p.photoPath.Length > 0)
                {
                    // **刪除舊的主圖片**
                    if (!string.IsNullOrEmpty(x.Pphoto))
                    {
                        string oldPhotoPath = Path.Combine(_enviro.WebRootPath, "images", x.Pphoto);
                        if (System.IO.File.Exists(oldPhotoPath))
                        {
                            System.IO.File.Delete(oldPhotoPath);
                        }
                    }

                    // **儲存新的主圖片**
                    string photoName = Guid.NewGuid().ToString() + Path.GetExtension(p.photoPath.FileName);
                    x.Pphoto = photoName;
                    string savePath = Path.Combine(_enviro.WebRootPath, "images", photoName);

                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        p.photoPath.CopyTo(stream);
                    }
                }

                // **處理附加圖片**
                if (photos != null && photos.Count > 0)
                {
                    var oldImages = db.Tpimages.Where(img => img.Pid == p.Pid).ToList();
                    foreach (var oldImg in oldImages)
                    {
                        string oldImagePath = Path.Combine(_enviro.WebRootPath, "images", oldImg.Piname);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    db.Tpimages.RemoveRange(oldImages);

                    // **儲存新圖片**
                    foreach (var photo in photos)
                    {
                        if (photo.Length > 0)
                        {
                            string imageName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                            string imagePath = Path.Combine(_enviro.WebRootPath, "images", imageName);

                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                photo.CopyTo(stream);
                            }
                            db.Tpimages.Add(new Tpimage { Pid = p.Pid, Piname = imageName });
                        }
                    }
                }

                // **更新商品規格**
                db.TproductInventories.RemoveRange(db.TproductInventories.Where(i => i.Pid == p.Pid));
                if (p.TproductInventories != null && p.TproductInventories.Count > 0)
                {
                    foreach (var inv in p.TproductInventories)
                    {
                        inv.Pid = p.Pid;
                        inv.PlastUpdated = DateTime.Now;
                        db.TproductInventories.Add(inv);
                    }
                }

                db.SaveChanges();

                // **計算並更新總庫存**
                x.Pinventory = db.TproductInventories.Where(i => i.Pid == x.Pid).Sum(i => i.Pstock);
                db.SaveChanges();


                return Json(new { success = true, message = "商品修改成功!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "修改商品時發生錯誤: " + ex.Message });
            }
        }
    }
}
