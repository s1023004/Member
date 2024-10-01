var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// 在这里注册所有服务
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();
// 添加内存缓存（Session 需要依赖它）
builder.Services.AddDistributedMemoryCache();  // 使用内存缓存来存储 Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 设置 Session 过期时间
    options.Cookie.HttpOnly = true;  // 设置 Session Cookie 只能通过 HTTP 访问
    options.Cookie.IsEssential = true; // 标记为必要 Cookie，用户隐私设置下不会阻止
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//app.UseSession();  // 使用 Session 中间件
// 注册 IHttpClientFactory 服务

app.UseHttpsRedirection();
app.UseStaticFiles();

// 使用 Session 中间件
app.UseSession();  // 确保中间件已启用

// 其他中间件
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
