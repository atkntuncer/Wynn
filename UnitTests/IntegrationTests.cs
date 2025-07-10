using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wynn;
using Wynn.Validation;

namespace UnitTests
{
	[TestFixture]
	public class IntegrationTests
	{
		private string _testDirectory;

		[SetUp]
		public void Setup()
		{
			_testDirectory = Path.Combine(Path.GetTempPath(), "IntegrationTests", Guid.NewGuid().ToString());
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

		#region End-to-End Workflow Tests

		[Test]
		public async Task FullWorkflow_ValidData_ProcessesSuccessfully()
		{
			// Arrange - Create test files
			var ordersJson = @"[
				{""OrderId"": 1, ""ProductId"": 101, ""Quantity"": 2, ""DeliveryAt"": ""2025-07-10T10:00:00"", ""CreatedAt"": ""2025-07-08T09:00:00"", ""DeliveryAddress"": ""123 Main St""},
				{""OrderId"": 1, ""ProductId"": 102, ""Quantity"": 1, ""DeliveryAt"": ""2025-07-10T10:00:00"", ""CreatedAt"": ""2025-07-08T09:00:00"", ""DeliveryAddress"": ""123 Main St""},
				{""OrderId"": 2, ""ProductId"": 101, ""Quantity"": 1, ""DeliveryAt"": ""2025-07-11T14:00:00"", ""CreatedAt"": ""2025-07-08T09:30:00"", ""DeliveryAddress"": ""456 Oak Ave""}
			]";

			var productsJson = @"[
				{""ProductId"": 101, ""ProductName"": ""Pizza"", ""Price"": 12.99},
				{""ProductId"": 102, ""ProductName"": ""Burger"", ""Price"": 8.99}
			]";

			var ingredientsJson = @"[
				{""ProductId"": 101, ""Ingredients"": [{""Ingredient"": ""Cheese"", ""Amount"": 200}, {""Ingredient"": ""Tomato"", ""Amount"": 100}]},
				{""ProductId"": 102, ""Ingredients"": [{""Ingredient"": ""Beef"", ""Amount"": 150}, {""Ingredient"": ""Cheese"", ""Amount"": 50}]}
			]";

			var ordersFile = Path.Combine(_testDirectory, "orders.json");
			var productsFile = Path.Combine(_testDirectory, "products.json");
			var ingredientsFile = Path.Combine(_testDirectory, "ingredients.json");

			File.WriteAllText(ordersFile, ordersJson);
			File.WriteAllText(productsFile, productsJson);
			File.WriteAllText(ingredientsFile, ingredientsJson);

			// Act
			var orders = FileLoader.LoadOrders(ordersFile);
			var products = FileLoader.LoadProducts(productsFile);
			var ingredients = FileLoader.LoadIngredients(ingredientsFile);

			// Validate all data
			await Validate.ValidateAll(orders, products, ingredients);

			// Calculate totals
			var orderTotals = PriceCalculator.CalculateOrderTotals(orders, products);
			var ingredientTotals = PriceCalculator.CalculateIngredientTotals(orders, ingredients);

			// Assert
			Assert.That(orders.Count, Is.EqualTo(3));
			Assert.That(products.Count, Is.EqualTo(2));
			Assert.That(ingredients.Count, Is.EqualTo(2));

			// Order totals
			Assert.That(orderTotals.Count, Is.EqualTo(2));
			Assert.That(orderTotals[1], Is.EqualTo(34.97m)); // (12.99*2) + (8.99*1) = 34.97
			Assert.That(orderTotals[2], Is.EqualTo(12.99m)); // 12.99*1 = 12.99

