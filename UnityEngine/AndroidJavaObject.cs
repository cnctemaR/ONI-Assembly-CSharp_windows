using System;
using System.Text;

namespace UnityEngine
{
	public class AndroidJavaObject : IDisposable
	{
		public AndroidJavaObject(string className, params object[] args)
			: this()
		{
			this._AndroidJavaObject(className, args);
		}

		internal AndroidJavaObject(IntPtr jobject)
			: this()
		{
			if (jobject == IntPtr.Zero)
			{
				throw new Exception("JNI: Init'd AndroidJavaObject with null ptr!");
			}
			IntPtr objectClass = AndroidJNISafe.GetObjectClass(jobject);
			this.m_jobject = AndroidJNI.NewGlobalRef(jobject);
			this.m_jclass = AndroidJNI.NewGlobalRef(objectClass);
			AndroidJNISafe.DeleteLocalRef(objectClass);
		}

		internal AndroidJavaObject()
		{
		}

		public void Dispose()
		{
			this._Dispose();
		}

		public void Call(string methodName, params object[] args)
		{
			this._Call(methodName, args);
		}

		public void CallStatic(string methodName, params object[] args)
		{
			this._CallStatic(methodName, args);
		}

		public FieldType Get<FieldType>(string fieldName)
		{
			return this._Get<FieldType>(fieldName);
		}

		public void Set<FieldType>(string fieldName, FieldType val)
		{
			this._Set<FieldType>(fieldName, val);
		}

		public FieldType GetStatic<FieldType>(string fieldName)
		{
			return this._GetStatic<FieldType>(fieldName);
		}

		public void SetStatic<FieldType>(string fieldName, FieldType val)
		{
			this._SetStatic<FieldType>(fieldName, val);
		}

		public IntPtr GetRawObject()
		{
			return this._GetRawObject();
		}

		public IntPtr GetRawClass()
		{
			return this._GetRawClass();
		}

		public ReturnType Call<ReturnType>(string methodName, params object[] args)
		{
			return this._Call<ReturnType>(methodName, args);
		}

		public ReturnType CallStatic<ReturnType>(string methodName, params object[] args)
		{
			return this._CallStatic<ReturnType>(methodName, args);
		}

		protected void DebugPrint(string msg)
		{
			if (AndroidJavaObject.enableDebugPrints)
			{
				Debug.Log(msg);
			}
		}

		protected void DebugPrint(string call, string methodName, string signature, object[] args)
		{
			if (AndroidJavaObject.enableDebugPrints)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (object obj in args)
				{
					stringBuilder.Append(", ");
					stringBuilder.Append((obj != null) ? obj.GetType().ToString() : "<null>");
				}
				Debug.Log(string.Concat(new string[]
				{
					call,
					"(\"",
					methodName,
					"\"",
					stringBuilder.ToString(),
					") = ",
					signature
				}));
			}
		}

		private void _AndroidJavaObject(string className, params object[] args)
		{
			this.DebugPrint("Creating AndroidJavaObject from " + className);
			if (args == null)
			{
				args = new object[1];
			}
			using (AndroidJavaObject androidJavaObject = AndroidJavaObject.FindClass(className))
			{
				this.m_jclass = AndroidJNI.NewGlobalRef(androidJavaObject.GetRawObject());
				jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
				try
				{
					IntPtr constructorID = AndroidJNIHelper.GetConstructorID(this.m_jclass, args);
					IntPtr intPtr = AndroidJNISafe.NewObject(this.m_jclass, constructorID, array);
					this.m_jobject = AndroidJNI.NewGlobalRef(intPtr);
					AndroidJNISafe.DeleteLocalRef(intPtr);
				}
				finally
				{
					AndroidJNIHelper.DeleteJNIArgArray(args, array);
				}
			}
		}

