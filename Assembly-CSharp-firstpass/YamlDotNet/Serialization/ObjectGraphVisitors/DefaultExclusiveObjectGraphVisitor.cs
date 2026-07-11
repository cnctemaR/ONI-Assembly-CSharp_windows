using System;
using System.Collections.Generic;
using System.ComponentModel;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public sealed class DefaultExclusiveObjectGraphVisitor : ChainedObjectGraphVisitor
	{
		public DefaultExclusiveObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor)
			: base(nextVisitor)
		{
		}

		private static object GetDefault(Type type)
		{
			return (!type.IsValueType()) ? null : Activator.CreateInstance(type);
		}

		public override bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value, IEmitter context)
		{
			return !DefaultExclusiveObjectGraphVisitor._objectComparer.Equals(value, DefaultExclusiveObjectGraphVisitor.GetDefault(value.Type)) && base.EnterMapping(key, value, context);
		}

		public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
		{
			DefaultValueAttribute customAttribute = key.GetCustomAttribute<DefaultValueAttribute>();
			object obj = ((customAttribute == null) ? DefaultExclusiveObjectGraphVisitor.GetDefault(key.Type) : customAttribute.Value);
			return !DefaultExclusiveObjectGraphVisitor._objectComparer.Equals(value.Value, obj) && base.EnterMapping(key, value, context);
		}

		private static readonly IEqualityComparer<object> _objectComparer = EqualityComparer<object>.Default;
	}
}
