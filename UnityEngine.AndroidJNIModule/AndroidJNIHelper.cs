using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Android;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[NativeHeader("Modules/AndroidJNI/Public/AndroidJNIBindingsHelpers.h")]
	[StaticAccessor("AndroidJNIBindingsHelpers", StaticAccessorType.DoubleColon)]
	[NativeConditional("PLATFORM_ANDROID")]
	public static class AndroidJNIHelper
	{
		public static extern bool debug
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static IntPtr GetConstructorID(IntPtr javaClass)
		{
			return AndroidJNIHelper.GetConstructorID(javaClass, "");
		}

		public static IntPtr GetConstructorID(IntPtr javaClass, [DefaultValue("")] string signature)
		{
			return _AndroidJNIHelper.GetConstructorID(javaClass, signature);
		}

		public static IntPtr GetMethodID(IntPtr javaClass, string methodName)
		{
			return AndroidJNIHelper.GetMethodID(javaClass, methodName, "", false);
		}

		public static IntPtr GetMethodID(IntPtr javaClass, string methodName, [DefaultValue("")] string signature)
		{
			return AndroidJNIHelper.GetMethodID(javaClass, methodName, signature, false);
		}

		public static IntPtr GetMethodID(IntPtr javaClass, string methodName, [DefaultValue("")] string signature, [DefaultValue("false")] bool isStatic)
		{
			return _AndroidJNIHelper.GetMethodID(javaClass, methodName, signature, isStatic);
		}

		public static IntPtr GetFieldID(IntPtr javaClass, string fieldName)
		{
			return AndroidJNIHelper.GetFieldID(javaClass, fieldName, "", false);
		}

		public static IntPtr GetFieldID(IntPtr javaClass, string fieldName, [DefaultValue("")] string signature)
		{
			return AndroidJNIHelper.GetFieldID(javaClass, fieldName, signature, false);
		}

		public static IntPtr GetFieldID(IntPtr javaClass, string fieldName, [DefaultValue("")] string signature, [DefaultValue("false")] bool isStatic)
		{
			return _AndroidJNIHelper.GetFieldID(javaClass, fieldName, signature, isStatic);
		}

		public static IntPtr CreateJavaRunnable(AndroidJavaRunnable jrunnable)
		{
			return _AndroidJNIHelper.CreateJavaRunnable(jrunnable);
		}

		public static IntPtr CreateJavaProxy(AndroidJavaProxy proxy)
		{
			GCHandle gchandle = GCHandle.Alloc(proxy);
			IntPtr intPtr;
			try
			{
				intPtr = _AndroidJNIHelper.CreateJavaProxy(AndroidApp.UnityPlayerRaw, GCHandle.ToIntPtr(gchandle), proxy);
			}
			catch
			{
				gchandle.Free();
				throw;
			}
			return intPtr;
		}

		public static IntPtr ConvertToJNIArray(Array array)
		{
			return _AndroidJNIHelper.ConvertToJNIArray(array);
		}

		public static jvalue[] CreateJNIArgArray(object[] args)
		{
			jvalue[] array = new jvalue[args.Length];
			_AndroidJNIHelper.CreateJNIArgArray(args, array);
			return array;
		}

		public static void CreateJNIArgArray(object[] args, Span<jvalue> jniArgs)
		{
			bool flag = args.Length != jniArgs.Length;
			if (flag)
			{
				throw new ArgumentException(string.Format("Both arrays must be of the same length, but are {0} and {1}", args.Length, jniArgs.Length));
			}
			_AndroidJNIHelper.CreateJNIArgArray(args, jniArgs);
		}

		public static void DeleteJNIArgArray(object[] args, jvalue[] jniArgs)
		{
			_AndroidJNIHelper.DeleteJNIArgArray(args, jniArgs);
		}

		public static void DeleteJNIArgArray(object[] args, Span<jvalue> jniArgs)
		{
			_AndroidJNIHelper.DeleteJNIArgArray(args, jniArgs);
		}

		public static IntPtr GetConstructorID(IntPtr jclass, object[] args)
		{
			return _AndroidJNIHelper.GetConstructorID(jclass, args);
		}

		public static IntPtr GetMethodID(IntPtr jclass, string methodName, object[] args, bool isStatic)
		{
			return _AndroidJNIHelper.GetMethodID(jclass, methodName, args, isStatic);
		}

		public static string GetSignature(object obj)
		{
			return _AndroidJNIHelper.GetSignature(obj);
		}

		public static string GetSignature(object[] args)
		{
			return _AndroidJNIHelper.GetSignature(args);
		}

		public static ArrayType ConvertFromJNIArray<ArrayType>(IntPtr array)
		{
			return _AndroidJNIHelper.ConvertFromJNIArray<ArrayType>(array);
		}

		public static IntPtr GetMethodID<ReturnType>(IntPtr jclass, string methodName, object[] args, bool isStatic)
		{
			return _AndroidJNIHelper.GetMethodID<ReturnType>(jclass, methodName, args, isStatic);
		}

		public static IntPtr GetFieldID<FieldType>(IntPtr jclass, string fieldName, bool isStatic)
		{
			return _AndroidJNIHelper.GetFieldID<FieldType>(jclass, fieldName, isStatic);
		}

		public static string GetSignature<ReturnType>(object[] args)
		{
			return _AndroidJNIHelper.GetSignature<ReturnType>(args);
		}

		private unsafe static IntPtr Box(jvalue val, string boxedClass, string signature)
		{
			IntPtr intPtr = AndroidJNISafe.FindClass(boxedClass);
			IntPtr intPtr2;
			try
			{
				IntPtr staticMethodID = AndroidJNISafe.GetStaticMethodID(intPtr, "valueOf", signature);
				Span<jvalue> span = new Span<jvalue>((void*)(&val), 1);
				intPtr2 = AndroidJNISafe.CallStaticObjectMethod(intPtr, staticMethodID, span);
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(intPtr);
			}
			return intPtr2;
		}

		public static IntPtr Box(sbyte value)
		{
			return AndroidJNIHelper.Box(new jvalue
			{
				b = value
			}, "java/lang/Byte", "(B)Ljava/lang/Byte;");
		}

		public static IntPtr Box(short value)
		{
			return AndroidJNIHelper.Box(new jvalue
			{
				s = value
			}, "java/lang/Short", "(S)Ljava/lang/Short;");
		}

		public static IntPtr Box(int value)
		{
			return AndroidJNIHelper.Box(new jvalue
			{
				i = value
			}, "java/lang/Integer", "(I)Ljava/lang/Integer;");
		}

		public static IntPtr Box(long value)
		{
			return AndroidJNIHelper.Box(new jvalue
			{
				j = value
			}, "java/lang/Long", "(J)Ljava/lang/Long;");
		}

		public static IntPtr Box(float value)
		{
			return AndroidJNIHelper.Box(new jvalue
			{
				f = value
			}, "java/lang/Float", "(F)Ljava/lang/Float;");
		}

		public static IntPtr Box(double value)
		{
			return AndroidJNIHelper.Box(new jvalue
			{
				d = value
			}, "java/lang/Double", "(D)Ljava/lang/Double;");
		}

		public static IntPtr Box(char value)
		{
			return AndroidJNIHelper.Box(new jvalue
			{
				c = value
			}, "java/lang/Character", "(C)Ljava/lang/Character;");
		}

		public static IntPtr Box(bool value)
		{
			return AndroidJNIHelper.Box(new jvalue
			{
				z = value
			}, "java/lang/Boolean", "(Z)Ljava/lang/Boolean;");
		}

		private static IntPtr GetUnboxMethod(IntPtr obj, string methodName, string signature)
		{
			IntPtr objectClass = AndroidJNISafe.GetObjectClass(obj);
			IntPtr methodID;
			try
			{
				methodID = AndroidJNISafe.GetMethodID(objectClass, methodName, signature);
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(objectClass);
			}
			return methodID;
		}

		public static void Unbox(IntPtr obj, out sbyte value)
		{
			IntPtr unboxMethod = AndroidJNIHelper.GetUnboxMethod(obj, "byteValue", "()B");
			value = AndroidJNISafe.CallSByteMethod(obj, unboxMethod, default(Span<jvalue>));
		}

		public static void Unbox(IntPtr obj, out short value)
		{
			IntPtr unboxMethod = AndroidJNIHelper.GetUnboxMethod(obj, "shortValue", "()S");
			value = AndroidJNISafe.CallShortMethod(obj, unboxMethod, default(Span<jvalue>));
		}

		public static void Unbox(IntPtr obj, out int value)
		{
			IntPtr unboxMethod = AndroidJNIHelper.GetUnboxMethod(obj, "intValue", "()I");
			value = AndroidJNISafe.CallIntMethod(obj, unboxMethod, default(Span<jvalue>));
		}

		public static void Unbox(IntPtr obj, out long value)
		{
			IntPtr unboxMethod = AndroidJNIHelper.GetUnboxMethod(obj, "longValue", "()J");
			value = AndroidJNISafe.CallLongMethod(obj, unboxMethod, default(Span<jvalue>));
		}

		public static void Unbox(IntPtr obj, out float value)
		{
			IntPtr unboxMethod = AndroidJNIHelper.GetUnboxMethod(obj, "floatValue", "()F");
			value = AndroidJNISafe.CallFloatMethod(obj, unboxMethod, default(Span<jvalue>));
		}

		public static void Unbox(IntPtr obj, out double value)
		{
			IntPtr unboxMethod = AndroidJNIHelper.GetUnboxMethod(obj, "doubleValue", "()D");
			value = AndroidJNISafe.CallDoubleMethod(obj, unboxMethod, default(Span<jvalue>));
		}

		public static void Unbox(IntPtr obj, out char value)
		{
			IntPtr unboxMethod = AndroidJNIHelper.GetUnboxMethod(obj, "charValue", "()C");
			value = AndroidJNISafe.CallCharMethod(obj, unboxMethod, default(Span<jvalue>));
		}

		public static void Unbox(IntPtr obj, out bool value)
		{
			IntPtr unboxMethod = AndroidJNIHelper.GetUnboxMethod(obj, "booleanValue", "()Z");
			value = AndroidJNISafe.CallBooleanMethod(obj, unboxMethod, default(Span<jvalue>));
		}
	}
}
