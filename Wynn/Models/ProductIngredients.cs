using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wynn.Models
{
	public class ProductIngredients
	{
		public int ProductId { get; set; }
		public List<IngredientInfo> Ingredients { get; set; }
	}
}
