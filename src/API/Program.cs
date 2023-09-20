using API.Private.MinimalModule;
using API.Private.Swagger;
using Application;
using Application.Pipeline.Middlewares;
using Domain.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel server options
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(30); // Set the request timeout (e.g., 5 minutes)
});

builder.Services.InjectDependencies(builder.Configuration, builder.Environment);

// Add services to the container.
builder.Services.RegisterModules();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerOptions(builder.Environment);
builder.Services.AddHealthChecks();

builder.Services.AddCors(p => p.AddPolicy("corsapp", cors =>
{
    cors.WithOrigins(builder.Configuration.GetAllowedOrigins())
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders("Content-Disposition", "Content-Type");
}));

var app = builder.Build();

//app.UseExceptionHandler("/Error");

//HTTP Strict Transport Security : Method used by server to declare that they should only be accessed using HTTPS (secure connection) only
app.UseHsts();
app.UseHttpsRedirection();

//app.UseStaticFiles();

app.UseRouting();
app.UseCors("corsapp");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddlewares();
app.UseHealthChecks("/health");

if (builder.Environment.IsDevelopment())
{
    app.UseSwaggerOptions();
    app.MapSwaggerDoc();
}

app.MapEndpoints();

app.Run();