			// Ingredient totals
			Assert.That(ingredientTotals.Count, Is.EqualTo(2));
			Assert.That(ingredientTotals[1]["Cheese"], Is.EqualTo(450)); // (200*2) + (50*1) = 450
			Assert.That(ingredientTotals[1]["Tomato"], Is.EqualTo(200)); // 100*2 = 200
			Assert.That(ingredientTotals[1]["Beef"], Is.EqualTo(150)); // 150*1 = 150
			Assert.That(ingredientTotals[2]["Cheese"], Is.EqualTo(200)); // 200*1 = 200
			Assert.That(ingredientTotals[2]["Tomato"], Is.EqualTo(100)); // 100*1 = 100
		}

		#endregion

		#region FileLoader and PriceCalculator Integration

		[Test]
		public void FileLoaderAndPriceCalculator_Integration_CalculatesCorrectTotals()
		{
			// Arrange
			var ordersJson = @"[
				{""OrderId"": 1, ""ProductId"": 101, ""Quantity"": 3, ""DeliveryAt"": ""2025-07-10T10:00:00"", ""CreatedAt"": ""2025-07-08T09:00:00"", ""DeliveryAddress"": ""123 Main St""},
				{""OrderId"": 2, ""ProductId"": 102, ""Quantity"": 2, ""DeliveryAt"": ""2025-07-11T14:00:00"", ""CreatedAt"": ""2025-07-08T09:30:00"", ""DeliveryAddress"": ""456 Oak Ave""}
			]";

			var productsJson = @"[
				{""ProductId"": 101, ""ProductName"": ""Pizza"", ""Price"": 15.50},
				{""ProductId"": 102, ""ProductName"": ""Burger"", ""Price"": 9.25}
			]";

			var ordersFile = Path.Combine(_testDirectory, "orders.json");
			var productsFile = Path.Combine(_testDirectory, "products.json");

			File.WriteAllText(ordersFile, ordersJson);
			File.WriteAllText(productsFile, productsJson);

			// Act
			var orders = FileLoader.LoadOrders(ordersFile);
			var products = FileLoader.LoadProducts(productsFile);
			var orderTotals = PriceCalculator.CalculateOrderTotals(orders, products);

			// Assert
			Assert.That(orderTotals[1], Is.EqualTo(46.50m)); // 15.50 * 3
			Assert.That(orderTotals[2], Is.EqualTo(18.50m)); // 9.25 * 2
		}

		[Test]
		public void FileLoaderAndPriceCalculator_CsvAndIngredients_IntegrationTest()
		{
			// Arrange
			var ordersCsv = @"OrderId,ProductId,Quantity,DeliveryAt,CreatedAt,DeliveryAddress
1,101,2,2025-07-10T10:00:00,2025-07-08T09:00:00,123 Main St
1,102,1,2025-07-10T10:00:00,2025-07-08T09:00:00,123 Main St";

			var ingredientsJson = @"[
				{""ProductId"": 101, ""Ingredients"": [{""Ingredient"": ""Flour"", ""Amount"": 500}, {""Ingredient"": ""Water"", ""Amount"": 250}]},
				{""ProductId"": 102, ""Ingredients"": [{""Ingredient"": ""Meat"", ""Amount"": 200}, {""Ingredient"": ""Flour"", ""Amount"": 100}]}
			]";

			var ordersFile = Path.Combine(_testDirectory, "orders.csv");
			var ingredientsFile = Path.Combine(_testDirectory, "ingredients.json");

			File.WriteAllText(ordersFile, ordersCsv);
			File.WriteAllText(ingredientsFile, ingredientsJson);

			// Act
			var orders = FileLoader.LoadOrders(ordersFile);
			var ingredients = FileLoader.LoadIngredients(ingredientsFile);
			var ingredientTotals = PriceCalculator.CalculateIngredientTotals(orders, ingredients);

			// Assert
			Assert.That(orders.Count, Is.EqualTo(2));
			Assert.That(ingredientTotals[1]["Flour"], Is.EqualTo(1100)); // (500*2) + (100*1)
			Assert.That(ingredientTotals[1]["Water"], Is.EqualTo(500)); // 250*2
			Assert.That(ingredientTotals[1]["Meat"], Is.EqualTo(200)); // 200*1
		}

		#endregion

		#region FileLoader and Validation Integration

		[Test]
		public async Task FileLoaderAndValidation_ValidData_PassesValidation()
		{
			// Arrange
			var ordersJson = @"[
				{""OrderId"": 1, ""ProductId"": 101, ""Quantity"": 2, ""DeliveryAt"": ""2025-07-10T10:00:00"", ""CreatedAt"": ""2025-07-08T09:00:00"", ""DeliveryAddress"": ""123 Main St""}
			]";

			var productsJson = @"[
				{""ProductId"": 101, ""ProductName"": ""Pizza"", ""Price"": 12.99}
			]";

			var ingredientsJson = @"[
				{""ProductId"": 101, ""Ingredients"": [{""Ingredient"": ""Cheese"", ""Amount"": 200}]}
			]";

			var ordersFile = Path.Combine(_testDirectory, "orders.json");
			var productsFile = Path.Combine(_testDirectory, "products.json");
			var ingredientsFile = Path.Combine(_testDirectory, "ingredients.json");

			File.WriteAllText(ordersFile, ordersJson);
			File.WriteAllText(productsFile, productsJson);
			File.WriteAllText(ingredientsFile, ingredientsJson);

			// Act & Assert
			Assert.DoesNotThrowAsync(async () =>
			{
				var orders = FileLoader.LoadOrders(ordersFile);
				var products = FileLoader.LoadProducts(productsFile);
				var ingredients = FileLoader.LoadIngredients(ingredientsFile);

				await Validate.ValidateAll(orders, products, ingredients);
			});
		}

		#endregion

		#region Complex Scenarios

		[Test]
		public async Task EdgeCase_MissingProductsInCalculation_HandlesGracefully()
		{
			// Arrange
			var ordersJson = @"[
				{""OrderId"": 1, ""ProductId"": 101, ""Quantity"": 2, ""DeliveryAt"": ""2025-07-10T10:00:00"", ""CreatedAt"": ""2025-07-08T09:00:00"", ""DeliveryAddress"": ""123 Main St""},
				{""OrderId"": 2, ""ProductId"": 999, ""Quantity"": 1, ""DeliveryAt"": ""2025-07-11T14:00:00"", ""CreatedAt"": ""2025-07-08T09:30:00"", ""DeliveryAddress"": ""456 Oak Ave""}
			]";

			var productsJson = @"[
				{""ProductId"": 101, ""ProductName"": ""Pizza"", ""Price"": 12.99}
			]";

			var ingredientsJson = @"[
				{""ProductId"": 101, ""Ingredients"": [{""Ingredient"": ""Cheese"", ""Amount"": 200}]}
			]";

			var ordersFile = Path.Combine(_testDirectory, "orders.json");
			var productsFile = Path.Combine(_testDirectory, "products.json");
			var ingredientsFile = Path.Combine(_testDirectory, "ingredients.json");

			File.WriteAllText(ordersFile, ordersJson);
			File.WriteAllText(productsFile, productsJson);
			File.WriteAllText(ingredientsFile, ingredientsJson);

			// Act
			var orders = FileLoader.LoadOrders(ordersFile);
			var products = FileLoader.LoadProducts(productsFile);
			var ingredients = FileLoader.LoadIngredients(ingredientsFile);

			await Validate.ValidateAll(orders, products, ingredients);

			var orderTotals = PriceCalculator.CalculateOrderTotals(orders, products);
			var ingredientTotals = PriceCalculator.CalculateIngredientTotals(orders, ingredients);

			// Assert
			Assert.That(orderTotals.Count, Is.EqualTo(1)); // Only order 1 has valid product
			Assert.That(orderTotals[1], Is.EqualTo(25.98m)); // 12.99 * 2
			Assert.That(ingredientTotals.Count, Is.EqualTo(1)); // Only order 1 has valid ingredients
			Assert.That(ingredientTotals[1]["Cheese"], Is.EqualTo(400)); // 200 * 2
		}

		#endregion

		#region File Format Integration

		[Test]
		public void MixedFileFormats_CsvOrdersJsonProducts_WorksTogether()
		{
			// Arrange
			var ordersCsv = @"OrderId,ProductId,Quantity,DeliveryAt,CreatedAt,DeliveryAddress
1,101,3,2025-07-10T10:00:00,2025-07-08T09:00:00,123 Main St
2,102,2,2025-07-11T14:00:00,2025-07-08T09:30:00,456 Oak Ave";

			var productsJson = @"[
				{""ProductId"": 101, ""ProductName"": ""Pizza"", ""Price"": 8.75},
				{""ProductId"": 102, ""ProductName"": ""Burger"", ""Price"": 6.25}
			]";

			var ordersFile = Path.Combine(_testDirectory, "orders.csv");
			var productsFile = Path.Combine(_testDirectory, "products.json");

			File.WriteAllText(ordersFile, ordersCsv);
			File.WriteAllText(productsFile, productsJson);

			// Act
			var orders = FileLoader.LoadOrders(ordersFile);
			var products = FileLoader.LoadProducts(productsFile);
			var orderTotals = PriceCalculator.CalculateOrderTotals(orders, products);

			// Assert
			Assert.That(orders.Count, Is.EqualTo(2));
			Assert.That(products.Count, Is.EqualTo(2));
			Assert.That(orderTotals[1], Is.EqualTo(26.25m)); // 8.75 * 3
			Assert.That(orderTotals[2], Is.EqualTo(12.50m)); // 6.25 * 2
		}

		#endregion
	}
}
