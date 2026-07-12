using System;
using System.Reflection;

namespace System
{
	public class ResolveEventArgs : EventArgs
	{
		public ResolveEventArgs(string name)
		{
			this.Name = name;
		}

		public ResolveEventArgs(string name, Assembly requestingAssembly)
		{
			this.Name = name;
			this.RequestingAssembly = requestingAssembly;
		}

		public string Name { get; }

		public Assembly RequestingAssembly { get; }
	}
}
