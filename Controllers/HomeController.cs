using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Text;
//using System.Text.Json;
using Member.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var client = _httpClientFactory.CreateClient();
        var loginData = new { id = model.id, password = model.password };

        var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://exam-api.deta-it.com.tw/api/Authenticate/Login", content);

        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadAsStringAsync();
            // 存储 Token 在 Session 或 Cookie 中
            HttpContext.Session.SetString("JWTToken", token);
            return RedirectToAction("Index", "Home");
        }

        // 登录失败处理
        ModelState.AddModelError(string.Empty, "Invalid login attempt");
        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // 从 Session 获取 JWT Token
        var token = HttpContext.Session.GetString("JWTToken");
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();  // 如果没有 token，返回未授权
        }

        // 创建 HttpClient 实例
        var client = _httpClientFactory.CreateClient();
        token = token.Replace("\"", "");
        // 设置 Authorization Header
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 创建请求体
        var requestBody = new { keyword = "" }; // 替换为实际的关键字

        var response = await client.PostAsJsonAsync("https://exam-api.deta-it.com.tw/api/Member/GetList", requestBody);

        // 检查响应状态
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<List<Member1>>();
            return View(result);  // 返回视图，并将成员数据传递给它
        }
        else
        {
            // 获取错误信息
            var errorContent = await response.Content.ReadAsStringAsync();

            // 输出调试信息
            Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            Console.WriteLine($"Error content: {errorContent}");

            // 记录错误并返回错误视图
            ModelState.AddModelError("", $"Error: {response.StatusCode}, Details: {errorContent}");
            return View("Error");
        }        
    }
    [HttpPost]

    public async Task<IActionResult> Index(string keyword)
    {
        try
        {
            if (keyword == null)
            {
                keyword = "";
            }
            // 从 Session 获取 JWT Token
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();  // 如果没有 token，返回未授权
            }

            // 创建 HttpClient 实例
            var client = _httpClientFactory.CreateClient();
            token = token.Replace("\"" , "");
            // 设置 Authorization Header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);

            // 创建请求体
            var requestBody = new { keyword = keyword }; // 替换为实际的关键字

            var response = await client.PostAsJsonAsync("https://exam-api.deta-it.com.tw/api/Member/GetList", requestBody);

            // 检查响应状态
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<Member1>>();
                return View(result);  // 返回视图，并将成员数据传递给它
            }
            else
            {
                // 获取错误信息
                var errorContent = await response.Content.ReadAsStringAsync();

                // 输出调试信息
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                Console.WriteLine($"Error content: {errorContent}");

                // 记录错误并返回错误视图
                ModelState.AddModelError("", $"Error: {response.StatusCode}, Details: {errorContent}");
                return View("Error");
            }
        }
        catch(Exception ex)
        {
            string message = ex.Message;
            ModelState.AddModelError("", message);
            return View("Error");
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Member1 member)
    {
        // 从 Session 获取 JWT Token
        var token = HttpContext.Session.GetString("JWTToken");
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();  // 如果没有 token，返回未授权
        }

        // 创建 HttpClient 实例
        var client = _httpClientFactory.CreateClient();
        token = token.Replace("\"", "");
        // 设置 Authorization Header
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 创建请求体
        var requestBody = new { keyword = "" }; // 替换为实际的关键字

        var response = await client.PostAsJsonAsync("https://exam-api.deta-it.com.tw/api/Member/GetList", requestBody);

        // 检查响应状态
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<List<Member1>>();
            foreach(var item in result)
            {
                if(item.id == member.id)
                {
                    // 使用 TempData 传递错误消息
                    TempData["ErrorMessage"] = "帳號已有人使用";
                    return View();
                }
            }
        }
        else
        {
            // 获取错误信息
            var errorContent = await response.Content.ReadAsStringAsync();

            // 输出调试信息
            Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            Console.WriteLine($"Error content: {errorContent}");

            // 记录错误并返回错误视图
            ModelState.AddModelError("", $"Error: {response.StatusCode}, Details: {errorContent}");
            return View("Error");
        }
        member.pk = "";
        member.enable = "Y";        

        // 假设这是调用外部 API 的 URL
        string apiUrl = "https://exam-api.deta-it.com.tw/api/Member/Set";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        // 将会员对象序列化为 JSON
        var jsonContent = JsonConvert.SerializeObject(member);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // 设置 Authorization Header（如果需要）
        // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your_token");

        // 发送 POST 请求
        response = await client.PostAsync(apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            TempData["Message"] = "會員新增成功";
            return View();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return BadRequest($"外部 API 调用失败: {errorContent}");
        }        
    }    
    public async Task<IActionResult> Details(string pk)
    {
        var token = HttpContext.Session.GetString("JWTToken");
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();  // 如果没有 token，返回未授权
        }

        // 创建 HttpClient 实例
        var client = _httpClientFactory.CreateClient();
        token = token.Replace("\"", "");
        // 设置 Authorization Header
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 创建请求体
        var requestBody = new { pk = pk }; // 替换为实际的关键字

        var response = await client.PostAsJsonAsync("https://exam-api.deta-it.com.tw/api/Member/Get", requestBody);

        // 检查响应状态
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<Member1>();
            return View(result);  // 返回视图，并将成员数据传递给它
        }
        else
        {
            // 获取错误信息
            var errorContent = await response.Content.ReadAsStringAsync();

            // 输出调试信息
            Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            Console.WriteLine($"Error content: {errorContent}");

            // 记录错误并返回错误视图
            ModelState.AddModelError("", $"Error: {response.StatusCode}, Details: {errorContent}");
            return View("Error");
        }
        
    }

    public async Task<IActionResult> Edit(string pk)
    {
        var token = HttpContext.Session.GetString("JWTToken");
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();  // 如果没有 token，返回未授权
        }

        // 创建 HttpClient 实例
        var client = _httpClientFactory.CreateClient();
        token = token.Replace("\"", "");
        // 设置 Authorization Header
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 创建请求体
        var requestBody = new { pk = pk }; // 替换为实际的关键字

        var response = await client.PostAsJsonAsync("https://exam-api.deta-it.com.tw/api/Member/Get", requestBody);

        // 检查响应状态
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<Member1>();            
            return View(result);  // 返回视图，并将成员数据传递给它
        }
        else
        {
            // 获取错误信息
            var errorContent = await response.Content.ReadAsStringAsync();

            // 输出调试信息
            Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            Console.WriteLine($"Error content: {errorContent}");

            // 记录错误并返回错误视图
            ModelState.AddModelError("", $"Error: {response.StatusCode}, Details: {errorContent}");
            return View("Error");
        }

    }
    [HttpPost]
    public async Task<IActionResult> Edit(Member1 member)
    {
        var token = HttpContext.Session.GetString("JWTToken");
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();  // 如果没有 token，返回未授权
        }

        // 创建 HttpClient 实例
        var client = _httpClientFactory.CreateClient();
        token = token.Replace("\"", "");
        // 设置 Authorization Header
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 创建请求体        
        var content = new StringContent(JsonConvert.SerializeObject(member), Encoding.UTF8, "application/json");
        var response = await client.PostAsJsonAsync("https://exam-api.deta-it.com.tw/api/Member/Set", member);

        // 检查响应状态
        if (response.IsSuccessStatusCode)
        {            
            return View(member);  // 返回视图，并将成员数据传递给它
        }
        else
        {
            // 获取错误信息
            var errorContent = await response.Content.ReadAsStringAsync();

            // 输出调试信息
            Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            Console.WriteLine($"Error content: {errorContent}");

            // 记录错误并返回错误视图
            ModelState.AddModelError("", $"Error: {response.StatusCode}, Details: {errorContent}");
            return View("Error");
        }

    }

    public async Task<IActionResult> Delete(string pk)
    {
        var token = HttpContext.Session.GetString("JWTToken");
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();  // 如果没有 token，返回未授权
        }

        // 创建 HttpClient 实例
        var client = _httpClientFactory.CreateClient();
        token = token.Replace("\"", "");
        // 设置 Authorization Header
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 创建请求体
        var requestBody = new { pk = pk }; // 替换为实际的关键字

        var response = await client.PostAsJsonAsync("https://exam-api.deta-it.com.tw/api/Member/Delete", requestBody);

        // 检查响应状态
        if (response.IsSuccessStatusCode)
        { 
            return Json("刪除成功");  // 返回视图，并将成员数据传递给它
        }
        else
        {
            // 获取错误信息
            var errorContent = await response.Content.ReadAsStringAsync();

            // 输出调试信息
            Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            Console.WriteLine($"Error content: {errorContent}");

            // 记录错误并返回错误视图
            ModelState.AddModelError("", $"Error: {response.StatusCode}, Details: {errorContent}");
            return View("Error");
        }

    }
}