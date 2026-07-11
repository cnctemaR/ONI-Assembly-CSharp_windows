using System;
using System.Data.Common;
using System.Globalization;

namespace System.Data.Sql
{
	public sealed class SqlDataSourceEnumerator : DbDataSourceEnumerator
	{
		private SqlDataSourceEnumerator()
		{
		}

		public static SqlDataSourceEnumerator Instance
		{
			get
			{
				return SqlDataSourceEnumerator.SingletonInstance;
			}
		}

		public override DataTable GetDataSources()
		{
			this.timeoutTime = 0L;
			throw new NotImplementedException();
		}

		private static DataTable ParseServerEnumString(string serverInstances)
		{
			DataTable dataTable = new DataTable("SqlDataSources");
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add("ServerName", typeof(string));
			dataTable.Columns.Add("InstanceName", typeof(string));
			dataTable.Columns.Add("IsClustered", typeof(string));
			dataTable.Columns.Add("Version", typeof(string));
			string text = null;
			string text2 = null;
			string text3 = null;
			string text4 = null;
			string[] array = serverInstances.Split(new char[1]);
			for (int i = 0; i < array.Length; i++)
			{
				string text5 = array[i].Trim(new char[1]);
				if (text5.Length != 0)
				{
					foreach (string text6 in text5.Split(new char[] { ';' }))
					{
						if (text == null)
						{
							foreach (string text7 in text6.Split(new char[] { '\\' }))
							{
								if (text == null)
								{
									text = text7;
								}
								else
								{
									text2 = text7;
								}
							}
						}
						else if (text3 == null)
						{
							text3 = text6.Substring(SqlDataSourceEnumerator._clusterLength);
						}
						else
						{
							text4 = text6.Substring(SqlDataSourceEnumerator._versionLength);
						}
					}
					string text8 = "ServerName='" + text + "'";
					if (!ADP.IsEmpty(text2))
					{
						text8 = text8 + " AND InstanceName='" + text2 + "'";
					}
					if (dataTable.Select(text8).Length == 0)
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow[0] = text;
						dataRow[1] = text2;
						dataRow[2] = text3;
						dataRow[3] = text4;
						dataTable.Rows.Add(dataRow);
					}
					text = null;
					text2 = null;
					text3 = null;
					text4 = null;
				}
			}
			foreach (object obj in dataTable.Columns)
			{
				((DataColumn)obj).ReadOnly = true;
			}
			return dataTable;
		}

		private static readonly SqlDataSourceEnumerator SingletonInstance = new SqlDataSourceEnumerator();

		internal const string ServerName = "ServerName";

		internal const string InstanceName = "InstanceName";

		internal const string IsClustered = "IsClustered";

		internal const string Version = "Version";

		private long timeoutTime;

		private static string _Version = "Version:";

		private static string _Cluster = "Clustered:";

		private static int _clusterLength = SqlDataSourceEnumerator._Cluster.Length;

		private static int _versionLength = SqlDataSourceEnumerator._Version.Length;
	}
}
