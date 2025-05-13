using Microsoft.EntityFrameworkCore;
using Project_1;
using Project_1.Data;
using Project_1.Minio;
using Project_1.Repository;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<MedicalDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<RepositoryFactory>();


builder.Services.AddSingleton<ImageService>(provider =>
    new ImageService(
        "localhost:9000",   // MinIO server endpoint
        "admin",            // Access key
        "admin123"          // Secret key
    )
);



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "html/index.html" }
});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
