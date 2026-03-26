var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres(ResourceNames.Postgres);
var postgresdb = postgres.AddDatabase(ResourceNames.Database);

var api = builder.AddGolangApp(ResourceNames.Api, "../Demo.Backend")
  .WithHttpEndpoint(port: 8081, env: "PORT")
  .WithReference(postgresdb)
  .WaitFor(postgresdb);

builder.AddJavaScriptApp(ResourceNames.Web, "../Demo.Frontend")
  .WithHttpEndpoint(port: 5173, env: "PORT")
  .WithReference(api)
  .WaitFor(api);

builder.Build().Run();