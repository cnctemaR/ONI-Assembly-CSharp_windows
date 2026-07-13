using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	[RequiredByNativeCode]
	public sealed class MakeSerializableAttribute : Attribute
	{
		public MakeSerializableAttribute(Type type)
		{
			bool flag = type == null;
			if (flag)
			{
				throw new ArgumentException("type is null.");
			}
			bool isValueType = type.IsValueType;
			if (isValueType)
			{
				throw new ArgumentException("type Type cannot be a value type.");
			}
			bool isInterface = type.IsInterface;
			if (isInterface)
			{
				throw new ArgumentException("type Type cannot be an interface type.");
			}
			bool flag2 = !type.IsClass;
			if (flag2)
			{
				throw new ArgumentException("type Type must be a class");
			}
			this.serializableType = type;
		}

		[RequiredByNativeCode]
		private Type GetSerializableType()
		{
			return this.serializableType;
		}

		private Type serializableType;
	}
}
