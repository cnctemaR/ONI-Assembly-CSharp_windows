using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil.Cil;
using MonoMod.Utils;

namespace MonoMod.Cil
{
	internal static class FastDelegateInvokers
	{
		[GetFastDelegateInvokersArray(16)]
		[return: Nullable(new byte[] { 1, 0, 1, 1 })]
		private static ValueTuple<MethodInfo, Type>[] GetInvokers()
		{
			return new ValueTuple<MethodInfo, Type>[]
			{
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidVal1", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidVal1<>)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeVal1", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeVal1<, >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidRef1", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidRef1<>)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeRef1", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeRef1<, >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidVal2", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidVal2<, >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeVal2", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeVal2<, , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidRef2", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidRef2<, >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeRef2", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeRef2<, , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidVal3", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidVal3<, , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeVal3", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeVal3<, , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidRef3", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidRef3<, , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeRef3", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeRef3<, , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidVal4", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidVal4<, , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeVal4", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeVal4<, , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidRef4", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidRef4<, , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeRef4", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeRef4<, , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidVal5", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidVal5<, , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeVal5", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeVal5<, , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidRef5", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidRef5<, , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeRef5", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeRef5<, , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidVal6", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidVal6<, , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeVal6", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeVal6<, , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidRef6", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidRef6<, , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeRef6", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeRef6<, , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidVal7", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidVal7<, , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeVal7", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeVal7<, , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidRef7", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidRef7<, , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeRef7", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeRef7<, , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidVal8", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidVal8<, , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeVal8", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeVal8<, , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidRef8", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidRef8<, , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeRef8", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeRef8<, , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidVal9", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidVal9<, , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeVal9", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeVal9<, , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidRef9", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidRef9<, , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeRef9", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeRef9<, , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidVal10", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidVal10<, , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeVal10", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeVal10<, , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidRef10", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidRef10<, , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeRef10", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeRef10<, , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidVal11", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidVal11<, , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeVal11", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeVal11<, , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidRef11", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidRef11<, , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeRef11", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeRef11<, , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidVal12", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidVal12<, , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeVal12", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeVal12<, , , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidRef12", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidRef12<, , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeRef12", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeRef12<, , , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidVal13", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidVal13<, , , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeVal13", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeVal13<, , , , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidRef13", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidRef13<, , , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeRef13", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeRef13<, , , , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidVal14", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidVal14<, , , , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeVal14", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeVal14<, , , , , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidRef14", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidRef14<, , , , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeRef14", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeRef14<, , , , , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidVal15", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidVal15<, , , , , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeVal15", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeVal15<, , , , , , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidRef15", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidRef15<, , , , , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeRef15", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeRef15<, , , , , , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidVal16", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidVal16<, , , , , , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeVal16", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeVal16<, , , , , , , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeVoidRef16", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.VoidRef16<, , , , , , , , , , , , , , , >)),
				new ValueTuple<MethodInfo, Type>(typeof(FastDelegateInvokers).GetMethod("InvokeTypeRef16", BindingFlags.Static | BindingFlags.NonPublic), typeof(FastDelegateInvokers.TypeRef16<, , , , , , , , , , , , , , , , >))
			};
		}

