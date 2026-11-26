using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
class Program
{
    static string cs = @"Server=(localdb)\MSSQLLocalDB;Database=AutoSalonDB;Trusted_Connection=True;";
    static async Task<int> Exec(SqlConnection c, string sql, params SqlParameter[] p)
    {
        using var cmd = new SqlCommand(sql, c);
        cmd.Parameters.AddRange(p);
        return await cmd.ExecuteNonQueryAsync();
    }
    static async Task<object> Scalar(SqlConnection c, string sql, params SqlParameter[] p)
    {
        using var cmd = new SqlCommand(sql, c);
        cmd.Parameters.AddRange(p);
        return await cmd.ExecuteScalarAsync();
    }
    static SqlParameter P(string name, object value) => new(name, value);
    static async Task Setup(SqlConnection c)
    {
        string sql = @"
IF OBJECT_ID('Brands') IS NULL
CREATE TABLE Brands(
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(50),
    Country NVARCHAR(50));
IF OBJECT_ID('Cars') IS NULL
CREATE TABLE Cars(
    Id INT IDENTITY PRIMARY KEY,
    BrandId INT NOT NULL REFERENCES Brands(Id),
    Model NVARCHAR(50),
    Year INT,
    Mileage INT,
    Price DECIMAL(12,2),
    Color NVARCHAR(30),
    IsSold BIT DEFAULT 0);
IF OBJECT_ID('Customers') IS NULL
CREATE TABLE Customers(
    Id INT IDENTITY PRIMARY KEY,
    FullName NVARCHAR(100),
    Phone NVARCHAR(20),
    Passport NVARCHAR(30));

IF OBJECT_ID('Orders') IS NULL
CREATE TABLE Orders(
    Id INT IDENTITY PRIMARY KEY,
    CarId INT REFERENCES Cars(Id),
    CustomerId INT REFERENCES Customers(Id),
    OrderDate DATETIME DEFAULT GETDATE(),
    FinalPrice DECIMAL(12,2));";
        await Exec(c, sql);
        await Exec(c, @"
IF NOT EXISTS (SELECT 1 FROM Brands)
INSERT INTO Brands(Name, Country) VALUES
('Toyota','Japan'),('BMW','Germany'),
('Mercedes-Benz','Germany'),('Hyundai','South Korea');
IF NOT EXISTS (SELECT 1 FROM Cars)
INSERT INTO Cars(BrandId,Model,Year,Mileage,Price,Color,IsSold)
VALUES
((SELECT Id FROM Brands WHERE Name='Toyota'),'Camry',2023,15000,28000,'White',0),
((SELECT Id FROM Brands WHERE Name='Toyota'),'Corolla',2022,22000,19000,'Grey',0),
((SELECT Id FROM Brands WHERE Name='BMW'),'X5',2024,8000,65000,'Black',0),
((SELECT Id FROM Brands WHERE Name='BMW'),'3 Series',2021,40000,32000,'Blue',1),
((SELECT Id FROM Brands WHERE Name='Mercedes-Benz'),'E-Class',2023,12000,52000,'Silver',0),
((SELECT Id FROM Brands WHERE Name='Hyundai'),'Tucson',2022,25000,24000,'Red',0);
IF NOT EXISTS (SELECT 1 FROM Customers)
INSERT INTO Customers(FullName,Phone,Passport) VALUES
('Ivan Petrov','111111','AA111'),
('Anna Ivanova','222222','BB222'),
('Petr Sidorov','333333','CC333'),
('Maria Smirnova','444444','DD444');");
    }
    static async Task ShowCars(SqlConnection c, string sql,
        params SqlParameter[] p)
    {
        using var cmd = new SqlCommand(sql, c);
        cmd.Parameters.AddRange(p);
        using var r = await cmd.ExecuteReaderAsync();

        while (await r.ReadAsync())
            Console.WriteLine(
                $"{r["Id"]}. {r["Brand"]} {r["Model"]} | " +
                $"{r["Year"]} | {r["Mileage"]} km | " +
                $"{r["Price"]} | {r["Color"]} | " +
                $"{(Convert.ToBoolean(r["IsSold"]) ? "Sold" : "Available")}");
    }
    static string carQuery = @"
SELECT c.Id, b.Name AS Brand, c.Model, c.Year,
c.Mileage, c.Price, c.Color, c.IsSold
FROM Cars c JOIN Brands b ON c.BrandId=b.Id ";
    static async Task Main()
    {
        using var c = new SqlConnection(cs);

        try
        {
            await c.OpenAsync();
            await Setup(c);

            while (true)
            {
                Console.WriteLine(@"
1. All cars
2. Cars by brand
3. Unsold cars
4. Cars by price range
5. Cars from year
6. Average price by brand
7. Most expensive car
8. Create order
9. All orders
0. Exit");

                Console.Write("Choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ShowCars(c, carQuery);
                        break;

                    case "2":
                        Console.Write("Brand: ");
                        string brand = Console.ReadLine();
                        await ShowCars(c, carQuery + "WHERE b.Name=@n",
                            P("@n", brand));
                        break;

                    case "3":
                        await ShowCars(c, carQuery + "WHERE c.IsSold=0");
                        break;

                    case "4":
                        Console.Write("Minimum price: ");
                        decimal min = decimal.Parse(Console.ReadLine());
                        Console.Write("Maximum price: ");
                        decimal max = decimal.Parse(Console.ReadLine());

                        await ShowCars(c, carQuery +
                            "WHERE c.Price BETWEEN @min AND @max",
                            P("@min", min), P("@max", max));
                        break;

                    case "5":
                        Console.Write("Year: ");
                        int year = int.Parse(Console.ReadLine());
                        await ShowCars(c, carQuery + "WHERE c.Year>=@y",
                            P("@y", year));
                        break;

                    case "6":
                        using (var cmd = new SqlCommand(
                            "SELECT Name FROM Brands", c))
                        using (var r = await cmd.ExecuteReaderAsync())
                        {
                            var brands = new List<string>();
                            while (await r.ReadAsync())
                                brands.Add(r.GetString(0));

                            r.Close();

                            foreach (string b in brands)
                            {
                                object avg = await Scalar(c, @"
SELECT AVG(Price) FROM Cars
WHERE BrandId=(SELECT Id FROM Brands WHERE Name=@n)",
                                    P("@n", b));

                                Console.WriteLine($"{b}: " +
                                    (avg == DBNull.Value ? "No cars" :
                                    Convert.ToDecimal(avg).ToString("F2")));
                            }
                        }
                        break;

                    case "7":
                        await ShowCars(c, carQuery +
                            "ORDER BY c.Price DESC OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY");
                        break;

                    case "8":
                        Console.Write("Car ID: ");
                        int carId = int.Parse(Console.ReadLine());
                        Console.Write("Customer ID: ");
                        int customerId = int.Parse(Console.ReadLine());

                        using (var t = (SqlTransaction)await c.BeginTransactionAsync())
                        {
                            try
                            {
                                using var check = new SqlCommand(
                                    "SELECT COUNT(*) FROM Customers WHERE Id=@id",
                                    c, t);
                                check.Parameters.AddWithValue("@id", customerId);

                                if (Convert.ToInt32(await check.ExecuteScalarAsync()) == 0)
                                {
                                    await t.RollbackAsync();
                                    Console.WriteLine("Customer not found.");
                                    break;
                                }

                                using var priceCmd = new SqlCommand(
                                    "SELECT Price FROM Cars WHERE Id=@id AND IsSold=0",
                                    c, t);
                                priceCmd.Parameters.AddWithValue("@id", carId);
                                object price = await priceCmd.ExecuteScalarAsync();

                                if (price == null)
                                {
                                    await t.RollbackAsync();
                                    Console.WriteLine("Car not found or already sold.");
                                    break;
                                }

                                using var update = new SqlCommand(
                                    "UPDATE Cars SET IsSold=1 WHERE Id=@id",
                                    c, t);
                                update.Parameters.AddWithValue("@id", carId);
                                await update.ExecuteNonQueryAsync();

                                using var insert = new SqlCommand(@"
INSERT INTO Orders(CarId,CustomerId,FinalPrice)
VALUES(@car,@customer,@price)", c, t);
                                insert.Parameters.AddWithValue("@car", carId);
                                insert.Parameters.AddWithValue("@customer", customerId);
                                insert.Parameters.AddWithValue("@price", price);
                                await insert.ExecuteNonQueryAsync();

                                await t.CommitAsync();
                                Console.WriteLine("Order created.");
                            }
                            catch
                            {
                                try { await t.RollbackAsync(); } catch { }
                                throw;
                            }
                        }
                        break;

                    case "9":
                        using (var cmd = new SqlCommand(@"
SELECT o.Id, cu.FullName, b.Name, ca.Model,
o.OrderDate, o.FinalPrice
FROM Orders o
JOIN Customers cu ON o.CustomerId=cu.Id
JOIN Cars ca ON o.CarId=ca.Id
JOIN Brands b ON ca.BrandId=b.Id", c))
                        using (var r = await cmd.ExecuteReaderAsync())
                        {
                            while (await r.ReadAsync())
                                Console.WriteLine(
                                    $"{r["Id"]}. {r["FullName"]} | " +
                                    $"{r["Name"]} {r["Model"]} | " +
                                    $"{r["OrderDate"]} | {r["FinalPrice"]}");
                        }
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }
        catch (SqlException e)
        {
            Console.WriteLine($"SQL error {e.Number}: {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
        finally
        {
            await c.CloseAsync();
        }
    }
}
