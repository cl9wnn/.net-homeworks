using API.Extensions;
using Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddServices();
builder.Services.AddSwaggerDocumentation(builder.Environment);
builder.Services.AddValidation();
builder.Services.AddPostgreSqlDb(builder.Configuration);
builder.Services.AddMappings();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation(builder.Environment);
    app.Services.ApplyMigrations();
}

app.MapControllers();

app.Run();