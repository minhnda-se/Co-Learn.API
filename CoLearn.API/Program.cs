using CoLearn.Infrastructure;
using CoLearn.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container
builder.Services.AddControllers();

// 2. Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Add Infrastructure (DbContext, Repositories, UnitOfWork, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

// 4. Add Services (Application layer services, business logic)
builder.Services.AddServices();

var app = builder.Build();

// 5. Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
