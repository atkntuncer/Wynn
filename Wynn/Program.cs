using Wynn.Validation;

namespace Wynn
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var orders = FileLoader.LoadOrders(@"order.json");// or "orders.csv" 
			var products = FileLoader.LoadProducts(@"products.json");// insert file path here
            var ingredients = FileLoader.LoadIngredients(@"ingridients.json");// insert file path here

            await Validate.ValidateAll(orders, products, ingredients);

            var orderTotals = PriceCalculator.CalculateOrderTotals(orders, products);
            var ingredientTotals = PriceCalculator.CalculateIngredientTotals(orders, ingredients);

			Console.WriteLine("Order Totals:");
			foreach (var order in orderTotals)
			{
				Console.WriteLine($"Order ID: {order.Key}, Total: {order.Value:C}");
			}

			Console.WriteLine("\nIngredient Totals:");
			foreach (var order in ingredientTotals)
			{
				Console.WriteLine($"OrderId: {order.Key}");
				foreach (var ingridient in order.Value)
				{
					Console.WriteLine($"Ingridient name: {ingridient.Key}, Total amount: {ingridient.Value}");
				}
				Console.WriteLine("");
			}
		}
    }
}
