using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Harmony
{
	public class DelegateTypeFactory
	{
		public DelegateTypeFactory()
		{
			DelegateTypeFactory.counter++;
			AssemblyName assemblyName = new AssemblyName("HarmonyDTFAssembly" + DelegateTypeFactory.counter);
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			this.module = assemblyBuilder.DefineDynamicModule("HarmonyDTFModule" + DelegateTypeFactory.counter);
		}

		public Type CreateDelegateType(MethodInfo method)
		{
			TypeAttributes typeAttributes = TypeAttributes.Public | TypeAttributes.Sealed;
			TypeBuilder typeBuilder = this.module.DefineType("HarmonyDTFType" + DelegateTypeFactory.counter, typeAttributes, typeof(MulticastDelegate));
			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName, CallingConventions.Standard, new Type[]
			{
				typeof(object),
				typeof(IntPtr)
			});
			constructorBuilder.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
			ParameterInfo[] parameters = method.GetParameters();
			MethodBuilder methodBuilder = typeBuilder.DefineMethod("Invoke", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig, method.ReturnType, parameters.Types());
			methodBuilder.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
			for (int i = 0; i < parameters.Length; i++)
			{
				methodBuilder.DefineParameter(i + 1, ParameterAttributes.None, parameters[i].Name);
			}
			return typeBuilder.CreateType();
		}

		private readonly ModuleBuilder module;

		private static int counter;
	}
}
