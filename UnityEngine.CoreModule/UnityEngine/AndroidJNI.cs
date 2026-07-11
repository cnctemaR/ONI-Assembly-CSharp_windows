using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>'Raw' JNI interface to Android Dalvik (Java) VM from Mono (CS/JS).
	///
	/// Note: Using raw JNI functions requires advanced knowledge of the Android Java Native Interface (JNI). Please take note.</para>
	/// </summary>
	public class AndroidJNI
	{
		private AndroidJNI()
		{
		}

		/// <summary>
		///   <para>Attaches the current thread to a Java (Dalvik) VM.</para>
		/// </summary>
		public static int AttachCurrentThread()
		{
			return 0;
		}

		/// <summary>
		///   <para>Detaches the current thread from a Java (Dalvik) VM.</para>
		/// </summary>
		public static int DetachCurrentThread()
		{
			return 0;
		}

		/// <summary>
		///   <para>Returns the version of the native method interface.</para>
		/// </summary>
		public static int GetVersion()
		{
			return 0;
		}

		/// <summary>
		///   <para>This function loads a locally-defined class.</para>
		/// </summary>
		/// <param name="name"></param>
		public static IntPtr FindClass(string name)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Converts a &lt;tt&gt;java.lang.reflect.Method&lt;tt&gt; or &lt;tt&gt;java.lang.reflect.Constructor&lt;tt&gt; object to a method ID.</para>
		/// </summary>
		/// <param name="refMethod"></param>
		public static IntPtr FromReflectedMethod(IntPtr refMethod)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Converts a &lt;tt&gt;java.lang.reflect.Field&lt;/tt&gt; to a field ID.</para>
		/// </summary>
		/// <param name="refField"></param>
		public static IntPtr FromReflectedField(IntPtr refField)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Converts a method ID derived from clazz to a &lt;tt&gt;java.lang.reflect.Method&lt;tt&gt; or &lt;tt&gt;java.lang.reflect.Constructor&lt;tt&gt; object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="methodID"></param>
		/// <param name="isStatic"></param>
		public static IntPtr ToReflectedMethod(IntPtr clazz, IntPtr methodID, bool isStatic)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Converts a field ID derived from cls to a &lt;tt&gt;java.lang.reflect.Field&lt;/tt&gt; object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		/// <param name="isStatic"></param>
		public static IntPtr ToReflectedField(IntPtr clazz, IntPtr fieldID, bool isStatic)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>If &lt;tt&gt;clazz&lt;tt&gt; represents any class other than the class &lt;tt&gt;Object&lt;tt&gt;, then this function returns the object that represents the superclass of the class specified by &lt;tt&gt;clazz&lt;/tt&gt;.</para>
		/// </summary>
		/// <param name="clazz"></param>
		public static IntPtr GetSuperclass(IntPtr clazz)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Determines whether an object of &lt;tt&gt;clazz1&lt;tt&gt; can be safely cast to &lt;tt&gt;clazz2&lt;tt&gt;.</para>
		/// </summary>
		/// <param name="clazz1"></param>
		/// <param name="clazz2"></param>
		public static bool IsAssignableFrom(IntPtr clazz1, IntPtr clazz2)
		{
			return false;
		}

		/// <summary>
		///   <para>Causes a &lt;tt&gt;java.lang.Throwable&lt;/tt&gt; object to be thrown.</para>
		/// </summary>
		/// <param name="obj"></param>
		public static int Throw(IntPtr obj)
		{
			return 0;
		}

		/// <summary>
		///   <para>Constructs an exception object from the specified class with the &lt;tt&gt;message&lt;/tt&gt; specified by message and causes that exception to be thrown.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="message"></param>
		public static int ThrowNew(IntPtr clazz, string message)
		{
			return 0;
		}

		/// <summary>
		///   <para>Determines if an exception is being thrown.</para>
		/// </summary>
		public static IntPtr ExceptionOccurred()
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Prints an exception and a backtrace of the stack to the &lt;tt&gt;logcat&lt;/tt&gt;</para>
		/// </summary>
		public static void ExceptionDescribe()
		{
		}

		/// <summary>
		///   <para>Clears any exception that is currently being thrown.</para>
		/// </summary>
		public static void ExceptionClear()
		{
		}

		/// <summary>
		///   <para>Raises a fatal error and does not expect the VM to recover. This function does not return.</para>
		/// </summary>
		/// <param name="message"></param>
		public static void FatalError(string message)
		{
		}

		/// <summary>
		///   <para>Creates a new local reference frame, in which at least a given number of local references can be created.</para>
		/// </summary>
		/// <param name="capacity"></param>
		public static int PushLocalFrame(int capacity)
		{
			return 0;
		}

		/// <summary>
		///   <para>Pops off the current local reference frame, frees all the local references, and returns a local reference in the previous local reference frame for the given &lt;tt&gt;result&lt;/tt&gt; object.</para>
		/// </summary>
		/// <param name="ptr"></param>
		public static IntPtr PopLocalFrame(IntPtr ptr)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Creates a new global reference to the object referred to by the &lt;tt&gt;obj&lt;/tt&gt; argument.</para>
		/// </summary>
		/// <param name="obj"></param>
		public static IntPtr NewGlobalRef(IntPtr obj)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Deletes the global reference pointed to by &lt;tt&gt;obj&lt;/tt&gt;.</para>
		/// </summary>
		/// <param name="obj"></param>
		public static void DeleteGlobalRef(IntPtr obj)
		{
		}

		/// <summary>
		///   <para>Creates a new local reference that refers to the same object as &lt;tt&gt;obj&lt;/tt&gt;.</para>
		/// </summary>
		/// <param name="obj"></param>
		public static IntPtr NewLocalRef(IntPtr obj)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Deletes the local reference pointed to by &lt;tt&gt;obj&lt;/tt&gt;.</para>
		/// </summary>
		/// <param name="obj"></param>
		public static void DeleteLocalRef(IntPtr obj)
		{
		}

		/// <summary>
		///   <para>Tests whether two references refer to the same Java object.</para>
		/// </summary>
		/// <param name="obj1"></param>
		/// <param name="obj2"></param>
		public static bool IsSameObject(IntPtr obj1, IntPtr obj2)
		{
			return false;
		}

		/// <summary>
		///   <para>Ensures that at least a given number of local references can be created in the current thread.</para>
		/// </summary>
		/// <param name="capacity"></param>
		public static int EnsureLocalCapacity(int capacity)
		{
			return 0;
		}

		/// <summary>
		///   <para>Allocates a new Java object without invoking any of the constructors for the object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		public static IntPtr AllocObject(IntPtr clazz)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Constructs a new Java object. The method ID indicates which constructor method to invoke. This ID must be obtained by calling GetMethodID() with &lt;init&gt; as the method name and void (V) as the return type.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static IntPtr NewObject(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Returns the class of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		public static IntPtr GetObjectClass(IntPtr obj)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Tests whether an object is an instance of a class.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="clazz"></param>
		public static bool IsInstanceOf(IntPtr obj, IntPtr clazz)
		{
			return false;
		}

		/// <summary>
		///   <para>Returns the method ID for an instance (nonstatic) method of a class or interface.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="name"></param>
		/// <param name="sig"></param>
		public static IntPtr GetMethodID(IntPtr clazz, string name, string sig)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Returns the field ID for an instance (nonstatic) field of a class.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="name"></param>
		/// <param name="sig"></param>
		public static IntPtr GetFieldID(IntPtr clazz, string name, string sig)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Returns the method ID for a static method of a class.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="name"></param>
		/// <param name="sig"></param>
		public static IntPtr GetStaticMethodID(IntPtr clazz, string name, string sig)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Returns the field ID for a static field of a class.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="name"></param>
		/// <param name="sig"></param>
		public static IntPtr GetStaticFieldID(IntPtr clazz, string name, string sig)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Constructs a new &lt;tt&gt;java.lang.String&lt;/tt&gt; object from an array of characters in modified UTF-8 encoding.</para>
		/// </summary>
		/// <param name="bytes"></param>
		public static IntPtr NewStringUTF(string bytes)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Returns the length in bytes of the modified UTF-8 representation of a string.</para>
		/// </summary>
		/// <param name="str"></param>
		public static int GetStringUTFLength(IntPtr str)
		{
			return 0;
		}

		/// <summary>
		///   <para>Returns a managed string object representing the string in modified UTF-8 encoding.</para>
		/// </summary>
		/// <param name="str"></param>
		public static string GetStringUTFChars(IntPtr str)
		{
			return "";
		}

		/// <summary>
		///   <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static string CallStringMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return "";
		}

		/// <summary>
		///   <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static IntPtr CallObjectMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static int CallIntMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return 0;
		}

		/// <summary>
		///   <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static bool CallBooleanMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return false;
		}

		/// <summary>
		///   <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static short CallShortMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return 0;
		}

		/// <summary>
		///   <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static byte CallByteMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return 0;
		}

		/// <summary>
		///   <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static char CallCharMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return '0';
		}

		/// <summary>
		///   <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static float CallFloatMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return 0f;
		}

		/// <summary>
		///   <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static double CallDoubleMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return 0.0;
		}

		/// <summary>
		///   <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static long CallLongMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return 0L;
		}

		/// <summary>
		///   <para>Calls an instance (nonstatic) Java method defined by &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static void CallVoidMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
		}

		/// <summary>
		///   <para>This function returns the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		public static string GetStringField(IntPtr obj, IntPtr fieldID)
		{
			return "";
		}

		/// <summary>
		///   <para>This function returns the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		public static IntPtr GetObjectField(IntPtr obj, IntPtr fieldID)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>This function returns the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		public static bool GetBooleanField(IntPtr obj, IntPtr fieldID)
		{
			return false;
		}

		/// <summary>
		///   <para>This function returns the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		public static byte GetByteField(IntPtr obj, IntPtr fieldID)
		{
			return 0;
		}

		/// <summary>
		///   <para>This function returns the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		public static char GetCharField(IntPtr obj, IntPtr fieldID)
		{
			return '0';
		}

		/// <summary>
		///   <para>This function returns the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		public static short GetShortField(IntPtr obj, IntPtr fieldID)
		{
			return 0;
		}

		/// <summary>
		///   <para>This function returns the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		public static int GetIntField(IntPtr obj, IntPtr fieldID)
		{
			return 0;
		}

		/// <summary>
		///   <para>This function returns the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		public static long GetLongField(IntPtr obj, IntPtr fieldID)
		{
			return 0L;
		}

		/// <summary>
		///   <para>This function returns the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		public static float GetFloatField(IntPtr obj, IntPtr fieldID)
		{
			return 0f;
		}

		/// <summary>
		///   <para>This function returns the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		public static double GetDoubleField(IntPtr obj, IntPtr fieldID)
		{
			return 0.0;
		}

		/// <summary>
		///   <para>This function sets the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetStringField(IntPtr obj, IntPtr fieldID, string val)
		{
		}

		/// <summary>
		///   <para>This function sets the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetObjectField(IntPtr obj, IntPtr fieldID, IntPtr val)
		{
		}

		/// <summary>
		///   <para>This function sets the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetBooleanField(IntPtr obj, IntPtr fieldID, bool val)
		{
		}

		/// <summary>
		///   <para>This function sets the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetByteField(IntPtr obj, IntPtr fieldID, byte val)
		{
		}

		/// <summary>
		///   <para>This function sets the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetCharField(IntPtr obj, IntPtr fieldID, char val)
		{
		}

		/// <summary>
		///   <para>This function sets the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetShortField(IntPtr obj, IntPtr fieldID, short val)
		{
		}

		/// <summary>
		///   <para>This function sets the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetIntField(IntPtr obj, IntPtr fieldID, int val)
		{
		}

		/// <summary>
		///   <para>This function sets the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetLongField(IntPtr obj, IntPtr fieldID, long val)
		{
		}

		/// <summary>
		///   <para>This function sets the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetFloatField(IntPtr obj, IntPtr fieldID, float val)
		{
		}

		/// <summary>
		///   <para>This function sets the value of an instance (nonstatic) field of an object.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetDoubleField(IntPtr obj, IntPtr fieldID, double val)
		{
		}

		/// <summary>
		///   <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static string CallStaticStringMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return "";
		}

		/// <summary>
		///   <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static IntPtr CallStaticObjectMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static int CallStaticIntMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return 0;
		}

		/// <summary>
		///   <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static bool CallStaticBooleanMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return false;
		}

		/// <summary>
		///   <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static short CallStaticShortMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return 0;
		}

		/// <summary>
		///   <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static byte CallStaticByteMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return 0;
		}

		/// <summary>
		///   <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static char CallStaticCharMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return '0';
		}

		/// <summary>
		///   <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static float CallStaticFloatMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return 0f;
		}

		/// <summary>
		///   <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static double CallStaticDoubleMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return 0.0;
		}

		/// <summary>
		///   <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static long CallStaticLongMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return 0L;
		}

		/// <summary>
		///   <para>Invokes a static method on a Java object, according to the specified &lt;tt&gt;methodID&lt;tt&gt;, optionally passing an array of arguments (&lt;tt&gt;args&lt;tt&gt;) to the method.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="methodID"></param>
		/// <param name="args"></param>
		public static void CallStaticVoidMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
		}

		/// <summary>
		///   <para>This function returns the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		public static string GetStaticStringField(IntPtr clazz, IntPtr fieldID)
		{
			return "";
		}

		/// <summary>
		///   <para>This function returns the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		public static IntPtr GetStaticObjectField(IntPtr clazz, IntPtr fieldID)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>This function returns the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		public static bool GetStaticBooleanField(IntPtr clazz, IntPtr fieldID)
		{
			return false;
		}

		/// <summary>
		///   <para>This function returns the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		public static byte GetStaticByteField(IntPtr clazz, IntPtr fieldID)
		{
			return 0;
		}

		/// <summary>
		///   <para>This function returns the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		public static char GetStaticCharField(IntPtr clazz, IntPtr fieldID)
		{
			return '0';
		}

		/// <summary>
		///   <para>This function returns the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		public static short GetStaticShortField(IntPtr clazz, IntPtr fieldID)
		{
			return 0;
		}

		/// <summary>
		///   <para>This function returns the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		public static int GetStaticIntField(IntPtr clazz, IntPtr fieldID)
		{
			return 0;
		}

		/// <summary>
		///   <para>This function returns the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		public static long GetStaticLongField(IntPtr clazz, IntPtr fieldID)
		{
			return 0L;
		}

		/// <summary>
		///   <para>This function returns the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		public static float GetStaticFloatField(IntPtr clazz, IntPtr fieldID)
		{
			return 0f;
		}

		/// <summary>
		///   <para>This function returns the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		public static double GetStaticDoubleField(IntPtr clazz, IntPtr fieldID)
		{
			return 0.0;
		}

		/// <summary>
		///   <para>This function ets the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetStaticStringField(IntPtr clazz, IntPtr fieldID, string val)
		{
		}

		/// <summary>
		///   <para>This function ets the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetStaticObjectField(IntPtr clazz, IntPtr fieldID, IntPtr val)
		{
		}

		/// <summary>
		///   <para>This function ets the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetStaticBooleanField(IntPtr clazz, IntPtr fieldID, bool val)
		{
		}

		/// <summary>
		///   <para>This function ets the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetStaticByteField(IntPtr clazz, IntPtr fieldID, byte val)
		{
		}

		/// <summary>
		///   <para>This function ets the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetStaticCharField(IntPtr clazz, IntPtr fieldID, char val)
		{
		}

		/// <summary>
		///   <para>This function ets the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetStaticShortField(IntPtr clazz, IntPtr fieldID, short val)
		{
		}

		/// <summary>
		///   <para>This function ets the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetStaticIntField(IntPtr clazz, IntPtr fieldID, int val)
		{
		}

		/// <summary>
		///   <para>This function ets the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetStaticLongField(IntPtr clazz, IntPtr fieldID, long val)
		{
		}

		/// <summary>
		///   <para>This function ets the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetStaticFloatField(IntPtr clazz, IntPtr fieldID, float val)
		{
		}

		/// <summary>
		///   <para>This function ets the value of a static field of an object.</para>
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="fieldID"></param>
		/// <param name="val"></param>
		public static void SetStaticDoubleField(IntPtr clazz, IntPtr fieldID, double val)
		{
		}

		/// <summary>
		///   <para>Convert a managed array of System.Boolean to a Java array of &lt;tt&gt;boolean&lt;/tt&gt;.</para>
		/// </summary>
		/// <param name="array"></param>
		public static IntPtr ToBooleanArray(bool[] array)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Convert a managed array of System.Byte to a Java array of &lt;tt&gt;byte&lt;/tt&gt;.</para>
		/// </summary>
		/// <param name="array"></param>
		public static IntPtr ToByteArray(byte[] array)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Convert a managed array of System.Char to a Java array of &lt;tt&gt;char&lt;/tt&gt;.</para>
		/// </summary>
		/// <param name="array"></param>
		public static IntPtr ToCharArray(char[] array)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Convert a managed array of System.Int16 to a Java array of &lt;tt&gt;short&lt;/tt&gt;.</para>
		/// </summary>
		/// <param name="array"></param>
		public static IntPtr ToShortArray(short[] array)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Convert a managed array of System.Int32 to a Java array of &lt;tt&gt;int&lt;/tt&gt;.</para>
		/// </summary>
		/// <param name="array"></param>
		public static IntPtr ToIntArray(int[] array)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Convert a managed array of System.Int64 to a Java array of &lt;tt&gt;long&lt;/tt&gt;.</para>
		/// </summary>
		/// <param name="array"></param>
		public static IntPtr ToLongArray(long[] array)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Convert a managed array of System.Single to a Java array of &lt;tt&gt;float&lt;/tt&gt;.</para>
		/// </summary>
		/// <param name="array"></param>
		public static IntPtr ToFloatArray(float[] array)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Convert a managed array of System.Double to a Java array of &lt;tt&gt;double&lt;/tt&gt;.</para>
		/// </summary>
		/// <param name="array"></param>
		public static IntPtr ToDoubleArray(double[] array)
		{
			return IntPtr.Zero;
		}

		public static IntPtr ToObjectArray(IntPtr[] array, IntPtr arrayClass)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Convert a managed array of System.IntPtr, representing Java objects, to a Java array of &lt;tt&gt;java.lang.Object&lt;/tt&gt;.</para>
		/// </summary>
		/// <param name="array"></param>
		public static IntPtr ToObjectArray(IntPtr[] array)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Convert a Java array of &lt;tt&gt;boolean&lt;/tt&gt; to a managed array of System.Boolean.</para>
		/// </summary>
		/// <param name="array"></param>
		public static bool[] FromBooleanArray(IntPtr array)
		{
			return null;
		}

		/// <summary>
		///   <para>Convert a Java array of &lt;tt&gt;byte&lt;/tt&gt; to a managed array of System.Byte.</para>
		/// </summary>
		/// <param name="array"></param>
		public static byte[] FromByteArray(IntPtr array)
		{
			return null;
		}

		/// <summary>
		///   <para>Convert a Java array of &lt;tt&gt;char&lt;/tt&gt; to a managed array of System.Char.</para>
		/// </summary>
		/// <param name="array"></param>
		public static char[] FromCharArray(IntPtr array)
		{
			return null;
		}

		/// <summary>
		///   <para>Convert a Java array of &lt;tt&gt;short&lt;/tt&gt; to a managed array of System.Int16.</para>
		/// </summary>
		/// <param name="array"></param>
		public static short[] FromShortArray(IntPtr array)
		{
			return null;
		}

		/// <summary>
		///   <para>Convert a Java array of &lt;tt&gt;int&lt;/tt&gt; to a managed array of System.Int32.</para>
		/// </summary>
		/// <param name="array"></param>
		public static int[] FromIntArray(IntPtr array)
		{
			return null;
		}

		/// <summary>
		///   <para>Convert a Java array of &lt;tt&gt;long&lt;/tt&gt; to a managed array of System.Int64.</para>
		/// </summary>
		/// <param name="array"></param>
		public static long[] FromLongArray(IntPtr array)
		{
			return null;
		}

		/// <summary>
		///   <para>Convert a Java array of &lt;tt&gt;float&lt;/tt&gt; to a managed array of System.Single.</para>
		/// </summary>
		/// <param name="array"></param>
		public static float[] FromFloatArray(IntPtr array)
		{
			return null;
		}

		/// <summary>
		///   <para>Convert a Java array of &lt;tt&gt;double&lt;/tt&gt; to a managed array of System.Double.</para>
		/// </summary>
		/// <param name="array"></param>
		public static double[] FromDoubleArray(IntPtr array)
		{
			return null;
		}

		/// <summary>
		///   <para>Convert a Java array of &lt;tt&gt;java.lang.Object&lt;/tt&gt; to a managed array of System.IntPtr, representing Java objects.</para>
		/// </summary>
		/// <param name="array"></param>
		public static IntPtr[] FromObjectArray(IntPtr array)
		{
			return null;
		}

		/// <summary>
		///   <para>Returns the number of elements in the array.</para>
		/// </summary>
		/// <param name="array"></param>
		public static int GetArrayLength(IntPtr array)
		{
			return 0;
		}

		/// <summary>
		///   <para>Construct a new primitive array object.</para>
		/// </summary>
		/// <param name="size"></param>
		public static IntPtr NewBooleanArray(int size)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Construct a new primitive array object.</para>
		/// </summary>
		/// <param name="size"></param>
		public static IntPtr NewByteArray(int size)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Construct a new primitive array object.</para>
		/// </summary>
		/// <param name="size"></param>
		public static IntPtr NewCharArray(int size)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Construct a new primitive array object.</para>
		/// </summary>
		/// <param name="size"></param>
		public static IntPtr NewShortArray(int size)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Construct a new primitive array object.</para>
		/// </summary>
		/// <param name="size"></param>
		public static IntPtr NewIntArray(int size)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Construct a new primitive array object.</para>
		/// </summary>
		/// <param name="size"></param>
		public static IntPtr NewLongArray(int size)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Construct a new primitive array object.</para>
		/// </summary>
		/// <param name="size"></param>
		public static IntPtr NewFloatArray(int size)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Construct a new primitive array object.</para>
		/// </summary>
		/// <param name="size"></param>
		public static IntPtr NewDoubleArray(int size)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Constructs a new array holding objects in class &lt;tt&gt;clazz&lt;tt&gt;. All elements are initially set to &lt;tt&gt;obj&lt;tt&gt;.</para>
		/// </summary>
		/// <param name="size"></param>
		/// <param name="clazz"></param>
		/// <param name="obj"></param>
		public static IntPtr NewObjectArray(int size, IntPtr clazz, IntPtr obj)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Returns the value of one element of a primitive array.</para>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public static bool GetBooleanArrayElement(IntPtr array, int index)
		{
			return false;
		}

		/// <summary>
		///   <para>Returns the value of one element of a primitive array.</para>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public static byte GetByteArrayElement(IntPtr array, int index)
		{
			return 0;
		}

		/// <summary>
		///   <para>Returns the value of one element of a primitive array.</para>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public static char GetCharArrayElement(IntPtr array, int index)
		{
			return '0';
		}

		/// <summary>
		///   <para>Returns the value of one element of a primitive array.</para>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public static short GetShortArrayElement(IntPtr array, int index)
		{
			return 0;
		}

		/// <summary>
		///   <para>Returns the value of one element of a primitive array.</para>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public static int GetIntArrayElement(IntPtr array, int index)
		{
			return 0;
		}

		/// <summary>
		///   <para>Returns the value of one element of a primitive array.</para>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public static long GetLongArrayElement(IntPtr array, int index)
		{
			return 0L;
		}

		/// <summary>
		///   <para>Returns the value of one element of a primitive array.</para>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public static float GetFloatArrayElement(IntPtr array, int index)
		{
			return 0f;
		}

		/// <summary>
		///   <para>Returns the value of one element of a primitive array.</para>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public static double GetDoubleArrayElement(IntPtr array, int index)
		{
			return 0.0;
		}

		/// <summary>
		///   <para>Returns an element of an &lt;tt&gt;Object&lt;/tt&gt; array.</para>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public static IntPtr GetObjectArrayElement(IntPtr array, int index)
		{
			return IntPtr.Zero;
		}

		/// <summary>
		///   <para>Sets the value of one element in a primitive array.</para>
		/// </summary>
		/// <param name="array">The array of native booleans.</param>
		/// <param name="index">Index of the array element to set.</param>
		/// <param name="val">The value to set - for 'true' use 1, for 'false' use 0.</param>
		public static void SetBooleanArrayElement(IntPtr array, int index, byte val)
		{
		}

		/// <summary>
		///   <para>Sets the value of one element in a primitive array.</para>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		/// <param name="val"></param>
		public static void SetByteArrayElement(IntPtr array, int index, sbyte val)
		{
		}

		/// <summary>
		///   <para>Sets the value of one element in a primitive array.</para>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		/// <param name="val"></param>
		public static void SetCharArrayElement(IntPtr array, int index, char val)
		{
		}

		/// <summary>
		///   <para>Sets the value of one element in a primitive array.</para>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		/// <param name="val"></param>
		public static void SetShortArrayElement(IntPtr array, int index, short val)
		{
		}

		/// <summary>
		///   <para>Sets the value of one element in a primitive array.</para>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		/// <param name="val"></param>
		public static void SetIntArrayElement(IntPtr array, int index, int val)
		{
		}

		/// <summary>
		///   <para>Sets the value of one element in a primitive array.</para>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		/// <param name="val"></param>
		public static void SetLongArrayElement(IntPtr array, int index, long val)
		{
		}

		/// <summary>
		///   <para>Sets the value of one element in a primitive array.</para>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		/// <param name="val"></param>
		public static void SetFloatArrayElement(IntPtr array, int index, float val)
		{
		}

		/// <summary>
		///   <para>Sets the value of one element in a primitive array.</para>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		/// <param name="val"></param>
		public static void SetDoubleArrayElement(IntPtr array, int index, double val)
		{
		}

		/// <summary>
		///   <para>Sets an element of an &lt;tt&gt;Object&lt;/tt&gt; array.</para>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		/// <param name="obj"></param>
		public static void SetObjectArrayElement(IntPtr array, int index, IntPtr obj)
		{
		}
	}
}
