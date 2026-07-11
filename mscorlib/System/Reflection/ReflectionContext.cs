using System;

namespace System.Reflection
{
	public abstract class ReflectionContext
	{
		public abstract Assembly MapAssembly(Assembly assembly);

		public abstract TypeInfo MapType(TypeInfo type);

		public virtual TypeInfo GetTypeForObject(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return this.MapType(value.GetType().GetTypeInfo());
		}
	}
}
