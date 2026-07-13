using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Core.Platforms.Runtimes
{
	internal class Core80Runtime : Core70Runtime
	{
		[NullableContext(1)]
		public Core80Runtime(ISystem system, IArchitecture arch)
			: base(system, arch)
		{
		}

		protected override Guid ExpectedJitVersion
		{
			get
			{
				return Core80Runtime.JitVersionGuid;
			}
		}

		protected override int VtableIndexICorJitInfoAllocMem
		{
			get
			{
				return 154;
			}
		}

		protected override int ICorJitInfoFullVtableCount
		{
			get
			{
				return 170;
			}
		}

		private static readonly Guid JitVersionGuid = new Guid(1271838981U, 54608, 19037, 177, 235, 39, 111, byte.MaxValue, 104, 209, 131);
	}
}
