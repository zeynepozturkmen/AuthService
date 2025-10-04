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
                PermitLimit = 100,                      // toplam izin sayýsý
                Window = TimeSpan.FromMinutes(1),       // pencere uzunluðu
                SegmentsPerWindow = 6,                  // pencere 6 segmente bölünür (örn. 10sn)
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0                          // fazla istekleri sýraya alma (0 => direkt reddet)
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

// Middleware : RateLimit  kontrolü
app.UseRateLimiter();
app.MapControllers();

app.Run();
