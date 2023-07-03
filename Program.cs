var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// File upload max size, these settings will take affect on all APIs
// https://github.com/dotnet/aspnetcore/issues/20369#issuecomment-607057822
builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
{
   options.Limits.MaxRequestBodySize = 1_000_000; // if don't set default value is: 30 MB
});
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(x =>
{
   x.ValueLengthLimit = 1_000_000;
   x.MultipartBodyLengthLimit = 1_000_000; // if don't set default value is: 128 MB
   x.MultipartHeadersLengthLimit = 1_000_000;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
