using System;

namespace Mono.Cecil
{
	internal sealed class AssemblyResolveEventArgs : EventArgs
	{
		public AssemblyNameReference AssemblyReference
		{
			get
			{
				return this.reference;
			}
		}

		public AssemblyResolveEventArgs(AssemblyNameReference reference)
		{
			this.reference = reference;
		}

		private readonly AssemblyNameReference reference;
	}
}
