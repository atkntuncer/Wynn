using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wynn;
using Wynn.Models;

namespace UnitTests
{
	[TestFixture]
	public class PriceCalculatorTests
	{
		#region CalculateOrderTotals Tests

		[Test]
		public void CalculateOrderTotals_ValidOrdersAndProducts_ReturnsCorrectTotals()
		{
			// Arrange
			var orders = new List<Order>
			{
				new Order { OrderId = 1, ProductId = 101, Quantity = 2 },
				new Order { OrderId = 2, ProductId = 102, Quantity = 1 }
			};

			var products = new List<Product>
			{
				new Product { ProductId = 101, ProductName = "Pizza", Price = 12.99m },
				new Product { ProductId = 102, ProductName = "Burger", Price = 8.99m }
			};

			// Act
			var result = PriceCalculator.CalculateOrderTotals(orders, products);

			// Assert
			Assert.That(result.Count, Is.EqualTo(2));
			Assert.That(result[1], Is.EqualTo(25.98m)); // 12.99 * 2
			Assert.That(result[2], Is.EqualTo(8.99m));  // 8.99 * 1
		}

		[Test]
		public void CalculateOrderTotals_MultipleOrdersWithSameOrderId_AggregatesCorrectly()
		{
			// Arrange
			var orders = new List<Order>
			{
				new Order { OrderId = 1, ProductId = 101, Quantity = 2 },
				new Order { OrderId = 1, ProductId = 102, Quantity = 1 },
				new Order { OrderId = 1, ProductId = 101, Quantity = 1 }
			};

			var products = new List<Product>
			{
				new Product { ProductId = 101, ProductName = "Pizza", Price = 10.00m },
				new Product { ProductId = 102, ProductName = "Burger", Price = 5.00m }
			};

			// Act
			var result = PriceCalculator.CalculateOrderTotals(orders, products);

			// Assert
			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result[1], Is.EqualTo(35.00m)); // (10*2) + (5*1) + (10*1) = 35
		}

