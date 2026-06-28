using System;

namespace Mono.Data.Tds.Protocol
{
	public sealed class Tds42 : Tds
	{
		public Tds42(string server, int port)
			: this(server, port, 512, 15)
		{
		}

		public Tds42(string server, int port, int packetSize, int timeout)
			: base(server, port, packetSize, timeout, Tds42.Version)
		{
		}

		public override bool Connect(TdsConnectionParameters connectionParameters)
		{
			if (base.IsConnected)
			{
				throw new InvalidOperationException("The connection is already open.");
			}
			base.SetCharset(connectionParameters.Charset);
			base.SetLanguage(connectionParameters.Language);
			byte b = 0;
			byte[] array = new byte[0];
			base.Comm.StartPacket(TdsPacketType.Logon);
			byte[] array2 = base.Comm.Append(connectionParameters.Hostname, 30, b);
			base.Comm.Append((byte)((array2.Length >= 30) ? 30 : array2.Length));
			array2 = base.Comm.Append(connectionParameters.User, 30, b);
			base.Comm.Append((byte)((array2.Length >= 30) ? 30 : array2.Length));
			array2 = base.Comm.Append(connectionParameters.Password, 30, b);
			base.Comm.Append((byte)((array2.Length >= 30) ? 30 : array2.Length));
			base.Comm.Append("00000116", 8, b);
			base.Comm.Append(array, 16, b);
			base.Comm.Append(0);
			base.Comm.Append(160);
			base.Comm.Append(36);
			base.Comm.Append(204);
			base.Comm.Append(80);
			base.Comm.Append(18);
			base.Comm.Append(8);
			base.Comm.Append(3);
			base.Comm.Append(1);
			base.Comm.Append(6);
			base.Comm.Append(10);
			base.Comm.Append(9);
			base.Comm.Append(1);
			base.Comm.Append(1);
			base.Comm.Append(0);
			base.Comm.Append(0);
			base.Comm.Append(array, 7, b);
			array2 = base.Comm.Append(connectionParameters.ApplicationName, 30, b);
			base.Comm.Append((byte)((array2.Length >= 30) ? 30 : array2.Length));
			array2 = base.Comm.Append(base.DataSource, 30, b);
			base.Comm.Append((byte)((array2.Length >= 30) ? 30 : array2.Length));
			base.Comm.Append(array, 2, b);
			array2 = base.Comm.Append(connectionParameters.Password, 253, b);
			base.Comm.Append((byte)((array2.Length >= 253) ? 255 : (array2.Length + 2)));
			base.Comm.Append((byte)Tds42.Version / 10);
			base.Comm.Append((byte)Tds42.Version % 10);
			base.Comm.Append(0);
			base.Comm.Append(0);
			array2 = base.Comm.Append(connectionParameters.ProgName, 10, b);
			base.Comm.Append((byte)((array2.Length >= 10) ? 10 : array2.Length));
			base.Comm.Append(6);
			base.Comm.Append(0);
			base.Comm.Append(0);
			base.Comm.Append(0);
			base.Comm.Append(0);
			base.Comm.Append(13);
			base.Comm.Append(17);
			array2 = base.Comm.Append(base.Language, 30, b);
			base.Comm.Append((byte)((array2.Length >= 30) ? 30 : array2.Length));
			base.Comm.Append(1);
			base.Comm.Append(0);
			base.Comm.Append(array, 8, b);
			base.Comm.Append(0);
			base.Comm.Append(0);
			array2 = base.Comm.Append(base.Charset, 30, b);
			base.Comm.Append((byte)((array2.Length >= 30) ? 30 : array2.Length));
			base.Comm.Append(1);
			array2 = base.Comm.Append(base.PacketSize.ToString(), 6, b);
			base.Comm.Append(3);
			base.Comm.Append(array, 8, b);
			base.Comm.SendPacket();
			base.MoreResults = true;
			base.SkipToEnd();
			return base.IsConnected;
		}

		protected override void ProcessColumnInfo()
		{
			int tdsShort = (int)base.Comm.GetTdsShort();
			int i = 0;
			while (i < tdsShort)
			{
				byte b = 0;
				byte b2 = 0;
				byte[] array = new byte[4];
				for (int j = 0; j < 4; j++)
				{
					array[j] = base.Comm.GetByte();
					i++;
				}
				bool flag = (array[2] & 1) > 0;
				bool flag2 = (array[2] & 12) > 0;
				string text = string.Empty;
				TdsColumnType @byte = (TdsColumnType)base.Comm.GetByte();
				i++;
				int num;
				if (@byte == TdsColumnType.Text || @byte == TdsColumnType.Image)
				{
					base.Comm.Skip(4L);
					i += 4;
					int tdsShort2 = (int)base.Comm.GetTdsShort();
					i += 2;
					text = base.Comm.GetString(tdsShort2);
					i += tdsShort2;
					num = int.MinValue;
				}
				else if (@byte == TdsColumnType.Decimal || @byte == TdsColumnType.Numeric)
				{
					num = (int)base.Comm.GetByte();
					i++;
					b2 = base.Comm.GetByte();
					i++;
					b = base.Comm.GetByte();
					i++;
				}
				else if (Tds.IsFixedSizeColumn(@byte))
				{
					num = Tds.LookupBufferSize(@byte);
				}
				else
				{
					num = (int)(base.Comm.GetByte() & byte.MaxValue);
					i++;
				}
				TdsDataColumn tdsDataColumn = new TdsDataColumn();
				int num2 = base.Columns.Add(tdsDataColumn);
				tdsDataColumn.ColumnType = new TdsColumnType?(@byte);
				tdsDataColumn.ColumnSize = new int?(num);
				tdsDataColumn.ColumnName = base.ColumnNames[num2] as string;
				tdsDataColumn.NumericPrecision = new short?((short)b2);
				tdsDataColumn.NumericScale = new short?((short)b);
				tdsDataColumn.IsReadOnly = new bool?(!flag2);
				tdsDataColumn.BaseTableName = text;
				tdsDataColumn.AllowDBNull = new bool?(flag);
			}
		}

		public static readonly TdsVersion Version = TdsVersion.tds42;
	}
}
