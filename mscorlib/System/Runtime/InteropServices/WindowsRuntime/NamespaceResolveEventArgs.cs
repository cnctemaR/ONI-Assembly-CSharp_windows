using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[ComVisible(false)]
	public class NamespaceResolveEventArgs : EventArgs
	{
		public NamespaceResolveEventArgs(string namespaceName, Assembly requestingAssembly)
		{
			this.NamespaceName = namespaceName;
			this.RequestingAssembly = requestingAssembly;
			this.ResolvedAssemblies = new Collection<Assembly>();
		}

		public string NamespaceName { get; private set; }

		public Assembly RequestingAssembly { get; private set; }

		public Collection<Assembly> ResolvedAssemblies { get; private set; }
	}
}