		[NullableContext(1)]
		[return: TupleElementNames(new string[] { "Invoker", "Delegate" })]
		[return: Nullable(new byte[] { 0, 1, 1 })]
		private static ValueTuple<MethodInfo, Type>? TryGetInvokerForSig(MethodSignature sig)
		{
			if (sig.ParameterCount == 0)
			{
				return null;
			}
			if (sig.ParameterCount > 16)
			{
				return null;
			}
			if (sig.ReturnType.IsByRef || sig.ReturnType.IsByRefLike())
			{
				return null;
			}
			if (sig.FirstParameter.IsByRefLike())
			{
				return null;
			}
			if (sig.Parameters.Skip<Type>(1).Any<Type>((Type t) => t.IsByRef || t.IsByRefLike()))
			{
				return null;
			}
			int num = 0;
			num |= (((sig.ReturnType != typeof(void)) > false) ? 1 : 0);
			num |= (sig.FirstParameter.IsByRef ? 2 : 0);
			num |= sig.ParameterCount - 1 << 2;
			ValueTuple<MethodInfo, Type> valueTuple = FastDelegateInvokers.invokers[num];
			MethodInfo item = valueTuple.Item1;
			Type item2 = valueTuple.Item2;
			Type[] array = new Type[sig.ParameterCount + (num & 1)];
			int num2 = 0;
			if ((num & 1) != 0)
			{
				array[num2++] = sig.ReturnType;
			}
			foreach (Type type in sig.Parameters)
			{
				if (type.IsByRef)
				{
					type = type.GetElementType();
				}
				array[num2++] = type;
			}
			Helpers.Assert(num2 == array.Length, null, "i == typeParams.Length");
			return new ValueTuple<MethodInfo, Type>?(new ValueTuple<MethodInfo, Type>(item.MakeGenericMethod(array), item2.MakeGenericType(array)));
		}

		[NullableContext(1)]
		[return: TupleElementNames(new string[] { "Invoker", "Delegate" })]
		[return: Nullable(new byte[] { 0, 1, 1 })]
		public static ValueTuple<MethodInfo, Type>? GetDelegateInvoker(Type delegateType)
		{
			Helpers.ThrowIfArgumentNull<Type>(delegateType, "delegateType");
			if (!typeof(Delegate).IsAssignableFrom(delegateType))
			{
				throw new ArgumentException("Argument not a delegate type", "delegateType");
			}
			Tuple<MethodInfo, Type> value = FastDelegateInvokers.invokerCache.GetValue(delegateType, delegate(Type delegateType)
			{
				MethodInfo method = delegateType.GetMethod("Invoke");
				MethodSignature methodSignature = MethodSignature.ForMethod(method, true);
				if (methodSignature.ParameterCount == 0)
				{
					return new Tuple<MethodInfo, Type>(null, delegateType);
				}
				ValueTuple<MethodInfo, Type>? valueTuple = FastDelegateInvokers.TryGetInvokerForSig(methodSignature);
				if (valueTuple != null)
				{
					ValueTuple<MethodInfo, Type> valueOrDefault = valueTuple.GetValueOrDefault();
					return new Tuple<MethodInfo, Type>(valueOrDefault.Item1, valueOrDefault.Item2);
				}
				Type[] array = new Type[methodSignature.ParameterCount + 1];
				int i = 0;
				foreach (Type type in methodSignature.Parameters)
				{
					array[i++] = type;
				}
				array[methodSignature.ParameterCount] = delegateType;
				string text = "MMIL:Invoke<";
				Type declaringType = method.DeclaringType;
				Tuple<MethodInfo, Type> tuple;
				using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(text + ((declaringType != null) ? declaringType.FullName : null) + ">", method.ReturnType, array))
				{
					ILProcessor ilprocessor = dynamicMethodDefinition.GetILProcessor();
					ilprocessor.Emit(OpCodes.Ldarg, methodSignature.ParameterCount);
					for (i = 0; i < methodSignature.ParameterCount; i++)
					{
						ilprocessor.Emit(OpCodes.Ldarg, i);
					}
					ilprocessor.Emit(OpCodes.Callvirt, method);
					ilprocessor.Emit(OpCodes.Ret);
					tuple = new Tuple<MethodInfo, Type>(dynamicMethodDefinition.Generate(), delegateType);
				}
				return tuple;
			});
			if (value.Item1 == null)
			{
				return null;
			}
			return new ValueTuple<MethodInfo, Type>?(new ValueTuple<MethodInfo, Type>(value.Item1, value.Item2));
		}

		private static void InvokeVoidVal1<T0>(T0 _0, FastDelegateInvokers.VoidVal1<T0> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidVal1<T0>>(del, "del")(_0);
		}

