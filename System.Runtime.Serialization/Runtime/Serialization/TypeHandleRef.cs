using System;

namespace System.Runtime.Serialization
{
	internal class TypeHandleRef
	{
		public TypeHandleRef()
		{
		}

		public TypeHandleRef(RuntimeTypeHandle value)
		{
			this.value = value;
		}

		public RuntimeTypeHandle Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		private RuntimeTypeHandle value;
	}
}
