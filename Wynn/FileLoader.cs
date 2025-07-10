using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Wynn.Models;

namespace Wynn
{
	public static class FileLoader
	{
		public static List<Order> LoadOrders(string filePath)
		{
			if (!File.Exists(filePath))
			{
				Console.WriteLine($"Orders file not found: {filePath}");
				return new List<Order>();
			}

			if (filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
			{
				var json = File.ReadAllText(filePath);
				return JsonSerializer.Deserialize<List<Order>>(json) ?? new List<Order>();
			}
			else if (filePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
			{
				var lines = File.ReadAllLines(filePath).Skip(1); // skip header
				var orders = new List<Order>();
				foreach (var line in lines)
				{
					var parts = line.Split(',');
					if (parts.Length < 6) continue;
					orders.Add(new Order
					{
						OrderId = int.TryParse(parts[0], out var x) ? x : 0,
						ProductId = int.TryParse(parts[1], out var q) ? q : 0,
						Quantity = int.TryParse(parts[2], out var z) ? z : 0,
						DeliveryAt = DateTime.Parse(parts[3], CultureInfo.InvariantCulture),
						CreatedAt = DateTime.Parse(parts[4], CultureInfo.InvariantCulture),
						DeliveryAddress = parts[5]
					});
				}
				return orders;
			}
			else
			{
				Console.WriteLine("Unsupported order file format.");
				return new List<Order>();
			}
		}

		public static List<Product> LoadProducts(string filePath)
		{
			if (!File.Exists(filePath))
			{
				Console.WriteLine($"Products file not found: {filePath}");
				return new List<Product>();
			}
			var json = File.ReadAllText(filePath);
			return JsonSerializer.Deserialize<List<Product>>(json) ?? new List<Product>();
		}

		// 4. Load ingredients
		public static List<ProductIngredients> LoadIngredients(string filePath)
		{
			if (!File.Exists(filePath))
			{
				Console.WriteLine($"Ingredients file not found: {filePath}");
				return new List<ProductIngredients>();
			}
			var json = File.ReadAllText(filePath);
			return JsonSerializer.Deserialize<List<ProductIngredients>>(json) ?? new List<ProductIngredients>();
		}
	}
}
