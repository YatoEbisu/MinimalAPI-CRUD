using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("Persons"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", async (AppDbContext dbContext) =>
{
    var persons = await dbContext.Persons.ToListAsync();
    return Results.Ok(persons);
}).WithName("GetAll");

app.MapGet("/{id}", async (Guid id, AppDbContext dbContext) =>
{
    var person = await dbContext.Persons.FirstOrDefaultAsync(p => p.Id == id);
    if (person == null) return Results.NotFound();
    return Results.Ok(person);
}).WithName("Get");

app.MapPost("/", async (Person obj, AppDbContext dbContext) =>
{
    dbContext.Persons.Add(obj);
    await dbContext.SaveChangesAsync();
    return Results.Ok(obj);
}).WithName("Create");

app.MapPut("/{id}", async (Guid id, Person obj, AppDbContext dbContext) =>
{
    dbContext.Entry(obj).State = EntityState.Modified;
    await dbContext.SaveChangesAsync();
    return Results.Ok(obj);
}).WithName("Update");

app.MapDelete("/{id}", async (Guid id, AppDbContext dbContext) =>
{
    var person = await dbContext.Persons.FirstOrDefaultAsync(p => p.Id == id);
    if(person == null) return Results.NotFound();

    dbContext.Persons.Remove(person);
    await dbContext.SaveChangesAsync();

    return Results.Ok();
}).WithName("Delete");

app.Run();

public class Person
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Birthday { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions opt) : base(opt)
    {
    }
    public DbSet<Person> Persons { get; set; }
}