using MudBlazor.Services;
using Mythologist_Client_WASM.Client.Services;
using Mythologist_Client_WASM.Components;
using Mythologist_Client_WASM.Hubs;
using Mythologist_Client_WASM.Services;
using SharedLogic.Services;
using Microsoft.Extensions.Azure;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

//Enable the [ApiController] (So we can have a webserver to do discord api authentication)
builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
});

builder.Services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);

builder.Services.AddSingleton(sp => new HttpClient());
builder.Services.AddSingleton<IDatabaseConnectionService, DatabaseConnectionService>(provider => new DatabaseConnectionService(builder.Configuration));
builder.Services.AddSingleton<IClientsService, ClientsService>();
builder.Services.AddSingleton<IGameRoomService, GameRoomService>();

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["StorageConnectionString:blob"]!, preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["StorageConnectionString:queue"]!, preferMsi: true);
});

var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapControllers();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapHub<GameHub>("/gamehub");

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Mythologist_Client_WASM.Client._Imports).Assembly);

app.Run();
