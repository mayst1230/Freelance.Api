using Freelance.Api.Extensions;
using Npgsql;
using System.IdentityModel.Tokens.Jwt;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
NpgsqlConnection.GlobalTypeMapper.UseNetTopologySuite();

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();
app.Configure(builder.Configuration);
app.Run();

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1050:Declare types in namespaces", Justification = "<Pending>")]
public partial class Program { }
