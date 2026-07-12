using System;

namespace Unity.Properties
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class GeneratePropertyBagsForTypesQualifiedWithAttribute : Attribute
	{
		public Type Type { get; }

		public TypeGenerationOptions Options { get; }

		public GeneratePropertyBagsForTypesQualifiedWithAttribute(Type type, TypeGenerationOptions options = TypeGenerationOptions.Default)
		{
			bool flag = type == null;
			if (flag)
			{
				throw new ArgumentException("type is null.");
			}
			bool flag2 = !type.IsInterface;
			if (flag2)
			{
				throw new ArgumentException("GeneratePropertyBagsForTypesQualifiedWithAttribute Type must be an interface type.");
			}
			this.Type = type;
			this.Options = options;
		}
	}
}
