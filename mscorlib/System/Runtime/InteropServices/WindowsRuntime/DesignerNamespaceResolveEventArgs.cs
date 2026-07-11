using System;
using System.Collections.ObjectModel;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[ComVisible(false)]
	public class DesignerNamespaceResolveEventArgs : EventArgs
	{
		public DesignerNamespaceResolveEventArgs(string namespaceName)
		{
			this.NamespaceName = namespaceName;
			this.ResolvedAssemblyFiles = new Collection<string>();
		}

		public string NamespaceName { get; private set; }

		public Collection<string> ResolvedAssemblyFiles { get; private set; }
	}
}
