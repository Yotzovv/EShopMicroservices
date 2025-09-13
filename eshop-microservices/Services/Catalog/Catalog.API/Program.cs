using Catalog.API.BuildingBlocks.ValidationBehavior;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("DefaultConnection")!);
}).UseLightweightSessions();

builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddCarter();

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapCarter();

app.Run();