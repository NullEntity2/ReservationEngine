using Microsoft.EntityFrameworkCore;

namespace ReservationEngine.ApiService;

public class SeatReservationStore(ReservationContext context)
{
    public async Task<string[]> GetReservedSeatsAsync(int theaterId, CancellationToken cancellationToken = default)
    {
        return await context.SeatReservations
            .Where(s => s.TheaterId == theaterId)
            .Select(s => s.SeatId)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<SeatReservationOutcome> TryReserveAsync(int theaterId, IReadOnlyCollection<string> seatIds, CancellationToken cancellationToken = default)
    {
        var conflicts = await context.SeatReservations
            .Where(s => s.TheaterId == theaterId && seatIds.Contains(s.SeatId))
            .Select(s => s.SeatId)
            .ToArrayAsync(cancellationToken);

        if (conflicts.Length > 0)
        {
            return new SeatReservationOutcome(false, conflicts);
        }

        var reservedAt = DateTimeOffset.UtcNow;
        context.SeatReservations.AddRange(seatIds.Select(seatId => new SeatReservation
        {
            TheaterId = theaterId,
            SeatId = seatId,
            ReservedAt = reservedAt
        }));

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // A concurrent request reserved one of these seats first; report the real conflicts.
            foreach (var entry in context.ChangeTracker.Entries<SeatReservation>().ToArray())
            {
                entry.State = EntityState.Detached;
            }

            conflicts = await context.SeatReservations
                .Where(s => s.TheaterId == theaterId && seatIds.Contains(s.SeatId))
                .Select(s => s.SeatId)
                .ToArrayAsync(cancellationToken);

            return new SeatReservationOutcome(false, conflicts);
        }

        return new SeatReservationOutcome(true, []);
    }
}

public record SeatReservationOutcome(bool Succeeded, string[] ConflictingSeats);
