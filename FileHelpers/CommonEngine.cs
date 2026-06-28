using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using FileHelpers.Options;

namespace FileHelpers
{
	public static class CommonEngine
	{
		public static object[] ReadFile(Type recordClass, string fileName)
		{
			return CommonEngine.ReadFile(recordClass, fileName, int.MaxValue);
		}

		public static object[] ReadFile(Type recordClass, string fileName, int maxRecords)
		{
			FileHelperEngine fileHelperEngine = new FileHelperEngine(recordClass);
			return fileHelperEngine.ReadFile(fileName, maxRecords);
		}

		public static DataTable ReadFileAsDT(Type recordClass, string fileName)
		{
			return CommonEngine.ReadFileAsDT(recordClass, fileName, -1);
		}

		public static DataTable ReadFileAsDT(Type recordClass, string fileName, int maxRecords)
		{
			FileHelperEngine fileHelperEngine = new FileHelperEngine(recordClass);
			return fileHelperEngine.ReadFileAsDT(fileName, maxRecords);
		}

		public static T[] ReadFile<T>(string fileName) where T : class
		{
			return CommonEngine.ReadFile<T>(fileName, int.MaxValue);
		}

		public static T[] ReadFile<T>(string fileName, int maxRecords) where T : class
		{
			FileHelperEngine<T> fileHelperEngine = new FileHelperEngine<T>();
			return fileHelperEngine.ReadFile(fileName, maxRecords);
		}

		public static object[] ReadString(Type recordClass, string input)
		{
			return CommonEngine.ReadString(recordClass, input, -1);
		}

		public static object[] ReadString(Type recordClass, string input, int maxRecords)
		{
			FileHelperEngine fileHelperEngine = new FileHelperEngine(recordClass);
			return fileHelperEngine.ReadString(input, maxRecords);
		}

		public static T[] ReadString<T>(string input) where T : class
		{
			FileHelperEngine<T> fileHelperEngine = new FileHelperEngine<T>();
			return fileHelperEngine.ReadString(input);
		}

		public static void WriteFile<T>(string fileName, IEnumerable<T> records) where T : class
		{
			FileHelperEngine<T> fileHelperEngine = new FileHelperEngine<T>();
			fileHelperEngine.WriteFile(fileName, records);
		}

		public static string WriteString<T>(IEnumerable<T> records) where T : class
		{
			FileHelperEngine<T> fileHelperEngine = new FileHelperEngine<T>();
			return fileHelperEngine.WriteString(records);
		}

		public static int TransformFileFast<TSource, TDest>(string sourceFile, string destFile) where TSource : class, ITransformable<TDest> where TDest : class
		{
			FileTransformEngine<TSource, TDest> fileTransformEngine = new FileTransformEngine<TSource, TDest>();
			return fileTransformEngine.TransformFileFast(sourceFile, destFile);
		}

		public static object[] TransformFile<TSource, TDest>(string sourceFile, string destFile) where TSource : class, ITransformable<TDest> where TDest : class
		{
			FileTransformEngine<TSource, TDest> fileTransformEngine = new FileTransformEngine<TSource, TDest>();
			return (object[])fileTransformEngine.TransformFile(sourceFile, destFile);
		}

		public static object[] ReadSortedFile(Type recordClass, string fileName)
		{
			if (!typeof(IComparable).IsAssignableFrom(recordClass))
			{
				throw new BadUsageException("The record class must implement the interface IComparable to use the Sort feature.");
			}
			FileHelperEngine fileHelperEngine = new FileHelperEngine(recordClass);
			object[] array = fileHelperEngine.ReadFile(fileName);
			if (array.Length == 0)
			{
				return array;
			}
			Array.Sort<object>(array);
			return array;
		}

		public static void SortFile(Type recordClass, string sourceFile, string sortedFile)
		{
			if (!typeof(IComparable).IsAssignableFrom(recordClass))
			{
				throw new BadUsageException("The record class must implement the interface IComparable to use the Sort feature.");
			}
			FileHelperEngine fileHelperEngine = new FileHelperEngine(recordClass);
			object[] array = fileHelperEngine.ReadFile(sourceFile);
			if (array.Length == 0)
			{
				fileHelperEngine.WriteFile(sortedFile, array);
			}
			Array.Sort<object>(array);
			fileHelperEngine.WriteFile(sortedFile, array);
		}

		public static void SortFileByField(Type recordClass, string fieldName, bool asc, string sourceFile, string sortedFile)
		{
			FileHelperEngine fileHelperEngine = new FileHelperEngine(recordClass);
			FieldInfo fieldInfo = fileHelperEngine.RecordInfo.GetFieldInfo(fieldName);
			if (fieldInfo == null)
			{
				throw new BadUsageException("The record class not contains the field " + fieldName);
			}
			object[] array = fileHelperEngine.ReadFile(sourceFile);
			IComparer comparer = new CommonEngine.FieldComparer(fieldInfo, asc);
			Array.Sort(array, comparer);
			fileHelperEngine.WriteFile(sortedFile, array);
		}

