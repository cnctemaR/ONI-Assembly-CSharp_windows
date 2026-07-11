using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>AndroidJavaObject is the Unity representation of a generic instance of java.lang.Object.</para>
	/// </summary>
	public class AndroidJavaObject : IDisposable
	{
		internal AndroidJavaObject()
		{
		}

		/// <summary>
		///   <para>Construct an AndroidJavaObject based on the name of the class.</para>
		/// </summary>
		/// <param name="className">Specifies the Java class name (e.g. "&lt;tt&gt;java.lang.String&lt;tt&gt;" or "&lt;tt&gt;javalangString&lt;tt&gt;").</param>
		/// <param name="args">An array of parameters passed to the constructor.</param>
		public AndroidJavaObject(string className, params object[] args)
		{
		}

		/// <summary>
		///   <para>IDisposable callback.</para>
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		///   <para>Call a Java method on an object.</para>
		/// </summary>
		/// <param name="methodName">Specifies which method to call.</param>
		/// <param name="args">An array of parameters passed to the method.</param>
		public void Call(string methodName, params object[] args)
		{
		}

		/// <summary>
		///   <para>Call a static Java method on a class.</para>
		/// </summary>
		/// <param name="methodName">Specifies which method to call.</param>
		/// <param name="args">An array of parameters passed to the method.</param>
		public void CallStatic(string methodName, params object[] args)
		{
		}

		public FieldType Get<FieldType>(string fieldName)
		{
			return default(FieldType);
		}

		public void Set<FieldType>(string fieldName, FieldType val)
		{
		}

		public FieldType GetStatic<FieldType>(string fieldName)
		{
			return default(FieldType);
		}

		public void SetStatic<FieldType>(string fieldName, FieldType val)
		{
		}

		/// <summary>
		///   <para>Retrieves the raw &lt;tt&gt;jobject&lt;/tt&gt; pointer to the Java object.
		///
		/// Note: Using raw JNI functions requires advanced knowledge of the Android Java Native Interface (JNI). Please take note.</para>
		/// </summary>
		public IntPtr GetRawObject()
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Retrieves the raw &lt;tt&gt;jclass&lt;/tt&gt; pointer to the Java class.
		///
		/// Note: Using raw JNI functions requires advanced knowledge of the Android Java Native Interface (JNI). Please take note.</para>
		/// </summary>
		public IntPtr GetRawClass()
		{
			return IntPtr.Zero;
		}

		public ReturnType Call<ReturnType>(string methodName, params object[] args)
		{
			return default(ReturnType);
		}

		public ReturnType CallStatic<ReturnType>(string methodName, params object[] args)
		{
			return default(ReturnType);
		}

		protected void DebugPrint(string msg)
		{
		}

		protected void DebugPrint(string call, string methodName, string signature, object[] args)
		{
		}

		~AndroidJavaObject()
		{
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		protected void _Dispose()
		{
		}

		protected void _Call(string methodName, params object[] args)
		{
		}

		protected ReturnType _Call<ReturnType>(string methodName, params object[] args)
		{
			return default(ReturnType);
		}

		protected FieldType _Get<FieldType>(string fieldName)
		{
			return default(FieldType);
		}

		protected void _Set<FieldType>(string fieldName, FieldType val)
		{
		}

		protected void _CallStatic(string methodName, params object[] args)
		{
		}

		protected ReturnType _CallStatic<ReturnType>(string methodName, params object[] args)
		{
			return default(ReturnType);
		}

		protected FieldType _GetStatic<FieldType>(string fieldName)
		{
			return default(FieldType);
		}

		protected void _SetStatic<FieldType>(string fieldName, FieldType val)
		{
		}

		protected IntPtr _GetRawObject()
		{
			return IntPtr.Zero;
		}

		protected IntPtr _GetRawClass()
		{
			return IntPtr.Zero;
		}

		protected static AndroidJavaObject FindClass(string name)
		{
			return null;
		}

		protected static AndroidJavaClass JavaLangClass
		{
			get
			{
				return null;
			}
		}
	}
}
