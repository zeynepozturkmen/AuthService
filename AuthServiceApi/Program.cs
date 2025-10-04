using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Rate Limiting ekle
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: httpContext.Request.Headers["X-Api-Key"].ToString() ?? "unknown",
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 100,                      // toplam izin say�s�
                Window = TimeSpan.FromMinutes(1),       // pencere uzunlu�u
                SegmentsPerWindow = 6,                  // pencere 6 segmente b�l�n�r (�rn. 10sn)
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0                          // fazla istekleri s�raya alma (0 => direkt reddet)
            }));

    options.RejectionStatusCode = 429; // Too Many Requests
});

builder.Services.AddControllers();
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

app.UseHttpsRedirection();

app.UseAuthorization();

// Middleware : RateLimit  kontrol�
app.UseRateLimiter();
app.MapControllers();

app.Run();
