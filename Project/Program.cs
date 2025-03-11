
using Project.Controllers;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using Project.Hubs;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(); // ? 確保支援 JObject

builder.Services.AddMemoryCache(); // ? 註冊記憶體快取

// 註冊 IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// 啟用 Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(45); // 45 分鐘不操作自動清除
    options.Cookie.HttpOnly = true; // 只能透過 HTTP 存取，防止 XSS 攻擊
    options.Cookie.IsEssential = true; // 確保 Session 在 GDPR 下仍然有效
});

// 添加基本的日誌服務，只輸出到控制台和偵錯視窗
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});
// Add services to the DI container.
builder.Services.AddDbContext<DbuniPayContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
// 在這裡替換原來的 AddSession() 配置
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the DI container.
builder.Services.AddDbContext<DbuniPayContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbuniPay"));
});
// WS 服務啟動
//builder.Services.AddSingleton<ChatController>();
//加入 SignalR
builder.Services.AddSignalR();
//允許跨域請求
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins("http://127.0.0.1:5500") // 指定允許的前端來源
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials(); // 允許憑證
    });
});
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
//跨域啟動
app.UseCors("AllowAll");
//加入 Hub
app.MapHub<ChatHub>("/chatroom");

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();