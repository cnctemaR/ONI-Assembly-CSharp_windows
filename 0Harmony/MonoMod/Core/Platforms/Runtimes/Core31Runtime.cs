using System;
using System.Runtime.CompilerServices;
using MonoMod.Core.Interop;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Runtimes
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class Core31Runtime : Core30Runtime
	{
		public Core31Runtime(ISystem system)
			: base(system)
		{
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
	}
}
