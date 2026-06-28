using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public sealed class DefaultExclusiveObjectGraphVisitor : ChainedObjectGraphVisitor
	{
		public DefaultExclusiveObjectGraphVisitor(IObjectGraphVisitor nextVisitor)
			: base(nextVisitor)
		{
		}

		private static object GetDefault(Type type)
		{
			if (!type.IsValueType())
			{
				return null;
			}
			return Activator.CreateInstance(type);
		}

		public override bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value)
		{
			return !DefaultExclusiveObjectGraphVisitor._objectComparer.Equals(value, DefaultExclusiveObjectGraphVisitor.GetDefault(value.Type)) && base.EnterMapping(key, value);
		}

		public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value)
		{
			DefaultValueAttribute customAttribute = key.GetCustomAttribute<DefaultValueAttribute>();
			object obj = ((customAttribute != null) ? customAttribute.Value : DefaultExclusiveObjectGraphVisitor.GetDefault(key.Type));
			return !DefaultExclusiveObjectGraphVisitor._objectComparer.Equals(value.Value, obj) && base.EnterMapping(key, value);
		}

		private static readonly IEqualityComparer<object> _objectComparer = EqualityComparer<object>.Default;
	}
}
