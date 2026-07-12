using System;
using System.Reflection;
using System.Reflection.Emit;

namespace MonoMod.Utils
{
	public abstract class DMDGenerator<TSelf> : _IDMDGenerator where TSelf : DMDGenerator<TSelf>, new()
	{
		protected abstract MethodInfo _Generate(DynamicMethodDefinition dmd, object context);

		MethodInfo _IDMDGenerator.Generate(DynamicMethodDefinition dmd, object context)
		{
			return DMDGenerator<TSelf>._Postbuild(this._Generate(dmd, context));
		}

		public static MethodInfo Generate(DynamicMethodDefinition dmd, object context = null)
		{
			TSelf tself;
			if ((tself = DMDGenerator<TSelf>._Instance) == null)
			{
				tself = (DMDGenerator<TSelf>._Instance = new TSelf());
			}
			return DMDGenerator<TSelf>._Postbuild(tself._Generate(dmd, context));
		}

		internal static MethodInfo _Postbuild(MethodInfo mi)
		{
			if (mi == null)
			{
				return null;
			}
			if (DynamicMethodDefinition._IsMono && !(mi is DynamicMethod) && mi.DeclaringType != null)
			{
				Module module = ((mi != null) ? mi.Module : null);
				if (module == null)
				{
					return mi;
				}
				Assembly assembly = module.Assembly;
				if (((assembly != null) ? assembly.GetType() : null) == null)
				{
					return mi;
				}
				assembly.SetMonoCorlibInternal(true);
			}
			return mi;
		}

		private static TSelf _Instance;
	}
}
