using System;
using System.Runtime.Serialization;
using System.Text;

namespace System.Reflection
{
	internal abstract class RuntimeMethodInfo : MethodInfo, ISerializable
	{
		internal BindingFlags BindingFlags
		{
			get
			{
				return BindingFlags.Default;
			}
		}

		public override Module Module
		{
			get
			{
				return this.GetRuntimeModule();
			}
		}

		private RuntimeType ReflectedTypeInternal
		{
			get
			{
				return (RuntimeType)this.ReflectedType;
			}
		}

		internal override string FormatNameAndSig(bool serialization)
		{
			StringBuilder stringBuilder = new StringBuilder(this.Name);
			TypeNameFormatFlags typeNameFormatFlags = (serialization ? TypeNameFormatFlags.FormatSerialization : TypeNameFormatFlags.FormatBasic);
			if (this.IsGenericMethod)
			{
				stringBuilder.Append(RuntimeMethodHandle.ConstructInstantiation(this, typeNameFormatFlags));
			}
			stringBuilder.Append("(");
			ParameterInfo.FormatParameters(stringBuilder, this.GetParametersNoCopy(), this.CallingConvention, serialization);
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public override Delegate CreateDelegate(Type delegateType)
		{
			return Delegate.CreateDelegate(delegateType, this);
		}

		public override Delegate CreateDelegate(Type delegateType, object target)
		{
			return Delegate.CreateDelegate(delegateType, target, this);
		}

		public override string ToString()
		{
			return this.ReturnType.FormatTypeName() + " " + this.FormatNameAndSig(false);
		}

		internal RuntimeModule GetRuntimeModule()
		{
			return ((RuntimeType)this.DeclaringType).GetRuntimeModule();
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			MemberInfoSerializationHolder.GetSerializationInfo(info, this.Name, this.ReflectedTypeInternal, this.ToString(), this.SerializationToString(), MemberTypes.Method, (this.IsGenericMethod & !this.IsGenericMethodDefinition) ? this.GetGenericArguments() : null);
		}

		internal string SerializationToString()
		{
			return this.ReturnType.FormatTypeName(true) + " " + this.FormatNameAndSig(true);
		}
	}
}
