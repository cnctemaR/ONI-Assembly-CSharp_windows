using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil.Cil;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Runtimes
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class Core100Runtime : Core90Runtime
	{
		public Core100Runtime(ISystem system, IArchitecture arch)
			: base(system, arch)
		{
		}

		protected override Guid ExpectedJitVersion
		{
			get
			{
				return Core100Runtime.JitVersionGuid;
			}
		}

		protected override int VtableIndexICorJitInfoAllocMem
		{
			get
			{
				return 160;
			}
		}

		protected override int ICorJitInfoFullVtableCount
		{
			get
			{
				return 176;
			}
		}

		protected unsafe override void MakeAssemblySystemAssembly(Assembly assembly)
		{
			IntPtr intPtr = (IntPtr)Core21Runtime.RuntimeAssemblyPtrField.GetValue(assembly);
			int num = IntPtr.Size + IntPtr.Size + IntPtr.Size;
			IntPtr intPtr2 = *(IntPtr*)((byte*)(void*)intPtr + num);
			int num2 = IntPtr.Size + (FxCoreBaseRuntime.IsDebugClr ? (IntPtr.Size + 4 + 4 + 4 + IntPtr.Size + 4) : 0) + IntPtr.Size + 4 + ((IntPtr.Size == 8) ? 4 : 0) + IntPtr.Size + IntPtr.Size + IntPtr.Size + 4;
			if (FxCoreBaseRuntime.IsDebugClr && IntPtr.Size == 8)
			{
				num2 += 8;
			}
			((byte*)(void*)intPtr2)[num2] = 1;
		}

		protected override MethodInfo MakeCreateRuntimeMethodInfoStub(Type methodHandleInternal)
		{
			ConstructorInfo constructorInfo = methodHandleInternal.GetConstructors((BindingFlags)(-1))[0];
			Type type = typeof(RuntimeMethodHandle).Assembly.GetType("System.RuntimeMethodInfoStub");
			ConstructorInfo constructor = type.GetConstructor(new Type[]
			{
				methodHandleInternal,
				typeof(object)
			});
			MethodInfo methodInfo;
			using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition("new RuntimeMethodInfoStub", type, new Type[]
			{
				typeof(IntPtr),
				typeof(object)
			}))
			{
				ILProcessor ilprocessor = dynamicMethodDefinition.GetILProcessor();
				ilprocessor.Emit(OpCodes.Ldarg_0);
				ilprocessor.Emit(OpCodes.Newobj, constructorInfo);
				ilprocessor.Emit(OpCodes.Ldarg_1);
				ilprocessor.Emit(OpCodes.Newobj, constructor);
				ilprocessor.Emit(OpCodes.Ret);
				methodInfo = dynamicMethodDefinition.Generate();
			}
			return methodInfo;
		}

		protected override MethodInfo GetOrCreateGetTypeFromHandleUnsafe()
		{
			MethodInfo method = typeof(RuntimeTypeHandle).GetMethod("GetRuntimeTypeFromHandleMaybeNull", (BindingFlags)(-1), null, new Type[] { typeof(IntPtr) }, null);
			Helpers.Assert(method != null, null, "method is not null");
			return method;
		}

		private static readonly Guid JitVersionGuid = new Guid(2056043606U, 40473, 17185, 128, 185, 160, 210, 197, 120, 201, 69);
	}
}
