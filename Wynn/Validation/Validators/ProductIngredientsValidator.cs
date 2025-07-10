using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wynn.Models;

namespace Wynn.Validation.Validators
{
	public class ProductIngredientsValidator : AbstractValidator<ProductIngredients>
	{
		public ProductIngredientsValidator()
		{
			RuleFor(x => x.ProductId)
				.GreaterThan(0).WithMessage("ProductId must be greater than 0.");

			RuleFor(x => x.Ingredients)
				.NotNull().WithMessage("Ingredients list is required.")
				.Must(list => list != null && list.Count > 0 && list.Count <= 5)
				.WithMessage("Ingredients list must have 1 to 5 items.");

			RuleForEach(x => x.Ingredients)
				.SetValidator(new IngredientInfoValidator());
		}
	}
}
