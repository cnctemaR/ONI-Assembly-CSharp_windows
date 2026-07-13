using System;

namespace UnityEngine
{
	internal class AndroidJNISafe
	{
		public static void CheckException()
		{
			IntPtr intPtr = AndroidJNI.ExceptionOccurred();
			bool flag = intPtr != IntPtr.Zero;
			if (flag)
			{
				AndroidJNI.ExceptionClear();
				IntPtr intPtr2 = AndroidJNI.FindClass("java/lang/Throwable");
				IntPtr intPtr3 = AndroidJNI.FindClass("android/util/Log");
				try
				{
					IntPtr methodID = AndroidJNI.GetMethodID(intPtr2, "toString", "()Ljava/lang/String;");
					IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(intPtr3, "getStackTraceString", "(Ljava/lang/Throwable;)Ljava/lang/String;");
					string text = AndroidJNI.CallStringMethod(intPtr, methodID, new jvalue[0]);
					jvalue[] array = new jvalue[1];
					array[0].l = intPtr;
					string text2 = AndroidJNI.CallStaticStringMethod(intPtr3, staticMethodID, array);
					throw new AndroidJavaException(text, text2);
				}
				finally
				{
					AndroidJNISafe.DeleteLocalRef(intPtr);
					AndroidJNISafe.DeleteLocalRef(intPtr2);
					AndroidJNISafe.DeleteLocalRef(intPtr3);
				}
			}
		}

		public static void DeleteGlobalRef(IntPtr globalref)
		{
			bool flag = globalref != IntPtr.Zero;
			if (flag)
			{
				AndroidJNI.DeleteGlobalRef(globalref);
			}
		}

		public static void QueueDeleteGlobalRef(IntPtr globalref)
		{
			bool flag = globalref != IntPtr.Zero;
			if (flag)
			{
				AndroidJNI.QueueDeleteGlobalRef(globalref);
			}
		}

		public static void DeleteWeakGlobalRef(IntPtr globalref)
		{
			bool flag = globalref != IntPtr.Zero;
			if (flag)
			{
				AndroidJNI.DeleteWeakGlobalRef(globalref);
			}
		}

		public static void DeleteLocalRef(IntPtr localref)
		{
			bool flag = localref != IntPtr.Zero;
			if (flag)
			{
				AndroidJNI.DeleteLocalRef(localref);
			}
		}

