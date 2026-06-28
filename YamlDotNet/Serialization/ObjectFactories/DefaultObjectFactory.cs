using System;
using System.Collections.Generic;

namespace YamlDotNet.Serialization.ObjectFactories
{
	public sealed class DefaultObjectFactory : IObjectFactory
	{
		public object Create(Type type)
		{
			Type type2;
			if (type.IsInterface() && DefaultObjectFactory.defaultInterfaceImplementations.TryGetValue(type.GetGenericTypeDefinition(), out type2))
			{
				type = type2.MakeGenericType(type.GetGenericArguments());
			}
			object obj;
			try
			{
				obj = Activator.CreateInstance(type);
			}
			catch (Exception ex)
			{
				string text = string.Format("Failed to create an instance of type '{0}'.", type);
				throw new InvalidOperationException(text, ex);
			}
			return obj;
		}

		private static readonly Dictionary<Type, Type> defaultInterfaceImplementations = new Dictionary<Type, Type>
		{
			{
				typeof(IEnumerable<>),
				typeof(List<>)
			},
			{
				typeof(ICollection<>),
				typeof(List<>)
			},
			{
				typeof(IList<>),
				typeof(List<>)
			},
			{
				typeof(IDictionary<, >),
				typeof(Dictionary<, >)
			}
		};
	}
}
