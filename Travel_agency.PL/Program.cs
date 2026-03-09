using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Travel_agency.BLL;
using Travel_agency.BLL.Abstractions;
using Travel_agency.BLL.Auth;
using Travel_agency.BLL.Services;
using Travel_agency.DataAccess;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Repository;
using Travel_agency.PL;
using Travel_agency.PL.Middlewares;
using Travel_agency.PL.Models.Requests;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TravelAgencyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalConnection"))); // Use Local or Default connection string 

builder.Services.AddScoped<IHotelBookingRepository, HotelBookingRepository>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IHotelRoomRepository, HotelRoomRepository>();
builder.Services.AddScoped<ITicketBookingRepository, TicketBookingRepository>();
builder.Services.AddScoped<ITourBookingRepository, TourBookingRepository>();
builder.Services.AddScoped<ITourRepository, TourRepository>();
builder.Services.AddScoped<ITransportRepository, TransportRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(BLLMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(PLMappingProfile).Assembly);

builder.Services.AddScoped<IHotelBookingService, HotelBookingService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IHotelRoomService, HotelRoomService>();
builder.Services.AddScoped<ITicketBookingService, TicketBookingService>();
builder.Services.AddScoped<ITourBookingService, TourBookingService>();
builder.Services.AddScoped<ITourQueryService, TourQueryService>();
builder.Services.AddScoped<ITourService, TourService>();
builder.Services.AddScoped<ITransportService, TransportService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJWTProvider, JWTProvider>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    error = "Unauthorized",
                    statusCode = 401,
                    message = "Token is missing or invalid"
                });
                return context.Response.WriteAsync(result);
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    error = "Forbidden",
                    statusCode = 403,
                    message = "You are not allowed to access this resource"
                });
                return context.Response.WriteAsync(result);
            }
        };

    });


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Input your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    c.MapType<LoginRequest>(() => new OpenApiSchema
    {
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema>
        {
            { "email", new OpenApiSchema { Type = "string", Format = "email" } },
            { "password", new OpenApiSchema { Type = "string" } }
        }
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TravelAgencyDbContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();