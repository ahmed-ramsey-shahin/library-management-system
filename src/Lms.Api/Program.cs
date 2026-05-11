using Lms.Api;
using Lms.Application;
using Lms.Application.Common.Configurations;
using Lms.Infrastructure;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<BorrowSettings>().BindConfiguration("BorrowSettings");
builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddApi(builder.Configuration);
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Library Management System API V1");
        options.EnableDeepLinking();
        options.DisplayRequestDuration();
        options.EnableFilter();
    });
    app.MapScalarApiReference();
}
else
{
    app.UseHsts();
}

app.UseCoreMiddlewars(builder.Configuration);
app.MapControllers();
app.UseAntiforgery();

app.Run();
