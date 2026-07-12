using System;

namespace Database
{
	public class Dreams : ResourceSet<Dream>
	{
		public Dreams(ResourceSet parent)
			: base("Dreams", parent)
		{
			this.ExplorerDream = new Dream("ExplorerDream", this, "dream_tear_swirly_kanim", new string[0]);
		}

		public Dream ExplorerDream;
	}
}
