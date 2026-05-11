using Lms.Api;
using Lms.Application;
using Lms.Application.Common.Configurations;
using Lms.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<BorrowSettings>().BindConfiguration("BorrowSettings");
builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddApi(builder.Configuration);
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.UseCoreMiddlewars(builder.Configuration);

if (app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.MapControllers();

app.Run();
