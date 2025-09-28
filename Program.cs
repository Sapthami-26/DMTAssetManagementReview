using DMTAssetManagement.Repositories;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Add services to the container. ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Dependency Injection for the Repository
builder.Services.AddScoped<IAssetRepository, AssetRepository>();

// --- 2. Build the application ---
var app = builder.Build();

// --- 3. Configure the HTTP request pipeline. ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Map the controllers
app.MapControllers();

app.Run();