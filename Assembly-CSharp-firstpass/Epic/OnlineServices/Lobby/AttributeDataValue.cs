using System;

namespace Epic.OnlineServices.Lobby
{
	public class AttributeDataValue
	{
		public static implicit operator AttributeDataValue(long value)
		{
			return new AttributeDataValue
			{
				AsInt64 = new long?(value)
			};
		}

		public static implicit operator AttributeDataValue(double value)
		{
			return new AttributeDataValue
			{
				AsDouble = new double?(value)
			};
		}

		public static implicit operator AttributeDataValue(bool value)
		{
			return new AttributeDataValue
			{
				AsBool = new bool?(value)
			};
		}

		public static implicit operator AttributeDataValue(string value)
		{
			return new AttributeDataValue
			{
				AsUtf8 = value
			};
		}

		public long? AsInt64
		{
			get
			{
				long? @default = Helper.GetDefault<long?>();
				Helper.TryMarshalGet<long, AttributeType>(this.m_AsInt64, out @default, this.m_ValueType, AttributeType.Int64);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<long, AttributeType>(ref this.m_AsInt64, value, ref this.m_ValueType, AttributeType.Int64, this);
			}
		}

		public double? AsDouble
		{
			get
			{
				double? @default = Helper.GetDefault<double?>();
				Helper.TryMarshalGet<double, AttributeType>(this.m_AsDouble, out @default, this.m_ValueType, AttributeType.Double);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<double, AttributeType>(ref this.m_AsDouble, value, ref this.m_ValueType, AttributeType.Double, this);
			}
		}

		public bool? AsBool
		{
			get
			{
				bool? @default = Helper.GetDefault<bool?>();
				Helper.TryMarshalGet<bool, AttributeType>(this.m_AsBool, out @default, this.m_ValueType, AttributeType.Boolean);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<bool, AttributeType>(ref this.m_AsBool, value, ref this.m_ValueType, AttributeType.Boolean, this);
			}
		}

		public string AsUtf8
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string, AttributeType>(this.m_AsUtf8, out @default, this.m_ValueType, AttributeType.String);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string, AttributeType>(ref this.m_AsUtf8, value, ref this.m_ValueType, AttributeType.String, this);
			}
		}

		public AttributeType ValueType
		{
			get
			{
				return this.m_ValueType;
			}
			private set
			{
				this.m_ValueType = value;
			}
		}

		private long m_AsInt64;

		private double m_AsDouble;

		private bool m_AsBool;

		private string m_AsUtf8;

		private AttributeType m_ValueType;
	}
}
