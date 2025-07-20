using Polly;
using Polly.Extensions.Http;
using SearchService.Data;
using SearchService.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Đăng ký dịch vụ HttpClient để gọi đến dịch vụ Auction: thêm Policy để tự động Retry khi gặp lỗi tạm thời
builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await DbInititalizer.InitDb(app);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
});
app.Run();

// Tự động Retry khi gặp lỗi: thử lại liên tục mỗi lần cách nhau 3 giây, cho đến khi thành công hoặc bị hủy.
// HandleTransientHttpError: Xử lý các lỗi tạm thời như: lỗi mạng, lỗi server (5xx), timeout
// OrResult: Xử lý các lỗi trả về mã trạng thái HTTP cụ thể, trong đoạn code này là NotFound (404)
static IAsyncPolicy<HttpResponseMessage> GetPolicy()
    => HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
    .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
