using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QLKhachSanAPI.DataAccess;
using QLKhachSanAPI.Models;
using QLKhachSanAPI.Models.DAL;
using QLKhachSanAPI.Services.Implements;
using QLKhachSanAPI.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");
    options.SupportedCultures = new List<System.Globalization.CultureInfo> { new System.Globalization.CultureInfo("en-US") };
    options.SupportedUICultures = new List<System.Globalization.CultureInfo> { new System.Globalization.CultureInfo("en-US") };
});


#region AppDbContext & Identity
var connectionString = builder.Configuration.GetConnectionString("MSSQLConnection") ?? throw new InvalidOperationException("Connection string 'SQLServerConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// to use 'AddDefaultIdentity': install package 'Microsoft.AspNetCore.Identity.UI' !!! NET 7
builder.Services.AddDefaultIdentity<ApplicationUser>()
    //.AddUserManager<CustomUserManager>()    // use the implemented CustomUserManager with password hashing algorithm replaced by SHA256
    //.AddSignInManager<CustomSignInManager<ApplicationUser>>()   // only for MVC projects with cookie auth to use SHA256 password hashing algorithm
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();
#endregion


#region Scoped Services dependency injection:
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthTokenService, AuthTokenService>();
builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IGuestService, GuestService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IReservationRoomService, ReservationRoomService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IBillService, BillService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IGuestServiceService, GuestServiceService>();
builder.Services.AddScoped<IReportService, ReportService>();
//Unit Of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
#endregion

// Add services to the container.

builder.Services.AddControllers();


#region SwaggerUI:
// Add Swagger UI for api debugging:
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo  // Microsoft.OpenApi.Models
    {
        Version = "v1",
        Title = "DotNet_Nhom4" +
        "",
        Description = "Quản lý đặt phòng khách sạn WebApi",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Huynh Quoc Thai - Project Manager",
            Email = "4701104188@student.hcmue.edu.vn",   // string.Empty
          
        },
        License = new OpenApiLicense
        {
            Name = "Use under LICX",
            Url = new Uri("https://example.com/license"),
        }
    });
    // Configuring Swagger UI Authorization with Swagger
    #region Accepting Bearer Token:
    // tutorial: https://code-maze.com/swagger-authorization-aspnet-core
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
            {
                new OpenApiSecurityScheme
                {
                     Reference = new OpenApiReference
                     {
                         Type=ReferenceType.SecurityScheme,
                         Id="Bearer"
                     }
                },
                new string[]{}
            }
    });
    // End of Configuring Authorization with Swagger UI to accept bearerJWT
    #endregion
});
#endregion


#region LocalStorage Jwt Authentication:

var key = Encoding.UTF8.GetBytes(builder.Configuration["ApplicationSettings:JWT_Secret"]); //from appsettings.json

// @Warning: if use Cookie to authenticate private endpoints, disable these line below
// cuz ASP.NET Core can only use ONE auth scheme (LocalSotrage or Cookie) only!!! 

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = false;
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidIssuer = "https://localhost:7000", //some string, normally web url,  
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});
#endregion


#region CORS
builder.Services.AddCors(policy => {
    policy.AddPolicy("defaultPolicy", options => {
        options.AllowAnyHeader();
        options.AllowAnyMethod();
        options.AllowAnyOrigin();
        options.WithHeaders("Access-Control-Allow-Origin");

    });
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors("defaultPolicy");


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed default admin account:
SeedData.EnsurePopulated(app);

app.Run();
