using System;
using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.EventEmitters
{
	public sealed class TypeAssigningEventEmitter : ChainedEventEmitter
	{
		public TypeAssigningEventEmitter(IEventEmitter nextEmitter, bool requireTagWhenStaticAndActualTypesAreDifferent, IDictionary<Type, string> tagMappings)
			: base(nextEmitter)
		{
			this.requireTagWhenStaticAndActualTypesAreDifferent = requireTagWhenStaticAndActualTypesAreDifferent;
			this.tagMappings = tagMappings;
		}

		public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
		{
			ScalarStyle scalarStyle = ScalarStyle.Plain;
			TypeCode typeCode = ((eventInfo.Source.Value == null) ? TypeCode.Empty : eventInfo.Source.Type.GetTypeCode());
			switch (typeCode)
			{
			case TypeCode.Empty:
				eventInfo.Tag = "tag:yaml.org,2002:null";
				eventInfo.RenderedValue = string.Empty;
				goto IL_0206;
			case TypeCode.Boolean:
				eventInfo.Tag = "tag:yaml.org,2002:bool";
				eventInfo.RenderedValue = YamlFormatter.FormatBoolean(eventInfo.Source.Value);
				goto IL_0206;
			case TypeCode.Char:
			case TypeCode.String:
				eventInfo.Tag = "tag:yaml.org,2002:str";
				eventInfo.RenderedValue = eventInfo.Source.Value.ToString();
				scalarStyle = ScalarStyle.Any;
				goto IL_0206;
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
				eventInfo.Tag = "tag:yaml.org,2002:int";
				eventInfo.RenderedValue = YamlFormatter.FormatNumber(eventInfo.Source.Value);
				goto IL_0206;
			case TypeCode.Single:
				eventInfo.Tag = "tag:yaml.org,2002:float";
				eventInfo.RenderedValue = YamlFormatter.FormatNumber((float)eventInfo.Source.Value);
				goto IL_0206;
			case TypeCode.Double:
				eventInfo.Tag = "tag:yaml.org,2002:float";
				eventInfo.RenderedValue = YamlFormatter.FormatNumber((double)eventInfo.Source.Value);
				goto IL_0206;
			case TypeCode.Decimal:
				eventInfo.Tag = "tag:yaml.org,2002:float";
				eventInfo.RenderedValue = YamlFormatter.FormatNumber(eventInfo.Source.Value);
				goto IL_0206;
			case TypeCode.DateTime:
				eventInfo.Tag = "tag:yaml.org,2002:timestamp";
				eventInfo.RenderedValue = YamlFormatter.FormatDateTime(eventInfo.Source.Value);
				goto IL_0206;
			}
			if (!(eventInfo.Source.Type == typeof(TimeSpan)))
			{
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "TypeCode.{0} is not supported.", typeCode));
			}
			eventInfo.RenderedValue = YamlFormatter.FormatTimeSpan(eventInfo.Source.Value);
			IL_0206:
			eventInfo.IsPlainImplicit = true;
			if (eventInfo.Style == ScalarStyle.Any)
			{
				eventInfo.Style = scalarStyle;
			}
			base.Emit(eventInfo, emitter);
		}

		public override void Emit(MappingStartEventInfo eventInfo, IEmitter emitter)
		{
			this.AssignTypeIfNeeded(eventInfo);
			base.Emit(eventInfo, emitter);
		}

		public override void Emit(SequenceStartEventInfo eventInfo, IEmitter emitter)
		{
			this.AssignTypeIfNeeded(eventInfo);
			base.Emit(eventInfo, emitter);
		}

		private void AssignTypeIfNeeded(ObjectEventInfo eventInfo)
		{
			string text = null;
			if (this.tagMappings.TryGetValue(eventInfo.Source.Type, out text))
			{
				eventInfo.Tag = text;
			}
			else if (this.requireTagWhenStaticAndActualTypesAreDifferent && eventInfo.Source.Value != null && eventInfo.Source.Type != eventInfo.Source.StaticType)
			{
				throw new YamlException(string.Concat(new string[]
				{
					string.Format("Cannot serialize type '{0}' where a '{1}' was expected ", eventInfo.Source.Type.FullName, eventInfo.Source.StaticType.FullName),
					string.Format("because no tag mapping has been registered for '{0}', ", eventInfo.Source.Type.FullName),
					"which means that it won't be possible to deserialize the document.\n",
					"Register a tag mapping using the SerializerBuilder.WithTagMapping method.\n\n",
					string.Format("E.g: builder.WithTagMapping(\"!{0}\", typeof({1}));", eventInfo.Source.Type.Name, eventInfo.Source.Type.FullName)
				}));
			}
		}

		private readonly bool requireTagWhenStaticAndActualTypesAreDifferent;

		private IDictionary<Type, string> tagMappings;
	}
}
