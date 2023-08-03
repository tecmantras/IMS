using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using SignInManagement.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json",optional:false,reloadOnChange:true)
    .AddEnvironmentVariables();
var urls = builder.Configuration.GetValue<string>("FrontEndUrls:urls");
builder.Services.AddCors(op =>
{
    op.AddPolicy("corspolicy", builder => builder.WithOrigins("http://localhost:3000", "http://192.168.1.8:3000").AllowAnyMethod()
    .AllowAnyHeader().AllowCredentials());

});
builder.Services.AddOcelot(builder.Configuration);
builder.Services.ADDCustomJwtAuthentication();
var app = builder.Build();
app.UseCors("corspolicy");

await app.UseOcelot();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
