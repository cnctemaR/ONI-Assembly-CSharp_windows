using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	[ComVisible(true)]
	public sealed class ComSourceInterfacesAttribute : Attribute
	{
		public ComSourceInterfacesAttribute(string sourceInterfaces)
		{
			this.internalValue = sourceInterfaces;
		}

		public ComSourceInterfacesAttribute(Type sourceInterface)
		{
			this.internalValue = sourceInterface.ToString();
		}

		public ComSourceInterfacesAttribute(Type sourceInterface1, Type sourceInterface2)
		{
			this.internalValue = sourceInterface1.ToString() + sourceInterface2.ToString();
		}

		public ComSourceInterfacesAttribute(Type sourceInterface1, Type sourceInterface2, Type sourceInterface3)
		{
			this.internalValue = sourceInterface1.ToString() + sourceInterface2.ToString() + sourceInterface3.ToString();
		}

		public ComSourceInterfacesAttribute(Type sourceInterface1, Type sourceInterface2, Type sourceInterface3, Type sourceInterface4)
		{
			this.internalValue = sourceInterface1.ToString() + sourceInterface2.ToString() + sourceInterface3.ToString() + sourceInterface4.ToString();
		}

		public string Value
		{
			get
			{
				return this.internalValue;
			}
		}

		private string internalValue;
	}
}
