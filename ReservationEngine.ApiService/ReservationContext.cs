using Microsoft.EntityFrameworkCore;

namespace ReservationEngine.ApiService;

public class ReservationContext(DbContextOptions<ReservationContext> options) : DbContext(options)
{
    public DbSet<SeatReservation> SeatReservations => Set<SeatReservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SeatReservation>(entity =>
        {
            entity.HasKey(s => s.SeatId);
            entity.Property(s => s.SeatId).HasMaxLength(10);

            entity.HasData(
                SeededSeats.Select(seatId => new SeatReservation
                {
                    SeatId = seatId,
                    ReservedAt = SeedReservationTimestamp
                }));
        });
    }

    private static readonly DateTimeOffset SeedReservationTimestamp = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private static readonly string[] SeededSeats =
    [
        "A3", "A4", "B7", "C1", "C2", "C10", "D5", "D6", "D7",
        "E9", "F2", "F3", "G11", "G12", "H1", "H8"
    ];
}

public class SeatReservation
{
    public required string SeatId { get; set; }
    public DateTimeOffset ReservedAt { get; set; }
}
