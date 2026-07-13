using System;
using System.Runtime.CompilerServices;
using MonoMod.Core.Interop;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Runtimes
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class Core50Runtime : Core31Runtime
	{
		public Core50Runtime(ISystem system)
			: base(system)
		{
		}

		protected override Guid ExpectedJitVersion
		{
			get
			{
				return Core50Runtime.JitVersionGuid;
			}
		}

		protected override int VtableIndexICorJitCompilerGetVersionGuid
		{
			get
			{
				return 2;
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

		private static readonly Guid JitVersionGuid = new Guid(2783888292U, 16758, 17319, 140, 43, 160, 91, 85, 29, 79, 73);
	}
}
