using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wynn.Models;

namespace Wynn.Validation.Validators
{
	public class ProductValidator : AbstractValidator<Product>
	{
		public ProductValidator()
		{
			RuleFor(x => x.ProductId)
				.GreaterThan(0).WithMessage("ProductId must be greater than 0.");

			RuleFor(x => x.ProductName)
				.NotEmpty().WithMessage("ProductName is required.");

			RuleFor(x => x.Price)
				.GreaterThan(0).WithMessage("Price must be greater than 0.");
		}
	}
}
