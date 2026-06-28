using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public sealed class CustomSerializationObjectGraphVisitor : ChainedObjectGraphVisitor
	{
		public CustomSerializationObjectGraphVisitor(IEmitter emitter, IObjectGraphVisitor nextVisitor, IEnumerable<IYamlTypeConverter> typeConverters)
			: base(nextVisitor)
		{
			this.emitter = emitter;
			this.typeConverters = ((typeConverters != null) ? typeConverters.ToList<IYamlTypeConverter>() : Enumerable.Empty<IYamlTypeConverter>());
		}

		public override bool Enter(IObjectDescriptor value)
		{
			IYamlTypeConverter yamlTypeConverter = this.typeConverters.FirstOrDefault<IYamlTypeConverter>((IYamlTypeConverter t) => t.Accepts(value.Type));
			if (yamlTypeConverter != null)
			{
				yamlTypeConverter.WriteYaml(this.emitter, value.Value, value.Type);
				return false;
			}
			IYamlSerializable yamlSerializable = value as IYamlSerializable;
			if (yamlSerializable != null)
			{
				yamlSerializable.WriteYaml(this.emitter);
				return false;
			}
			return base.Enter(value);
		}

		private readonly IEmitter emitter;

		private readonly IEnumerable<IYamlTypeConverter> typeConverters;
	}
}
