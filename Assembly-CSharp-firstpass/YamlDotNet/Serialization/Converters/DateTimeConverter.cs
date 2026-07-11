using System;
using System.Globalization;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.Converters
{
	public class DateTimeConverter : IYamlTypeConverter
	{
		public DateTimeConverter(DateTimeKind kind = DateTimeKind.Utc, IFormatProvider provider = null, params string[] formats)
		{
			this.kind = ((kind != DateTimeKind.Unspecified) ? kind : DateTimeKind.Utc);
			this.provider = provider ?? CultureInfo.InvariantCulture;
			this.formats = formats.DefaultIfEmpty("G").ToArray<string>();
		}

		public bool Accepts(Type type)
		{
			return type == typeof(DateTime);
		}

		public object ReadYaml(IParser parser, Type type)
		{
			string value = ((Scalar)parser.Current).Value;
			DateTimeStyles dateTimeStyles = ((this.kind != DateTimeKind.Local) ? DateTimeStyles.AssumeUniversal : DateTimeStyles.AssumeLocal);
			DateTime dateTime = DateTime.ParseExact(value, this.formats, this.provider, dateTimeStyles);
			dateTime = DateTimeConverter.EnsureDateTimeKind(dateTime, this.kind);
			parser.MoveNext();
			return dateTime;
		}

		public void WriteYaml(IEmitter emitter, object value, Type type)
		{
			DateTime dateTime = (DateTime)value;
			string text = ((this.kind != DateTimeKind.Local) ? dateTime.ToUniversalTime() : dateTime.ToLocalTime()).ToString(this.formats.First<string>(), this.provider);
			emitter.Emit(new Scalar(null, null, text, ScalarStyle.Any, true, false));
		}

		private static DateTime EnsureDateTimeKind(DateTime dt, DateTimeKind kind)
		{
			if (dt.Kind == DateTimeKind.Local && kind == DateTimeKind.Utc)
			{
				return dt.ToUniversalTime();
			}
			if (dt.Kind == DateTimeKind.Utc && kind == DateTimeKind.Local)
			{
				return dt.ToLocalTime();
			}
			return dt;
		}

		private readonly DateTimeKind kind;

		private readonly IFormatProvider provider;

		private readonly string[] formats;
	}
}
