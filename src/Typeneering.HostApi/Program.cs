using Serilog;
using Typeneering.HostApi.Endpoints;
using Typeneering.HostApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddApiServices(builder.Configuration)
                .AddAppServices(builder.Configuration)
                .ServicesConfiguration();

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseSerilogRequestLogging();

#if DEBUG
app.MapOpenApi().AllowAnonymous();
app.UseStatusCodePages();
app.UseSwagger();
app.UseSwaggerUI();
app.MapSwagger().AllowAnonymous();
#endif

// TODO: Configure Postgres in deeper detail and pgadmin email(?) and nginx workers, open ports and general production settings
// TODO: Setup SSL in reverse-proxy
// TODO: Setup jwt for production, like security keys
// TODO: Setup Logging configuration for production
// TODO: Remove github api keys
// app.UseHttpsRedirection();
// app.UseHsts();

app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler();


app.MapGroup("/user").MapUserRoutes();
app.MapGroup("/session").MapSessionRoutes();
app.MapGroup("/user-preference").MapUserPreferenceRoutes();

await app.RunAsync();
