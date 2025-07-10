using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wynn.Models;

namespace Wynn.Validation.Validators
{
	public class IngredientInfoValidator : AbstractValidator<IngredientInfo>
	{
		public IngredientInfoValidator()
		{
			RuleFor(x => x.Ingredient)
				.NotEmpty().WithMessage("Ingredient name is required.");

			RuleFor(x => x.Amount)
				.GreaterThan(0).WithMessage("Ingredient amount must be greater than 0.");
		}
	}
}
