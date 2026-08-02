var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume();
var postgresdb = postgres.AddDatabase("postgresdb");

var apiService = builder.AddProject<Projects.ReservationEngine_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.ReservationEngine_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
