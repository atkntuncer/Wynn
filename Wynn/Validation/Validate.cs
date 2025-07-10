
using Wynn.Models;
using Wynn.Validation.Validators;

namespace Wynn.Validation
{
	public static class Validate
	{
		public static async Task ValidateAll(List<Order> orders, List<Product> products, List<ProductIngredients> ingredients)
		{
			//for bigger applications it's better using a dependency injection container to manage initialization but for our case hard coupling is not a big problem
			var orderValidator = new OrderValidator();
			var productValidator = new ProductValidator();
			var productIngredientValidator = new ProductIngredientsValidator();
			var ingredientValidator = new IngredientInfoValidator();

			var orderTasks = orders.Select(async order => new
			{
				EntityType = "Order",
				Key = $"OrderId: {order.OrderId}",
				Result = await orderValidator.ValidateAsync(order)
			});

			var productTasks = products.Select(async product => new
			{
				EntityType = "Product",
				Key = $"ProductId: {product.ProductId}",
				Result = await productValidator.ValidateAsync(product)
			});

			var productIngredientTasks = ingredients.Select(async pi => new
			{
				EntityType = "ProductIngredients",
				Key = $"ProductId: {pi.ProductId}",
				Result = await productIngredientValidator.ValidateAsync(pi)
			});

			var allTasks = orderTasks
				.Concat(productTasks)
				.Concat(productIngredientTasks);

			var results = await Task.WhenAll(allTasks);

			var validationErrorCounter = 0;
			foreach (var item in results)
			{
				if (!item.Result.IsValid)
				{
					//For simplicity, we print validation errors to the console but normally these should be logged or handled in a user-friendly way
					Console.WriteLine($"Validation failed for {item.EntityType} ({item.Key}):");
					foreach (var error in item.Result.Errors)
					{
						Console.WriteLine($" - {error.ErrorMessage}");
						validationErrorCounter++;
					}
				}
			}
			if (validationErrorCounter > 0)
			{
				//For simplicity, we throw an exception if any validation fails but normally these errors should be handled gracefully
				throw new Exception("Validation failed, please check validation messages!");
			}
		}
	}
}
