using System;

namespace Database
{
	public class Shirts : ResourceSet<Shirt>
	{
		public Shirts()
		{
			this.Hot00 = base.Add(new Shirt("body_shirt_hot_shearling"));
			this.Decor00 = base.Add(new Shirt("body_shirt_decor01"));
		}

		public Shirt Hot00;

		public Shirt Decor00;
	}
}