		private static TResult InvokeTypeVal1<TResult, T0>(T0 _0, FastDelegateInvokers.TypeVal1<TResult, T0> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeVal1<TResult, T0>>(del, "del")(_0);
		}

		private static void InvokeVoidRef1<T0>(ref T0 _0, FastDelegateInvokers.VoidRef1<T0> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidRef1<T0>>(del, "del")(ref _0);
		}

		private static TResult InvokeTypeRef1<TResult, T0>(ref T0 _0, FastDelegateInvokers.TypeRef1<TResult, T0> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeRef1<TResult, T0>>(del, "del")(ref _0);
		}

		private static void InvokeVoidVal2<T0, T1>(T0 _0, T1 _1, FastDelegateInvokers.VoidVal2<T0, T1> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidVal2<T0, T1>>(del, "del")(_0, _1);
		}

		private static TResult InvokeTypeVal2<TResult, T0, T1>(T0 _0, T1 _1, FastDelegateInvokers.TypeVal2<TResult, T0, T1> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeVal2<TResult, T0, T1>>(del, "del")(_0, _1);
		}

		private static void InvokeVoidRef2<T0, T1>(ref T0 _0, T1 _1, FastDelegateInvokers.VoidRef2<T0, T1> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidRef2<T0, T1>>(del, "del")(ref _0, _1);
		}

		private static TResult InvokeTypeRef2<TResult, T0, T1>(ref T0 _0, T1 _1, FastDelegateInvokers.TypeRef2<TResult, T0, T1> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeRef2<TResult, T0, T1>>(del, "del")(ref _0, _1);
		}

		private static void InvokeVoidVal3<T0, T1, T2>(T0 _0, T1 _1, T2 _2, FastDelegateInvokers.VoidVal3<T0, T1, T2> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidVal3<T0, T1, T2>>(del, "del")(_0, _1, _2);
		}

		private static TResult InvokeTypeVal3<TResult, T0, T1, T2>(T0 _0, T1 _1, T2 _2, FastDelegateInvokers.TypeVal3<TResult, T0, T1, T2> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeVal3<TResult, T0, T1, T2>>(del, "del")(_0, _1, _2);
		}

		private static void InvokeVoidRef3<T0, T1, T2>(ref T0 _0, T1 _1, T2 _2, FastDelegateInvokers.VoidRef3<T0, T1, T2> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidRef3<T0, T1, T2>>(del, "del")(ref _0, _1, _2);
		}

		private static TResult InvokeTypeRef3<TResult, T0, T1, T2>(ref T0 _0, T1 _1, T2 _2, FastDelegateInvokers.TypeRef3<TResult, T0, T1, T2> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeRef3<TResult, T0, T1, T2>>(del, "del")(ref _0, _1, _2);
		}

		private static void InvokeVoidVal4<T0, T1, T2, T3>(T0 _0, T1 _1, T2 _2, T3 _3, FastDelegateInvokers.VoidVal4<T0, T1, T2, T3> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidVal4<T0, T1, T2, T3>>(del, "del")(_0, _1, _2, _3);
		}

		private static TResult InvokeTypeVal4<TResult, T0, T1, T2, T3>(T0 _0, T1 _1, T2 _2, T3 _3, FastDelegateInvokers.TypeVal4<TResult, T0, T1, T2, T3> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeVal4<TResult, T0, T1, T2, T3>>(del, "del")(_0, _1, _2, _3);
		}

		private static void InvokeVoidRef4<T0, T1, T2, T3>(ref T0 _0, T1 _1, T2 _2, T3 _3, FastDelegateInvokers.VoidRef4<T0, T1, T2, T3> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidRef4<T0, T1, T2, T3>>(del, "del")(ref _0, _1, _2, _3);
		}

		private static TResult InvokeTypeRef4<TResult, T0, T1, T2, T3>(ref T0 _0, T1 _1, T2 _2, T3 _3, FastDelegateInvokers.TypeRef4<TResult, T0, T1, T2, T3> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeRef4<TResult, T0, T1, T2, T3>>(del, "del")(ref _0, _1, _2, _3);
		}

