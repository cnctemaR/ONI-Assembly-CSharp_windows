using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	public class AssemblyLoadEventArgs : EventArgs
	{
		public AssemblyLoadEventArgs(Assembly loadedAssembly)
		{
			this.m_loadedAssembly = loadedAssembly;
		}

		public Assembly LoadedAssembly
		{
			get
			{
				return this.m_loadedAssembly;
			}
		}

		private Assembly m_loadedAssembly;
	}
}
