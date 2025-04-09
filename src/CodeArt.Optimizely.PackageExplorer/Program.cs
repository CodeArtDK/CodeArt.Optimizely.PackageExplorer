using CodeArt.Optimizely.PackageExplorer;
using CodeArt.Optimizely.PackageExplorer.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<PackageService>();
builder.Services.AddMudServices();

await builder.Build().RunAsync();
