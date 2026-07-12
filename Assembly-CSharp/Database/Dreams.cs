using System;

namespace Database
{
	public class Dreams : ResourceSet<Dream>
	{
		public Dreams(ResourceSet parent)
			: base("Dreams", parent)
		{
			this.CommonDream = new Dream("CommonDream", this, "dream_tear_swirly_kanim", new string[] { "dreamIcon_journal" });
		}

		public Dream CommonDream;
	}
}
