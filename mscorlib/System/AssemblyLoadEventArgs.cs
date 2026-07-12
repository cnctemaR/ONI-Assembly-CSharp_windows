using System;
using System.Reflection;

namespace System
{
	public class AssemblyLoadEventArgs : EventArgs
	{
		public AssemblyLoadEventArgs(Assembly loadedAssembly)
		{
			this.LoadedAssembly = loadedAssembly;
		}

		public Assembly LoadedAssembly { get; }
	}
}
