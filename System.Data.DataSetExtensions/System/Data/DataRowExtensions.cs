using System;

namespace System.Data
{
	public static class DataRowExtensions
	{
		public static T Field<T>(this DataRow row, string columnName)
		{
			DataSetUtil.CheckArgumentNull<DataRow>(row, "row");
			return DataRowExtensions.UnboxT<T>.s_unbox(row[columnName]);
		}

		public static T Field<T>(this DataRow row, DataColumn column)
		{
			DataSetUtil.CheckArgumentNull<DataRow>(row, "row");
			return DataRowExtensions.UnboxT<T>.s_unbox(row[column]);
		}

		public static T Field<T>(this DataRow row, int columnIndex)
		{
			DataSetUtil.CheckArgumentNull<DataRow>(row, "row");
			return DataRowExtensions.UnboxT<T>.s_unbox(row[columnIndex]);
		}

		public static T Field<T>(this DataRow row, int columnIndex, DataRowVersion version)
		{
			DataSetUtil.CheckArgumentNull<DataRow>(row, "row");
			return DataRowExtensions.UnboxT<T>.s_unbox(row[columnIndex, version]);
		}

		public static T Field<T>(this DataRow row, string columnName, DataRowVersion version)
		{
			DataSetUtil.CheckArgumentNull<DataRow>(row, "row");
			return DataRowExtensions.UnboxT<T>.s_unbox(row[columnName, version]);
		}

		public static T Field<T>(this DataRow row, DataColumn column, DataRowVersion version)
		{
			DataSetUtil.CheckArgumentNull<DataRow>(row, "row");
			return DataRowExtensions.UnboxT<T>.s_unbox(row[column, version]);
		}

		public static void SetField<T>(this DataRow row, int columnIndex, T value)
		{
			DataSetUtil.CheckArgumentNull<DataRow>(row, "row");
			row[columnIndex] = value ?? DBNull.Value;
		}

		public static void SetField<T>(this DataRow row, string columnName, T value)
		{
			DataSetUtil.CheckArgumentNull<DataRow>(row, "row");
			row[columnName] = value ?? DBNull.Value;
		}

		public static void SetField<T>(this DataRow row, DataColumn column, T value)
		{
			DataSetUtil.CheckArgumentNull<DataRow>(row, "row");
			row[column] = value ?? DBNull.Value;
		}

		private static class UnboxT<T>
		{
			private static Converter<object, T> Create()
			{
				if (default(T) == null)
				{
					return new Converter<object, T>(DataRowExtensions.UnboxT<T>.ReferenceOrNullableField);
				}
				return new Converter<object, T>(DataRowExtensions.UnboxT<T>.ValueField);
			}

			private static T ReferenceOrNullableField(object value)
			{
				if (DBNull.Value != value)
				{
					return (T)((object)value);
				}
				return default(T);
			}

			private static T ValueField(object value)
			{
				if (DBNull.Value == value)
				{
					throw DataSetUtil.InvalidCast(string.Format("Cannot cast DBNull. Value to type '{0}'. Please use a nullable type.", typeof(T).ToString()));
				}
				return (T)((object)value);
			}

			internal static readonly Converter<object, T> s_unbox = DataRowExtensions.UnboxT<T>.Create();
		}
	}
}
