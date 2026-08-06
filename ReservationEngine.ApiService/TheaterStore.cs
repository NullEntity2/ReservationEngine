using Microsoft.EntityFrameworkCore;

namespace ReservationEngine.ApiService;

public class TheaterStore(ReservationContext context)
{
    public async Task<Theater[]> GetTheatersAsync(CancellationToken cancellationToken = default)
    {
        return await context.Theaters
            .OrderBy(t => t.Id)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Theater?> GetTheaterAsync(int theaterId, CancellationToken cancellationToken = default)
    {
        return await context.Theaters
            .FirstOrDefaultAsync(t => t.Id == theaterId, cancellationToken);
    }
}
