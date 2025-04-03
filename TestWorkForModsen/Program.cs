using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestWorkForModsen.Repository;
using TestWorkForModsen.Tools;
using Microsoft.OpenApi.Models;
using TestWorkForModsen.Middleware;
using TestWorkForModsen.Services;
using FluentValidation;
using TestWorkForModsen.Data.Models.DTOs;
using TestWorkForModsen.Services.Services;
using TestWorkForModsen.Data.Repository;
using Microsoft.AspNetCore.Identity;
using TestWorkForModsen.Data;
using TestWorkForModsen.Models;
using System.Security.Cryptography.X509Certificates;
using TestWorkForModsen.Data.Data;
using TestWorkForModsen.Data.Models;
using TestWorkForModsen.Services.Validators;
using TestWorkForModsen.Data.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("TestWorkForModsen.Data")));
if (builder.Environment.IsDevelopment() && Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
{
    builder.WebHost.UseUrls("http://localhost:8080");
}
// Репозитории
builder.Services.AddScoped<IAccountRepository<Account>, AccountRepository>();
builder.Services.AddScoped<IEventRepository<Event>, EventRepository>();
builder.Services.AddScoped<IUserRepository<User>, UserRepository>();
builder.Services.AddScoped<IConnectorEventUserRepository<ConnectorEventUser>, ConnectorEventUserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAuthRepository<RefreshToken>, AuthRepository>();

// Сервисы
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IConnectorEventUserService, ConnectorEventUserService>();
builder.Services.AddScoped<IConnectorEventUserRepository<ConnectorEventUser>, 
    ConnectorEventUserRepository>(); 
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
// Валидаторы
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddScoped<IValidator<AccountDto>, AccountValidator>();
builder.Services.AddScoped<IValidator<EventDto>, EventValidator>();
builder.Services.AddScoped<IValidator<EventUpdateDto>, EventUpdateValidator>();
builder.Services.AddScoped<IValidator<ConnectorEventUserDto>, ConnectorEventUserDtoValidator>();
builder.Services.AddScoped<IValidator<ConnectorEventUserCreateDto>, ConnectorEventUserCreateDtoValidator>();
builder.Services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidator>();
builder.Services.AddScoped<IValidator<RegisterRequestDto>, RegisterRequestValidator>();
builder.Services.AddScoped<IValidator<PaginationDto>, PaginationValidator>();
builder.Services.AddScoped<IValidator<EventDto>, EventValidator>();
builder.Services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidator>();
builder.Services.AddScoped<IValidator<EventCreateDto>, EventCreateDtoValidator>();
builder.Services.AddValidatorsFromAssembly(typeof(AccountValidator).Assembly);
builder.Services.AddScoped<IValidator<RefreshToken>, RefreshTokenValidator>();
// AutoMapper (одна регистрация для всех профилей)
builder.Services.AddAutoMapper(assemblies: AppDomain.CurrentDomain.GetAssemblies());
// Специальные сервисы
builder.Services.AddSingleton<IPasswordHasher<Account>, PasswordHasher<Account>>();
builder.Services.AddHttpClient("AccountApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:8081"); 
    //Тут при продакшене нужно будет указать публичный адресс
});
builder.Services.AddAuthorization();
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
// Настраиваем аутентификацию
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero 
        };
    });
builder.Services.AddAuthorization(options =>
{
    // Только для админов
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    // Только для обычных пользователей
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    // Для всех авторизованных (админы + пользователи)
    options.AddPolicy("AnyAuthenticated", policy => policy.RequireAuthenticatedUser());
});
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TestWork Events API", Version = "v1" });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("https://localhost:8081", "http://localhost:8080")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
// Применяем миграции при запуске приложения
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        Console.WriteLine("Начинаю применение миграции");
        dbContext.Database.Migrate();
        Console.WriteLine("Миграция применена");
    }
    catch (Exception ex)
    {
        Console.WriteLine("При приминении миграции что-то пошло не так:", ex.Message);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Error");
    app.UseHsts();
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TestWorkForModsen API v1"));
}

// Инициализация базы данных
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DatabaseContext>();
        var passwordHasher = new PasswordHasher<User>();
        DbInitializer.Initialize(context, passwordHasher); 
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
//При запуске в VS будет выдаваться ошибка, связанная с тем,
//что VS выдает собственный сертификат разработчика и игнорирует создаваемый в композиции