using System;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
	public class CatalogReflectionContextAttribute : Attribute
	{
		public CatalogReflectionContextAttribute(Type reflectionContextType)
		{
			Requires.NotNull<Type>(reflectionContextType, "reflectionContextType");
			this._reflectionContextType = reflectionContextType;
		}

		public ReflectionContext CreateReflectionContext()
		{
			Assumes.NotNull<Type>(this._reflectionContextType);
			ReflectionContext reflectionContext = null;
			try
			{
				reflectionContext = (ReflectionContext)Activator.CreateInstance(this._reflectionContextType);
			}
			catch (InvalidCastException ex)
			{
				throw new InvalidOperationException(Strings.ReflectionContext_Type_Required, ex);
			}
			catch (MissingMethodException ex2)
			{
				throw new MissingMethodException(Strings.ReflectionContext_Requires_DefaultConstructor, ex2);
			}
			return reflectionContext;
		}

		private Type _reflectionContextType;
	}
}