		private static void InvokeVoidVal5<T0, T1, T2, T3, T4>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, FastDelegateInvokers.VoidVal5<T0, T1, T2, T3, T4> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidVal5<T0, T1, T2, T3, T4>>(del, "del")(_0, _1, _2, _3, _4);
		}

		private static TResult InvokeTypeVal5<TResult, T0, T1, T2, T3, T4>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, FastDelegateInvokers.TypeVal5<TResult, T0, T1, T2, T3, T4> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeVal5<TResult, T0, T1, T2, T3, T4>>(del, "del")(_0, _1, _2, _3, _4);
		}

		private static void InvokeVoidRef5<T0, T1, T2, T3, T4>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, FastDelegateInvokers.VoidRef5<T0, T1, T2, T3, T4> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidRef5<T0, T1, T2, T3, T4>>(del, "del")(ref _0, _1, _2, _3, _4);
		}

		private static TResult InvokeTypeRef5<TResult, T0, T1, T2, T3, T4>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, FastDelegateInvokers.TypeRef5<TResult, T0, T1, T2, T3, T4> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeRef5<TResult, T0, T1, T2, T3, T4>>(del, "del")(ref _0, _1, _2, _3, _4);
		}

		private static void InvokeVoidVal6<T0, T1, T2, T3, T4, T5>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, FastDelegateInvokers.VoidVal6<T0, T1, T2, T3, T4, T5> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidVal6<T0, T1, T2, T3, T4, T5>>(del, "del")(_0, _1, _2, _3, _4, _5);
		}

		private static TResult InvokeTypeVal6<TResult, T0, T1, T2, T3, T4, T5>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, FastDelegateInvokers.TypeVal6<TResult, T0, T1, T2, T3, T4, T5> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeVal6<TResult, T0, T1, T2, T3, T4, T5>>(del, "del")(_0, _1, _2, _3, _4, _5);
		}

		private static void InvokeVoidRef6<T0, T1, T2, T3, T4, T5>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, FastDelegateInvokers.VoidRef6<T0, T1, T2, T3, T4, T5> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidRef6<T0, T1, T2, T3, T4, T5>>(del, "del")(ref _0, _1, _2, _3, _4, _5);
		}

		private static TResult InvokeTypeRef6<TResult, T0, T1, T2, T3, T4, T5>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, FastDelegateInvokers.TypeRef6<TResult, T0, T1, T2, T3, T4, T5> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeRef6<TResult, T0, T1, T2, T3, T4, T5>>(del, "del")(ref _0, _1, _2, _3, _4, _5);
		}

		private static void InvokeVoidVal7<T0, T1, T2, T3, T4, T5, T6>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, FastDelegateInvokers.VoidVal7<T0, T1, T2, T3, T4, T5, T6> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidVal7<T0, T1, T2, T3, T4, T5, T6>>(del, "del")(_0, _1, _2, _3, _4, _5, _6);
		}

		private static TResult InvokeTypeVal7<TResult, T0, T1, T2, T3, T4, T5, T6>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, FastDelegateInvokers.TypeVal7<TResult, T0, T1, T2, T3, T4, T5, T6> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeVal7<TResult, T0, T1, T2, T3, T4, T5, T6>>(del, "del")(_0, _1, _2, _3, _4, _5, _6);
		}

		private static void InvokeVoidRef7<T0, T1, T2, T3, T4, T5, T6>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, FastDelegateInvokers.VoidRef7<T0, T1, T2, T3, T4, T5, T6> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidRef7<T0, T1, T2, T3, T4, T5, T6>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6);
		}

		private static TResult InvokeTypeRef7<TResult, T0, T1, T2, T3, T4, T5, T6>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, FastDelegateInvokers.TypeRef7<TResult, T0, T1, T2, T3, T4, T5, T6> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeRef7<TResult, T0, T1, T2, T3, T4, T5, T6>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6);
		}

		private static void InvokeVoidVal8<T0, T1, T2, T3, T4, T5, T6, T7>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, FastDelegateInvokers.VoidVal8<T0, T1, T2, T3, T4, T5, T6, T7> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidVal8<T0, T1, T2, T3, T4, T5, T6, T7>>(del, "del")(_0, _1, _2, _3, _4, _5, _6, _7);
		}

		private static TResult InvokeTypeVal8<TResult, T0, T1, T2, T3, T4, T5, T6, T7>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, FastDelegateInvokers.TypeVal8<TResult, T0, T1, T2, T3, T4, T5, T6, T7> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeVal8<TResult, T0, T1, T2, T3, T4, T5, T6, T7>>(del, "del")(_0, _1, _2, _3, _4, _5, _6, _7);
		}

		private static void InvokeVoidRef8<T0, T1, T2, T3, T4, T5, T6, T7>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, FastDelegateInvokers.VoidRef8<T0, T1, T2, T3, T4, T5, T6, T7> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidRef8<T0, T1, T2, T3, T4, T5, T6, T7>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6, _7);
		}

		private static TResult InvokeTypeRef8<TResult, T0, T1, T2, T3, T4, T5, T6, T7>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, FastDelegateInvokers.TypeRef8<TResult, T0, T1, T2, T3, T4, T5, T6, T7> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeRef8<TResult, T0, T1, T2, T3, T4, T5, T6, T7>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6, _7);
		}

		private static void InvokeVoidVal9<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, FastDelegateInvokers.VoidVal9<T0, T1, T2, T3, T4, T5, T6, T7, T8> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidVal9<T0, T1, T2, T3, T4, T5, T6, T7, T8>>(del, "del")(_0, _1, _2, _3, _4, _5, _6, _7, _8);
		}

		private static TResult InvokeTypeVal9<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, FastDelegateInvokers.TypeVal9<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeVal9<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8>>(del, "del")(_0, _1, _2, _3, _4, _5, _6, _7, _8);
		}

		private static void InvokeVoidRef9<T0, T1, T2, T3, T4, T5, T6, T7, T8>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, FastDelegateInvokers.VoidRef9<T0, T1, T2, T3, T4, T5, T6, T7, T8> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidRef9<T0, T1, T2, T3, T4, T5, T6, T7, T8>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6, _7, _8);
		}

		private static TResult InvokeTypeRef9<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, FastDelegateInvokers.TypeRef9<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeRef9<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6, _7, _8);
		}

		private static void InvokeVoidVal10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, FastDelegateInvokers.VoidVal10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidVal10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>(del, "del")(_0, _1, _2, _3, _4, _5, _6, _7, _8, _9);
		}

		private static TResult InvokeTypeVal10<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, FastDelegateInvokers.TypeVal10<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeVal10<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>(del, "del")(_0, _1, _2, _3, _4, _5, _6, _7, _8, _9);
		}

		private static void InvokeVoidRef10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, FastDelegateInvokers.VoidRef10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidRef10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6, _7, _8, _9);
		}

		private static TResult InvokeTypeRef10<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, FastDelegateInvokers.TypeRef10<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeRef10<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6, _7, _8, _9);
		}

		private static void InvokeVoidVal11<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, FastDelegateInvokers.VoidVal11<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidVal11<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(del, "del")(_0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10);
		}

		private static TResult InvokeTypeVal11<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, FastDelegateInvokers.TypeVal11<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeVal11<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(del, "del")(_0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10);
		}

		private static void InvokeVoidRef11<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, FastDelegateInvokers.VoidRef11<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidRef11<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10);
		}

		private static TResult InvokeTypeRef11<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, FastDelegateInvokers.TypeRef11<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeRef11<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10);
		}

		private static void InvokeVoidVal12<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, FastDelegateInvokers.VoidVal12<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidVal12<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(del, "del")(_0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11);
		}

		private static TResult InvokeTypeVal12<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, FastDelegateInvokers.TypeVal12<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeVal12<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(del, "del")(_0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11);
		}

		private static void InvokeVoidRef12<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, FastDelegateInvokers.VoidRef12<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidRef12<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11);
		}

		private static TResult InvokeTypeRef12<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, FastDelegateInvokers.TypeRef12<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeRef12<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11);
		}

		private static void InvokeVoidVal13<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, FastDelegateInvokers.VoidVal13<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidVal13<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(del, "del")(_0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12);
		}

		private static TResult InvokeTypeVal13<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, FastDelegateInvokers.TypeVal13<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeVal13<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(del, "del")(_0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12);
		}

		private static void InvokeVoidRef13<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, FastDelegateInvokers.VoidRef13<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidRef13<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12);
		}

		private static TResult InvokeTypeRef13<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, FastDelegateInvokers.TypeRef13<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeRef13<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12);
		}

		private static void InvokeVoidVal14<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, FastDelegateInvokers.VoidVal14<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidVal14<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>(del, "del")(_0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13);
		}

		private static TResult InvokeTypeVal14<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, FastDelegateInvokers.TypeVal14<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeVal14<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>(del, "del")(_0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13);
		}

		private static void InvokeVoidRef14<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, FastDelegateInvokers.VoidRef14<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidRef14<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13);
		}

		private static TResult InvokeTypeRef14<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, FastDelegateInvokers.TypeRef14<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeRef14<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13);
		}

		private static void InvokeVoidVal15<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14, FastDelegateInvokers.VoidVal15<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidVal15<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>(del, "del")(_0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14);
		}

		private static TResult InvokeTypeVal15<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14, FastDelegateInvokers.TypeVal15<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeVal15<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>(del, "del")(_0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14);
		}

		private static void InvokeVoidRef15<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14, FastDelegateInvokers.VoidRef15<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidRef15<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14);
		}

		private static TResult InvokeTypeRef15<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14, FastDelegateInvokers.TypeRef15<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeRef15<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14);
		}

		private static void InvokeVoidVal16<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14, T15 _15, FastDelegateInvokers.VoidVal16<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidVal16<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>(del, "del")(_0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15);
		}

		private static TResult InvokeTypeVal16<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14, T15 _15, FastDelegateInvokers.TypeVal16<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeVal16<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>(del, "del")(_0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15);
		}

		private static void InvokeVoidRef16<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14, T15 _15, FastDelegateInvokers.VoidRef16<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> del)
		{
			Helpers.ThrowIfNull<FastDelegateInvokers.VoidRef16<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15);
		}

		private static TResult InvokeTypeRef16<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14, T15 _15, FastDelegateInvokers.TypeRef16<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> del)
		{
			return Helpers.ThrowIfNull<FastDelegateInvokers.TypeRef16<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>(del, "del")(ref _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15);
		}

		[Nullable(new byte[] { 1, 0, 1, 1 })]
		private static readonly ValueTuple<MethodInfo, Type>[] invokers = FastDelegateInvokers.GetInvokers();

		private const int MaxFastInvokerParams = 16;

		[Nullable(new byte[] { 1, 1, 1, 2, 1 })]
		private static readonly ConditionalWeakTable<Type, Tuple<MethodInfo, Type>> invokerCache = new ConditionalWeakTable<Type, Tuple<MethodInfo, Type>>();

		private delegate void VoidVal1<T0>(T0 _0);

		private delegate TResult TypeVal1<TResult, T0>(T0 _0);

		private delegate void VoidRef1<T0>(ref T0 _0);

		private delegate TResult TypeRef1<TResult, T0>(ref T0 _0);

		private delegate void VoidVal2<T0, T1>(T0 _0, T1 _1);

		private delegate TResult TypeVal2<TResult, T0, T1>(T0 _0, T1 _1);

		private delegate void VoidRef2<T0, T1>(ref T0 _0, T1 _1);

		private delegate TResult TypeRef2<TResult, T0, T1>(ref T0 _0, T1 _1);

		private delegate void VoidVal3<T0, T1, T2>(T0 _0, T1 _1, T2 _2);

		private delegate TResult TypeVal3<TResult, T0, T1, T2>(T0 _0, T1 _1, T2 _2);

		private delegate void VoidRef3<T0, T1, T2>(ref T0 _0, T1 _1, T2 _2);

		private delegate TResult TypeRef3<TResult, T0, T1, T2>(ref T0 _0, T1 _1, T2 _2);

		private delegate void VoidVal4<T0, T1, T2, T3>(T0 _0, T1 _1, T2 _2, T3 _3);

		private delegate TResult TypeVal4<TResult, T0, T1, T2, T3>(T0 _0, T1 _1, T2 _2, T3 _3);

		private delegate void VoidRef4<T0, T1, T2, T3>(ref T0 _0, T1 _1, T2 _2, T3 _3);

		private delegate TResult TypeRef4<TResult, T0, T1, T2, T3>(ref T0 _0, T1 _1, T2 _2, T3 _3);

		private delegate void VoidVal5<T0, T1, T2, T3, T4>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4);

		private delegate TResult TypeVal5<TResult, T0, T1, T2, T3, T4>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4);

		private delegate void VoidRef5<T0, T1, T2, T3, T4>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4);

		private delegate TResult TypeRef5<TResult, T0, T1, T2, T3, T4>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4);

		private delegate void VoidVal6<T0, T1, T2, T3, T4, T5>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5);

		private delegate TResult TypeVal6<TResult, T0, T1, T2, T3, T4, T5>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5);

		private delegate void VoidRef6<T0, T1, T2, T3, T4, T5>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5);

		private delegate TResult TypeRef6<TResult, T0, T1, T2, T3, T4, T5>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5);

		private delegate void VoidVal7<T0, T1, T2, T3, T4, T5, T6>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6);

		private delegate TResult TypeVal7<TResult, T0, T1, T2, T3, T4, T5, T6>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6);

		private delegate void VoidRef7<T0, T1, T2, T3, T4, T5, T6>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6);

		private delegate TResult TypeRef7<TResult, T0, T1, T2, T3, T4, T5, T6>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6);

		private delegate void VoidVal8<T0, T1, T2, T3, T4, T5, T6, T7>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7);

		private delegate TResult TypeVal8<TResult, T0, T1, T2, T3, T4, T5, T6, T7>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7);

		private delegate void VoidRef8<T0, T1, T2, T3, T4, T5, T6, T7>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7);

		private delegate TResult TypeRef8<TResult, T0, T1, T2, T3, T4, T5, T6, T7>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7);

		private delegate void VoidVal9<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8);

		private delegate TResult TypeVal9<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8);

		private delegate void VoidRef9<T0, T1, T2, T3, T4, T5, T6, T7, T8>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8);

		private delegate TResult TypeRef9<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8);

		private delegate void VoidVal10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9);

		private delegate TResult TypeVal10<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9);

		private delegate void VoidRef10<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9);

		private delegate TResult TypeRef10<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9);

		private delegate void VoidVal11<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10);

		private delegate TResult TypeVal11<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10);

		private delegate void VoidRef11<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10);

		private delegate TResult TypeRef11<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10);

		private delegate void VoidVal12<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11);

		private delegate TResult TypeVal12<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11);

		private delegate void VoidRef12<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11);

		private delegate TResult TypeRef12<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11);

		private delegate void VoidVal13<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12);

		private delegate TResult TypeVal13<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12);

		private delegate void VoidRef13<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12);

		private delegate TResult TypeRef13<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12);

		private delegate void VoidVal14<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13);

		private delegate TResult TypeVal14<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13);

		private delegate void VoidRef14<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13);

		private delegate TResult TypeRef14<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13);

		private delegate void VoidVal15<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14);

		private delegate TResult TypeVal15<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14);

		private delegate void VoidRef15<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14);

		private delegate TResult TypeRef15<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14);

		private delegate void VoidVal16<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14, T15 _15);

		private delegate TResult TypeVal16<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14, T15 _15);

		private delegate void VoidRef16<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14, T15 _15);

		private delegate TResult TypeRef16<TResult, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(ref T0 _0, T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14, T15 _15);
	}
}
