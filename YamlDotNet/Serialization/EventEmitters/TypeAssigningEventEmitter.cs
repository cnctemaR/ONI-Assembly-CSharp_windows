using System;
using System.Globalization;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.EventEmitters
{
	public sealed class TypeAssigningEventEmitter : ChainedEventEmitter
	{
		public TypeAssigningEventEmitter(IEventEmitter nextEmitter, bool assignTypeWhenDifferent)
			: base(nextEmitter)
		{
			this._assignTypeWhenDifferent = assignTypeWhenDifferent;
		}

		public override void Emit(ScalarEventInfo eventInfo)
		{
			ScalarStyle scalarStyle = ScalarStyle.Plain;
			TypeCode typeCode = ((eventInfo.Source.Value != null) ? eventInfo.Source.Type.GetTypeCode() : TypeCode.Empty);
			switch (typeCode)
			{
			case TypeCode.Empty:
				eventInfo.Tag = "tag:yaml.org,2002:null";
				eventInfo.RenderedValue = "";
				goto IL_01A6;
			case TypeCode.Boolean:
				eventInfo.Tag = "tag:yaml.org,2002:bool";
				eventInfo.RenderedValue = YamlFormatter.FormatBoolean(eventInfo.Source.Value);
				goto IL_01A6;
			case TypeCode.Char:
			case TypeCode.String:
				eventInfo.Tag = "tag:yaml.org,2002:str";
				eventInfo.RenderedValue = eventInfo.Source.Value.ToString();
				scalarStyle = ScalarStyle.Any;
				goto IL_01A6;
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
				goto IL_01A6;
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				eventInfo.Tag = "tag:yaml.org,2002:float";
				eventInfo.RenderedValue = YamlFormatter.FormatNumber(eventInfo.Source.Value);
				goto IL_01A6;
			case TypeCode.DateTime:
				eventInfo.Tag = "tag:yaml.org,2002:timestamp";
				eventInfo.RenderedValue = YamlFormatter.FormatDateTime(eventInfo.Source.Value);
				goto IL_01A6;
			}
			if (eventInfo.Source.Type != typeof(TimeSpan))
			{
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "TypeCode.{0} is not supported.", new object[] { typeCode }));
			}
			eventInfo.RenderedValue = YamlFormatter.FormatTimeSpan(eventInfo.Source.Value);
			IL_01A6:
			eventInfo.IsPlainImplicit = true;
			if (eventInfo.Style == ScalarStyle.Any)
			{
				eventInfo.Style = scalarStyle;
			}
			base.Emit(eventInfo);
		}

		public override void Emit(MappingStartEventInfo eventInfo)
		{
			this.AssignTypeIfDifferent(eventInfo);
			base.Emit(eventInfo);
		}

		public override void Emit(SequenceStartEventInfo eventInfo)
		{
			this.AssignTypeIfDifferent(eventInfo);
			base.Emit(eventInfo);
		}

		private void AssignTypeIfDifferent(ObjectEventInfo eventInfo)
		{
			if (this._assignTypeWhenDifferent && eventInfo.Source.Value != null && eventInfo.Source.Type != eventInfo.Source.StaticType)
			{
				eventInfo.Tag = "!" + eventInfo.Source.Type.AssemblyQualifiedName;
			}
		}

		private readonly bool _assignTypeWhenDifferent;
	}
}
