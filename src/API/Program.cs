using API.Private.MinimalModule;
using API.Private.Swagger;
using Application;
using Application.Pipeline.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterModules();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerOptions(builder.Environment);

builder.Services.InjectDependencies(builder.Configuration, builder.Environment);
builder.Services.AddHealthChecks();

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*")
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

app.AddSwagger();
app.MapSwaggerDoc();

app.MapEndpoints();

app.Run();
