using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Runtimes
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class MonoRuntime : IRuntime
	{
		public RuntimeKind Target
		{
			get
			{
				return RuntimeKind.Mono;
			}
		}

		public RuntimeFeature Features
		{
			get
			{
				return RuntimeFeature.PreciseGC | RuntimeFeature.GenericSharing | RuntimeFeature.DisableInlining | RuntimeFeature.RequiresMethodPinning | RuntimeFeature.RequiresMethodIdentification | RuntimeFeature.RequiresCustomMethodCompile;
			}
		}

		public Abi Abi { get; }

		private static TypeClassification LinuxAmd64Classifier(Type type, bool isReturn)
		{
			if (type.IsEnum)
			{
				type = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).First<FieldInfo>().FieldType;
			}
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Empty:
				return TypeClassification.InRegister;
			case TypeCode.Object:
			case TypeCode.DBNull:
			case TypeCode.String:
				return TypeClassification.InRegister;
			case TypeCode.Boolean:
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
				return TypeClassification.InRegister;
			case TypeCode.Single:
			case TypeCode.Double:
				return TypeClassification.InRegister;
			}
			if (type.IsPointer)
			{
				return TypeClassification.InRegister;
			}
			if (type.IsByRef)
			{
				return TypeClassification.InRegister;
			}
			if (type == typeof(IntPtr) || type == typeof(UIntPtr))
			{
				return TypeClassification.InRegister;
			}
			if (type == typeof(void))
			{
				return TypeClassification.InRegister;
			}
			Helpers.Assert(type.IsValueType, null, "type.IsValueType");
			return MonoRuntime.ClassifyValueType(type, true);
		}

		private static TypeClassification ClassifyValueType(Type type, bool isReturn)
		{
			int managedSize = type.GetManagedSize();
			bool flag = (!isReturn || managedSize != 8) && (isReturn || managedSize > 16);
			if (managedSize == 0)
			{
				return TypeClassification.InRegister;
			}
			if (!flag)
			{
				int num = ((managedSize > 8) ? 2 : 1);
				int num2 = 1;
				int num3 = 1;
				if (isReturn && num != 1)
				{
					num3 = (num2 = 2);
				}
				if (num2 == 2 || num3 == 2)
				{
					num2 = 2;
				}
				TypeClassification typeClassification;
				if (num2 != 1)
				{
					if (num2 != 2)
					{
						throw new InvalidOperationException();
					}
					typeClassification = TypeClassification.OnStack;
				}
				else
				{
					typeClassification = TypeClassification.InRegister;
				}
				return typeClassification;
			}
			if (!isReturn)
			{
				return TypeClassification.OnStack;
			}
			return TypeClassification.ByReference;
		}

		private static IEnumerable<FieldInfo> NestedValutypeFields(Type type)
		{
			MonoRuntime.<NestedValutypeFields>d__10 <NestedValutypeFields>d__ = new MonoRuntime.<NestedValutypeFields>d__10(-2);
			<NestedValutypeFields>d__.<>3__type = type;
			return <NestedValutypeFields>d__;
		}

		public MonoRuntime(ISystem system)
		{
			this.system = system;
			Abi? defaultAbi = system.DefaultAbi;
			if (defaultAbi != null)
			{
				Abi abi = defaultAbi.GetValueOrDefault();
				OSKind oskind = PlatformDetection.OS.GetKernel();
				bool flag = oskind == OSKind.OSX || oskind == OSKind.Linux;
				if (flag && PlatformDetection.Architecture == ArchitectureKind.x86_64)
				{
					Abi abi2 = abi;
					Classifier classifier;
					if ((classifier = MonoRuntime.<>O.<0>__LinuxAmd64Classifier) == null)
					{
						classifier = (MonoRuntime.<>O.<0>__LinuxAmd64Classifier = new Classifier(MonoRuntime.LinuxAmd64Classifier));
					}
					abi2.Classifier = classifier;
					abi = abi2;
				}
				oskind = PlatformDetection.OS;
				flag = oskind == OSKind.Windows || oskind == OSKind.Wine;
				bool flag2 = flag;
				if (flag2)
				{
					ArchitectureKind architecture = PlatformDetection.Architecture;
					bool flag3 = architecture - ArchitectureKind.x86 <= 1;
					flag2 = flag3;
				}
				if (flag2)
				{
					Abi abi2 = abi;
					abi2.ArgumentOrder = new SpecialArgumentKind[]
					{
						SpecialArgumentKind.ThisPointer,
						SpecialArgumentKind.ReturnBuffer,
						SpecialArgumentKind.UserArguments
					};
					abi = abi2;
				}
				this.Abi = abi;
				return;
			}
			throw new InvalidOperationException("Cannot use Mono system, because the underlying system doesn't provide a default ABI!");
		}

		[Nullable(2)]
		[method: NullableContext(2)]
		[field: Nullable(2)]
		public event OnMethodCompiledCallback OnMethodCompiled;

		public unsafe void DisableInlining(MethodBase method)
		{
			ushort* ptr = (long)this.GetMethodHandle(method).Value / 2L + 2L;
			ushort* ptr2 = ptr;
			*ptr2 |= 8;
		}

		public RuntimeMethodHandle GetMethodHandle(MethodBase method)
		{
			if (method is DynamicMethod)
			{
				MethodInfo dynamicMethod_CreateDynMethod = MonoRuntime._DynamicMethod_CreateDynMethod;
				if (dynamicMethod_CreateDynMethod != null)
				{
					dynamicMethod_CreateDynMethod.Invoke(method, ArrayEx.Empty<object>());
				}
				if (MonoRuntime._DynamicMethod_mhandle != null)
				{
					return (RuntimeMethodHandle)MonoRuntime._DynamicMethod_mhandle.GetValue(method);
				}
			}
			return method.MethodHandle;
		}

		public bool RequiresGenericContext(MethodBase method)
		{
			return false;
		}

		[return: Nullable(2)]
		public IDisposable PinMethodIfNeeded(MethodBase method)
		{
			method = this.GetIdentifiable(method);
			MonoRuntime.PrivateMethodPin orAdd = this.pinnedMethods.GetOrAdd(method, delegate(MethodBase m)
			{
				MonoRuntime.PrivateMethodPin privateMethodPin = new MonoRuntime.PrivateMethodPin(this);
				privateMethodPin.Pin.Method = m;
				RuntimeMethodHandle runtimeMethodHandle = (privateMethodPin.Pin.Handle = this.GetMethodHandle(m));
				this.pinnedHandles[runtimeMethodHandle] = privateMethodPin;
				this.DisableInlining(method);
				Type declaringType = method.DeclaringType;
				if (declaringType != null)
				{
					bool isGenericType = declaringType.IsGenericType;
				}
				return privateMethodPin;
			});
			Interlocked.Increment(ref orAdd.Pin.Count);
			return new MonoRuntime.PinHandle(orAdd);
		}

		private void UnpinOnce(MonoRuntime.PrivateMethodPin pin)
		{
			if (Interlocked.Decrement(ref pin.Pin.Count) <= 0)
			{
				MonoRuntime.PrivateMethodPin privateMethodPin;
				this.pinnedMethods.TryRemove(pin.Pin.Method, out privateMethodPin);
				this.pinnedHandles.TryRemove(pin.Pin.Handle, out privateMethodPin);
			}
		}

		public MethodBase GetIdentifiable(MethodBase method)
		{
			MonoRuntime.PrivateMethodPin privateMethodPin;
			if (!this.pinnedHandles.TryGetValue(this.GetMethodHandle(method), out privateMethodPin))
			{
				return method;
			}
			return privateMethodPin.Pin.Method;
		}

		public IntPtr GetMethodEntryPoint(MethodBase method)
		{
			MonoRuntime.PrivateMethodPin privateMethodPin;
			if (this.pinnedMethods.TryGetValue(method, out privateMethodPin))
			{
				return privateMethodPin.Pin.Handle.GetFunctionPointer();
			}
			return this.GetMethodHandle(method).GetFunctionPointer();
		}

		public void Compile(MethodBase method)
		{
			this.GetMethodHandle(method).GetFunctionPointer();
		}

		private readonly ISystem system;

		private static readonly MethodInfo _DynamicMethod_CreateDynMethod = typeof(DynamicMethod).GetMethod("CreateDynMethod", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly FieldInfo _DynamicMethod_mhandle = typeof(DynamicMethod).GetField("mhandle", BindingFlags.Instance | BindingFlags.NonPublic);

		private readonly ConcurrentDictionary<MethodBase, MonoRuntime.PrivateMethodPin> pinnedMethods = new ConcurrentDictionary<MethodBase, MonoRuntime.PrivateMethodPin>();

		private readonly ConcurrentDictionary<RuntimeMethodHandle, MonoRuntime.PrivateMethodPin> pinnedHandles = new ConcurrentDictionary<RuntimeMethodHandle, MonoRuntime.PrivateMethodPin>();

		[Nullable(0)]
		private sealed class PrivateMethodPin
		{
			public PrivateMethodPin(MonoRuntime runtime)
			{
				this.runtime = runtime;
			}

			public void UnpinOnce()
			{
				this.runtime.UnpinOnce(this);
			}

			private readonly MonoRuntime runtime;

			public MonoRuntime.MethodPinInfo Pin;
		}

		[NullableContext(0)]
		private sealed class PinHandle : IDisposable
		{
			[NullableContext(1)]
			public PinHandle(MonoRuntime.PrivateMethodPin pin)
			{
				this.pin = pin;
			}

			private void Dispose(bool disposing)
			{
				if (!this.disposedValue)
				{
					this.pin.UnpinOnce();
					this.disposedValue = true;
				}
			}

			~PinHandle()
			{
				this.Dispose(false);
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			[Nullable(1)]
			private readonly MonoRuntime.PrivateMethodPin pin;

			private bool disposedValue;
		}

		[Nullable(0)]
		private struct MethodPinInfo
		{
			public override string ToString()
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 3);
				defaultInterpolatedStringHandler.AppendLiteral("(MethodPinInfo: ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(this.Count);
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<MethodBase>(this.Method);
				defaultInterpolatedStringHandler.AppendLiteral(", 0x");
				defaultInterpolatedStringHandler.AppendFormatted<long>((long)this.Handle.Value, "X");
				defaultInterpolatedStringHandler.AppendLiteral(")");
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}

			public int Count;

			public MethodBase Method;

			public RuntimeMethodHandle Handle;
		}

		[CompilerGenerated]
		private static class <>O
		{
			[Nullable(0)]
			public static Classifier <0>__LinuxAmd64Classifier;
		}
	}
}
