using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using UserManagememet.Data.Context;
using UserManagememet.Data.Implementations;
using UserManagememet.Data.Interface;
using UserManagememet.Data.Model;
using UserManagement.Services.IRepositories;
using UserManagement.Services.Repositories;
using SignInManagement.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddCors(op =>
{
    op.AddPolicy("corpolicy", builder => builder.WithOrigins("http://localhost:3000", "http://192.168.1.8:3000").AllowAnyMethod()
    .AllowAnyHeader().AllowCredentials());
    //op.AddPolicy("corpolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod()
    //.AllowAnyHeader().AllowCredentials());

});
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
    options.SignIn.RequireConfirmedEmail = true;
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
builder.Services.ADDCustomJwtAuthentication();
builder.Services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IEmailHelper, EmailHelper>();
var app = builder.Build();
app.UseCors("corpolicy");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
