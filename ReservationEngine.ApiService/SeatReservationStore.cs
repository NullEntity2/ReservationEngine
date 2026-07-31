namespace ReservationEngine.ApiService;

public class SeatReservationStore
{
    private readonly object gate = new();

    private readonly HashSet<string> reservedSeats = new(StringComparer.OrdinalIgnoreCase)
    {
        "A3", "A4", "B7", "C1", "C2", "C10", "D5", "D6", "D7",
        "E9", "F2", "F3", "G11", "G12", "H1", "H8"
    };

    public string[] GetReservedSeats()
    {
        lock (gate)
        {
            return [.. reservedSeats];
        }
    }

    public SeatReservationOutcome TryReserve(IReadOnlyCollection<string> seatIds)
    {
        lock (gate)
        {
            var conflicts = seatIds.Where(reservedSeats.Contains).ToArray();
            if (conflicts.Length > 0)
            {
                return new SeatReservationOutcome(false, conflicts);
            }

            foreach (var seatId in seatIds)
            {
                reservedSeats.Add(seatId);
            }

            return new SeatReservationOutcome(true, []);
        }
    }
}

public record SeatReservationOutcome(bool Succeeded, string[] ConflictingSeats);
