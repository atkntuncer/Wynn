using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wynn.Models
{
	public class Order
	{
		public int OrderId { get; set; }
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public DateTime DeliveryAt { get; set; }
		public DateTime CreatedAt { get; set; }
		public string? DeliveryAddress { get; set; }
	}
}
