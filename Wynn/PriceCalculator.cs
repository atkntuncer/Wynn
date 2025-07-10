using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wynn.Models;

namespace Wynn
{
	public static class PriceCalculator
	{
		public static Dictionary<int, decimal> CalculateOrderTotals(List<Order> orders, List<Product> products)
		{
			var productDict = products.ToDictionary(p => p.ProductId, p => p.Price);
			var result = new Dictionary<int, decimal>();

			foreach (var order in orders)
			{
				if (productDict.TryGetValue(order.ProductId, out var price))
				{
					if (!result.TryAdd(order.OrderId, price * order.Quantity))
					{
						result[order.OrderId] += price * order.Quantity;
					}
				}
			}
			return result;
		}

		public static Dictionary<int, Dictionary<string, double>> CalculateIngredientTotals(List<Order> orders, List<ProductIngredients> ingredients)
		{
			var ingredientDict = ingredients.ToDictionary(i => i.ProductId, i => i.Ingredients);
			var result = new Dictionary<int, Dictionary<string, double>>();

			foreach (var order in orders)
			{
				if (!ingredientDict.TryGetValue(order.ProductId, out var prodIngr))
				{
					Console.WriteLine($"No ingredients found for ProductId {order.ProductId}");
					continue;
				}

				if (!result.TryGetValue(order.OrderId, out var orderIngrDict))
				{
					orderIngrDict = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
					result[order.OrderId] = orderIngrDict;
				}

				foreach (var ingr in prodIngr)
				{
					if (!orderIngrDict.TryAdd(ingr.Ingredient!, ingr.Amount * order.Quantity))
					{
						orderIngrDict[ingr.Ingredient!] += ingr.Amount * order.Quantity;
					}
				}
			}
			return result;
		}
	}
}
