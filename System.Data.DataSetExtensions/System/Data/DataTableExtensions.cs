using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.Data
{
	public static class DataTableExtensions
	{
		public static EnumerableRowCollection<DataRow> AsEnumerable(this DataTable source)
		{
			DataSetUtil.CheckArgumentNull<DataTable>(source, "source");
			return new EnumerableRowCollection<DataRow>(source);
		}

		public static DataTable CopyToDataTable<T>(this IEnumerable<T> source) where T : DataRow
		{
			DataSetUtil.CheckArgumentNull<IEnumerable<T>>(source, "source");
			return DataTableExtensions.LoadTableFromEnumerable<T>(source, null, null, null);
		}

		public static void CopyToDataTable<T>(this IEnumerable<T> source, DataTable table, LoadOption options) where T : DataRow
		{
			DataSetUtil.CheckArgumentNull<IEnumerable<T>>(source, "source");
			DataSetUtil.CheckArgumentNull<DataTable>(table, "table");
			DataTableExtensions.LoadTableFromEnumerable<T>(source, table, new LoadOption?(options), null);
		}

		public static void CopyToDataTable<T>(this IEnumerable<T> source, DataTable table, LoadOption options, FillErrorEventHandler errorHandler) where T : DataRow
		{
			DataSetUtil.CheckArgumentNull<IEnumerable<T>>(source, "source");
			DataSetUtil.CheckArgumentNull<DataTable>(table, "table");
			DataTableExtensions.LoadTableFromEnumerable<T>(source, table, new LoadOption?(options), errorHandler);
		}

		private static DataTable LoadTableFromEnumerable<T>(IEnumerable<T> source, DataTable table, LoadOption? options, FillErrorEventHandler errorHandler) where T : DataRow
		{
			if (options != null)
			{
				LoadOption value = options.Value;
				if (value - LoadOption.OverwriteChanges > 2)
				{
					throw DataSetUtil.InvalidLoadOption(options.Value);
				}
			}
			using (IEnumerator<T> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					DataTable dataTable = table;
					if (dataTable == null)
					{
						throw DataSetUtil.InvalidOperation("The source contains no DataRows.");
					}
					return dataTable;
				}
				else
				{
					if (table == null)
					{
						DataRow dataRow = enumerator.Current;
						if (dataRow == null)
						{
							throw DataSetUtil.InvalidOperation("The source contains a DataRow reference that is null.");
						}
						table = new DataTable
						{
							Locale = CultureInfo.CurrentCulture
						};
						foreach (object obj in dataRow.Table.Columns)
						{
							DataColumn dataColumn = (DataColumn)obj;
							table.Columns.Add(dataColumn.ColumnName, dataColumn.DataType);
						}
					}
					table.BeginLoadData();
					try
					{
						do
						{
							DataRow dataRow = enumerator.Current;
							if (dataRow != null)
							{
								object[] array = null;
								try
								{
									DataRowState rowState = dataRow.RowState;
									switch (rowState)
									{
									case DataRowState.Detached:
										if (!dataRow.HasVersion(DataRowVersion.Proposed))
										{
											throw DataSetUtil.InvalidOperation("The source contains a detached DataRow that cannot be copied to the DataTable.");
										}
										break;
									case DataRowState.Unchanged:
									case DataRowState.Added:
										break;
									case DataRowState.Detached | DataRowState.Unchanged:
										goto IL_0172;
									default:
										if (rowState == DataRowState.Deleted)
										{
											throw DataSetUtil.InvalidOperation("The source contains a deleted DataRow that cannot be copied to the DataTable.");
										}
										if (rowState != DataRowState.Modified)
										{
											goto IL_0172;
										}
										break;
									}
									array = dataRow.ItemArray;
									if (options != null)
									{
										table.LoadDataRow(array, options.Value);
									}
									else
									{
										table.LoadDataRow(array, true);
									}
									goto IL_01DA;
									IL_0172:
									throw DataSetUtil.InvalidDataRowState(dataRow.RowState);
								}
								catch (Exception ex)
								{
									if (!DataSetUtil.IsCatchableExceptionType(ex))
									{
										throw;
									}
									FillErrorEventArgs e = null;
									if (errorHandler != null)
									{
										e = new FillErrorEventArgs(table, array)
										{
											Errors = ex
										};
										errorHandler(enumerator, e);
									}
									if (e == null)
									{
										throw;
									}
									if (!e.Continue)
									{
										if ((e.Errors ?? ex) == ex)
										{
											throw;
										}
										throw e.Errors;
									}
								}
							}
							IL_01DA:;
						}
						while (enumerator.MoveNext());
					}
					finally
					{
						table.EndLoadData();
					}
				}
			}
			return table;
		}

		public static DataView AsDataView(this DataTable table)
		{
			throw new PlatformNotSupportedException();
		}

		public static DataView AsDataView<T>(this EnumerableRowCollection<T> source) where T : DataRow
		{
			throw new PlatformNotSupportedException();
		}
	}
}
