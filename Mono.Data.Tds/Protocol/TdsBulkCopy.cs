using System;
using System.Collections;

namespace Mono.Data.Tds.Protocol
{
	public class TdsBulkCopy
	{
		public TdsBulkCopy(Tds tds)
		{
			this.tds = tds;
		}

		public bool SendColumnMetaData(string colMetaData)
		{
			this.tds.Comm.StartPacket(TdsPacketType.Query);
			this.tds.Comm.Append(colMetaData);
			this.tds.ExecBulkCopyMetaData(30, false);
			return true;
		}

		public bool BulkCopyStart(TdsMetaParameterCollection parameters)
		{
			this.tds.Comm.StartPacket(TdsPacketType.Bulk);
			this.tds.Comm.Append(129);
			short num = 0;
			foreach (object obj in ((IEnumerable)parameters))
			{
				TdsMetaParameter tdsMetaParameter = (TdsMetaParameter)obj;
				if (tdsMetaParameter.Value == null)
				{
					num += 1;
				}
			}
			this.tds.Comm.Append(num);
			if (parameters != null)
			{
				foreach (object obj2 in ((IEnumerable)parameters))
				{
					TdsMetaParameter tdsMetaParameter2 = (TdsMetaParameter)obj2;
					if (tdsMetaParameter2.Value == null)
					{
						this.tds.Comm.Append(0);
						this.tds.Comm.Append(10);
						this.WriteParameterInfo(tdsMetaParameter2);
						this.tds.Comm.Append((byte)tdsMetaParameter2.ParameterName.Length);
						this.tds.Comm.Append(tdsMetaParameter2.ParameterName);
					}
				}
			}
			return true;
		}

		public bool BulkCopyData(object o, int size, bool isNewRow)
		{
			if (isNewRow)
			{
				this.tds.Comm.Append(209);
			}
			if (size > 0)
			{
				this.tds.Comm.Append((short)size);
			}
			this.tds.Comm.Append(o);
			return true;
		}

		public bool BulkCopyEnd()
		{
			this.tds.Comm.Append(253);
			this.tds.ExecBulkCopy(30, false);
			return true;
		}

		private void WriteParameterInfo(TdsMetaParameter param)
		{
			param.IsNullable = true;
			TdsColumnType metaType = param.GetMetaType();
			param.IsNullable = false;
			this.tds.Comm.Append((byte)metaType);
			int num;
			if (param.Size == 0)
			{
				num = param.GetActualSize();
			}
			else
			{
				num = param.Size;
			}
			if (metaType == TdsColumnType.BigNVarChar)
			{
				num <<= 1;
			}
			if (this.tds.IsLargeType(metaType))
			{
				this.tds.Comm.Append((short)num);
			}
			else if (this.tds.IsBlobType(metaType))
			{
				this.tds.Comm.Append(num);
			}
			else
			{
				this.tds.Comm.Append((byte)num);
			}
			if (param.TypeName == "decimal" || param.TypeName == "numeric")
			{
				this.tds.Comm.Append((param.Precision == 0) ? 29 : param.Precision);
				this.tds.Comm.Append(param.Scale);
			}
		}

		private Tds tds;
	}
}