		public static void SortRecordsByField(object[] records, string fieldName)
		{
			CommonEngine.SortRecordsByField(records, fieldName, true);
		}

		public static void SortRecordsByField(object[] records, string fieldName, bool ascending)
		{
			if (records.Length > 0 && records[0] != null)
			{
				FileHelperEngine fileHelperEngine = new FileHelperEngine(records[0].GetType());
				FieldInfo fieldInfo = fileHelperEngine.RecordInfo.GetFieldInfo(fieldName);
				if (fieldInfo == null)
				{
					throw new BadUsageException("The record class not contains the field " + fieldName);
				}
				IComparer comparer = new CommonEngine.FieldComparer(fieldInfo, ascending);
				Array.Sort(records, comparer);
			}
		}

		public static void SortRecords(object[] records)
		{
			if (records.Length > 0 && records[0] != null)
			{
				Type type = records[0].GetType();
				if (!typeof(IComparable).IsAssignableFrom(type))
				{
					throw new BadUsageException("The record class must implement the interface IComparable to use the Sort feature.");
				}
				Array.Sort<object>(records);
			}
		}

		public static DataTable RecordsToDataTable(ICollection records)
		{
			return CommonEngine.RecordsToDataTable(records, -1);
		}

		public static DataTable RecordsToDataTable(ICollection records, int maxRecords)
		{
			IRecordInfo recordInfo = null;
			foreach (object obj in records)
			{
				if (obj != null)
				{
					recordInfo = RecordInfo.Resolve(obj.GetType());
					break;
				}
			}
			if (recordInfo == null)
			{
				return new DataTable();
			}
			return recordInfo.Operations.RecordsToDataTable(records, maxRecords);
		}

		public static DataTable RecordsToDataTable(ICollection records, Type recordType)
		{
			return CommonEngine.RecordsToDataTable(records, recordType, -1);
		}

		public static DataTable RecordsToDataTable(ICollection records, Type recordType, int maxRecords)
		{
			IRecordInfo recordInfo = RecordInfo.Resolve(recordType);
			return recordInfo.Operations.RecordsToDataTable(records, maxRecords);
		}

		public static void MergeFiles(Type recordType, string file1, string file2, string destinationFile)
		{
			using (FileHelperAsyncEngine fileHelperAsyncEngine = new FileHelperAsyncEngine(recordType))
			{
				using (FileHelperAsyncEngine fileHelperAsyncEngine2 = new FileHelperAsyncEngine(recordType))
				{
					fileHelperAsyncEngine2.BeginWriteFile(destinationFile);
					fileHelperAsyncEngine.BeginReadFile(file1);
					object[] array = fileHelperAsyncEngine.ReadNexts(50);
					while (array.Length > 0)
					{
						fileHelperAsyncEngine2.WriteNexts(array);
						array = fileHelperAsyncEngine.ReadNexts(50);
					}
					fileHelperAsyncEngine.Close();
					fileHelperAsyncEngine.BeginReadFile(file2);
					array = fileHelperAsyncEngine.ReadNexts(50);
					while (array.Length > 0)
					{
						fileHelperAsyncEngine2.WriteNexts(array);
						array = fileHelperAsyncEngine.ReadNexts(50);
					}
				}
			}
		}

		public static object[] MergeAndSortFile(Type recordType, string file1, string file2, string destFile, string field)
		{
			return CommonEngine.MergeAndSortFile(recordType, file1, file2, destFile, field, true);
		}

		public static object[] MergeAndSortFile(Type recordType, string file1, string file2, string destFile, string field, bool ascending)
		{
			FileHelperEngine fileHelperEngine = new FileHelperEngine(recordType);
			List<object> list = fileHelperEngine.ReadFileAsList(file1);
			list.AddRange(fileHelperEngine.ReadFileAsList(file2));
			object[] array = list.ToArray();
			CommonEngine.SortRecordsByField(array, field, ascending);
			fileHelperEngine.WriteFile(destFile, array);
			return array;
		}

		public static object[] MergeAndSortFile(Type recordType, string file1, string file2, string destFile)
		{
			FileHelperEngine fileHelperEngine = new FileHelperEngine(recordType);
			List<object> list = fileHelperEngine.ReadFileAsList(file1);
			list.AddRange(fileHelperEngine.ReadFileAsList(file2));
			object[] array = list.ToArray();
			CommonEngine.SortRecords(array);
			fileHelperEngine.WriteFile(destFile, array);
			return array;
		}

		public static void DataTableToCsv(DataTable dt, string filename)
		{
			CsvEngine.DataTableToCsv(dt, filename);
		}

		public static void DataTableToCsv(DataTable dt, string filename, char delimiter)
		{
			CsvEngine.DataTableToCsv(dt, filename, new CsvOptions("Tempo", delimiter, dt.Columns.Count));
		}

