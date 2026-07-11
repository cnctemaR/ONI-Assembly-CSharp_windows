using System;
using UnityEngine.Internal;

namespace UnityEngine
{
	/// <summary>
	///   <para>Helper interface for JNI interaction; signature creation and method lookups.
	///
	/// Note: Using raw JNI functions requires advanced knowledge of the Android Java Native Interface (JNI). Please take note.</para>
	/// </summary>
	public class AndroidJNIHelper
	{
		private AndroidJNIHelper()
		{
		}

		/// <summary>
		///   <para>Set debug to true to log calls through the AndroidJNIHelper.</para>
		/// </summary>
		public static bool debug
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		/// <summary>
		///   <para>Scans a particular Java class for a constructor method matching a signature.</para>
		/// </summary>
		/// <param name="javaClass">Raw JNI Java class object (obtained by calling AndroidJNI.FindClass).</param>
		/// <param name="signature">Constructor method signature (e.g. obtained by calling AndroidJNIHelper.GetSignature).</param>
		public static IntPtr GetConstructorID(IntPtr javaClass)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Scans a particular Java class for a constructor method matching a signature.</para>
		/// </summary>
		/// <param name="javaClass">Raw JNI Java class object (obtained by calling AndroidJNI.FindClass).</param>
		/// <param name="signature">Constructor method signature (e.g. obtained by calling AndroidJNIHelper.GetSignature).</param>
		public static IntPtr GetConstructorID(IntPtr javaClass, [DefaultValue("")] string signature)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Scans a particular Java class for a method matching a name and a signature.</para>
		/// </summary>
		/// <param name="javaClass">Raw JNI Java class object (obtained by calling AndroidJNI.FindClass).</param>
		/// <param name="methodName">Name of the method as declared in Java.</param>
		/// <param name="signature">Method signature (e.g. obtained by calling AndroidJNIHelper.GetSignature).</param>
		/// <param name="isStatic">Set to &lt;tt&gt;true&lt;tt&gt; for static methods; &lt;tt&gt;false&lt;tt&gt; for instance (nonstatic) methods.</param>
		public static IntPtr GetMethodID(IntPtr javaClass, string methodName)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Scans a particular Java class for a method matching a name and a signature.</para>
		/// </summary>
		/// <param name="javaClass">Raw JNI Java class object (obtained by calling AndroidJNI.FindClass).</param>
		/// <param name="methodName">Name of the method as declared in Java.</param>
		/// <param name="signature">Method signature (e.g. obtained by calling AndroidJNIHelper.GetSignature).</param>
		/// <param name="isStatic">Set to &lt;tt&gt;true&lt;tt&gt; for static methods; &lt;tt&gt;false&lt;tt&gt; for instance (nonstatic) methods.</param>
		public static IntPtr GetMethodID(IntPtr javaClass, string methodName, [DefaultValue("")] string signature)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Scans a particular Java class for a method matching a name and a signature.</para>
		/// </summary>
		/// <param name="javaClass">Raw JNI Java class object (obtained by calling AndroidJNI.FindClass).</param>
		/// <param name="methodName">Name of the method as declared in Java.</param>
		/// <param name="signature">Method signature (e.g. obtained by calling AndroidJNIHelper.GetSignature).</param>
		/// <param name="isStatic">Set to &lt;tt&gt;true&lt;tt&gt; for static methods; &lt;tt&gt;false&lt;tt&gt; for instance (nonstatic) methods.</param>
		public static IntPtr GetMethodID(IntPtr javaClass, string methodName, [DefaultValue("")] string signature, [DefaultValue("false")] bool isStatic)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Scans a particular Java class for a field matching a name and a signature.</para>
		/// </summary>
		/// <param name="javaClass">Raw JNI Java class object (obtained by calling AndroidJNI.FindClass).</param>
		/// <param name="fieldName">Name of the field as declared in Java.</param>
		/// <param name="signature">Field signature (e.g. obtained by calling AndroidJNIHelper.GetSignature).</param>
		/// <param name="isStatic">Set to &lt;tt&gt;true&lt;tt&gt; for static fields; &lt;tt&gt;false&lt;tt&gt; for instance (nonstatic) fields.</param>
		public static IntPtr GetFieldID(IntPtr javaClass, string fieldName)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Scans a particular Java class for a field matching a name and a signature.</para>
		/// </summary>
		/// <param name="javaClass">Raw JNI Java class object (obtained by calling AndroidJNI.FindClass).</param>
		/// <param name="fieldName">Name of the field as declared in Java.</param>
		/// <param name="signature">Field signature (e.g. obtained by calling AndroidJNIHelper.GetSignature).</param>
		/// <param name="isStatic">Set to &lt;tt&gt;true&lt;tt&gt; for static fields; &lt;tt&gt;false&lt;tt&gt; for instance (nonstatic) fields.</param>
		public static IntPtr GetFieldID(IntPtr javaClass, string fieldName, [DefaultValue("")] string signature)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Scans a particular Java class for a field matching a name and a signature.</para>
		/// </summary>
		/// <param name="javaClass">Raw JNI Java class object (obtained by calling AndroidJNI.FindClass).</param>
		/// <param name="fieldName">Name of the field as declared in Java.</param>
		/// <param name="signature">Field signature (e.g. obtained by calling AndroidJNIHelper.GetSignature).</param>
		/// <param name="isStatic">Set to &lt;tt&gt;true&lt;tt&gt; for static fields; &lt;tt&gt;false&lt;tt&gt; for instance (nonstatic) fields.</param>
		public static IntPtr GetFieldID(IntPtr javaClass, string fieldName, [DefaultValue("")] string signature, [DefaultValue("false")] bool isStatic)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Creates a UnityJavaRunnable object (implements java.lang.Runnable).</para>
		/// </summary>
		/// <param name="runnable">A delegate representing the java.lang.Runnable.</param>
		/// <param name="jrunnable"></param>
		public static IntPtr CreateJavaRunnable(AndroidJavaRunnable jrunnable)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Creates a java proxy object which connects to the supplied proxy implementation.</para>
		/// </summary>
		/// <param name="proxy">An implementatinon of a java interface in c#.</param>
		public static IntPtr CreateJavaProxy(AndroidJavaProxy proxy)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Creates a Java array from a managed array.</para>
		/// </summary>
		/// <param name="array">Managed array to be converted into a Java array object.</param>
		public static IntPtr ConvertToJNIArray(Array array)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Creates the parameter array to be used as argument list when invoking Java code through CallMethod() in AndroidJNI.</para>
		/// </summary>
		/// <param name="args">An array of objects that should be converted to Call parameters.</param>
		public static jvalue[] CreateJNIArgArray(object[] args)
		{
			return null;
		}

