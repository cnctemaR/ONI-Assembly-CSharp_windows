using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;

namespace System.Data.SqlClient
{
	internal sealed class SqlCachedBuffer : INullable
	{
		private SqlCachedBuffer()
		{
		}

		private SqlCachedBuffer(List<byte[]> cachedBytes)
		{
			this._cachedBytes = cachedBytes;
		}

		internal List<byte[]> CachedBytes
		{
			get
			{
				return this._cachedBytes;
			}
		}

		internal static bool TryCreate(SqlMetaDataPriv metadata, TdsParser parser, TdsParserStateObject stateObj, out SqlCachedBuffer buffer)
		{
			int num = 0;
			List<byte[]> list = new List<byte[]>();
			buffer = null;
			ulong num2;
			if (!parser.TryPlpBytesLeft(stateObj, out num2))
			{
				return false;
			}
			while (num2 != 0UL)
			{
				do
				{
					num = ((num2 > 2048UL) ? 2048 : ((int)num2));
					byte[] array = new byte[num];
					if (!stateObj.TryReadPlpBytes(ref array, 0, num, out num))
					{
						return false;
					}
					if (list.Count == 0)
					{
						SqlCachedBuffer.AddByteOrderMark(array, list);
					}
					list.Add(array);
					num2 -= (ulong)((long)num);
				}
				while (num2 > 0UL);
				if (!parser.TryPlpBytesLeft(stateObj, out num2))
				{
					return false;
				}
				if (num2 <= 0UL)
				{
					break;
				}
				continue;
			}
			buffer = new SqlCachedBuffer(list);
			return true;
		}

		private static void AddByteOrderMark(byte[] byteArr, List<byte[]> cachedBytes)
		{
			if (byteArr.Length < 2 || byteArr[0] != 223 || byteArr[1] != 255)
			{
				cachedBytes.Add(TdsEnums.XMLUNICODEBOMBYTES);
			}
		}

		internal Stream ToStream()
		{
			return new SqlCachedStream(this);
		}

		public override string ToString()
		{
			if (this.IsNull)
			{
				throw new SqlNullValueException();
			}
			if (this._cachedBytes.Count == 0)
			{
				return string.Empty;
			}
			return new SqlXml(this.ToStream()).Value;
		}

		internal SqlString ToSqlString()
		{
			if (this.IsNull)
			{
				return SqlString.Null;
			}
			return new SqlString(this.ToString());
		}

		internal SqlXml ToSqlXml()
		{
			return new SqlXml(this.ToStream());
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal XmlReader ToXmlReader()
		{
			return SqlTypeWorkarounds.SqlXmlCreateSqlXmlReader(this.ToStream(), false, false);
		}

		public bool IsNull
		{
			get
			{
				return this._cachedBytes == null;
			}
		}

		public static readonly SqlCachedBuffer Null = new SqlCachedBuffer();

		private const int _maxChunkSize = 2048;

		private List<byte[]> _cachedBytes;
	}
}
