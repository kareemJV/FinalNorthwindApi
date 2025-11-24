using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// CORS Aktiv
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddScoped<IDbConnection>(_ =>

    new SqlConnection(builder.Configuration.GetConnectionString("Sql"))

);

var app = builder.Build();

app.UseCors();
app.MapGet("/", () => Results.Ok("API Running"));


app.MapGet("/api/products", async (IDbConnection db, string? search) =>
{
    var baseSql = """
                SELECT TOP 50 p.productID, p.productName, p.unitPrice, p.unitsInStock, c.categoryName, s.contactName AS SupplierName
                FROM Products p
                JOIN Suppliers s ON p.SupplierID = s.SupplierID
                JOIN Categories c ON p.CategoryID = c.CategoryID
""";
    if (!string.IsNullOrWhiteSpace(search))
    {
        var sql = baseSql + " WHERE (p.productName LIKE @term OR c.categoryName LIKE @term OR s.ContactName LIKE @term) ORDER BY p.productID";
        var term = $"%{search}%";
        var result = await db.QueryAsync(sql, new { term });
        return Results.Ok(result);
    }
    var finalSql = baseSql + " ORDER BY p.productID";
    var all = await db.QueryAsync(finalSql);
    return Results.Ok(all);
});


app.MapGet("/api/products/{id}", async (IDbConnection db, int id) =>
{
    var sql = """
                SELECT p.productID, p.productName, p.unitPrice, p.unitsInStock, c.categoryName, s.contactName AS SupplierName
                FROM Products p
                JOIN Suppliers s ON p.SupplierID = s.SupplierID
                JOIN Categories c ON p.CategoryID = c.CategoryID
                WHERE p.productID = @id
""";
    var product = await db.QuerySingleOrDefaultAsync(sql, new { id });
    return product is null ? Results.NotFound() : Results.Ok(product);
});






app.MapGet("/api/customers", async (IDbConnection db) =>
{

    var sql = """


SELECT *
FROM Customers

""";

    var a = await db.QueryAsync(sql);
    return Results.Ok(a);

});









app.Run();