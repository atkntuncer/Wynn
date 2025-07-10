using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wynn;

namespace UnitTests
{
	[TestFixture]
	public class FileLoaderTests
	{
		private string _testDirectory;

		[SetUp]
		public void Setup()
		{
			_testDirectory = Path.Combine(Path.GetTempPath(), "FileLoaderTests", Guid.NewGuid().ToString());
			Directory.CreateDirectory(_testDirectory);
		}

		[TearDown]
		public void TearDown()
		{
			if (Directory.Exists(_testDirectory))
			{
				Directory.Delete(_testDirectory, true);
			}
		}

		#region Order Tests

		[Test]
		public void LoadOrders_ValidJsonFile_ReturnsOrders()
		{
			// Arrange
			var jsonContent = @"[
                {
                    ""OrderId"": 1,
                    ""ProductId"": 101,
                    ""Quantity"": 2,
                    ""DeliveryAt"": ""2025-07-10T10:00:00"",
                    ""CreatedAt"": ""2025-07-08T09:00:00"",
                    ""DeliveryAddress"": ""123 Main St""
                },
                {
                    ""OrderId"": 2,
                    ""ProductId"": 102,
                    ""Quantity"": 1,
                    ""DeliveryAt"": ""2025-07-11T14:00:00"",
                    ""CreatedAt"": ""2025-07-08T09:30:00"",
                    ""DeliveryAddress"": ""456 Oak Ave""
                }
            ]";
			var filePath = Path.Combine(_testDirectory, "orders.json");
			File.WriteAllText(filePath, jsonContent);

			// Act
			var orders = FileLoader.LoadOrders(filePath);

			// Assert
			Assert.That(orders.Count, Is.EqualTo(2));
			Assert.That(orders[0].OrderId, Is.EqualTo(1));
			Assert.That(orders[0].ProductId, Is.EqualTo(101));
			Assert.That(orders[0].Quantity, Is.EqualTo(2));
			Assert.That(orders[0].DeliveryAddress, Is.EqualTo("123 Main St"));
		}

		[Test]
		public void LoadOrders_ValidCsvFile_ReturnsOrders()
		{
			// Arrange
			var csvContent = @"OrderId,ProductId,Quantity,DeliveryAt,CreatedAt,DeliveryAddress
1,101,2,2025-07-10T10:00:00,2025-07-08T09:00:00,123 Main St
2,102,1,2025-07-11T14:00:00,2025-07-08T09:30:00,456 Oak Ave";
			var filePath = Path.Combine(_testDirectory, "orders.csv");
			File.WriteAllText(filePath, csvContent);

			// Act
			var orders = FileLoader.LoadOrders(filePath);

			// Assert
			Assert.That(orders.Count, Is.EqualTo(2));
			Assert.That(orders[0].OrderId, Is.EqualTo(1));
			Assert.That(orders[0].ProductId, Is.EqualTo(101));
			Assert.That(orders[0].Quantity, Is.EqualTo(2));
			Assert.That(orders[0].DeliveryAddress, Is.EqualTo("123 Main St"));
		}

		[Test]
		public void LoadOrders_NonExistentFile_ReturnsEmptyList()
		{
			// Arrange
			var filePath = Path.Combine(_testDirectory, "nonexistent.json");

			// Act
			var orders = FileLoader.LoadOrders(filePath);

			// Assert
			Assert.That(orders, Is.Empty);
		}

		[Test]
		public void LoadOrders_UnsupportedFileFormat_ReturnsEmptyList()
		{
			// Arrange
			var filePath = Path.Combine(_testDirectory, "orders.txt");
			File.WriteAllText(filePath, "some content");

			// Act
			var orders = FileLoader.LoadOrders(filePath);

			// Assert
			Assert.That(orders, Is.Empty);
		}

		[Test]
		public void LoadOrders_EmptyJsonFile_ReturnsEmptyList()
		{
			// Arrange
			var filePath = Path.Combine(_testDirectory, "empty.json");
			File.WriteAllText(filePath, "[]");

			// Act
			var orders = FileLoader.LoadOrders(filePath);

			// Assert
			Assert.That(orders, Is.Empty);
		}

		[Test]
		public void LoadOrders_CsvWithInvalidRows_SkipsInvalidRows()
		{
			// Arrange
			var csvContent = @"OrderId,ProductId,Quantity,DeliveryAt,CreatedAt,DeliveryAddress
1,101,2,2025-07-10T10:00:00,2025-07-08T09:00:00,123 Main St
incomplete,row
2,102,1,2025-07-11T14:00:00,2025-07-08T09:30:00,456 Oak Ave";
			var filePath = Path.Combine(_testDirectory, "orders_invalid.csv");
			File.WriteAllText(filePath, csvContent);

			// Act
			var orders = FileLoader.LoadOrders(filePath);

			// Assert
			Assert.That(orders.Count, Is.EqualTo(2));
			Assert.That(orders[0].OrderId, Is.EqualTo(1));
			Assert.That(orders[1].OrderId, Is.EqualTo(2));
		}

		[Test]
		public void LoadOrders_CsvWithMalformedData_HandlesGracefully()
		{
			// Arrange
			var csvContent = @"OrderId,ProductId,Quantity,DeliveryAt,CreatedAt,DeliveryAddress
invalid,101,2,2025-07-10T10:00:00,2025-07-08T09:00:00,123 Main St
1,invalid,2,2025-07-10T10:00:00,2025-07-08T09:00:00,123 Main St";
			var filePath = Path.Combine(_testDirectory, "orders_malformed.csv");
			File.WriteAllText(filePath, csvContent);

			// Act
			var orders = FileLoader.LoadOrders(filePath);

			// Assert
			Assert.That(orders.Count, Is.EqualTo(2));
			Assert.That(orders[0].OrderId, Is.EqualTo(0)); // TryParse failed, defaulted to 0
			Assert.That(orders[1].ProductId, Is.EqualTo(0)); // TryParse failed, defaulted to 0
		}

