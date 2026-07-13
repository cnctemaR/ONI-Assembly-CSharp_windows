using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	[ComVisible(true)]
	public sealed class ComSourceInterfacesAttribute : Attribute
	{
		public ComSourceInterfacesAttribute(string sourceInterfaces)
		{
			this._val = sourceInterfaces;
		}

		public ComSourceInterfacesAttribute(Type sourceInterface)
		{
			this._val = sourceInterface.FullName;
		}

		public ComSourceInterfacesAttribute(Type sourceInterface1, Type sourceInterface2)
		{
			this._val = sourceInterface1.FullName + "\0" + sourceInterface2.FullName;
		}

		public ComSourceInterfacesAttribute(Type sourceInterface1, Type sourceInterface2, Type sourceInterface3)
		{
			this._val = string.Concat(new string[] { sourceInterface1.FullName, "\0", sourceInterface2.FullName, "\0", sourceInterface3.FullName });
		}

		public ComSourceInterfacesAttribute(Type sourceInterface1, Type sourceInterface2, Type sourceInterface3, Type sourceInterface4)
		{
			this._val = string.Concat(new string[] { sourceInterface1.FullName, "\0", sourceInterface2.FullName, "\0", sourceInterface3.FullName, "\0", sourceInterface4.FullName });
		}

		public string Value
		{
			get
			{
				return this._val;
			}
		}

		internal string _val;
	}
}
