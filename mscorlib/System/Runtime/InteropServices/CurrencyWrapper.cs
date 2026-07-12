using System;

namespace System.Runtime.InteropServices
{
	public sealed class CurrencyWrapper
	{
		public CurrencyWrapper(decimal obj)
		{
			this.m_WrappedObject = obj;
		}

		public CurrencyWrapper(object obj)
		{
			if (!(obj is decimal))
			{
				throw new ArgumentException("Object must be of type Decimal.", "obj");
			}
			this.m_WrappedObject = (decimal)obj;
		}

		public decimal WrappedObject
		{
			get
			{
				return this.m_WrappedObject;
			}
		}

		private decimal m_WrappedObject;
	}
}
