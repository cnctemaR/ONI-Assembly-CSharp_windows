using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class AssemblyKeyFileAttribute : Attribute
	{
		public AssemblyKeyFileAttribute(string keyFile)
		{
			this.name = keyFile;
		}

		public string KeyFile
		{
			get
			{
				return this.name;
			}
		}

		private string name;
	}
}
