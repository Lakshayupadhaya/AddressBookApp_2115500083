using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using FluentValidation.AspNetCore;
using FluentValidation;
using AutoMapper;
using BusinessLayer.Mapping;
using BusinessLayer.Interface;
using BusinessLayer.Service;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using BusinessLayer.Email;
using BusinessLayer.RabbitMQ;
using StackExchange.Redis;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;
using ModelLayer.Validator;
using RepositoryLayer.Helper;

var builder = WebApplication.CreateBuilder(args);

//  Configure Logging Correctly
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

//  Register Services
builder.Services.AddScoped<IAddressBookBL, AddressBookBL>();
builder.Services.AddScoped<IAddressBookRL, AddressBookRL>();
builder.Services.AddScoped<IUserAutharisationBL, UserAutharisationBL>();
builder.Services.AddScoped<IUserAutharisationRL, UserAutharisationRL>();
builder.Services.AddScoped<Jwt>();
builder.Services.AddScoped<EmailHelper>();

//  Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<AddressBookValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

//  Manually register AutoMapper
var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new AutoMapperProfile()));
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

//  Configure Database Connection
var connectionString = builder.Configuration.GetConnectionString("SqlConnection");
builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(connectionString));

//  Configure Redis
var redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
{
    EndPoints = { "localhost:6379" },
    AbortOnConnectFail = false,
    ConnectTimeout = 15000,   // Increased connection timeout
    SyncTimeout = 10000,      // Increased synchronous timeout
    AsyncTimeout = 10000,
    KeepAlive = 180           // Keep connection alive
});
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

// Commented alternative Redis setup
// builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(
//     builder.Configuration["Redis:ConnectionString"]));
// builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

//  Configure RabbitMQ
builder.Services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory()
{
    HostName = "localhost",
    DispatchConsumersAsync = true // Allow async processing
});

//  Register RabbitMQ Connection
builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = sp.GetRequiredService<IConnectionFactory>();
    return factory.CreateConnection(); // Create and return the RabbitMQ connection
});

//  Register RabbitMQ Channel
builder.Services.AddSingleton<IModel>(sp =>
{
    var connection = sp.GetRequiredService<IConnection>();
    return connection.CreateModel(); // Create and return the RabbitMQ channel
});

//  Register Producer and Consumer
builder.Services.AddSingleton<Producer>();
builder.Services.AddHostedService<Consumer>();

// Adding authentication (commented for now)
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer
// (options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,  // We can also disable this by false and not give issuer in our token
//         ValidateAudience = true, // We can also disable this by false and not give audience in our token
//         ValidateLifetime = true,
//         ValidIssuer = builder.Configuration["Jwt:Issuer"],
//         ValidAudience = builder.Configuration["Jwt:Audience"],
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//     };
// });

//  Add Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//  Configure Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
