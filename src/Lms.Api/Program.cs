using Lms.Application;
using Lms.Application.Common.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<BorrowSettings>().BindConfiguration("BorrowSettings");
builder.Services.AddApplicationLayer();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
