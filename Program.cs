using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// --- Add services to the container ---

// 1. Adds the services for your API controllers
builder.Services.AddControllers();

// 2. Adds the CORS policy to allow your Angular app to call the API
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          // This is your Angular app's default address
                          policy.WithOrigins("http://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// 3. Adds the core Authentication and Authorization services
// This is the fix for the crash
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


// --- Build the application ---
var app = builder.Build();


// --- Configure the HTTP request pipeline ---


app.UseHttpsRedirection();

// 4. Adds routing middleware
app.UseRouting();

// 5. Apply the CORS policy
app.UseCors(MyAllowSpecificOrigins);

// 6. Add Authentication middleware (MUST be before Authorization)
app.UseAuthentication();

// 7. Your original line, which will no longer crash
app.UseAuthorization();

// 8. Maps your controller endpoints
app.MapControllers();

app.Run();