using System;

namespace ProcGen
{
	[Serializable]
	public class LoreCollectionOverride
	{
		public string id { get; set; }

		public string collection { get; set; }

		public LoreCollectionOverride.OrderRule orderRule { get; private set; }

		public LoreCollectionOverride()
		{
			this.orderRule = LoreCollectionOverride.OrderRule.Prepend;
		}

		public enum OrderRule
		{
			Prepend,
			Append,
			Replace,
			Invalid
		}
	}
}
