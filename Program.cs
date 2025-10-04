using Microsoft.AspNetCore.OpenApi;
//using Serilog;
using RestFullApi.Logging;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


////Adding serilog
////  Call CreateLogger() to get an ILogger instance
//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
//    .WriteTo.File("log/villaLogs.txt",rollingInterval:RollingInterval.Day).CreateLogger();
//builder.Host.UseSerilog();



//builder.Services.AddControllers();
//builder.Services.AddControllers().AddNewtonsoftJson(); //here AddNewtonsoftJson is used for input or output function
builder.Services.AddControllers(
    option =>
    {
        //option.ReturnHttpNotAcceptable=true; //for plain text
    }).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters(); // for xml formate support

builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ILogging, LoggingV2>();


//builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