		#endregion

		#region Product Tests

		[Test]
		public void LoadProducts_ValidJsonFile_ReturnsProducts()
		{
			// Arrange
			var jsonContent = @"[
                {
                    ""ProductId"": 101,
                    ""ProductName"": ""Pizza"",
                    ""Price"": 12.99
                },
                {
                    ""ProductId"": 102,
                    ""ProductName"": ""Burger"",
                    ""Price"": 8.99
                }
            ]";
			var filePath = Path.Combine(_testDirectory, "products.json");
			File.WriteAllText(filePath, jsonContent);

			// Act
			var products = FileLoader.LoadProducts(filePath);

			// Assert
			Assert.That(products.Count, Is.EqualTo(2));
			Assert.That(products[0].ProductId, Is.EqualTo(101));
			Assert.That(products[0].ProductName, Is.EqualTo("Pizza"));
			Assert.That(products[0].Price, Is.EqualTo(12.99m));
		}

		[Test]
		public void LoadProducts_NonExistentFile_ReturnsEmptyList()
		{
			// Arrange
			var filePath = Path.Combine(_testDirectory, "nonexistent_products.json");

			// Act
			var products = FileLoader.LoadProducts(filePath);

			// Assert
			Assert.That(products, Is.Empty);
		}

		[Test]
		public void LoadProducts_EmptyJsonFile_ReturnsEmptyList()
		{
			// Arrange
			var filePath = Path.Combine(_testDirectory, "empty_products.json");
			File.WriteAllText(filePath, "[]");

			// Act
			var products = FileLoader.LoadProducts(filePath);

			// Assert
			Assert.That(products, Is.Empty);
		}

		#endregion

		#region Ingredients Tests

		[Test]
		public void LoadIngredients_ValidJsonFile_ReturnsIngredients()
		{
			// Arrange
			var jsonContent = @"[
                {
                    ""ProductId"": 101,
                    ""Ingredients"": [
                        { ""Ingredient"": ""Cheese"", ""Amount"": 200 },
                        { ""Ingredient"": ""Tomato"", ""Amount"": 100 }
                    ]
                },
                {
                    ""ProductId"": 102,
                    ""Ingredients"": [
                        { ""Ingredient"": ""Beef"", ""Amount"": 150 },
                        { ""Ingredient"": ""Cheese"", ""Amount"": 50 }
                    ]
                }
            ]";
			var filePath = Path.Combine(_testDirectory, "ingredients.json");
			File.WriteAllText(filePath, jsonContent);

			// Act
			var ingredients = FileLoader.LoadIngredients(filePath);

			// Assert
			Assert.That(ingredients.Count, Is.EqualTo(2));
			Assert.That(ingredients[0].ProductId, Is.EqualTo(101));
			Assert.That(ingredients[0].Ingredients.Count, Is.EqualTo(2));
			Assert.That(ingredients[0].Ingredients[0].Ingredient, Is.EqualTo("Cheese"));
			Assert.That(ingredients[0].Ingredients[0].Amount, Is.EqualTo(200));
		}

		[Test]
		public void LoadIngredients_NonExistentFile_ReturnsEmptyList()
		{
			// Arrange
			var filePath = Path.Combine(_testDirectory, "nonexistent_ingredients.json");

			// Act
			var ingredients = FileLoader.LoadIngredients(filePath);

			// Assert
			Assert.That(ingredients, Is.Empty);
		}

		[Test]
		public void LoadIngredients_EmptyJsonFile_ReturnsEmptyList()
		{
			// Arrange
			var filePath = Path.Combine(_testDirectory, "empty_ingredients.json");
			File.WriteAllText(filePath, "[]");

			// Act
			var ingredients = FileLoader.LoadIngredients(filePath);

			// Assert
			Assert.That(ingredients, Is.Empty);
		}

		#endregion

		#region Edge Cases

		[Test]
		public void LoadOrders_JsonFileWithNullValues_HandlesGracefully()
		{
			// Arrange
			var jsonContent = @"[
                {
                    ""OrderId"": 1,
                    ""ProductId"": 101,
                    ""Quantity"": 2,
                    ""DeliveryAt"": ""2025-07-10T10:00:00"",
                    ""CreatedAt"": ""2025-07-08T09:00:00"",
                    ""DeliveryAddress"": null
                }
            ]";
			var filePath = Path.Combine(_testDirectory, "orders_with_nulls.json");
			File.WriteAllText(filePath, jsonContent);

			// Act
			var orders = FileLoader.LoadOrders(filePath);

			// Assert
			Assert.That(orders.Count, Is.EqualTo(1));
			Assert.That(orders[0].DeliveryAddress, Is.Null);
		}

		[Test]
		public void LoadOrders_CaseInsensitiveFileExtensions_WorksCorrectly()
		{
			// Arrange - Test .JSON extension
			var jsonContent = @"[{""OrderId"": 1, ""ProductId"": 101, ""Quantity"": 2, ""DeliveryAt"": ""2025-07-10T10:00:00"", ""CreatedAt"": ""2025-07-08T09:00:00"", ""DeliveryAddress"": ""123 Main St""}]";
			var jsonFilePath = Path.Combine(_testDirectory, "orders.JSON");
			File.WriteAllText(jsonFilePath, jsonContent);

			// Act
			var ordersFromJson = FileLoader.LoadOrders(jsonFilePath);

			// Assert
			Assert.That(ordersFromJson.Count, Is.EqualTo(1));
		}

		#endregion
	}
}
