using Microsoft.EntityFrameworkCore;
using MessageAppBackend.Database;
using MessageAppBackend.DbModels;
using Microsoft.AspNetCore.Identity;
using MessageAppBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MessageAppBackend.Services.Interfaces;
using MessageAppBackend;
using Microsoft.OpenApi.Models;
using NLog.Web;
using MessageAppBackend.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseNLog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MessageAppDbContext>((opt) =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    opt.UseSqlServer(connectionString);
});

builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<MessageAppDbContext>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!)),

        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],

        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],

        ValidateLifetime = true,

        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autoryzacja JWT wykorzystuj¹ca schemat Bearer. WprowadŸ 'Bearer' [spacja] a nastêpnie swój token w polu poni¿ej.\n\n Przyk³ad: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header, 
        Type = SecuritySchemeType.Http, 
        Scheme = "Bearer", 
        BearerFormat = "JWT" 
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MessageApp API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
