using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json",optional:false,reloadOnChange:true)
    .AddEnvironmentVariables();
builder.Services.AddCors(op =>
{
    op.AddPolicy("corspolicy", builder => builder.WithOrigins("http://localhost:3000").AllowAnyMethod()
    .AllowAnyHeader().AllowCredentials());

});
builder.Services.AddOcelot(builder.Configuration);
var app = builder.Build();
app.UseCors("corspolicy");

await app.UseOcelot();

app.Run();
