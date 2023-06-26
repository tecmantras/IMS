using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using UserManagememet.Data.Context;
using UserManagememet.Data.Implementations;
using UserManagememet.Data.Interface;
using UserManagememet.Data.Model;
using UserManagement.Services.IRepositories;
using UserManagement.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.User.AllowedUserNameCharacters = string.Empty;
}).AddEntityFrameworkStores<UserManangementDBContext>()
            .AddDefaultTokenProviders();

//var dbhost = Environment.GetEnvironmentVariable("DB_HOST");
//var dbName = Environment.GetEnvironmentVariable("DB_NAME");
//var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
//var connectionstring = $"Data Source={dbhost};Initial Catalog={dbName};User ID=sa;Password={dbPassword};Trusted_Connection=False; Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
//builder.Services.AddDbContext<UserManangementDBContext>(option =>option.UseSqlServer(connectionstring));

builder.Services.AddDbContext<UserManangementDBContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("userManagementDB"));
});

builder.Services.AddControllers().AddJsonOptions(x =>
               x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IAccountService, AccountService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
