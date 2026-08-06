using Microsoft.EntityFrameworkCore;

namespace ReservationEngine.ApiService;

public class ReservationContext(DbContextOptions<ReservationContext> options) : DbContext(options)
{
    public DbSet<Theater> Theaters => Set<Theater>();
    public DbSet<SeatReservation> SeatReservations => Set<SeatReservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Theater>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).HasMaxLength(100);

            entity.HasData(SeededTheaters);
        });

        modelBuilder.Entity<SeatReservation>(entity =>
        {
            entity.HasKey(s => new { s.TheaterId, s.SeatId });
            entity.Property(s => s.SeatId).HasMaxLength(10);

            entity.HasOne<Theater>()
                .WithMany()
                .HasForeignKey(s => s.TheaterId);

            entity.HasData(
                SeededSeats.SelectMany(theaterSeats => theaterSeats.SeatIds.Select(seatId => new SeatReservation
                {
                    TheaterId = theaterSeats.TheaterId,
                    SeatId = seatId,
                    ReservedAt = SeedReservationTimestamp
                })));
        });
    }

    private static readonly DateTimeOffset SeedReservationTimestamp = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private static readonly Theater[] SeededTheaters =
    [
        new() { Id = 1, Name = "Main Auditorium", RowCount = 8, ColumnsPerRow = 12 },
        new() { Id = 2, Name = "IMAX", RowCount = 10, ColumnsPerRow = 14 },
        new() { Id = 3, Name = "Studio 3", RowCount = 6, ColumnsPerRow = 10 },
    ];

    private static readonly (int TheaterId, string[] SeatIds)[] SeededSeats =
    [
        (1, ["A3", "A4", "B7", "C1", "C2", "C10", "D5", "D6", "D7", "E9", "F2", "F3", "G11", "G12", "H1", "H8"]),
        (2, ["A1", "A2", "B5", "C8", "C9", "D3", "E12", "F6", "F7", "G1"]),
        (3, ["A5", "B2", "B3", "C9", "D1", "E6"]),
    ];
}

public class Theater
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int RowCount { get; set; }
    public int ColumnsPerRow { get; set; }
}

public class SeatReservation
{
    public int TheaterId { get; set; }
    public required string SeatId { get; set; }
    public DateTimeOffset ReservedAt { get; set; }
}