		public static void DataTableToCsv(DataTable dt, string filename, CsvOptions options)
		{
			CsvEngine.DataTableToCsv(dt, filename, options);
		}

		public static DataTable CsvToDataTable(string filename, char delimiter)
		{
			return CsvEngine.CsvToDataTable(filename, delimiter);
		}

		public static DataTable CsvToDataTable(string filename, string classname, char delimiter)
		{
			return CsvEngine.CsvToDataTable(filename, classname, delimiter);
		}

		public static DataTable CsvToDataTable(string filename, string classname, char delimiter, bool hasHeader)
		{
			return CsvEngine.CsvToDataTable(filename, classname, delimiter, hasHeader);
		}

		public static DataTable CsvToDataTable(string filename, CsvOptions options)
		{
			return CsvEngine.CsvToDataTable(filename, options);
		}

		public static T[] RemoveDuplicateRecords<T>(T[] arr) where T : IComparableRecord<T>
		{
			if (arr == null || arr.Length <= 1)
			{
				return arr;
			}
			List<T> list = new List<T>();
			for (int i = 0; i < arr.Length; i++)
			{
				bool flag = true;
				for (int j = i + 1; j < arr.Length; j++)
				{
					if (arr[i].IsEqualRecord(arr[j]))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					list.Add(arr[i]);
				}
			}
			return list.ToArray();
		}

		public static string RawReadFirstLines(string file, int lines)
		{
			StringBuilder stringBuilder = new StringBuilder(Math.Min(lines * 50, 10000));
			StreamReader streamReader = new StreamReader(file);
			for (int i = 0; i < lines; i++)
			{
				string text = streamReader.ReadLine();
				if (text == null)
				{
					break;
				}
				stringBuilder.Append(text + StringHelper.NewLine);
			}
			streamReader.Close();
			return stringBuilder.ToString();
		}

		public static string[] RawReadFirstLinesArray(string file, int lines)
		{
			return CommonEngine.RawReadFirstLinesArray(file, lines, Encoding.Default);
		}

		public static string[] RawReadFirstLinesArray(string file, int lines, Encoding encoding)
		{
			List<string> list = new List<string>(lines);
			using (StreamReader streamReader = new StreamReader(file, encoding))
			{
				for (int i = 0; i < lines; i++)
				{
					string text = streamReader.ReadLine();
					if (text == null)
					{
						break;
					}
					list.Add(text);
				}
			}
			return list.ToArray();
		}

		public static IEnumerable<RecordIndexer> ReadCsv(string filename)
		{
			return CommonEngine.ReadCsv(filename, ',');
		}

		public static IEnumerable<RecordIndexer> ReadCsv(string filename, char delimiter)
		{
			return CommonEngine.ReadCsv(filename, delimiter, 0);
		}

		public static IEnumerable<RecordIndexer> ReadCsv(string filename, char delimiter, int headerLines)
		{
			return CommonEngine.ReadCsv(filename, delimiter, headerLines, Encoding.Default);
		}

		public static IEnumerable<RecordIndexer> ReadCsv(string filename, char delimiter, Encoding encoding)
		{
			return CommonEngine.ReadCsv(filename, delimiter, 0, encoding);
		}

		public static IEnumerable<RecordIndexer> ReadCsv(string filename, char delimiter, int headerLines, Encoding encoding)
		{
			FileHelperAsyncEngine<RecordIndexer> fileHelperAsyncEngine = new FileHelperAsyncEngine<RecordIndexer>(encoding);
			((DelimitedRecordOptions)fileHelperAsyncEngine.Options).Delimiter = delimiter.ToString();
			fileHelperAsyncEngine.Options.IgnoreFirstLines = headerLines;
			fileHelperAsyncEngine.BeginReadFile(filename);
			return fileHelperAsyncEngine;
		}

		public static void SortBigFile<T>(string source, string destination) where T : class, IComparable<T>
		{
			BigFileSorter<T> bigFileSorter = new BigFileSorter<T>();
			bigFileSorter.Sort(source, destination);
		}

		public static void SortBigFile<T>(Encoding encoding, string source, string destination) where T : class, IComparable<T>
		{
			BigFileSorter<T> bigFileSorter = new BigFileSorter<T>(encoding);
			bigFileSorter.Sort(source, destination);
		}

		internal class FieldComparer : IComparer
		{
			public FieldComparer(FieldInfo fi, bool asc)
			{
				this.mFieldInfo = fi;
				this.mAscending = (asc ? 1 : (-1));
				if (!typeof(IComparable).IsAssignableFrom(this.mFieldInfo.FieldType))
				{
					throw new BadUsageException("The field " + this.mFieldInfo.Name + " needs to implement the interface IComparable");
				}
			}

			public int Compare(object x, object y)
			{
				IComparable comparable = this.mFieldInfo.GetValue(x) as IComparable;
				return comparable.CompareTo(this.mFieldInfo.GetValue(y)) * this.mAscending;
			}

			private readonly FieldInfo mFieldInfo;

			private readonly int mAscending;
		}
	}
}
