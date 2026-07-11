using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public sealed class CustomSerializationObjectGraphVisitor : ChainedObjectGraphVisitor
	{
		public CustomSerializationObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor, IEnumerable<IYamlTypeConverter> typeConverters, ObjectSerializer nestedObjectSerializer)
			: base(nextVisitor)
		{
			this.typeConverters = ((typeConverters == null) ? Enumerable.Empty<IYamlTypeConverter>() : typeConverters.ToList<IYamlTypeConverter>());
			this.nestedObjectSerializer = nestedObjectSerializer;
		}

		public override bool Enter(IObjectDescriptor value, IEmitter context)
		{
			IYamlTypeConverter yamlTypeConverter = this.typeConverters.FirstOrDefault<IYamlTypeConverter>((IYamlTypeConverter t) => t.Accepts(value.Type));
			if (yamlTypeConverter != null)
			{
				yamlTypeConverter.WriteYaml(context, value.Value, value.Type);
				return false;
			}
			IYamlConvertible yamlConvertible = value.Value as IYamlConvertible;
			if (yamlConvertible != null)
			{
				yamlConvertible.Write(context, this.nestedObjectSerializer);
				return false;
			}
			IYamlSerializable yamlSerializable = value.Value as IYamlSerializable;
			if (yamlSerializable != null)
			{
				yamlSerializable.WriteYaml(context);
				return false;
			}
			return base.Enter(value, context);
		}

		private readonly IEnumerable<IYamlTypeConverter> typeConverters;

		private readonly ObjectSerializer nestedObjectSerializer;
	}
}
