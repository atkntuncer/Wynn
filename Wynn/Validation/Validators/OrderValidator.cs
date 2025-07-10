using FluentValidation;
using Wynn.Models;

namespace Wynn.Validation.Validators
{
	public class OrderValidator : AbstractValidator<Order>
	{
		public OrderValidator()
		{
			RuleFor(x => x.OrderId)
				.GreaterThan(0).WithMessage("OrderId must be greater than 0.");

			RuleFor(x => x.ProductId)
				.GreaterThan(0).WithMessage("ProductId must be greater than 0.");

			RuleFor(x => x.Quantity)
				.GreaterThan(0).WithMessage("Quantity must be greater than 0.");

			RuleFor(x => x.DeliveryAt)
				.GreaterThan(x => x.CreatedAt)
				.WithMessage("DeliveryAt must be after CreatedAt.");

			RuleFor(x => x.CreatedAt)
				.NotEmpty().WithMessage("CreatedAt is required.");

			RuleFor(x => x.DeliveryAddress)
				.NotEmpty().WithMessage("DeliveryAddress is required.");
		}
	}
}