		public static IntPtr NewString(string chars)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.NewString(chars);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static IntPtr NewStringUTF(string bytes)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.NewStringUTF(bytes);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static string GetStringChars(IntPtr str)
		{
			string stringChars;
			try
			{
				stringChars = AndroidJNI.GetStringChars(str);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return stringChars;
		}

		public static string GetStringUTFChars(IntPtr str)
		{
			string stringUTFChars;
			try
			{
				stringUTFChars = AndroidJNI.GetStringUTFChars(str);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return stringUTFChars;
		}

		public static IntPtr GetObjectClass(IntPtr ptr)
		{
			IntPtr objectClass;
			try
			{
				objectClass = AndroidJNI.GetObjectClass(ptr);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return objectClass;
		}

		public static IntPtr GetStaticMethodID(IntPtr clazz, string name, string sig)
		{
			IntPtr staticMethodID;
			try
			{
				staticMethodID = AndroidJNI.GetStaticMethodID(clazz, name, sig);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticMethodID;
		}

		public static IntPtr GetMethodID(IntPtr obj, string name, string sig)
		{
			IntPtr methodID;
			try
			{
				methodID = AndroidJNI.GetMethodID(obj, name, sig);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return methodID;
		}

		public static IntPtr GetFieldID(IntPtr clazz, string name, string sig)
		{
			IntPtr fieldID;
			try
			{
				fieldID = AndroidJNI.GetFieldID(clazz, name, sig);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return fieldID;
		}

		public static IntPtr GetStaticFieldID(IntPtr clazz, string name, string sig)
		{
			IntPtr staticFieldID;
			try
			{
				staticFieldID = AndroidJNI.GetStaticFieldID(clazz, name, sig);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticFieldID;
		}

		public static IntPtr FromReflectedMethod(IntPtr refMethod)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.FromReflectedMethod(refMethod);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static IntPtr FromReflectedField(IntPtr refField)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.FromReflectedField(refField);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static IntPtr FindClass(string name)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.FindClass(name);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static void PushLocalFrame(int capacity)
		{
			bool flag = AndroidJNI.PushLocalFrame(capacity) < 0;
			if (flag)
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static IntPtr NewObject(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.NewObject(clazz, methodID, new Span<jvalue>(args));
		}

		public static IntPtr NewObject(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.NewObject(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static void SetStaticObjectField(IntPtr clazz, IntPtr fieldID, IntPtr val)
		{
			try
			{
				AndroidJNI.SetStaticObjectField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStaticStringField(IntPtr clazz, IntPtr fieldID, string val)
		{
			try
			{
				AndroidJNI.SetStaticStringField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStaticCharField(IntPtr clazz, IntPtr fieldID, char val)
		{
			try
			{
				AndroidJNI.SetStaticCharField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStaticDoubleField(IntPtr clazz, IntPtr fieldID, double val)
		{
			try
			{
				AndroidJNI.SetStaticDoubleField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStaticFloatField(IntPtr clazz, IntPtr fieldID, float val)
		{
			try
			{
				AndroidJNI.SetStaticFloatField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStaticLongField(IntPtr clazz, IntPtr fieldID, long val)
		{
			try
			{
				AndroidJNI.SetStaticLongField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStaticShortField(IntPtr clazz, IntPtr fieldID, short val)
		{
			try
			{
				AndroidJNI.SetStaticShortField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStaticSByteField(IntPtr clazz, IntPtr fieldID, sbyte val)
		{
			try
			{
				AndroidJNI.SetStaticSByteField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStaticBooleanField(IntPtr clazz, IntPtr fieldID, bool val)
		{
			try
			{
				AndroidJNI.SetStaticBooleanField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStaticIntField(IntPtr clazz, IntPtr fieldID, int val)
		{
			try
			{
				AndroidJNI.SetStaticIntField(clazz, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static IntPtr GetStaticObjectField(IntPtr clazz, IntPtr fieldID)
		{
			IntPtr staticObjectField;
			try
			{
				staticObjectField = AndroidJNI.GetStaticObjectField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticObjectField;
		}

		public static string GetStaticStringField(IntPtr clazz, IntPtr fieldID)
		{
			string staticStringField;
			try
			{
				staticStringField = AndroidJNI.GetStaticStringField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticStringField;
		}

		public static char GetStaticCharField(IntPtr clazz, IntPtr fieldID)
		{
			char staticCharField;
			try
			{
				staticCharField = AndroidJNI.GetStaticCharField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticCharField;
		}

		public static double GetStaticDoubleField(IntPtr clazz, IntPtr fieldID)
		{
			double staticDoubleField;
			try
			{
				staticDoubleField = AndroidJNI.GetStaticDoubleField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticDoubleField;
		}

		public static float GetStaticFloatField(IntPtr clazz, IntPtr fieldID)
		{
			float staticFloatField;
			try
			{
				staticFloatField = AndroidJNI.GetStaticFloatField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticFloatField;
		}

		public static long GetStaticLongField(IntPtr clazz, IntPtr fieldID)
		{
			long staticLongField;
			try
			{
				staticLongField = AndroidJNI.GetStaticLongField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticLongField;
		}

		public static short GetStaticShortField(IntPtr clazz, IntPtr fieldID)
		{
			short staticShortField;
			try
			{
				staticShortField = AndroidJNI.GetStaticShortField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticShortField;
		}

		public static sbyte GetStaticSByteField(IntPtr clazz, IntPtr fieldID)
		{
			sbyte staticSByteField;
			try
			{
				staticSByteField = AndroidJNI.GetStaticSByteField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticSByteField;
		}

		public static bool GetStaticBooleanField(IntPtr clazz, IntPtr fieldID)
		{
			bool staticBooleanField;
			try
			{
				staticBooleanField = AndroidJNI.GetStaticBooleanField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticBooleanField;
		}

		public static int GetStaticIntField(IntPtr clazz, IntPtr fieldID)
		{
			int staticIntField;
			try
			{
				staticIntField = AndroidJNI.GetStaticIntField(clazz, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return staticIntField;
		}

		public static void CallStaticVoidMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			AndroidJNISafe.CallStaticVoidMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public static void CallStaticVoidMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			try
			{
				AndroidJNI.CallStaticVoidMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static IntPtr CallStaticObjectMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallStaticObjectMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public static IntPtr CallStaticObjectMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.CallStaticObjectMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static string CallStaticStringMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallStaticStringMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public static string CallStaticStringMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			string text = null;
			string text2;
			try
			{
				text = AndroidJNI.CallStaticStringMethod(clazz, methodID, args);
				text2 = text;
			}
			finally
			{
				bool flag = text == null;
				if (flag)
				{
					AndroidJNISafe.CheckException();
				}
			}
			return text2;
		}

		public static char CallStaticCharMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallStaticCharMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public static char CallStaticCharMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			char c;
			try
			{
				c = AndroidJNI.CallStaticCharMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return c;
		}

		public static double CallStaticDoubleMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallStaticDoubleMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public static double CallStaticDoubleMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			double num;
			try
			{
				num = AndroidJNI.CallStaticDoubleMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return num;
		}

		public static float CallStaticFloatMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallStaticFloatMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public static float CallStaticFloatMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			float num;
			try
			{
				num = AndroidJNI.CallStaticFloatMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return num;
		}

		public static long CallStaticLongMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallStaticLongMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public static long CallStaticLongMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			long num;
			try
			{
				num = AndroidJNI.CallStaticLongMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return num;
		}

		public static short CallStaticShortMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallStaticShortMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public static short CallStaticShortMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			short num;
			try
			{
				num = AndroidJNI.CallStaticShortMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return num;
		}

		public static sbyte CallStaticSByteMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallStaticSByteMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public static sbyte CallStaticSByteMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			sbyte b;
			try
			{
				b = AndroidJNI.CallStaticSByteMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return b;
		}

		public static bool CallStaticBooleanMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallStaticBooleanMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public static bool CallStaticBooleanMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			bool flag;
			try
			{
				flag = AndroidJNI.CallStaticBooleanMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return flag;
		}

		public static int CallStaticIntMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallStaticIntMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public static int CallStaticIntMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			int num;
			try
			{
				num = AndroidJNI.CallStaticIntMethod(clazz, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return num;
		}

		public static void SetObjectField(IntPtr obj, IntPtr fieldID, IntPtr val)
		{
			try
			{
				AndroidJNI.SetObjectField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetStringField(IntPtr obj, IntPtr fieldID, string val)
		{
			try
			{
				AndroidJNI.SetStringField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetCharField(IntPtr obj, IntPtr fieldID, char val)
		{
			try
			{
				AndroidJNI.SetCharField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetDoubleField(IntPtr obj, IntPtr fieldID, double val)
		{
			try
			{
				AndroidJNI.SetDoubleField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetFloatField(IntPtr obj, IntPtr fieldID, float val)
		{
			try
			{
				AndroidJNI.SetFloatField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetLongField(IntPtr obj, IntPtr fieldID, long val)
		{
			try
			{
				AndroidJNI.SetLongField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetShortField(IntPtr obj, IntPtr fieldID, short val)
		{
			try
			{
				AndroidJNI.SetShortField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetSByteField(IntPtr obj, IntPtr fieldID, sbyte val)
		{
			try
			{
				AndroidJNI.SetSByteField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetBooleanField(IntPtr obj, IntPtr fieldID, bool val)
		{
			try
			{
				AndroidJNI.SetBooleanField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static void SetIntField(IntPtr obj, IntPtr fieldID, int val)
		{
			try
			{
				AndroidJNI.SetIntField(obj, fieldID, val);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static IntPtr GetObjectField(IntPtr obj, IntPtr fieldID)
		{
			IntPtr objectField;
			try
			{
				objectField = AndroidJNI.GetObjectField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return objectField;
		}

		public static string GetStringField(IntPtr obj, IntPtr fieldID)
		{
			string stringField;
			try
			{
				stringField = AndroidJNI.GetStringField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return stringField;
		}

		public static char GetCharField(IntPtr obj, IntPtr fieldID)
		{
			char charField;
			try
			{
				charField = AndroidJNI.GetCharField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return charField;
		}

		public static double GetDoubleField(IntPtr obj, IntPtr fieldID)
		{
			double doubleField;
			try
			{
				doubleField = AndroidJNI.GetDoubleField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return doubleField;
		}

		public static float GetFloatField(IntPtr obj, IntPtr fieldID)
		{
			float floatField;
			try
			{
				floatField = AndroidJNI.GetFloatField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return floatField;
		}

		public static long GetLongField(IntPtr obj, IntPtr fieldID)
		{
			long longField;
			try
			{
				longField = AndroidJNI.GetLongField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return longField;
		}

		public static short GetShortField(IntPtr obj, IntPtr fieldID)
		{
			short shortField;
			try
			{
				shortField = AndroidJNI.GetShortField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return shortField;
		}

		public static sbyte GetSByteField(IntPtr obj, IntPtr fieldID)
		{
			sbyte sbyteField;
			try
			{
				sbyteField = AndroidJNI.GetSByteField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return sbyteField;
		}

		public static bool GetBooleanField(IntPtr obj, IntPtr fieldID)
		{
			bool booleanField;
			try
			{
				booleanField = AndroidJNI.GetBooleanField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return booleanField;
		}

		public static int GetIntField(IntPtr obj, IntPtr fieldID)
		{
			int intField;
			try
			{
				intField = AndroidJNI.GetIntField(obj, fieldID);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intField;
		}

		public static void CallVoidMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			AndroidJNISafe.CallVoidMethod(obj, methodID, new Span<jvalue>(args));
		}

		public static void CallVoidMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			try
			{
				AndroidJNI.CallVoidMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
		}

		public static IntPtr CallObjectMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallObjectMethod(obj, methodID, new Span<jvalue>(args));
		}

		public static IntPtr CallObjectMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.CallObjectMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static string CallStringMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallStringMethod(obj, methodID, new Span<jvalue>(args));
		}

		public static string CallStringMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			string text = null;
			string text2;
			try
			{
				text = AndroidJNI.CallStringMethod(obj, methodID, args);
				text2 = text;
			}
			finally
			{
				bool flag = text == null;
				if (flag)
				{
					AndroidJNISafe.CheckException();
				}
			}
			return text2;
		}

		public static char CallCharMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallCharMethod(obj, methodID, new Span<jvalue>(args));
		}

		public static char CallCharMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			char c;
			try
			{
				c = AndroidJNI.CallCharMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return c;
		}

		public static double CallDoubleMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallDoubleMethod(obj, methodID, new Span<jvalue>(args));
		}

		public static double CallDoubleMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			double num;
			try
			{
				num = AndroidJNI.CallDoubleMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return num;
		}

		public static float CallFloatMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallFloatMethod(obj, methodID, new Span<jvalue>(args));
		}

		public static float CallFloatMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			float num;
			try
			{
				num = AndroidJNI.CallFloatMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return num;
		}

		public static long CallLongMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallLongMethod(obj, methodID, new Span<jvalue>(args));
		}

		public static long CallLongMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			long num;
			try
			{
				num = AndroidJNI.CallLongMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return num;
		}

		public static short CallShortMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallShortMethod(obj, methodID, new Span<jvalue>(args));
		}

		public static short CallShortMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			short num;
			try
			{
				num = AndroidJNI.CallShortMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return num;
		}

		public static sbyte CallSByteMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallSByteMethod(obj, methodID, new Span<jvalue>(args));
		}

		public static sbyte CallSByteMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			sbyte b;
			try
			{
				b = AndroidJNI.CallSByteMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return b;
		}

		public static bool CallBooleanMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallBooleanMethod(obj, methodID, new Span<jvalue>(args));
		}

		public static bool CallBooleanMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			bool flag;
			try
			{
				flag = AndroidJNI.CallBooleanMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return flag;
		}

		public static int CallIntMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNISafe.CallIntMethod(obj, methodID, new Span<jvalue>(args));
		}

		public static int CallIntMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			int num;
			try
			{
				num = AndroidJNI.CallIntMethod(obj, methodID, args);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return num;
		}

		public static IntPtr[] FromObjectArray(IntPtr array)
		{
			IntPtr[] array2;
			try
			{
				array2 = AndroidJNI.FromObjectArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return array2;
		}

		public static char[] FromCharArray(IntPtr array)
		{
			char[] array2;
			try
			{
				array2 = AndroidJNI.FromCharArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return array2;
		}

		public static double[] FromDoubleArray(IntPtr array)
		{
			double[] array2;
			try
			{
				array2 = AndroidJNI.FromDoubleArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return array2;
		}

		public static float[] FromFloatArray(IntPtr array)
		{
			float[] array2;
			try
			{
				array2 = AndroidJNI.FromFloatArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return array2;
		}

		public static long[] FromLongArray(IntPtr array)
		{
			long[] array2;
			try
			{
				array2 = AndroidJNI.FromLongArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return array2;
		}

		public static short[] FromShortArray(IntPtr array)
		{
			short[] array2;
			try
			{
				array2 = AndroidJNI.FromShortArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return array2;
		}

		public static byte[] FromByteArray(IntPtr array)
		{
			byte[] array2;
			try
			{
				array2 = AndroidJNI.FromByteArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return array2;
		}

		public static sbyte[] FromSByteArray(IntPtr array)
		{
			sbyte[] array2;
			try
			{
				array2 = AndroidJNI.FromSByteArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return array2;
		}

		public static bool[] FromBooleanArray(IntPtr array)
		{
			bool[] array2;
			try
			{
				array2 = AndroidJNI.FromBooleanArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return array2;
		}

		public static int[] FromIntArray(IntPtr array)
		{
			int[] array2;
			try
			{
				array2 = AndroidJNI.FromIntArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return array2;
		}

		public static IntPtr ToObjectArray(IntPtr[] array)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.ToObjectArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static IntPtr ToObjectArray(IntPtr[] array, IntPtr type)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.ToObjectArray(array, type);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static IntPtr ToCharArray(char[] array)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.ToCharArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static IntPtr ToDoubleArray(double[] array)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.ToDoubleArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static IntPtr ToFloatArray(float[] array)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.ToFloatArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static IntPtr ToLongArray(long[] array)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.ToLongArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static IntPtr ToShortArray(short[] array)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.ToShortArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static IntPtr ToByteArray(byte[] array)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.ToByteArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static IntPtr ToSByteArray(sbyte[] array)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.ToSByteArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static IntPtr ToBooleanArray(bool[] array)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.ToBooleanArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static IntPtr ToIntArray(int[] array)
		{
			IntPtr intPtr;
			try
			{
				intPtr = AndroidJNI.ToIntArray(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return intPtr;
		}

		public static IntPtr GetObjectArrayElement(IntPtr array, int index)
		{
			IntPtr objectArrayElement;
			try
			{
				objectArrayElement = AndroidJNI.GetObjectArrayElement(array, index);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return objectArrayElement;
		}

		public static int GetArrayLength(IntPtr array)
		{
			int arrayLength;
			try
			{
				arrayLength = AndroidJNI.GetArrayLength(array);
			}
			finally
			{
				AndroidJNISafe.CheckException();
			}
			return arrayLength;
		}
	}
}
