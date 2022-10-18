using BlogManagement.Context;
using BlogManagement.Repositories;
using BlogManagement.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IUserRegistrationAsyncRepository, UserRegistrationAsyncRepository>();
builder.Services.AddScoped<IUserLoginAsyncRepository,UserLoginAsyncRepository>();
builder.Services.AddScoped<IBlogPostAsyncRepository, BlogPostAsyncRepository>();


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

app.UseAuthorization();

app.MapControllers();

app.Run();
