using Microsoft.EntityFrameworkCore;
using ReservationEngine.ApiService;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddScoped<SeatReservationStore>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddNpgsqlDbContext<ReservationContext>(connectionName: "postgresdb");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ReservationContext>();
    await context.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/", () => "API service is running. Navigate to /weatherforecast to see sample data.");

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/api/seats/reserved", async (SeatReservationStore store) => await store.GetReservedSeatsAsync())
    .WithName("GetReservedSeats");

app.MapPost("/api/reservations", async (ReserveSeatsRequest request, SeatReservationStore store) =>
{
    if (request.SeatIds is not { Length: > 0 })
    {
        return Results.BadRequest("At least one seat must be selected.");
    }

    var outcome = await store.TryReserveAsync(request.SeatIds);

    return outcome.Succeeded
        ? Results.Ok(new ReserveSeatsResponse(true, request.SeatIds, []))
        : Results.Conflict(new ReserveSeatsResponse(false, [], outcome.ConflictingSeats));
})
.WithName("ReserveSeats");

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record ReserveSeatsRequest(string[] SeatIds);
record ReserveSeatsResponse(bool Succeeded, string[] ConfirmedSeats, string[] ConflictingSeats);