		/// <summary>
		///   <para>Deletes any local jni references previously allocated by CreateJNIArgArray().</para>
		/// </summary>
		/// <param name="args">The array of arguments used as a parameter to CreateJNIArgArray().</param>
		/// <param name="jniArgs">The array returned by CreateJNIArgArray().</param>
		public static void DeleteJNIArgArray(object[] args, jvalue[] jniArgs)
		{
		}

		/// <summary>
		///   <para>Get a JNI method ID for a constructor based on calling arguments.</para>
		/// </summary>
		/// <param name="javaClass">Raw JNI Java class object (obtained by calling AndroidJNI.FindClass).</param>
		/// <param name="args">Array with parameters to be passed to the constructor when invoked.</param>
		/// <param name="jclass"></param>
		public static IntPtr GetConstructorID(IntPtr jclass, object[] args)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Get a JNI method ID based on calling arguments.</para>
		/// </summary>
		/// <param name="javaClass">Raw JNI Java class object (obtained by calling AndroidJNI.FindClass).</param>
		/// <param name="methodName">Name of the method as declared in Java.</param>
		/// <param name="args">Array with parameters to be passed to the method when invoked.</param>
		/// <param name="isStatic">Set to &lt;tt&gt;true&lt;tt&gt; for static methods; &lt;tt&gt;false&lt;tt&gt; for instance (nonstatic) methods.</param>
		/// <param name="jclass"></param>
		public static IntPtr GetMethodID(IntPtr jclass, string methodName, object[] args, bool isStatic)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Creates the JNI signature string for particular object type.</para>
		/// </summary>
		/// <param name="obj">Object for which a signature is to be produced.</param>
		public static string GetSignature(object obj)
		{
			return "";
		}

		/// <summary>
		///   <para>Creates the JNI signature string for an object parameter list.</para>
		/// </summary>
		/// <param name="args">Array of object for which a signature is to be produced.</param>
		public static string GetSignature(object[] args)
		{
			return "";
		}

		public static ArrayType ConvertFromJNIArray<ArrayType>(IntPtr array)
		{
			return default(ArrayType);
		}

		public static IntPtr GetMethodID<ReturnType>(IntPtr jclass, string methodName, object[] args, bool isStatic)
		{
			return IntPtr.Zero;
		}

		public static IntPtr GetFieldID<FieldType>(IntPtr jclass, string fieldName, bool isStatic)
		{
			return IntPtr.Zero;
		}

		public static string GetSignature<ReturnType>(object[] args)
		{
			return "";
		}
	}
}
