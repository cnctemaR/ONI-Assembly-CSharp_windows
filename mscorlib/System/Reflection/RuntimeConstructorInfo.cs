using System;
using System.Runtime.Serialization;

namespace System.Reflection
{
	internal abstract class RuntimeConstructorInfo : ConstructorInfo, ISerializable
	{
		public override Module Module
		{
			get
			{
				return this.GetRuntimeModule();
			}
		}

		internal RuntimeModule GetRuntimeModule()
		{
			return RuntimeTypeHandle.GetModule((RuntimeType)this.DeclaringType);
		}

		internal BindingFlags BindingFlags
		{
			get
			{
				return BindingFlags.Default;
			}
		}

		private RuntimeType ReflectedTypeInternal
		{
			get
			{
				return (RuntimeType)this.ReflectedType;
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			MemberInfoSerializationHolder.GetSerializationInfo(info, this.Name, this.ReflectedTypeInternal, this.ToString(), this.SerializationToString(), MemberTypes.Constructor, null);
		}

		internal string SerializationToString()
		{
			return this.FormatNameAndSig(true);
		}

		internal void SerializationInvoke(object target, SerializationInfo info, StreamingContext context)
		{
			base.Invoke(target, new object[] { info, context });
		}
	}
}
