using System;
using System.Runtime.Serialization;
using System.Text;

namespace System.Reflection
{
	internal abstract class RuntimePropertyInfo : PropertyInfo, ISerializable
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

		internal RuntimeType GetDeclaringTypeInternal()
		{
			return (RuntimeType)this.DeclaringType;
		}

		private RuntimeType ReflectedTypeInternal
		{
			get
			{
				return (RuntimeType)this.ReflectedType;
			}
		}

		internal RuntimeModule GetRuntimeModule()
		{
			return this.GetDeclaringTypeInternal().GetRuntimeModule();
		}

		public override string ToString()
		{
			return this.FormatNameAndSig(false);
		}

		private string FormatNameAndSig(bool serialization)
		{
			StringBuilder stringBuilder = new StringBuilder(this.PropertyType.FormatTypeName(serialization));
			stringBuilder.Append(" ");
			stringBuilder.Append(this.Name);
			ParameterInfo[] indexParameters = this.GetIndexParameters();
			if (indexParameters.Length != 0)
			{
				stringBuilder.Append(" [");
				ParameterInfo.FormatParameters(stringBuilder, indexParameters, (CallingConventions)0, serialization);
				stringBuilder.Append("]");
			}
			return stringBuilder.ToString();
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			MemberInfoSerializationHolder.GetSerializationInfo(info, this.Name, this.ReflectedTypeInternal, this.ToString(), this.SerializationToString(), MemberTypes.Property, null);
		}

		internal string SerializationToString()
		{
			return this.FormatNameAndSig(true);
		}
	}
}
