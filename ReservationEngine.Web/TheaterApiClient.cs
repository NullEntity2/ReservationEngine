namespace ReservationEngine.Web;

public class TheaterApiClient(HttpClient httpClient)
{
    public async Task<TheaterDto[]> GetTheatersAsync(CancellationToken cancellationToken = default)
    {
        var theaters = await httpClient.GetFromJsonAsync<TheaterDto[]>("/api/theaters", cancellationToken);
        return theaters ?? [];
    }

    public async Task<TheaterDto?> GetTheaterAsync(int theaterId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"/api/theaters/{theaterId}", cancellationToken);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<TheaterDto>(cancellationToken)
            : null;
    }
}

public record TheaterDto(int Id, string Name, int RowCount, int ColumnsPerRow);
