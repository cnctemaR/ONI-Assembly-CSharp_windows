using System;
using System.Globalization;
using System.IO;

namespace Newtonsoft.Json.Serialization
{
	internal class TraceJsonReader : JsonReader, IJsonLineInfo
	{
		public TraceJsonReader(JsonReader innerReader)
		{
			this._innerReader = innerReader;
			this._sw = new StringWriter(CultureInfo.InvariantCulture);
			this._sw.Write("Deserialized JSON: " + Environment.NewLine);
			this._textWriter = new JsonTextWriter(this._sw);
			this._textWriter.Formatting = Formatting.Indented;
		}

		public string GetDeserializedJsonMessage()
		{
			return this._sw.ToString();
		}

		public override bool Read()
		{
			bool flag = this._innerReader.Read();
			this._textWriter.WriteToken(this._innerReader, false, false);
			return flag;
		}

		public override int? ReadAsInt32()
		{
			int? num = this._innerReader.ReadAsInt32();
			this._textWriter.WriteToken(this._innerReader, false, false);
			return num;
		}

		public override string ReadAsString()
		{
			string text = this._innerReader.ReadAsString();
			this._textWriter.WriteToken(this._innerReader, false, false);
			return text;
		}

		public override byte[] ReadAsBytes()
		{
			byte[] array = this._innerReader.ReadAsBytes();
			this._textWriter.WriteToken(this._innerReader, false, false);
			return array;
		}

		public override decimal? ReadAsDecimal()
		{
			decimal? num = this._innerReader.ReadAsDecimal();
			this._textWriter.WriteToken(this._innerReader, false, false);
			return num;
		}

		public override DateTime? ReadAsDateTime()
		{
			DateTime? dateTime = this._innerReader.ReadAsDateTime();
			this._textWriter.WriteToken(this._innerReader, false, false);
			return dateTime;
		}

		public override DateTimeOffset? ReadAsDateTimeOffset()
		{
			DateTimeOffset? dateTimeOffset = this._innerReader.ReadAsDateTimeOffset();
			this._textWriter.WriteToken(this._innerReader, false, false);
			return dateTimeOffset;
		}

		public override int Depth
		{
			get
			{
				return this._innerReader.Depth;
			}
		}

		public override string Path
		{
			get
			{
				return this._innerReader.Path;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return this._innerReader.QuoteChar;
			}
			protected internal set
			{
				this._innerReader.QuoteChar = value;
			}
		}

		public override JsonToken TokenType
		{
			get
			{
				return this._innerReader.TokenType;
			}
		}

		public override object Value
		{
			get
			{
				return this._innerReader.Value;
			}
		}

		public override Type ValueType
		{
			get
			{
				return this._innerReader.ValueType;
			}
		}

		public override void Close()
		{
			this._innerReader.Close();
		}

		bool IJsonLineInfo.HasLineInfo()
		{
			IJsonLineInfo jsonLineInfo = this._innerReader as IJsonLineInfo;
			return jsonLineInfo != null && jsonLineInfo.HasLineInfo();
		}

		int IJsonLineInfo.LineNumber
		{
			get
			{
				IJsonLineInfo jsonLineInfo = this._innerReader as IJsonLineInfo;
				if (jsonLineInfo == null)
				{
					return 0;
				}
				return jsonLineInfo.LineNumber;
			}
		}

		int IJsonLineInfo.LinePosition
		{
			get
			{
				IJsonLineInfo jsonLineInfo = this._innerReader as IJsonLineInfo;
				if (jsonLineInfo == null)
				{
					return 0;
				}
				return jsonLineInfo.LinePosition;
			}
		}

		private readonly JsonReader _innerReader;

		private readonly JsonTextWriter _textWriter;

		private readonly StringWriter _sw;
	}
}
