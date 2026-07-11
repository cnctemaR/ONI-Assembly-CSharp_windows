using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyKeyFileAttribute : Attribute
	{
		public AssemblyKeyFileAttribute(string keyFile)
		{
			this.m_keyFile = keyFile;
		}

		public string KeyFile
		{
			get
			{
				return this.m_keyFile;
			}
		}

		private string m_keyFile;
	}
}
