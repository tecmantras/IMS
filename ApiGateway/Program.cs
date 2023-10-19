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
    op.AddPolicy("corspolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod()
    .AllowAnyHeader());

});
builder.Services.AddOcelot(builder.Configuration);
builder.Services.ADDCustomJwtAuthentication();
var app = builder.Build();
app.UseCors("corspolicy");
app.UseStaticFiles();
await app.UseOcelot();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