		~AndroidJavaObject()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.m_disposed)
			{
				this.m_disposed = true;
				AndroidJNISafe.DeleteGlobalRef(this.m_jobject);
				AndroidJNISafe.DeleteGlobalRef(this.m_jclass);
			}
		}

		protected void _Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void _Call(string methodName, params object[] args)
		{
			if (args == null)
			{
				args = new object[1];
			}
			IntPtr methodID = AndroidJNIHelper.GetMethodID(this.m_jclass, methodName, args, false);
			jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
			try
			{
				AndroidJNISafe.CallVoidMethod(this.m_jobject, methodID, array);
			}
			finally
			{
				AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		protected ReturnType _Call<ReturnType>(string methodName, params object[] args)
		{
			if (args == null)
			{
				args = new object[1];
			}
			IntPtr methodID = AndroidJNIHelper.GetMethodID<ReturnType>(this.m_jclass, methodName, args, false);
			jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
			ReturnType returnType;
			try
			{
				if (AndroidReflection.IsPrimitive(typeof(ReturnType)))
				{
					if (typeof(ReturnType) == typeof(int))
					{
						returnType = (ReturnType)((object)AndroidJNISafe.CallIntMethod(this.m_jobject, methodID, array));
					}
					else if (typeof(ReturnType) == typeof(bool))
					{
						returnType = (ReturnType)((object)AndroidJNISafe.CallBooleanMethod(this.m_jobject, methodID, array));
					}
					else if (typeof(ReturnType) == typeof(byte))
					{
						returnType = (ReturnType)((object)AndroidJNISafe.CallByteMethod(this.m_jobject, methodID, array));
					}
					else if (typeof(ReturnType) == typeof(short))
					{
						returnType = (ReturnType)((object)AndroidJNISafe.CallShortMethod(this.m_jobject, methodID, array));
					}
					else if (typeof(ReturnType) == typeof(long))
					{
						returnType = (ReturnType)((object)AndroidJNISafe.CallLongMethod(this.m_jobject, methodID, array));
					}
					else if (typeof(ReturnType) == typeof(float))
					{
						returnType = (ReturnType)((object)AndroidJNISafe.CallFloatMethod(this.m_jobject, methodID, array));
					}
					else if (typeof(ReturnType) == typeof(double))
					{
						returnType = (ReturnType)((object)AndroidJNISafe.CallDoubleMethod(this.m_jobject, methodID, array));
					}
					else if (typeof(ReturnType) == typeof(char))
					{
						returnType = (ReturnType)((object)AndroidJNISafe.CallCharMethod(this.m_jobject, methodID, array));
					}
					else
					{
						returnType = default(ReturnType);
					}
				}
				else if (typeof(ReturnType) == typeof(string))
				{
					returnType = (ReturnType)((object)AndroidJNISafe.CallStringMethod(this.m_jobject, methodID, array));
				}
				else if (typeof(ReturnType) == typeof(AndroidJavaClass))
				{
					IntPtr intPtr = AndroidJNISafe.CallObjectMethod(this.m_jobject, methodID, array);
					returnType = ((!(intPtr == IntPtr.Zero)) ? ((ReturnType)((object)AndroidJavaObject.AndroidJavaClassDeleteLocalRef(intPtr))) : default(ReturnType));
				}
				else if (typeof(ReturnType) == typeof(AndroidJavaObject))
				{
					IntPtr intPtr2 = AndroidJNISafe.CallObjectMethod(this.m_jobject, methodID, array);
					returnType = ((!(intPtr2 == IntPtr.Zero)) ? ((ReturnType)((object)AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(intPtr2))) : default(ReturnType));
				}
				else
				{
					if (!AndroidReflection.IsAssignableFrom(typeof(Array), typeof(ReturnType)))
					{
						throw new Exception("JNI: Unknown return type '" + typeof(ReturnType) + "'");
					}
					IntPtr intPtr3 = AndroidJNISafe.CallObjectMethod(this.m_jobject, methodID, array);
					returnType = ((!(intPtr3 == IntPtr.Zero)) ? ((ReturnType)((object)AndroidJNIHelper.ConvertFromJNIArray<ReturnType>(intPtr3))) : default(ReturnType));
				}
			}
			finally
			{
				AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
			return returnType;
		}

		protected FieldType _Get<FieldType>(string fieldName)
		{
			IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(this.m_jclass, fieldName, false);
			FieldType fieldType;
			if (AndroidReflection.IsPrimitive(typeof(FieldType)))
			{
				if (typeof(FieldType) == typeof(int))
				{
					fieldType = (FieldType)((object)AndroidJNISafe.GetIntField(this.m_jobject, fieldID));
				}
				else if (typeof(FieldType) == typeof(bool))
				{
					fieldType = (FieldType)((object)AndroidJNISafe.GetBooleanField(this.m_jobject, fieldID));
				}
				else if (typeof(FieldType) == typeof(byte))
				{
					fieldType = (FieldType)((object)AndroidJNISafe.GetByteField(this.m_jobject, fieldID));
				}
				else if (typeof(FieldType) == typeof(short))
				{
					fieldType = (FieldType)((object)AndroidJNISafe.GetShortField(this.m_jobject, fieldID));
				}
				else if (typeof(FieldType) == typeof(long))
				{
					fieldType = (FieldType)((object)AndroidJNISafe.GetLongField(this.m_jobject, fieldID));
				}
				else if (typeof(FieldType) == typeof(float))
				{
					fieldType = (FieldType)((object)AndroidJNISafe.GetFloatField(this.m_jobject, fieldID));
				}
				else if (typeof(FieldType) == typeof(double))
				{
					fieldType = (FieldType)((object)AndroidJNISafe.GetDoubleField(this.m_jobject, fieldID));
				}
				else if (typeof(FieldType) == typeof(char))
				{
					fieldType = (FieldType)((object)AndroidJNISafe.GetCharField(this.m_jobject, fieldID));
				}
				else
				{
					fieldType = default(FieldType);
				}
			}
			else if (typeof(FieldType) == typeof(string))
			{
				fieldType = (FieldType)((object)AndroidJNISafe.GetStringField(this.m_jobject, fieldID));
			}
			else if (typeof(FieldType) == typeof(AndroidJavaClass))
			{
				IntPtr objectField = AndroidJNISafe.GetObjectField(this.m_jobject, fieldID);
				fieldType = ((!(objectField == IntPtr.Zero)) ? ((FieldType)((object)AndroidJavaObject.AndroidJavaClassDeleteLocalRef(objectField))) : default(FieldType));
			}
			else if (typeof(FieldType) == typeof(AndroidJavaObject))
			{
				IntPtr objectField2 = AndroidJNISafe.GetObjectField(this.m_jobject, fieldID);
				fieldType = ((!(objectField2 == IntPtr.Zero)) ? ((FieldType)((object)AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(objectField2))) : default(FieldType));
			}
			else
			{
				if (!AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType)))
				{
					throw new Exception("JNI: Unknown field type '" + typeof(FieldType) + "'");
				}
				IntPtr objectField3 = AndroidJNISafe.GetObjectField(this.m_jobject, fieldID);
				fieldType = ((!(objectField3 == IntPtr.Zero)) ? ((FieldType)((object)AndroidJNIHelper.ConvertFromJNIArray<FieldType>(objectField3))) : default(FieldType));
			}
			return fieldType;
		}

		protected void _Set<FieldType>(string fieldName, FieldType val)
		{
			IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(this.m_jclass, fieldName, false);
			if (AndroidReflection.IsPrimitive(typeof(FieldType)))
			{
				if (typeof(FieldType) == typeof(int))
				{
					AndroidJNISafe.SetIntField(this.m_jobject, fieldID, (int)((object)val));
				}
				else if (typeof(FieldType) == typeof(bool))
				{
					AndroidJNISafe.SetBooleanField(this.m_jobject, fieldID, (bool)((object)val));
				}
				else if (typeof(FieldType) == typeof(byte))
				{
					AndroidJNISafe.SetByteField(this.m_jobject, fieldID, (byte)((object)val));
				}
				else if (typeof(FieldType) == typeof(short))
				{
					AndroidJNISafe.SetShortField(this.m_jobject, fieldID, (short)((object)val));
				}
				else if (typeof(FieldType) == typeof(long))
				{
					AndroidJNISafe.SetLongField(this.m_jobject, fieldID, (long)((object)val));
				}
				else if (typeof(FieldType) == typeof(float))
				{
					AndroidJNISafe.SetFloatField(this.m_jobject, fieldID, (float)((object)val));
				}
				else if (typeof(FieldType) == typeof(double))
				{
					AndroidJNISafe.SetDoubleField(this.m_jobject, fieldID, (double)((object)val));
				}
				else if (typeof(FieldType) == typeof(char))
				{
					AndroidJNISafe.SetCharField(this.m_jobject, fieldID, (char)((object)val));
				}
			}
			else if (typeof(FieldType) == typeof(string))
			{
				AndroidJNISafe.SetStringField(this.m_jobject, fieldID, (string)((object)val));
			}
			else if (typeof(FieldType) == typeof(AndroidJavaClass))
			{
				AndroidJNISafe.SetObjectField(this.m_jobject, fieldID, ((AndroidJavaClass)((object)val)).m_jclass);
			}
			else if (typeof(FieldType) == typeof(AndroidJavaObject))
			{
				AndroidJNISafe.SetObjectField(this.m_jobject, fieldID, ((AndroidJavaObject)((object)val)).m_jobject);
			}
			else
			{
				if (!AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType)))
				{
					throw new Exception("JNI: Unknown field type '" + typeof(FieldType) + "'");
				}
				IntPtr intPtr = AndroidJNIHelper.ConvertToJNIArray((Array)((object)val));
				AndroidJNISafe.SetObjectField(this.m_jclass, fieldID, intPtr);
			}
		}

		protected void _CallStatic(string methodName, params object[] args)
		{
			if (args == null)
			{
				args = new object[1];
			}
			IntPtr methodID = AndroidJNIHelper.GetMethodID(this.m_jclass, methodName, args, true);
			jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
			try
			{
				AndroidJNISafe.CallStaticVoidMethod(this.m_jclass, methodID, array);
			}
			finally
			{
				AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		protected ReturnType _CallStatic<ReturnType>(string methodName, params object[] args)
		{
			if (args == null)
			{
				args = new object[1];
			}
			IntPtr methodID = AndroidJNIHelper.GetMethodID<ReturnType>(this.m_jclass, methodName, args, true);
			jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
			ReturnType returnType;
			try
			{
				if (AndroidReflection.IsPrimitive(typeof(ReturnType)))
				{
					if (typeof(ReturnType) == typeof(int))
					{
						returnType = (ReturnType)((object)AndroidJNISafe.CallStaticIntMethod(this.m_jclass, methodID, array));
					}
					else if (typeof(ReturnType) == typeof(bool))
					{
						returnType = (ReturnType)((object)AndroidJNISafe.CallStaticBooleanMethod(this.m_jclass, methodID, array));
					}
					else if (typeof(ReturnType) == typeof(byte))
					{
						returnType = (ReturnType)((object)AndroidJNISafe.CallStaticByteMethod(this.m_jclass, methodID, array));
					}
					else if (typeof(ReturnType) == typeof(short))
					{
						returnType = (ReturnType)((object)AndroidJNISafe.CallStaticShortMethod(this.m_jclass, methodID, array));
					}
					else if (typeof(ReturnType) == typeof(long))
					{
						returnType = (ReturnType)((object)AndroidJNISafe.CallStaticLongMethod(this.m_jclass, methodID, array));
					}
					else if (typeof(ReturnType) == typeof(float))
					{
						returnType = (ReturnType)((object)AndroidJNISafe.CallStaticFloatMethod(this.m_jclass, methodID, array));
					}
					else if (typeof(ReturnType) == typeof(double))
					{
						returnType = (ReturnType)((object)AndroidJNISafe.CallStaticDoubleMethod(this.m_jclass, methodID, array));
					}
					else if (typeof(ReturnType) == typeof(char))
					{
						returnType = (ReturnType)((object)AndroidJNISafe.CallStaticCharMethod(this.m_jclass, methodID, array));
					}
					else
					{
						returnType = default(ReturnType);
					}
				}
				else if (typeof(ReturnType) == typeof(string))
				{
					returnType = (ReturnType)((object)AndroidJNISafe.CallStaticStringMethod(this.m_jclass, methodID, array));
				}
				else if (typeof(ReturnType) == typeof(AndroidJavaClass))
				{
					IntPtr intPtr = AndroidJNISafe.CallStaticObjectMethod(this.m_jclass, methodID, array);
					returnType = ((!(intPtr == IntPtr.Zero)) ? ((ReturnType)((object)AndroidJavaObject.AndroidJavaClassDeleteLocalRef(intPtr))) : default(ReturnType));
				}
				else if (typeof(ReturnType) == typeof(AndroidJavaObject))
				{
					IntPtr intPtr2 = AndroidJNISafe.CallStaticObjectMethod(this.m_jclass, methodID, array);
					returnType = ((!(intPtr2 == IntPtr.Zero)) ? ((ReturnType)((object)AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(intPtr2))) : default(ReturnType));
				}
				else
				{
					if (!AndroidReflection.IsAssignableFrom(typeof(Array), typeof(ReturnType)))
					{
						throw new Exception("JNI: Unknown return type '" + typeof(ReturnType) + "'");
					}
					IntPtr intPtr3 = AndroidJNISafe.CallStaticObjectMethod(this.m_jclass, methodID, array);
					returnType = ((!(intPtr3 == IntPtr.Zero)) ? ((ReturnType)((object)AndroidJNIHelper.ConvertFromJNIArray<ReturnType>(intPtr3))) : default(ReturnType));
				}
			}
			finally
			{
				AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
			return returnType;
		}

		protected FieldType _GetStatic<FieldType>(string fieldName)
		{
			IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(this.m_jclass, fieldName, true);
			FieldType fieldType;
			if (AndroidReflection.IsPrimitive(typeof(FieldType)))
			{
				if (typeof(FieldType) == typeof(int))
				{
					fieldType = (FieldType)((object)AndroidJNISafe.GetStaticIntField(this.m_jclass, fieldID));
				}
				else if (typeof(FieldType) == typeof(bool))
				{
					fieldType = (FieldType)((object)AndroidJNISafe.GetStaticBooleanField(this.m_jclass, fieldID));
				}
				else if (typeof(FieldType) == typeof(byte))
				{
					fieldType = (FieldType)((object)AndroidJNISafe.GetStaticByteField(this.m_jclass, fieldID));
				}
				else if (typeof(FieldType) == typeof(short))
				{
					fieldType = (FieldType)((object)AndroidJNISafe.GetStaticShortField(this.m_jclass, fieldID));
				}
				else if (typeof(FieldType) == typeof(long))
				{
					fieldType = (FieldType)((object)AndroidJNISafe.GetStaticLongField(this.m_jclass, fieldID));
				}
				else if (typeof(FieldType) == typeof(float))
				{
					fieldType = (FieldType)((object)AndroidJNISafe.GetStaticFloatField(this.m_jclass, fieldID));
				}
				else if (typeof(FieldType) == typeof(double))
				{
					fieldType = (FieldType)((object)AndroidJNISafe.GetStaticDoubleField(this.m_jclass, fieldID));
				}
				else if (typeof(FieldType) == typeof(char))
				{
					fieldType = (FieldType)((object)AndroidJNISafe.GetStaticCharField(this.m_jclass, fieldID));
				}
				else
				{
					fieldType = default(FieldType);
				}
			}
			else if (typeof(FieldType) == typeof(string))
			{
				fieldType = (FieldType)((object)AndroidJNISafe.GetStaticStringField(this.m_jclass, fieldID));
			}
			else if (typeof(FieldType) == typeof(AndroidJavaClass))
			{
				IntPtr staticObjectField = AndroidJNISafe.GetStaticObjectField(this.m_jclass, fieldID);
				fieldType = ((!(staticObjectField == IntPtr.Zero)) ? ((FieldType)((object)AndroidJavaObject.AndroidJavaClassDeleteLocalRef(staticObjectField))) : default(FieldType));
			}
			else if (typeof(FieldType) == typeof(AndroidJavaObject))
			{
				IntPtr staticObjectField2 = AndroidJNISafe.GetStaticObjectField(this.m_jclass, fieldID);
				fieldType = ((!(staticObjectField2 == IntPtr.Zero)) ? ((FieldType)((object)AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(staticObjectField2))) : default(FieldType));
			}
			else
			{
				if (!AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType)))
				{
					throw new Exception("JNI: Unknown field type '" + typeof(FieldType) + "'");
				}
				IntPtr staticObjectField3 = AndroidJNISafe.GetStaticObjectField(this.m_jclass, fieldID);
				fieldType = ((!(staticObjectField3 == IntPtr.Zero)) ? ((FieldType)((object)AndroidJNIHelper.ConvertFromJNIArray<FieldType>(staticObjectField3))) : default(FieldType));
			}
			return fieldType;
		}

		protected void _SetStatic<FieldType>(string fieldName, FieldType val)
		{
			IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(this.m_jclass, fieldName, true);
			if (AndroidReflection.IsPrimitive(typeof(FieldType)))
			{
				if (typeof(FieldType) == typeof(int))
				{
					AndroidJNISafe.SetStaticIntField(this.m_jclass, fieldID, (int)((object)val));
				}
				else if (typeof(FieldType) == typeof(bool))
				{
					AndroidJNISafe.SetStaticBooleanField(this.m_jclass, fieldID, (bool)((object)val));
				}
				else if (typeof(FieldType) == typeof(byte))
				{
					AndroidJNISafe.SetStaticByteField(this.m_jclass, fieldID, (byte)((object)val));
				}
				else if (typeof(FieldType) == typeof(short))
				{
					AndroidJNISafe.SetStaticShortField(this.m_jclass, fieldID, (short)((object)val));
				}
				else if (typeof(FieldType) == typeof(long))
				{
					AndroidJNISafe.SetStaticLongField(this.m_jclass, fieldID, (long)((object)val));
				}
				else if (typeof(FieldType) == typeof(float))
				{
					AndroidJNISafe.SetStaticFloatField(this.m_jclass, fieldID, (float)((object)val));
				}
				else if (typeof(FieldType) == typeof(double))
				{
					AndroidJNISafe.SetStaticDoubleField(this.m_jclass, fieldID, (double)((object)val));
				}
				else if (typeof(FieldType) == typeof(char))
				{
					AndroidJNISafe.SetStaticCharField(this.m_jclass, fieldID, (char)((object)val));
				}
			}
			else if (typeof(FieldType) == typeof(string))
			{
				AndroidJNISafe.SetStaticStringField(this.m_jclass, fieldID, (string)((object)val));
			}
			else if (typeof(FieldType) == typeof(AndroidJavaClass))
			{
				AndroidJNISafe.SetStaticObjectField(this.m_jclass, fieldID, ((AndroidJavaClass)((object)val)).m_jclass);
			}
			else if (typeof(FieldType) == typeof(AndroidJavaObject))
			{
				AndroidJNISafe.SetStaticObjectField(this.m_jclass, fieldID, ((AndroidJavaObject)((object)val)).m_jobject);
			}
			else
			{
				if (!AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType)))
				{
					throw new Exception("JNI: Unknown field type '" + typeof(FieldType) + "'");
				}
				IntPtr intPtr = AndroidJNIHelper.ConvertToJNIArray((Array)((object)val));
				AndroidJNISafe.SetStaticObjectField(this.m_jclass, fieldID, intPtr);
			}
		}

		internal static AndroidJavaObject AndroidJavaObjectDeleteLocalRef(IntPtr jobject)
		{
			AndroidJavaObject androidJavaObject;
			try
			{
				androidJavaObject = new AndroidJavaObject(jobject);
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(jobject);
			}
			return androidJavaObject;
		}

		internal static AndroidJavaClass AndroidJavaClassDeleteLocalRef(IntPtr jclass)
		{
			AndroidJavaClass androidJavaClass;
			try
			{
				androidJavaClass = new AndroidJavaClass(jclass);
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(jclass);
			}
			return androidJavaClass;
		}

		protected IntPtr _GetRawObject()
		{
			return this.m_jobject;
		}

		protected IntPtr _GetRawClass()
		{
			return this.m_jclass;
		}

		protected static AndroidJavaObject FindClass(string name)
		{
			return AndroidJavaObject.JavaLangClass.CallStatic<AndroidJavaObject>("forName", new object[] { name.Replace('/', '.') });
		}

		protected static AndroidJavaClass JavaLangClass
		{
			get
			{
				if (AndroidJavaObject.s_JavaLangClass == null)
				{
					AndroidJavaObject.s_JavaLangClass = new AndroidJavaClass(AndroidJNISafe.FindClass("java/lang/Class"));
				}
				return AndroidJavaObject.s_JavaLangClass;
			}
		}

		private static bool enableDebugPrints = false;

		private bool m_disposed = false;

		protected IntPtr m_jobject;

		protected IntPtr m_jclass;

		private static AndroidJavaClass s_JavaLangClass;
	}
}
