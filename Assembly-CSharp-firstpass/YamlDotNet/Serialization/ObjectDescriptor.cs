using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
	public sealed class ObjectDescriptor : IObjectDescriptor
	{
		public ObjectDescriptor(object value, Type type, Type staticType)
			: this(value, type, staticType, ScalarStyle.Any)
		{
		}

		public ObjectDescriptor(object value, Type type, Type staticType, ScalarStyle scalarStyle)
		{
			this.Value = value;
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			this.Type = type;
			if (staticType == null)
			{
				throw new ArgumentNullException("staticType");
			}
			this.StaticType = staticType;
			this.ScalarStyle = scalarStyle;
		}

		public object Value { get; private set; }

		public Type Type { get; private set; }

		public Type StaticType { get; private set; }

		public ScalarStyle ScalarStyle { get; private set; }
	}
}
