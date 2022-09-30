using Freelance.Api.Extensions;
using Npgsql;
using System.IdentityModel.Tokens.Jwt;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
NpgsqlConnection.GlobalTypeMapper.UseNetTopologySuite();

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();
app.Configure(builder.Configuration);
app.MapGet("/", () => "Hi! Add /swagger in your URL adress for use Swagger");
app.Run();
