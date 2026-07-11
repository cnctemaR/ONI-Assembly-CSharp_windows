using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Samples.Helpers;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace YamlDotNet.Samples
{
	public class DeserializeObjectGraph
	{
		public DeserializeObjectGraph(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Sample(Title = "Deserializing an object graph", Description = "Shows how to convert a YAML document to an object graph.")]
		public void Main()
		{
			StringReader stringReader = new StringReader("---\n            receipt:    Oz-Ware Purchase Invoice\n            date:        2007-08-06\n            customer:\n                given:   Dorothy\n                family:  Gale\n\n            items:\n                - part_no:   A4786\n                  descrip:   Water Bucket (Filled)\n                  price:     1.47\n                  quantity:  4\n\n                - part_no:   E1628\n                  descrip:   High Heeled \"Ruby\" Slippers\n                  price:     100.27\n                  quantity:  1\n\n            bill-to:  &id001\n                street: |-\n                        123 Tornado Alley\n                        Suite 16\n                city:   East Westville\n                state:  KS\n\n            ship-to:  *id001\n\n            specialDelivery: >\n                Follow the Yellow Brick\n                Road to the Emerald City.\n                Pay no attention to the\n                man behind the curtain.\n...");
			DeserializeObjectGraph.Order order = new DeserializerBuilder().WithNamingConvention(new CamelCaseNamingConvention()).Build().Deserialize<DeserializeObjectGraph.Order>(stringReader);
			this.output.WriteLine("Order");
			this.output.WriteLine("-----");
			this.output.WriteLine();
			foreach (DeserializeObjectGraph.OrderItem orderItem in order.Items)
			{
				this.output.WriteLine("{0}\t{1}\t{2}\t{3}", new object[] { orderItem.PartNo, orderItem.Quantity, orderItem.Price, orderItem.Descrip });
			}
			this.output.WriteLine();
			this.output.WriteLine("Shipping");
			this.output.WriteLine("--------");
			this.output.WriteLine();
			this.output.WriteLine(order.ShipTo.Street);
			this.output.WriteLine(order.ShipTo.City);
			this.output.WriteLine(order.ShipTo.State);
			this.output.WriteLine();
			this.output.WriteLine("Billing");
			this.output.WriteLine("-------");
			this.output.WriteLine();
			if (order.BillTo == order.ShipTo)
			{
				this.output.WriteLine("*same as shipping address*");
			}
			else
			{
				this.output.WriteLine(order.ShipTo.Street);
				this.output.WriteLine(order.ShipTo.City);
				this.output.WriteLine(order.ShipTo.State);
			}
			this.output.WriteLine();
			this.output.WriteLine("Delivery instructions");
			this.output.WriteLine("---------------------");
			this.output.WriteLine();
			this.output.WriteLine(order.SpecialDelivery);
		}

		private readonly ITestOutputHelper output;

		private const string Document = "---\n            receipt:    Oz-Ware Purchase Invoice\n            date:        2007-08-06\n            customer:\n                given:   Dorothy\n                family:  Gale\n\n            items:\n                - part_no:   A4786\n                  descrip:   Water Bucket (Filled)\n                  price:     1.47\n                  quantity:  4\n\n                - part_no:   E1628\n                  descrip:   High Heeled \"Ruby\" Slippers\n                  price:     100.27\n                  quantity:  1\n\n            bill-to:  &id001\n                street: |-\n                        123 Tornado Alley\n                        Suite 16\n                city:   East Westville\n                state:  KS\n\n            ship-to:  *id001\n\n            specialDelivery: >\n                Follow the Yellow Brick\n                Road to the Emerald City.\n                Pay no attention to the\n                man behind the curtain.\n...";

		public class Order
		{
			public string Receipt { get; set; }

			public DateTime Date { get; set; }

			public DeserializeObjectGraph.Customer Customer { get; set; }

			public List<DeserializeObjectGraph.OrderItem> Items { get; set; }

			[YamlMember(Alias = "bill-to", ApplyNamingConventions = false)]
			public DeserializeObjectGraph.Address BillTo { get; set; }

			[YamlMember(Alias = "ship-to", ApplyNamingConventions = false)]
			public DeserializeObjectGraph.Address ShipTo { get; set; }

			public string SpecialDelivery { get; set; }
		}

		public class Customer
		{
			public string Given { get; set; }

			public string Family { get; set; }
		}

		public class OrderItem
		{
			[YamlMember(Alias = "part_no", ApplyNamingConventions = false)]
			public string PartNo { get; set; }

			public string Descrip { get; set; }

			public decimal Price { get; set; }

			public int Quantity { get; set; }
		}

		public class Address
		{
			public string Street { get; set; }

			public string City { get; set; }

			public string State { get; set; }
		}
	}
}