		[Test]
		public void CalculateOrderTotals_OrderWithNonExistentProduct_SkipsOrder()
		{
			// Arrange
			var orders = new List<Order>
			{
				new Order { OrderId = 1, ProductId = 101, Quantity = 2 },
				new Order { OrderId = 2, ProductId = 999, Quantity = 1 } // Non-existent product
			};

			var products = new List<Product>
			{
				new Product { ProductId = 101, ProductName = "Pizza", Price = 12.99m }
			};

			// Act
			var result = PriceCalculator.CalculateOrderTotals(orders, products);

			// Assert
			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result.ContainsKey(1), Is.True);
			Assert.That(result.ContainsKey(2), Is.False);
			Assert.That(result[1], Is.EqualTo(25.98m));
		}

		[Test]
		public void CalculateOrderTotals_EmptyOrders_ReturnsEmptyDictionary()
		{
			// Arrange
			var orders = new List<Order>();
			var products = new List<Product>
			{
				new Product { ProductId = 101, ProductName = "Pizza", Price = 12.99m }
			};

			// Act
			var result = PriceCalculator.CalculateOrderTotals(orders, products);

			// Assert
			Assert.That(result, Is.Empty);
		}

		[Test]
		public void CalculateOrderTotals_EmptyProducts_ReturnsEmptyDictionary()
		{
			// Arrange
			var orders = new List<Order>
			{
				new Order { OrderId = 1, ProductId = 101, Quantity = 2 }
			};
			var products = new List<Product>();

			// Act
			var result = PriceCalculator.CalculateOrderTotals(orders, products);

			// Assert
			Assert.That(result, Is.Empty);
		}

		[Test]
		public void CalculateOrderTotals_ZeroQuantity_AddsZeroToTotal()
		{
			// Arrange
			var orders = new List<Order>
			{
				new Order { OrderId = 1, ProductId = 101, Quantity = 0 }
			};

			var products = new List<Product>
			{
				new Product { ProductId = 101, ProductName = "Pizza", Price = 12.99m }
			};

			// Act
			var result = PriceCalculator.CalculateOrderTotals(orders, products);

			// Assert
			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result[1], Is.EqualTo(0m));
		}

		#endregion

		#region CalculateIngredientTotals Tests

		[Test]
		public void CalculateIngredientTotals_ValidOrdersAndIngredients_ReturnsCorrectTotals()
		{
			// Arrange
			var orders = new List<Order>
			{
				new Order { OrderId = 1, ProductId = 101, Quantity = 2 },
				new Order { OrderId = 2, ProductId = 102, Quantity = 1 }
			};

			var ingredients = new List<ProductIngredients>
			{
				new ProductIngredients
				{
					ProductId = 101,
					Ingredients = new List<IngredientInfo>
					{
						new IngredientInfo { Ingredient = "Cheese", Amount = 100 },
						new IngredientInfo { Ingredient = "Tomato", Amount = 50 }
					}
				},
				new ProductIngredients
				{
					ProductId = 102,
					Ingredients = new List<IngredientInfo>
					{
						new IngredientInfo { Ingredient = "Beef", Amount = 150 }
					}
				}
			};

			// Act
			var result = PriceCalculator.CalculateIngredientTotals(orders, ingredients);

			// Assert
			Assert.That(result.Count, Is.EqualTo(2));
			Assert.That(result[1].Count, Is.EqualTo(2));
			Assert.That(result[1]["Cheese"], Is.EqualTo(200)); // 100 * 2
			Assert.That(result[1]["Tomato"], Is.EqualTo(100)); // 50 * 2
			Assert.That(result[2]["Beef"], Is.EqualTo(150));   // 150 * 1
		}

		[Test]
		public void CalculateIngredientTotals_MultipleOrdersWithSameOrderId_AggregatesIngredients()
		{
			// Arrange
			var orders = new List<Order>
			{
				new Order { OrderId = 1, ProductId = 101, Quantity = 2 },
				new Order { OrderId = 1, ProductId = 102, Quantity = 1 }
			};

			var ingredients = new List<ProductIngredients>
			{
				new ProductIngredients
				{
					ProductId = 101,
					Ingredients = new List<IngredientInfo>
					{
						new IngredientInfo { Ingredient = "Cheese", Amount = 100 }
					}
				},
				new ProductIngredients
				{
					ProductId = 102,
					Ingredients = new List<IngredientInfo>
					{
						new IngredientInfo { Ingredient = "Cheese", Amount = 50 }
					}
				}
			};

			// Act
			var result = PriceCalculator.CalculateIngredientTotals(orders, ingredients);

			// Assert
			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result[1]["Cheese"], Is.EqualTo(250)); // (100*2) + (50*1) = 250
		}

		[Test]
		public void CalculateIngredientTotals_OrderWithNonExistentProduct_SkipsOrderAndPrintsMessage()
		{
			// Arrange
			var orders = new List<Order>
			{
				new Order { OrderId = 1, ProductId = 101, Quantity = 2 },
				new Order { OrderId = 2, ProductId = 999, Quantity = 1 } // Non-existent product
			};

			var ingredients = new List<ProductIngredients>
			{
				new ProductIngredients
				{
					ProductId = 101,
					Ingredients = new List<IngredientInfo>
					{
						new IngredientInfo { Ingredient = "Cheese", Amount = 100 }
					}
				}
			};

			// Act
			var result = PriceCalculator.CalculateIngredientTotals(orders, ingredients);

			// Assert
			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result.ContainsKey(1), Is.True);
			Assert.That(result.ContainsKey(2), Is.False);
			Assert.That(result[1]["Cheese"], Is.EqualTo(200));
		}

		[Test]
		public void CalculateIngredientTotals_EmptyOrders_ReturnsEmptyDictionary()
		{
			// Arrange
			var orders = new List<Order>();
			var ingredients = new List<ProductIngredients>
			{
				new ProductIngredients
				{
					ProductId = 101,
					Ingredients = new List<IngredientInfo>
					{
						new IngredientInfo { Ingredient = "Cheese", Amount = 100 }
					}
				}
			};

			// Act
			var result = PriceCalculator.CalculateIngredientTotals(orders, ingredients);

			// Assert
			Assert.That(result, Is.Empty);
		}

		[Test]
		public void CalculateIngredientTotals_EmptyIngredients_ReturnsEmptyDictionary()
		{
			// Arrange
			var orders = new List<Order>
			{
				new Order { OrderId = 1, ProductId = 101, Quantity = 2 }
			};
			var ingredients = new List<ProductIngredients>();

			// Act
			var result = PriceCalculator.CalculateIngredientTotals(orders, ingredients);

			// Assert
			Assert.That(result, Is.Empty);
		}

		[Test]
		public void CalculateIngredientTotals_CaseInsensitiveIngredientNames_AggregatesCorrectly()
		{
			// Arrange
			var orders = new List<Order>
			{
				new Order { OrderId = 1, ProductId = 101, Quantity = 1 },
				new Order { OrderId = 1, ProductId = 102, Quantity = 1 }
			};

			var ingredients = new List<ProductIngredients>
			{
				new ProductIngredients
				{
					ProductId = 101,
					Ingredients = new List<IngredientInfo>
					{
						new IngredientInfo { Ingredient = "Cheese", Amount = 100 }
					}
				},
				new ProductIngredients
				{
					ProductId = 102,
					Ingredients = new List<IngredientInfo>
					{
						new IngredientInfo { Ingredient = "CHEESE", Amount = 50 } // Different case
					}
				}
			};

			// Act
			var result = PriceCalculator.CalculateIngredientTotals(orders, ingredients);

			// Assert
			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result[1].Count, Is.EqualTo(1)); // Should be aggregated due to case-insensitive comparison
			Assert.That(result[1]["Cheese"], Is.EqualTo(150)); // 100 + 50
		}

		[Test]
		public void CalculateIngredientTotals_ZeroQuantity_AddsZeroAmount()
		{
			// Arrange
			var orders = new List<Order>
			{
				new Order { OrderId = 1, ProductId = 101, Quantity = 0 }
			};

			var ingredients = new List<ProductIngredients>
			{
				new ProductIngredients
				{
					ProductId = 101,
					Ingredients = new List<IngredientInfo>
					{
						new IngredientInfo { Ingredient = "Cheese", Amount = 100 }
					}
				}
			};

			// Act
			var result = PriceCalculator.CalculateIngredientTotals(orders, ingredients);

			// Assert
			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result[1]["Cheese"], Is.EqualTo(0)); // 100 * 0
		}

		[Test]
		public void CalculateIngredientTotals_EmptyIngredientsListForProduct_CreatesEmptyOrderEntry()
		{
			// Arrange
			var orders = new List<Order>
			{
				new Order { OrderId = 1, ProductId = 101, Quantity = 2 }
			};

			var ingredients = new List<ProductIngredients>
			{
				new ProductIngredients
				{
					ProductId = 101,
					Ingredients = new List<IngredientInfo>() // Empty ingredients list
				}
			};

			// Act
			var result = PriceCalculator.CalculateIngredientTotals(orders, ingredients);

			// Assert
			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result[1], Is.Empty); // Order exists but no ingredients
		}

		#endregion
	}
}
