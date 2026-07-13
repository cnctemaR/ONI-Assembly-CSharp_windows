using System;
using System.Runtime.CompilerServices;
using MonoMod.Core.Interop;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Runtimes
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class Core30Runtime : Core21Runtime
	{
		public Core30Runtime(ISystem system)
			: base(system)
		{
		}

		protected override Guid ExpectedJitVersion
		{
			get
			{
				return Core30Runtime.JitVersionGuid;
			}
		}

		protected override CoreCLR.InvokeCompileMethodPtr InvokeCompileMethodPtr
		{
			get
			{
				return CoreCLR.V21.InvokeCompileMethodPtr;
			}
		}

		protected override Delegate CastCompileHookToRealType(Delegate del)
		{
			return del.CastDelegate<CoreCLR.V21.CompileMethodDelegate>();
		}

		private static readonly Guid JitVersionGuid = new Guid(3590962897U, 30769, 18940, 189, 73, 182, 240, 84, 221, 77, 70);
	}
}
