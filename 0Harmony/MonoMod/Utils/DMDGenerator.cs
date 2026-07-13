using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	internal abstract class DMDGenerator<[Nullable(0)] TSelf> : IDMDGenerator where TSelf : DMDGenerator<TSelf>, new()
	{
		protected abstract MethodInfo GenerateCore(DynamicMethodDefinition dmd, [Nullable(2)] object context);

		MethodInfo IDMDGenerator.Generate(DynamicMethodDefinition dmd, [Nullable(2)] object context)
		{
			return DMDGenerator<TSelf>.Postbuild(this.GenerateCore(dmd, context));
		}

		public static MethodInfo Generate(DynamicMethodDefinition dmd, [Nullable(2)] object context = null)
		{
			TSelf tself;
			if ((tself = DMDGenerator<TSelf>.Instance) == null)
			{
				tself = (DMDGenerator<TSelf>.Instance = new TSelf());
			}
			return DMDGenerator<TSelf>.Postbuild(tself.GenerateCore(dmd, context));
		}

		internal static MethodInfo Postbuild(MethodInfo mi)
		{
			if (PlatformDetection.Runtime == RuntimeKind.Mono && !(mi is DynamicMethod) && mi.DeclaringType != null)
			{
				Module module = mi.Module;
				if (module == null)
				{
					return mi;
				}
				Assembly assembly = module.Assembly;
				if (assembly.GetType() == null)
				{
					return mi;
				}
				assembly.SetMonoCorlibInternal(true);
			}
			return mi;
		}

		[Nullable(2)]
		private static TSelf Instance;
	}
}
