using MoviesSearch.Presentation.Components;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MoviesSearch.Core.Interfaces;
using MoviesSearch.Application.UseCases;
using MoviesSearch.Infrastructure.Repositories;
using MoviesSearch.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddHttpClient<IMovieRepository, OmdbClient>();
builder.Services.AddSingleton<ISearchHistoryRepository, InMemorySearchHistoryRepository>();
builder.Services.AddTransient<SearchMovies>();
builder.Services.AddTransient<GetMovieDetails>();
builder.Services.AddTransient<GetLatestSearches>();

// Stub for authentication services
// builder.Services.AddAuthentication(options => { /* Configure auth here */ });
// builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAntiforgery();

//app.MapStaticAssets();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
