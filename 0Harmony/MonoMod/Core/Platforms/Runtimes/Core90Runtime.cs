using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Core.Platforms.Runtimes
{
	internal class Core90Runtime : Core80Runtime
	{
		[NullableContext(1)]
		public Core90Runtime(ISystem system, IArchitecture arch)
			: base(system, arch)
		{
		}

		protected override Guid ExpectedJitVersion
		{
			get
			{
				return Core90Runtime.JitVersionGuid;
			}
		}

		protected override int VtableIndexICorJitInfoAllocMem
		{
			get
			{
				return 158;
			}
		}

		protected override int ICorJitInfoFullVtableCount
		{
			get
			{
				return 174;
			}
		}

		private static readonly Guid JitVersionGuid = new Guid(3592522360U, 39476, 19567, 141, 181, 7, 122, 6, 2, 47, 174);
	}
}
