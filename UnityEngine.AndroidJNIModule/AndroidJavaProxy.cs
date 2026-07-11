using System;
using System.Reflection;

namespace UnityEngine
{
	public class AndroidJavaProxy
	{
		public AndroidJavaProxy(string javaInterface)
			: this(new AndroidJavaClass(javaInterface))
		{
		}

		public AndroidJavaProxy(AndroidJavaClass javaInterface)
		{
			this.javaInterface = javaInterface;
		}

		~AndroidJavaProxy()
		{
			AndroidJNISafe.DeleteWeakGlobalRef(this.proxyObject);
		}

		public virtual AndroidJavaObject Invoke(string methodName, object[] args)
		{
			Exception ex = null;
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			Type[] array = new Type[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				array[i] = ((args[i] == null) ? typeof(AndroidJavaObject) : args[i].GetType());
			}
			try
			{
				MethodInfo method = base.GetType().GetMethod(methodName, bindingFlags, null, array, null);
				bool flag = method != null;
				if (flag)
				{
					return _AndroidJNIHelper.Box(method.Invoke(this, args));
				}
			}
			catch (TargetInvocationException ex2)
			{
				ex = ex2.InnerException;
			}
			catch (Exception ex3)
			{
				ex = ex3;
			}
			string[] array2 = new string[args.Length];
			for (int j = 0; j < array.Length; j++)
			{
				array2[j] = array[j].ToString();
			}
			bool flag2 = ex != null;
			if (flag2)
			{
				throw new TargetInvocationException(string.Concat(new object[]
				{
					base.GetType(),
					".",
					methodName,
					"(",
					string.Join(",", array2),
					")"
				}), ex);
			}
			AndroidReflection.SetNativeExceptionOnProxy(this.GetRawProxy(), new Exception(string.Concat(new object[]
			{
				"No such proxy method: ",
				base.GetType(),
				".",
				methodName,
				"(",
				string.Join(",", array2),
				")"
			})), true);
			return null;
		}

		public virtual AndroidJavaObject Invoke(string methodName, AndroidJavaObject[] javaArgs)
		{
			object[] array = new object[javaArgs.Length];
			for (int i = 0; i < javaArgs.Length; i++)
			{
				array[i] = _AndroidJNIHelper.Unbox(javaArgs[i]);
				bool flag = !(array[i] is AndroidJavaObject);
				if (flag)
				{
					bool flag2 = javaArgs[i] != null;
					if (flag2)
					{
						javaArgs[i].Dispose();
					}
				}
			}
			return this.Invoke(methodName, array);
		}

		public virtual bool equals(AndroidJavaObject obj)
		{
			IntPtr intPtr = ((obj == null) ? IntPtr.Zero : obj.GetRawObject());
			return AndroidJNI.IsSameObject(this.proxyObject, intPtr);
		}

		public virtual int hashCode()
		{
			jvalue[] array = new jvalue[1];
			array[0].l = this.GetRawProxy();
			return AndroidJNISafe.CallStaticIntMethod(AndroidJavaProxy.s_JavaLangSystemClass, AndroidJavaProxy.s_HashCodeMethodID, array);
		}

		public virtual string toString()
		{
			return this.ToString() + " <c# proxy java object>";
		}

		internal AndroidJavaObject GetProxyObject()
		{
			return AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(this.GetRawProxy());
		}

		internal IntPtr GetRawProxy()
		{
			IntPtr intPtr = IntPtr.Zero;
			bool flag = this.proxyObject != IntPtr.Zero;
			if (flag)
			{
				intPtr = AndroidJNI.NewLocalRef(this.proxyObject);
				bool flag2 = intPtr == IntPtr.Zero;
				if (flag2)
				{
					AndroidJNI.DeleteWeakGlobalRef(this.proxyObject);
					this.proxyObject = IntPtr.Zero;
				}
			}
			bool flag3 = intPtr == IntPtr.Zero;
			if (flag3)
			{
				intPtr = AndroidJNIHelper.CreateJavaProxy(this);
				this.proxyObject = AndroidJNI.NewWeakGlobalRef(intPtr);
			}
			return intPtr;
		}

		public readonly AndroidJavaClass javaInterface;

		internal IntPtr proxyObject = IntPtr.Zero;

		private static readonly GlobalJavaObjectRef s_JavaLangSystemClass = new GlobalJavaObjectRef(AndroidJNISafe.FindClass("java/lang/System"));

		private static readonly IntPtr s_HashCodeMethodID = AndroidJNIHelper.GetMethodID(AndroidJavaProxy.s_JavaLangSystemClass, "identityHashCode", "(Ljava/lang/Object;)I", true);
	}
}
