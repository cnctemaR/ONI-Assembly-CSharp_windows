using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Reflection.Emit
{
	[StructLayout(LayoutKind.Sequential)]
	internal class MethodOnTypeBuilderInst : MethodInfo
	{
		public MethodOnTypeBuilderInst(TypeBuilderInstantiation instantiation, MethodInfo base_method)
		{
			this.instantiation = instantiation;
			this.base_method = base_method;
		}

		internal MethodOnTypeBuilderInst(MethodOnTypeBuilderInst gmd, Type[] typeArguments)
		{
			this.instantiation = gmd.instantiation;
			this.base_method = gmd.base_method;
			this.method_arguments = new Type[typeArguments.Length];
			typeArguments.CopyTo(this.method_arguments, 0);
			this.generic_method_definition = gmd;
		}

		internal MethodOnTypeBuilderInst(MethodInfo method, Type[] typeArguments)
		{
			this.instantiation = method.DeclaringType;
			this.base_method = MethodOnTypeBuilderInst.ExtractBaseMethod(method);
			this.method_arguments = new Type[typeArguments.Length];
			typeArguments.CopyTo(this.method_arguments, 0);
			if (this.base_method != method)
			{
				this.generic_method_definition = method;
			}
		}

		private static MethodInfo ExtractBaseMethod(MethodInfo info)
		{
			if (info is MethodBuilder)
			{
				return info;
			}
			if (info is MethodOnTypeBuilderInst)
			{
				return ((MethodOnTypeBuilderInst)info).base_method;
			}
			if (info.IsGenericMethod)
			{
				info = info.GetGenericMethodDefinition();
			}
			Type declaringType = info.DeclaringType;
			if (!declaringType.IsGenericType || declaringType.IsGenericTypeDefinition)
			{
				return info;
			}
			return (MethodInfo)declaringType.Module.ResolveMethod(info.MetadataToken);
		}

		internal Type[] GetTypeArgs()
		{
			if (!this.instantiation.IsGenericType || this.instantiation.IsGenericParameter)
			{
				return null;
			}
			return this.instantiation.GetGenericArguments();
		}

		internal MethodInfo RuntimeResolve()
		{
			MethodInfo methodInfo = this.instantiation.InternalResolve().GetMethod(this.base_method);
			if (this.method_arguments != null)
			{
				Type[] array = new Type[this.method_arguments.Length];
				for (int i = 0; i < this.method_arguments.Length; i++)
				{
					array[i] = this.method_arguments[i].InternalResolve();
				}
				methodInfo = methodInfo.MakeGenericMethod(array);
			}
			return methodInfo;
		}

		public override Type DeclaringType
		{
			get
			{
				return this.instantiation;
			}
		}

		public override string Name
		{
			get
			{
				return this.base_method.Name;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return this.instantiation;
			}
		}

		public override Type ReturnType
		{
			get
			{
				return this.base_method.ReturnType;
			}
		}

		public override Module Module
		{
			get
			{
				return this.base_method.Module;
			}
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(this.ReturnType.ToString());
			stringBuilder.Append(" ");
			stringBuilder.Append(this.base_method.Name);
			stringBuilder.Append("(");
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return this.base_method.GetMethodImplementationFlags();
		}

		public override ParameterInfo[] GetParameters()
		{
			return this.GetParametersInternal();
		}

		internal override ParameterInfo[] GetParametersInternal()
		{
			throw new NotSupportedException();
		}

		public override int MetadataToken
		{
			get
			{
				return base.MetadataToken;
			}
		}

		internal override int GetParametersCount()
		{
			return this.base_method.GetParametersCount();
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override MethodAttributes Attributes
		{
			get
			{
				return this.base_method.Attributes;
			}
		}

		public override CallingConventions CallingConvention
		{
			get
			{
				return this.base_method.CallingConvention;
			}
		}

		public override MethodInfo MakeGenericMethod(params Type[] methodInstantiation)
		{
			if (!this.base_method.IsGenericMethodDefinition || this.method_arguments != null)
			{
				throw new InvalidOperationException("Method is not a generic method definition");
			}
			if (methodInstantiation == null)
			{
				throw new ArgumentNullException("methodInstantiation");
			}
			if (this.base_method.GetGenericArguments().Length != methodInstantiation.Length)
			{
				throw new ArgumentException("Incorrect length", "methodInstantiation");
			}
			for (int i = 0; i < methodInstantiation.Length; i++)
			{
				if (methodInstantiation[i] == null)
				{
					throw new ArgumentNullException("methodInstantiation");
				}
			}
			return new MethodOnTypeBuilderInst(this, methodInstantiation);
		}

		public override Type[] GetGenericArguments()
		{
			if (!this.base_method.IsGenericMethodDefinition)
			{
				return null;
			}
			Type[] array = this.method_arguments ?? this.base_method.GetGenericArguments();
			Type[] array2 = new Type[array.Length];
			array.CopyTo(array2, 0);
			return array2;
		}

		public override MethodInfo GetGenericMethodDefinition()
		{
			return this.generic_method_definition ?? this.base_method;
		}

		public override bool ContainsGenericParameters
		{
			get
			{
				if (this.base_method.ContainsGenericParameters)
				{
					return true;
				}
				if (!this.base_method.IsGenericMethodDefinition)
				{
					throw new NotSupportedException();
				}
				if (this.method_arguments == null)
				{
					return true;
				}
				Type[] array = this.method_arguments;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].ContainsGenericParameters)
					{
						return true;
					}
				}
				return false;
			}
		}

		public override bool IsGenericMethodDefinition
		{
			get
			{
				return this.base_method.IsGenericMethodDefinition && this.method_arguments == null;
			}
		}

		public override bool IsGenericMethod
		{
			get
			{
				return this.base_method.IsGenericMethodDefinition;
			}
		}

		public override MethodInfo GetBaseDefinition()
		{
			throw new NotSupportedException();
		}

		public override ParameterInfo ReturnParameter
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		private Type instantiation;

		private MethodInfo base_method;

		private Type[] method_arguments;

		private MethodInfo generic_method_definition;
	}
}
