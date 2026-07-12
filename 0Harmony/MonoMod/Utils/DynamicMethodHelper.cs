using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MonoMod.Utils
{
	internal static class DynamicMethodHelper
	{
		public static object GetReference(int id)
		{
			return DynamicMethodHelper.References[id];
		}

		public static void SetReference(int id, object obj)
		{
			DynamicMethodHelper.References[id] = obj;
		}

		private static int AddReference(object obj)
		{
			List<object> references = DynamicMethodHelper.References;
			int num;
			lock (references)
			{
				DynamicMethodHelper.References.Add(obj);
				num = DynamicMethodHelper.References.Count - 1;
			}
			return num;
		}

		public static void FreeReference(int id)
		{
			DynamicMethodHelper.References[id] = null;
		}

		public static DynamicMethod Stub(this DynamicMethod dm)
		{
			ILGenerator ilgenerator = dm.GetILGenerator();
			for (int i = 0; i < 32; i++)
			{
				ilgenerator.Emit(global::System.Reflection.Emit.OpCodes.Nop);
			}
			if (dm.ReturnType != typeof(void))
			{
				ilgenerator.DeclareLocal(dm.ReturnType);
				ilgenerator.Emit(global::System.Reflection.Emit.OpCodes.Ldloca_S, 0);
				ilgenerator.Emit(global::System.Reflection.Emit.OpCodes.Initobj, dm.ReturnType);
				ilgenerator.Emit(global::System.Reflection.Emit.OpCodes.Ldloc_0);
			}
			ilgenerator.Emit(global::System.Reflection.Emit.OpCodes.Ret);
			return dm;
		}

		public static DynamicMethodDefinition Stub(this DynamicMethodDefinition dmd)
		{
			ILProcessor ilprocessor = dmd.GetILProcessor();
			for (int i = 0; i < 32; i++)
			{
				ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Nop);
			}
			if (dmd.Definition.ReturnType != dmd.Definition.Module.TypeSystem.Void)
			{
				ilprocessor.Body.Variables.Add(new VariableDefinition(dmd.Definition.ReturnType));
				ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ldloca_S, 0);
				ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Initobj, dmd.Definition.ReturnType);
				ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ldloc_0);
			}
			ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ret);
			return dmd;
		}

		public static int EmitReference<T>(this ILGenerator il, T obj)
		{
			Type typeFromHandle = typeof(T);
			int num = DynamicMethodHelper.AddReference(obj);
			il.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4, num);
			il.Emit(global::System.Reflection.Emit.OpCodes.Call, DynamicMethodHelper._GetReference);
			if (typeFromHandle.IsValueType)
			{
				il.Emit(global::System.Reflection.Emit.OpCodes.Unbox_Any, typeFromHandle);
			}
			return num;
		}

		public static int EmitReference<T>(this ILProcessor il, T obj)
		{
			ModuleDefinition module = il.Body.Method.Module;
			Type typeFromHandle = typeof(T);
			int num = DynamicMethodHelper.AddReference(obj);
			il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, num);
			il.Emit(Mono.Cecil.Cil.OpCodes.Call, module.ImportReference(DynamicMethodHelper._GetReference));
			if (typeFromHandle.IsValueType)
			{
				il.Emit(Mono.Cecil.Cil.OpCodes.Unbox_Any, module.ImportReference(typeFromHandle));
			}
			return num;
		}

		public static int EmitGetReference<T>(this ILGenerator il, int id)
		{
			Type typeFromHandle = typeof(T);
			il.Emit(global::System.Reflection.Emit.OpCodes.Ldc_I4, id);
			il.Emit(global::System.Reflection.Emit.OpCodes.Call, DynamicMethodHelper._GetReference);
			if (typeFromHandle.IsValueType)
			{
				il.Emit(global::System.Reflection.Emit.OpCodes.Unbox_Any, typeFromHandle);
			}
			return id;
		}

		public static int EmitGetReference<T>(this ILProcessor il, int id)
		{
			ModuleDefinition module = il.Body.Method.Module;
			Type typeFromHandle = typeof(T);
			il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, id);
			il.Emit(Mono.Cecil.Cil.OpCodes.Call, module.ImportReference(DynamicMethodHelper._GetReference));
			if (typeFromHandle.IsValueType)
			{
				il.Emit(Mono.Cecil.Cil.OpCodes.Unbox_Any, module.ImportReference(typeFromHandle));
			}
			return id;
		}

		private static List<object> References = new List<object>();

		private static readonly MethodInfo _GetMethodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle) });

		private static readonly MethodInfo _GetReference = typeof(DynamicMethodHelper).GetMethod("GetReference");
	}
}
