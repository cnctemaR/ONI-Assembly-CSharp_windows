using System;

namespace Database
{
	public class Shirts : ResourceSet<Shirt>
	{
		public Shirts()
		{
			this.Hot00 = base.Add(new Shirt("body_shirt_hot01"));
			this.Hot01 = base.Add(new Shirt("body_shirt_hot02"));
			this.Decor00 = base.Add(new Shirt("body_shirt_decor01"));
			this.Cold00 = base.Add(new Shirt("body_shirt_cold01"));
			this.Cold01 = base.Add(new Shirt("body_shirt_cold02"));
		}

		public Shirt Hot00;

		public Shirt Hot01;

		public Shirt Decor00;

		public Shirt Cold00;

		public Shirt Cold01;
	}
}
