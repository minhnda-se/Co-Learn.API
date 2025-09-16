using CoLearn.API.Middlewares;
using CoLearn.Infrastructure;
using CoLearn.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.WriteIndented = true;
    });

// 2. Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Add Infrastructure (DbContext, Repositories, UnitOfWork, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

// 4. Add Services (Application layer services, business logic)
builder.Services.AddServices(builder.Configuration);

// 5. Add CORS (AllowAll cho dev/test)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

// 6. Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 🔹 Bật CORS ở đây
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
