using AuctionService.Data;
using Microsoft.EntityFrameworkCore;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>((opt) => {
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMassTransit((x) => {
    x.UsingRabbitMq((context, cfg) => {
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

try
{
    DbInitializer.InitDb(app);
}
catch (System.Exception e)
{
    System.Console.WriteLine(e);
}

app.Run();
