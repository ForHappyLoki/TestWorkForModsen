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
using TestWorkForModsen.Repository;
using TestWorkForModsen.Services;
using FluentValidation;
using TestWorkForModsen.Data.Models.DTOs;
using TestWorkForModsen.Data.Models.Validators;
using TestWorkForModsen.Services.Services;
using TestWorkForModsen.Data.Models.Mappings;
using TestWorkForModsen.Data.Repository;
using Microsoft.AspNetCore.Identity;
using TestWorkForModsen.Data;
using TestWorkForModsen.Models;
using TestWorkForModsen.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("TestWorkForModsen.Data")));

// �����������
builder.Services.AddScoped<IRepository<Account>, AccountRepository>();
builder.Services.AddScoped<IRepository<Event>, EventRepository>();
builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IRepository<ConnectorEventUser>, ConnectorEventUserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// �������
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IConnectorEventUserService, ConnectorEventUserService>();
builder.Services.AddScoped<IConnectorEventUserRepository<ConnectorEventUser>, 
    ConnectorEventUserRepository>();
// ����������
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
// AutoMapper (���� ����������� ��� ���� ��������)
builder.Services.AddAutoMapper(assemblies: AppDomain.CurrentDomain.GetAssemblies());
// ����������� �������
builder.Services.AddSingleton<IPasswordHasher<Account>, PasswordHasher<Account>>();
builder.Services.AddHttpClient("AccountApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:8081"); 
    //��� ��� ���������� ����� ����� ������� ��������� ������
});
builder.Services.AddAuthorization();
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
// ����������� ��������������
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
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TestWork Events API", Version = "v1" });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
// ��������� �������� ��� ������� ����������
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        Console.WriteLine("������� ���������� ��������");
        dbContext.Database.Migrate();
        Console.WriteLine("�������� ���������");
    }
    catch (Exception ex)
    {
        Console.WriteLine("��� ���������� �������� ���-�� ����� �� ���:", ex.Message);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TestWork Events API v1"));
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

// ������������� ���� ������
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DatabaseContext>();
        DbInitializer.Initialize(context); 
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

app.UseAuthorization();

app.MapRazorPages();

app.Run();
