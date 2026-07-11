using System;
using System.Dynamic.Utils;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Text;
using System.Threading;

namespace System.Linq.Expressions.Compiler
{
	internal sealed class AssemblyGen
	{
		private static AssemblyGen Assembly
		{
			get
			{
				if (AssemblyGen.s_assembly == null)
				{
					Interlocked.CompareExchange<AssemblyGen>(ref AssemblyGen.s_assembly, new AssemblyGen(), null);
				}
				return AssemblyGen.s_assembly;
			}
		}

		private AssemblyGen()
		{
			AssemblyName assemblyName = new AssemblyName("Snippets");
			CustomAttributeBuilder[] array = new CustomAttributeBuilder[]
			{
				new CustomAttributeBuilder(typeof(SecurityTransparentAttribute).GetConstructor(Type.EmptyTypes), Array.Empty<object>())
			};
			AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run, array);
			this._myModule = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
		}

		private TypeBuilder DefineType(string name, Type parent, TypeAttributes attr)
		{
			ContractUtils.RequiresNotNull(name, "name");
			ContractUtils.RequiresNotNull(parent, "parent");
			StringBuilder stringBuilder = new StringBuilder(name);
			int num = Interlocked.Increment(ref this._index);
			stringBuilder.Append("$");
			stringBuilder.Append(num);
			stringBuilder.Replace('+', '_').Replace('[', '_').Replace(']', '_')
				.Replace('*', '_')
				.Replace('&', '_')
				.Replace(',', '_')
				.Replace('\\', '_');
			name = stringBuilder.ToString();
			return this._myModule.DefineType(name, attr, parent);
		}

		internal static TypeBuilder DefineDelegateType(string name)
		{
			return AssemblyGen.Assembly.DefineType(name, typeof(MulticastDelegate), TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AutoClass);
		}

		private static AssemblyGen s_assembly;

		private readonly ModuleBuilder _myModule;

		private int _index;
	}
}
