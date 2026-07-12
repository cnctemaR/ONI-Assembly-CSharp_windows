using System;

namespace Unity.Properties
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class GeneratePropertyBagsForTypeAttribute : Attribute
	{
		public Type Type { get; }

		public GeneratePropertyBagsForTypeAttribute(Type type)
		{
			bool flag = !TypeTraits.IsContainer(type);
			if (flag)
			{
				throw new ArgumentException(type.Name + " is not a valid container type.");
			}
			this.Type = type;
		}
	}
}
