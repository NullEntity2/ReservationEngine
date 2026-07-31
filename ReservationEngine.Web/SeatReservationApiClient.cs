namespace ReservationEngine.Web;

public class SeatReservationApiClient(HttpClient httpClient)
{
    public async Task<HashSet<string>> GetReservedSeatsAsync(CancellationToken cancellationToken = default)
    {
        var seats = await httpClient.GetFromJsonAsync<string[]>("/api/seats/reserved", cancellationToken);
        return new HashSet<string>(seats ?? [], StringComparer.OrdinalIgnoreCase);
    }

    public async Task<ReserveSeatsResult> ReserveSeatsAsync(IReadOnlyCollection<string> seatIds, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/reservations", new ReserveSeatsRequest([.. seatIds]), cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<ReserveSeatsResponse>(cancellationToken);

        return new ReserveSeatsResult(
            response.IsSuccessStatusCode,
            payload?.ConflictingSeats ?? []);
    }
}

public record ReserveSeatsRequest(string[] SeatIds);
public record ReserveSeatsResponse(bool Succeeded, string[] ConfirmedSeats, string[] ConflictingSeats);
public record ReserveSeatsResult(bool Succeeded, string[] ConflictingSeats);
