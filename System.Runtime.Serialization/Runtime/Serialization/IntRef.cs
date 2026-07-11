using System;

namespace System.Runtime.Serialization
{
	internal class IntRef
	{
		public IntRef(int value)
		{
			this.value = value;
		}

		public int Value
		{
			get
			{
				return this.value;
			}
		}

		private int value;
	}
}
