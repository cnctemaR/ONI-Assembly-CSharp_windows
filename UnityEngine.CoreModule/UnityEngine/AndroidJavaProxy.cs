using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>This class can be used to implement any java interface. Any java vm method invocation matching the interface on the proxy object will automatically be passed to the c# implementation.</para>
	/// </summary>
	public class AndroidJavaProxy
	{
		/// <summary>
		///   <para></para>
		/// </summary>
		/// <param name="javaInterface">Java interface to be implemented by the proxy.</param>
		public AndroidJavaProxy(string javaInterface)
		{
		}

		/// <summary>
		///   <para></para>
		/// </summary>
		/// <param name="javaInterface">Java interface to be implemented by the proxy.</param>
		public AndroidJavaProxy(AndroidJavaClass javaInterface)
		{
		}

		/// <summary>
		///   <para>Called by the java vm whenever a method is invoked on the java proxy interface. You can override this to run special code on method invokation, or you can leave the implementation as is, and leave the default behavior which is to look for c# methods matching the signature of the java method.</para>
		/// </summary>
		/// <param name="methodName">Name of the invoked java method.</param>
		/// <param name="args">Arguments passed from the java vm - converted into AndroidJavaObject, AndroidJavaClass or a primitive.</param>
		/// <param name="javaArgs">Arguments passed from the java vm - all objects are represented by AndroidJavaObject, int for instance is represented by a java.lang.Integer object.</param>
		public virtual AndroidJavaObject Invoke(string methodName, object[] args)
		{
			return null;
		}

		/// <summary>
		///   <para>Called by the java vm whenever a method is invoked on the java proxy interface. You can override this to run special code on method invokation, or you can leave the implementation as is, and leave the default behavior which is to look for c# methods matching the signature of the java method.</para>
		/// </summary>
		/// <param name="methodName">Name of the invoked java method.</param>
		/// <param name="args">Arguments passed from the java vm - converted into AndroidJavaObject, AndroidJavaClass or a primitive.</param>
		/// <param name="javaArgs">Arguments passed from the java vm - all objects are represented by AndroidJavaObject, int for instance is represented by a java.lang.Integer object.</param>
		public virtual AndroidJavaObject Invoke(string methodName, AndroidJavaObject[] javaArgs)
		{
			return null;
		}

		/// <summary>
		///   <para>The equivalent of the java.lang.Object equals() method.</para>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>
		///   <para>Returns true when the objects are equal and false if otherwise.</para>
		/// </returns>
		public virtual bool equals(AndroidJavaObject obj)
		{
			return false;
		}

		/// <summary>
		///   <para>The equivalent of the java.lang.Object hashCode() method.</para>
		/// </summary>
		/// <returns>
		///   <para>Returns the hash code of the java proxy object.</para>
		/// </returns>
		public virtual int hashCode()
		{
			return 0;
		}

		/// <summary>
		///   <para>The equivalent of the java.lang.Object toString() method.</para>
		/// </summary>
		/// <returns>
		///   <para>Returns C# class name + " &lt;c# proxy java object&gt;".</para>
		/// </returns>
		public virtual string toString()
		{
			return "<c# proxy java object>";
		}

		/// <summary>
		///   <para>Java interface implemented by the proxy.</para>
		/// </summary>
		public readonly AndroidJavaClass javaInterface;
	}
}
