using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace System.Data.SqlTypes
{
	internal static class SqlTypeWorkarounds
	{
		internal static XmlReader SqlXmlCreateSqlXmlReader(Stream stream, bool closeInput = false, bool async = false)
		{
			XmlReaderSettings xmlReaderSettings = (closeInput ? (async ? SqlTypeWorkarounds.s_defaultXmlReaderSettingsAsyncCloseInput : SqlTypeWorkarounds.s_defaultXmlReaderSettingsCloseInput) : SqlTypeWorkarounds.s_defaultXmlReaderSettings);
			return XmlReader.Create(stream, xmlReaderSettings);
		}

		internal static DateTime SqlDateTimeToDateTime(int daypart, int timepart)
		{
			if (daypart < -53690 || daypart > 2958463 || timepart < 0 || timepart > 25919999)
			{
				throw new OverflowException(SQLResource.DateTimeOverflowMessage);
			}
			long ticks = new DateTime(1900, 1, 1).Ticks;
			long num = (long)daypart * 864000000000L;
			long num2 = (long)((double)timepart / 0.3 + 0.5) * 10000L;
			return new DateTime(ticks + num + num2);
		}

		internal static SqlMoney SqlMoneyCtor(long value, int ignored)
		{
			SqlTypeWorkarounds.SqlMoneyCaster sqlMoneyCaster = default(SqlTypeWorkarounds.SqlMoneyCaster);
			sqlMoneyCaster.Fake._fNotNull = true;
			sqlMoneyCaster.Fake._value = value;
			return sqlMoneyCaster.Real;
		}

		internal static long SqlMoneyToSqlInternalRepresentation(SqlMoney money)
		{
			SqlTypeWorkarounds.SqlMoneyCaster sqlMoneyCaster = default(SqlTypeWorkarounds.SqlMoneyCaster);
			sqlMoneyCaster.Real = money;
			if (money.IsNull)
			{
				throw new SqlNullValueException();
			}
			return sqlMoneyCaster.Fake._value;
		}

		internal static void SqlDecimalExtractData(SqlDecimal d, out uint data1, out uint data2, out uint data3, out uint data4)
		{
			SqlTypeWorkarounds.SqlDecimalCaster sqlDecimalCaster = new SqlTypeWorkarounds.SqlDecimalCaster
			{
				Real = d
			};
			data1 = sqlDecimalCaster.Fake._data1;
			data2 = sqlDecimalCaster.Fake._data2;
			data3 = sqlDecimalCaster.Fake._data3;
			data4 = sqlDecimalCaster.Fake._data4;
		}

		internal static SqlBinary SqlBinaryCtor(byte[] value, bool ignored)
		{
			SqlTypeWorkarounds.SqlBinaryCaster sqlBinaryCaster = default(SqlTypeWorkarounds.SqlBinaryCaster);
			sqlBinaryCaster.Fake._value = value;
			return sqlBinaryCaster.Real;
		}

		internal static SqlGuid SqlGuidCtor(byte[] value, bool ignored)
		{
			SqlTypeWorkarounds.SqlGuidCaster sqlGuidCaster = default(SqlTypeWorkarounds.SqlGuidCaster);
			sqlGuidCaster.Fake._value = value;
			return sqlGuidCaster.Real;
		}

		private static readonly XmlReaderSettings s_defaultXmlReaderSettings = new XmlReaderSettings
		{
			ConformanceLevel = ConformanceLevel.Fragment
		};

		private static readonly XmlReaderSettings s_defaultXmlReaderSettingsCloseInput = new XmlReaderSettings
		{
			ConformanceLevel = ConformanceLevel.Fragment,
			CloseInput = true
		};

		private static readonly XmlReaderSettings s_defaultXmlReaderSettingsAsyncCloseInput = new XmlReaderSettings
		{
			Async = true,
			ConformanceLevel = ConformanceLevel.Fragment,
			CloseInput = true
		};

		internal const SqlCompareOptions SqlStringValidSqlCompareOptionMask = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreNonSpace | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth | SqlCompareOptions.BinarySort | SqlCompareOptions.BinarySort2;

		private struct SqlMoneyLookalike
		{
			internal bool _fNotNull;

			internal long _value;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct SqlMoneyCaster
		{
			[FieldOffset(0)]
			internal SqlMoney Real;

			[FieldOffset(0)]
			internal SqlTypeWorkarounds.SqlMoneyLookalike Fake;
		}

		private struct SqlDecimalLookalike
		{
			internal byte _bStatus;

			internal byte _bLen;

			internal byte _bPrec;

			internal byte _bScale;

			internal uint _data1;

			internal uint _data2;

			internal uint _data3;

			internal uint _data4;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct SqlDecimalCaster
		{
			[FieldOffset(0)]
			internal SqlDecimal Real;

			[FieldOffset(0)]
			internal SqlTypeWorkarounds.SqlDecimalLookalike Fake;
		}

		private struct SqlBinaryLookalike
		{
			internal byte[] _value;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct SqlBinaryCaster
		{
			[FieldOffset(0)]
			internal SqlBinary Real;

			[FieldOffset(0)]
			internal SqlTypeWorkarounds.SqlBinaryLookalike Fake;
		}

		private struct SqlGuidLookalike
		{
			internal byte[] _value;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct SqlGuidCaster
		{
			[FieldOffset(0)]
			internal SqlGuid Real;

			[FieldOffset(0)]
			internal SqlTypeWorkarounds.SqlGuidLookalike Fake;
		}
	}
}
