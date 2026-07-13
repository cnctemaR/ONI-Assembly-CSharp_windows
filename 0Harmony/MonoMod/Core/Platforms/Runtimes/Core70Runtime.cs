using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Core.Platforms.Runtimes
{
	internal class Core70Runtime : Core60Runtime
	{
		[NullableContext(1)]
		public Core70Runtime(ISystem system, IArchitecture arch)
			: base(system, arch)
		{
		}

		protected override Guid ExpectedJitVersion
		{
			get
			{
				return Core70Runtime.JitVersionGuid;
			}
		}

		protected override int VtableIndexICorJitInfoAllocMem
		{
			get
			{
				return 159;
			}
		}

		protected override int ICorJitInfoFullVtableCount
		{
			get
			{
				return 175;
			}
		}

		private static readonly Guid JitVersionGuid = new Guid(1810136669U, 43307, 19734, 146, 128, 246, 61, 246, 70, 173, 164);
	}
}
