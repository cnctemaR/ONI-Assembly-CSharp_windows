using System;

namespace System.Runtime.CompilerServices
{
	public class StrongBox<T> : IStrongBox
	{
		public StrongBox(T value)
		{
			this.Value = value;
		}

		object IStrongBox.Value
		{
			get
			{
				return this.Value;
			}
			set
			{
				this.Value = (T)((object)value);
			}
		}

		public T Value;
	}
}
