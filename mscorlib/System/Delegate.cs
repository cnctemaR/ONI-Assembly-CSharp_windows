using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDual)]
	[Serializable]
	public abstract class Delegate : ICloneable, ISerializable
	{
		protected Delegate(object target, string method)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			this.m_target = target;
			this.data = new DelegateData();
			this.data.method_name = method;
		}

		protected Delegate(Type target, string method)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			this.data = new DelegateData();
			this.data.method_name = method;
			this.data.target_type = target;
		}

		public MethodInfo Method
		{
			get
			{
				if (this.method_info != null)
				{
					return this.method_info;
				}
				if (this.method != IntPtr.Zero)
				{
					this.method_info = (MethodInfo)MethodBase.GetMethodFromHandleNoGenericCheck(new RuntimeMethodHandle(this.method));
				}
				return this.method_info;
			}
		}

		public object Target
		{
			get
			{
				return this.m_target;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Delegate CreateDelegate_internal(Type type, object target, MethodInfo info, bool throwOnBindFailure);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetMulticastInvoke();

		private static bool arg_type_match(Type delArgType, Type argType)
		{
			bool flag = delArgType == argType;
			if (!flag && !argType.IsValueType && argType.IsAssignableFrom(delArgType))
			{
				flag = true;
			}
			return flag;
		}

		private static bool return_type_match(Type delReturnType, Type returnType)
		{
			bool flag = returnType == delReturnType;
			if (!flag && !returnType.IsValueType && delReturnType.IsAssignableFrom(returnType))
			{
				flag = true;
			}
			return flag;
		}

		public static Delegate CreateDelegate(Type type, object firstArgument, MethodInfo method, bool throwOnBindFailure)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			if (!type.IsSubclassOf(typeof(MulticastDelegate)))
			{
				throw new ArgumentException("type is not a subclass of Multicastdelegate");
			}
			MethodInfo methodInfo = type.GetMethod("Invoke");
			if (!Delegate.return_type_match(methodInfo.ReturnType, method.ReturnType))
			{
				if (throwOnBindFailure)
				{
					throw new ArgumentException("method return type is incompatible");
				}
				return null;
			}
			else
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				ParameterInfo[] parameters2 = method.GetParameters();
				bool flag;
				if (firstArgument != null)
				{
					if (!method.IsStatic)
					{
						flag = parameters2.Length == parameters.Length;
					}
					else
					{
						flag = parameters2.Length == parameters.Length + 1;
					}
				}
				else if (!method.IsStatic)
				{
					flag = parameters2.Length + 1 == parameters.Length;
				}
				else
				{
					flag = parameters2.Length == parameters.Length;
					if (!flag)
					{
						flag = parameters2.Length == parameters.Length + 1;
					}
				}
				if (!flag)
				{
					if (throwOnBindFailure)
					{
						throw new ArgumentException("method argument length mismatch");
					}
					return null;
				}
				else
				{
					bool flag2;
					if (firstArgument != null)
					{
						if (!method.IsStatic)
						{
							flag2 = Delegate.arg_type_match(firstArgument.GetType(), method.DeclaringType);
							for (int i = 0; i < parameters2.Length; i++)
							{
								flag2 &= Delegate.arg_type_match(parameters[i].ParameterType, parameters2[i].ParameterType);
							}
						}
						else
						{
							flag2 = Delegate.arg_type_match(firstArgument.GetType(), parameters2[0].ParameterType);
							for (int j = 1; j < parameters2.Length; j++)
							{
								flag2 &= Delegate.arg_type_match(parameters[j - 1].ParameterType, parameters2[j].ParameterType);
							}
						}
					}
					else if (!method.IsStatic)
					{
						flag2 = Delegate.arg_type_match(parameters[0].ParameterType, method.DeclaringType);
						for (int k = 0; k < parameters2.Length; k++)
						{
							flag2 &= Delegate.arg_type_match(parameters[k + 1].ParameterType, parameters2[k].ParameterType);
						}
					}
					else if (parameters.Length + 1 == parameters2.Length)
					{
						flag2 = !parameters2[0].ParameterType.IsValueType;
						for (int l = 0; l < parameters.Length; l++)
						{
							flag2 &= Delegate.arg_type_match(parameters[l].ParameterType, parameters2[l + 1].ParameterType);
						}
					}
					else
					{
						flag2 = true;
						for (int m = 0; m < parameters2.Length; m++)
						{
							flag2 &= Delegate.arg_type_match(parameters[m].ParameterType, parameters2[m].ParameterType);
						}
					}
					if (flag2)
					{
						Delegate @delegate = Delegate.CreateDelegate_internal(type, firstArgument, method, throwOnBindFailure);
						if (@delegate != null)
						{
							@delegate.original_method_info = method;
						}
						return @delegate;
					}
					if (throwOnBindFailure)
					{
						throw new ArgumentException("method arguments are incompatible");
					}
					return null;
				}
			}
		}

		public static Delegate CreateDelegate(Type type, object firstArgument, MethodInfo method)
		{
			return Delegate.CreateDelegate(type, firstArgument, method, true);
		}

		public static Delegate CreateDelegate(Type type, MethodInfo method, bool throwOnBindFailure)
		{
			return Delegate.CreateDelegate(type, null, method, throwOnBindFailure);
		}

		public static Delegate CreateDelegate(Type type, MethodInfo method)
		{
			return Delegate.CreateDelegate(type, method, true);
		}

		public static Delegate CreateDelegate(Type type, object target, string method)
		{
			return Delegate.CreateDelegate(type, target, method, false);
		}

		private static MethodInfo GetCandidateMethod(Type type, Type target, string method, BindingFlags bflags, bool ignoreCase, bool throwOnBindFailure)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			if (!type.IsSubclassOf(typeof(MulticastDelegate)))
			{
				throw new ArgumentException("type is not subclass of MulticastDelegate.");
			}
			MethodInfo methodInfo = type.GetMethod("Invoke");
			ParameterInfo[] parameters = methodInfo.GetParameters();
			Type[] array = new Type[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				array[i] = parameters[i].ParameterType;
			}
			BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.ExactBinding | bflags;
			if (ignoreCase)
			{
				bindingFlags |= BindingFlags.IgnoreCase;
			}
			MethodInfo methodInfo2 = null;
			for (Type type2 = target; type2 != null; type2 = type2.BaseType)
			{
				MethodInfo methodInfo3 = type2.GetMethod(method, bindingFlags, null, array, new ParameterModifier[0]);
				if (methodInfo3 != null && Delegate.return_type_match(methodInfo.ReturnType, methodInfo3.ReturnType))
				{
					methodInfo2 = methodInfo3;
					break;
				}
			}
			if (methodInfo2 != null)
			{
				return methodInfo2;
			}
			if (throwOnBindFailure)
			{
				throw new ArgumentException("Couldn't bind to method '" + method + "'.");
			}
			return null;
		}

		public static Delegate CreateDelegate(Type type, Type target, string method, bool ignoreCase, bool throwOnBindFailure)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			MethodInfo candidateMethod = Delegate.GetCandidateMethod(type, target, method, BindingFlags.Static, ignoreCase, throwOnBindFailure);
			if (candidateMethod == null)
			{
				return null;
			}
			return Delegate.CreateDelegate_internal(type, null, candidateMethod, throwOnBindFailure);
		}

		public static Delegate CreateDelegate(Type type, Type target, string method)
		{
			return Delegate.CreateDelegate(type, target, method, false, true);
		}

		public static Delegate CreateDelegate(Type type, Type target, string method, bool ignoreCase)
		{
			return Delegate.CreateDelegate(type, target, method, ignoreCase, true);
		}

		public static Delegate CreateDelegate(Type type, object target, string method, bool ignoreCase, bool throwOnBindFailure)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			MethodInfo candidateMethod = Delegate.GetCandidateMethod(type, target.GetType(), method, BindingFlags.Instance, ignoreCase, throwOnBindFailure);
			if (candidateMethod == null)
			{
				return null;
			}
			return Delegate.CreateDelegate_internal(type, target, candidateMethod, throwOnBindFailure);
		}

		public static Delegate CreateDelegate(Type type, object target, string method, bool ignoreCase)
		{
			return Delegate.CreateDelegate(type, target, method, ignoreCase, true);
		}

		public object DynamicInvoke(params object[] args)
		{
			return this.DynamicInvokeImpl(args);
		}

		protected virtual object DynamicInvokeImpl(object[] args)
		{
			if (this.Method == null)
			{
				Type[] array = new Type[args.Length];
				for (int i = 0; i < args.Length; i++)
				{
					array[i] = args[i].GetType();
				}
				this.method_info = this.m_target.GetType().GetMethod(this.data.method_name, array);
			}
			if (this.m_target != null && this.Method.IsStatic)
			{
				if (args != null)
				{
					object[] array2 = new object[args.Length + 1];
					args.CopyTo(array2, 1);
					array2[0] = this.m_target;
					args = array2;
				}
				else
				{
					args = new object[] { this.m_target };
				}
				return this.Method.Invoke(null, args);
			}
			return this.Method.Invoke(this.m_target, args);
		}

		public virtual object Clone()
		{
			return base.MemberwiseClone();
		}

		public override bool Equals(object obj)
		{
			Delegate @delegate = obj as Delegate;
			return @delegate != null && (@delegate.m_target == this.m_target && @delegate.method == this.method) && ((@delegate.data == null && this.data == null) || (@delegate.data != null && this.data != null && @delegate.data.target_type == this.data.target_type && @delegate.data.method_name == this.data.method_name));
		}

		public override int GetHashCode()
		{
			return this.method.GetHashCode() ^ ((this.m_target == null) ? 0 : this.m_target.GetHashCode());
		}

		protected virtual MethodInfo GetMethodImpl()
		{
			return this.Method;
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			DelegateSerializationHolder.GetDelegateData(this, info, context);
		}

		public virtual Delegate[] GetInvocationList()
		{
			return new Delegate[] { this };
		}

		public static Delegate Combine(Delegate a, Delegate b)
		{
			if (a == null)
			{
				if (b == null)
				{
					return null;
				}
				return b;
			}
			else
			{
				if (b == null)
				{
					return a;
				}
				if (a.GetType() != b.GetType())
				{
					throw new ArgumentException(Locale.GetText("Incompatible Delegate Types."));
				}
				return a.CombineImpl(b);
			}
		}

		[ComVisible(true)]
		public static Delegate Combine(params Delegate[] delegates)
		{
			if (delegates == null)
			{
				return null;
			}
			Delegate @delegate = null;
			foreach (Delegate delegate2 in delegates)
			{
				@delegate = Delegate.Combine(@delegate, delegate2);
			}
			return @delegate;
		}

		protected virtual Delegate CombineImpl(Delegate d)
		{
			throw new MulticastNotSupportedException(string.Empty);
		}

		public static Delegate Remove(Delegate source, Delegate value)
		{
			if (source == null)
			{
				return null;
			}
			return source.RemoveImpl(value);
		}

		protected virtual Delegate RemoveImpl(Delegate d)
		{
			if (this.Equals(d))
			{
				return null;
			}
			return this;
		}

		public static Delegate RemoveAll(Delegate source, Delegate value)
		{
			Delegate @delegate = source;
			while ((source = Delegate.Remove(source, value)) != @delegate)
			{
				@delegate = source;
			}
			return @delegate;
		}

		public static bool operator ==(Delegate d1, Delegate d2)
		{
			if (d1 == null)
			{
				return d2 == null;
			}
			return d2 != null && d1.Equals(d2);
		}

		public static bool operator !=(Delegate d1, Delegate d2)
		{
			return !(d1 == d2);
		}

		private IntPtr method_ptr;

		private IntPtr invoke_impl;

		private object m_target;

		private IntPtr method;

		private IntPtr delegate_trampoline;

		private IntPtr method_code;

		private MethodInfo method_info;

		private MethodInfo original_method_info;

		private DelegateData data;
	}
}
