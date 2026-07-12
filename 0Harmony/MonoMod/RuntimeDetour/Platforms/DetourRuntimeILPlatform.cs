using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Mono.Cecil.Cil;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour.Platforms
{
	public abstract class DetourRuntimeILPlatform : IDetourRuntimePlatform
	{
		protected abstract RuntimeMethodHandle GetMethodHandle(MethodBase method);

		public abstract bool OnMethodCompiledWillBeCalled { get; }

		public abstract event OnMethodCompiledEvent OnMethodCompiled;

		public unsafe DetourRuntimeILPlatform()
		{
			MethodInfo method = typeof(DetourRuntimeILPlatform).GetMethod("_SelftestGetRefPtr", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method2 = typeof(DetourRuntimeILPlatform).GetMethod("_SelftestGetRefPtrHook", BindingFlags.Static | BindingFlags.NonPublic);
			this._HookSelftest(method, method2);
			IntPtr intPtr = ((Func<IntPtr>)Delegate.CreateDelegate(typeof(Func<IntPtr>), this, method))();
			MethodInfo method3 = typeof(DetourRuntimeILPlatform).GetMethod("_SelftestGetStruct", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method4 = typeof(DetourRuntimeILPlatform).GetMethod("_SelftestGetStructHook", BindingFlags.Static | BindingFlags.NonPublic);
			this._HookSelftest(method3, method4);
			fixed (DetourRuntimeILPlatform.GlueThiscallStructRetPtrOrder* ptr = &this.GlueThiscallStructRetPtr)
			{
				DetourRuntimeILPlatform.GlueThiscallStructRetPtrOrder* ptr2 = ptr;
				((Func<IntPtr, IntPtr, IntPtr, DetourRuntimeILPlatform._SelftestStruct>)Delegate.CreateDelegate(typeof(Func<IntPtr, IntPtr, IntPtr, DetourRuntimeILPlatform._SelftestStruct>), this, method3))((IntPtr)((void*)ptr2), (IntPtr)((void*)ptr2), intPtr);
			}
			MethodInfo method5 = typeof(DetourRuntimeILPlatform._SelftestStruct).GetMethod("_SelftestGetInStruct", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method6 = typeof(DetourRuntimeILPlatform).GetMethod("_SelftestGetInStructHook", BindingFlags.Static | BindingFlags.NonPublic);
			this._HookSelftest(method5, method6);
			fixed (DetourRuntimeILPlatform.GlueThiscallStructRetPtrOrder* ptr = &this.GlueThiscallInStructRetPtr)
			{
				DetourRuntimeILPlatform.GlueThiscallStructRetPtrOrder* ptr3 = ptr;
				object obj = default(DetourRuntimeILPlatform._SelftestStruct);
				*ptr3 = (DetourRuntimeILPlatform.GlueThiscallStructRetPtrOrder)((Func<short>)Delegate.CreateDelegate(typeof(Func<short>), obj, method5))();
				if (*ptr3 == (DetourRuntimeILPlatform.GlueThiscallStructRetPtrOrder)(-1))
				{
					throw new Exception("_SelftestGetInStruct failed!");
				}
			}
			this.Pin(method);
			this.ReferenceNonDynamicPoolPtr = this.GetNativeStart(method);
			if (DynamicMethodDefinition.IsDynamicILAvailable)
			{
				MethodBase methodBase;
				using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(DetourRuntimeILPlatform._MemAllocScratchDummy))
				{
					dynamicMethodDefinition.Name = "MemAllocScratch<Reference>";
					methodBase = DMDGenerator<DMDEmitDynamicMethodGenerator>.Generate(dynamicMethodDefinition, null);
				}
				this.Pin(methodBase);
				this.ReferenceDynamicPoolPtr = this.GetNativeStart(methodBase);
			}
		}

		private void _HookSelftest(MethodInfo from, MethodInfo to)
		{
			this.Pin(from);
			this.Pin(to);
			NativeDetourData nativeDetourData = DetourHelper.Native.Create(this.GetNativeStart(from), this.GetNativeStart(to), null);
			DetourHelper.Native.MakeWritable(nativeDetourData);
			DetourHelper.Native.Apply(nativeDetourData);
			DetourHelper.Native.MakeExecutable(nativeDetourData);
			DetourHelper.Native.FlushICache(nativeDetourData);
			DetourHelper.Native.Free(nativeDetourData);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private IntPtr _SelftestGetRefPtr()
		{
			Console.Error.WriteLine("If you're reading this, the MonoMod.RuntimeDetour selftest failed.");
			throw new Exception("This method should've been detoured!");
		}

		private static IntPtr _SelftestGetRefPtrHook(IntPtr self)
		{
			return self;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private DetourRuntimeILPlatform._SelftestStruct _SelftestGetStruct(IntPtr x, IntPtr y, IntPtr thisPtr)
		{
			Console.Error.WriteLine("If you're reading this, the MonoMod.RuntimeDetour selftest failed.");
			throw new Exception("_SelftestGetStruct failed!");
		}

		private unsafe static void _SelftestGetStructHook(IntPtr a, IntPtr b, IntPtr c, IntPtr d, IntPtr e)
		{
			if (b == c)
			{
				*(int*)(void*)b = 0;
				return;
			}
			if (b == e)
			{
				*(int*)(void*)c = 2;
				return;
			}
			*(int*)(void*)c = 1;
		}

		private unsafe static short _SelftestGetInStructHook(IntPtr a)
		{
			*(short*)(void*)a = 2;
			return 0;
		}

		protected virtual IntPtr GetFunctionPointer(MethodBase method, RuntimeMethodHandle handle)
		{
			return handle.GetFunctionPointer();
		}

		protected virtual void PrepareMethod(MethodBase method, RuntimeMethodHandle handle)
		{
			RuntimeHelpers.PrepareMethod(handle);
		}

		protected virtual void PrepareMethod(MethodBase method, RuntimeMethodHandle handle, RuntimeTypeHandle[] instantiation)
		{
			RuntimeHelpers.PrepareMethod(handle, instantiation);
		}

		protected virtual void DisableInlining(MethodBase method, RuntimeMethodHandle handle)
		{
		}

		public virtual MethodBase GetIdentifiable(MethodBase method)
		{
			DetourRuntimeILPlatform.PrivateMethodPin privateMethodPin;
			if (!this.PinnedHandles.TryGetValue(this.GetMethodHandle(method), out privateMethodPin))
			{
				return method;
			}
			return privateMethodPin.Pin.Method;
		}

		public virtual DetourRuntimeILPlatform.MethodPinInfo GetPin(MethodBase method)
		{
			DetourRuntimeILPlatform.PrivateMethodPin privateMethodPin;
			if (!this.PinnedMethods.TryGetValue(method, out privateMethodPin))
			{
				return default(DetourRuntimeILPlatform.MethodPinInfo);
			}
			return privateMethodPin.Pin;
		}

		public virtual DetourRuntimeILPlatform.MethodPinInfo GetPin(RuntimeMethodHandle handle)
		{
			DetourRuntimeILPlatform.PrivateMethodPin privateMethodPin;
			if (!this.PinnedHandles.TryGetValue(handle, out privateMethodPin))
			{
				return default(DetourRuntimeILPlatform.MethodPinInfo);
			}
			return privateMethodPin.Pin;
		}

		public virtual DetourRuntimeILPlatform.MethodPinInfo[] GetPins()
		{
			return (from p in this.PinnedHandles.Values.ToArray<DetourRuntimeILPlatform.PrivateMethodPin>()
				select p.Pin).ToArray<DetourRuntimeILPlatform.MethodPinInfo>();
		}

		public virtual IntPtr GetNativeStart(MethodBase method)
		{
			method = this.GetIdentifiable(method);
			DetourRuntimeILPlatform.PrivateMethodPin privateMethodPin;
			if (this.PinnedMethods.TryGetValue(method, out privateMethodPin))
			{
				return this.GetFunctionPointer(method, privateMethodPin.Pin.Handle);
			}
			return this.GetFunctionPointer(method, this.GetMethodHandle(method));
		}

		public virtual void Pin(MethodBase method)
		{
			method = this.GetIdentifiable(method);
			Interlocked.Increment(ref this.PinnedMethods.GetOrAdd(method, delegate(MethodBase m)
			{
				DetourRuntimeILPlatform.PrivateMethodPin privateMethodPin = new DetourRuntimeILPlatform.PrivateMethodPin();
				privateMethodPin.Pin.Method = m;
				RuntimeMethodHandle runtimeMethodHandle = (privateMethodPin.Pin.Handle = this.GetMethodHandle(m));
				this.PinnedHandles[runtimeMethodHandle] = privateMethodPin;
				this.DisableInlining(method, runtimeMethodHandle);
				Type declaringType = method.DeclaringType;
				if (declaringType != null && declaringType.IsGenericType)
				{
					this.PrepareMethod(method, runtimeMethodHandle, (from type in method.DeclaringType.GetGenericArguments()
						select type.TypeHandle).ToArray<RuntimeTypeHandle>());
				}
				else
				{
					this.PrepareMethod(method, runtimeMethodHandle);
				}
				return privateMethodPin;
			}).Pin.Count);
		}

		public virtual void Unpin(MethodBase method)
		{
			method = this.GetIdentifiable(method);
			DetourRuntimeILPlatform.PrivateMethodPin privateMethodPin;
			if (!this.PinnedMethods.TryGetValue(method, out privateMethodPin))
			{
				return;
			}
			if (Interlocked.Decrement(ref privateMethodPin.Pin.Count) <= 0)
			{
				DetourRuntimeILPlatform.PrivateMethodPin privateMethodPin2;
				this.PinnedMethods.TryRemove(method, out privateMethodPin2);
				this.PinnedHandles.TryRemove(privateMethodPin.Pin.Handle, out privateMethodPin2);
			}
		}

		public MethodInfo CreateCopy(MethodBase method)
		{
			method = this.GetIdentifiable(method);
			if (method == null || (method.GetMethodImplementationFlags() & MethodImplAttributes.CodeTypeMask) != MethodImplAttributes.IL)
			{
				throw new InvalidOperationException("Uncopyable method: " + (((method != null) ? method.ToString() : null) ?? "NULL"));
			}
			MethodInfo methodInfo;
			using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(method))
			{
				methodInfo = dynamicMethodDefinition.Generate();
			}
			return methodInfo;
		}

		public bool TryCreateCopy(MethodBase method, out MethodInfo dm)
		{
			method = this.GetIdentifiable(method);
			if (method == null || (method.GetMethodImplementationFlags() & MethodImplAttributes.CodeTypeMask) != MethodImplAttributes.IL)
			{
				dm = null;
				return false;
			}
			bool flag;
			try
			{
				dm = this.CreateCopy(method);
				flag = true;
			}
			catch
			{
				dm = null;
				flag = false;
			}
			return flag;
		}

		public MethodBase GetDetourTarget(MethodBase from, MethodBase to)
		{
			to = this.GetIdentifiable(to);
			MethodInfo methodInfo = null;
			MethodInfo methodInfo2 = from as MethodInfo;
			if (methodInfo2 != null && !from.IsStatic)
			{
				MethodInfo methodInfo3 = to as MethodInfo;
				if (methodInfo3 != null && to.IsStatic && methodInfo2.ReturnType == methodInfo3.ReturnType && methodInfo2.ReturnType.IsValueType)
				{
					Type declaringType = from.DeclaringType;
					DetourRuntimeILPlatform.GlueThiscallStructRetPtrOrder glueThiscallStructRetPtrOrder;
					if ((glueThiscallStructRetPtrOrder = ((declaringType != null && declaringType.IsValueType) ? this.GlueThiscallInStructRetPtr : this.GlueThiscallStructRetPtr)) != DetourRuntimeILPlatform.GlueThiscallStructRetPtrOrder.Original)
					{
						int managedSize = methodInfo2.ReturnType.GetManagedSize();
						if (managedSize == 3 || managedSize == 5 || managedSize == 6 || managedSize == 7 || managedSize > IntPtr.Size)
						{
							Type thisParamType = from.GetThisParamType();
							Type type = methodInfo2.ReturnType.MakeByRefType();
							int num = 0;
							int num2 = 1;
							if (glueThiscallStructRetPtrOrder == DetourRuntimeILPlatform.GlueThiscallStructRetPtrOrder.RetThisArgs)
							{
								num = 1;
								num2 = 0;
							}
							List<Type> list = new List<Type> { thisParamType };
							list.Insert(num2, type);
							list.AddRange(from p in @from.GetParameters()
								select p.ParameterType);
							using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(string.Concat(new string[]
							{
								"Glue:ThiscallStructRetPtr<",
								from.GetID(null, null, true, false, true),
								",",
								to.GetID(null, null, true, false, true),
								">"
							}), typeof(void), list.ToArray()))
							{
								ILProcessor ilprocessor = dynamicMethodDefinition.GetILProcessor();
								ilprocessor.Emit(OpCodes.Ldarg, num2);
								ilprocessor.Emit(OpCodes.Ldarg, num);
								for (int i = 2; i < list.Count; i++)
								{
									ilprocessor.Emit(OpCodes.Ldarg, i);
								}
								ilprocessor.Emit(OpCodes.Call, ilprocessor.Body.Method.Module.ImportReference(to));
								ilprocessor.Emit(OpCodes.Stobj, ilprocessor.Body.Method.Module.ImportReference(methodInfo2.ReturnType));
								ilprocessor.Emit(OpCodes.Ret);
								methodInfo = dynamicMethodDefinition.Generate();
							}
						}
					}
				}
			}
			return methodInfo ?? to;
		}

		public uint TryMemAllocScratchCloseTo(IntPtr target, out IntPtr ptr, int size)
		{
			if (size == 0 || (long)size > (long)((ulong)DetourRuntimeILPlatform._MemAllocScratchDummySafeSize))
			{
				ptr = IntPtr.Zero;
				return 0U;
			}
			bool flag = Math.Abs((long)target - (long)this.ReferenceNonDynamicPoolPtr) < 1073741824L;
			bool flag2 = DynamicMethodDefinition.IsDynamicILAvailable && Math.Abs((long)target - (long)this.ReferenceDynamicPoolPtr) < 1073741824L;
			if (!flag && !flag2)
			{
				ptr = IntPtr.Zero;
				return 0U;
			}
			MethodBase methodBase;
			using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(DetourRuntimeILPlatform._MemAllocScratchDummy))
			{
				dynamicMethodDefinition.Name = string.Format("MemAllocScratch<{0:X16}>", (long)target);
				if (flag2)
				{
					methodBase = DMDGenerator<DMDEmitDynamicMethodGenerator>.Generate(dynamicMethodDefinition, null);
				}
				else
				{
					methodBase = DMDGenerator<DMDCecilGenerator>.Generate(dynamicMethodDefinition, null);
				}
			}
			this.Pin(methodBase);
			ptr = this.GetNativeStart(methodBase);
			DetourHelper.Native.MakeReadWriteExecutable(ptr, DetourRuntimeILPlatform._MemAllocScratchDummySafeSize);
			return DetourRuntimeILPlatform._MemAllocScratchDummySafeSize;
		}

		public static int MemAllocScratchDummy(int a, int b)
		{
			if (a >= 1024 && b >= 1024)
			{
				return a + b;
			}
			return DetourRuntimeILPlatform.MemAllocScratchDummy(a + b, b + 1);
		}

		protected DetourRuntimeILPlatform.GlueThiscallStructRetPtrOrder GlueThiscallStructRetPtr;

		protected DetourRuntimeILPlatform.GlueThiscallStructRetPtrOrder GlueThiscallInStructRetPtr;

		protected ConcurrentDictionary<MethodBase, DetourRuntimeILPlatform.PrivateMethodPin> PinnedMethods = new ConcurrentDictionary<MethodBase, DetourRuntimeILPlatform.PrivateMethodPin>();

		protected ConcurrentDictionary<RuntimeMethodHandle, DetourRuntimeILPlatform.PrivateMethodPin> PinnedHandles = new ConcurrentDictionary<RuntimeMethodHandle, DetourRuntimeILPlatform.PrivateMethodPin>();

		private IntPtr ReferenceNonDynamicPoolPtr;

		private IntPtr ReferenceDynamicPoolPtr;

		protected static readonly uint _MemAllocScratchDummySafeSize = 16U;

		protected static readonly MethodInfo _MemAllocScratchDummy = typeof(DetourRuntimeILPlatform).GetMethod("MemAllocScratchDummy", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

		private struct _SelftestStruct
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			public short _SelftestGetInStruct()
			{
				Console.Error.WriteLine("If you're reading this, the MonoMod.RuntimeDetour selftest failed.");
				return -1;
			}

			private readonly short Value;

			private readonly byte E1;

			private readonly byte E2;

			private readonly byte E3;
		}

		protected class PrivateMethodPin
		{
			public DetourRuntimeILPlatform.MethodPinInfo Pin;
		}

		public struct MethodPinInfo
		{
			public override string ToString()
			{
				return string.Format("(MethodPinInfo: {0}, {1}, 0x{2:X})", this.Count, this.Method, (long)this.Handle.Value);
			}

			public int Count;

			public MethodBase Method;

			public RuntimeMethodHandle Handle;
		}

		protected enum GlueThiscallStructRetPtrOrder
		{
			Original,
			ThisRetArgs,
			RetThisArgs
		}
	}
}
