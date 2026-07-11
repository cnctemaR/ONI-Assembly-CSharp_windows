using System;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Xml.Serialization
{
	internal class MethodBuilderInfo
	{
		public MethodBuilderInfo(MethodBuilder methodBuilder, Type[] parameterTypes)
		{
			this.MethodBuilder = methodBuilder;
			this.ParameterTypes = parameterTypes;
		}

		public void Validate(Type returnType, Type[] parameterTypes, MethodAttributes attributes)
		{
		}

		public readonly MethodBuilder MethodBuilder;

		public readonly Type[] ParameterTypes;
	}
}
