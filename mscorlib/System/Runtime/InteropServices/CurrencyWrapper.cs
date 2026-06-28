using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public sealed class CurrencyWrapper
	{
		public CurrencyWrapper(decimal obj)
		{
			this.currency = obj;
		}

		public CurrencyWrapper(object obj)
		{
			if (obj.GetType() != typeof(decimal))
			{
				throw new ArgumentException("obj has to be a Decimal type");
			}
			this.currency = (decimal)obj;
		}

		public decimal WrappedObject
		{
			get
			{
				return this.currency;
			}
		}

		private decimal currency;
	}
}
