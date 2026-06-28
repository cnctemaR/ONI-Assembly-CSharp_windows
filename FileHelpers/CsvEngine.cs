using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using FileHelpers.Dynamic;
using FileHelpers.Options;

namespace FileHelpers
{
	[DebuggerDisplay("CsvEngine. ErrorMode: {ErrorManager.ErrorMode.ToString()}. Encoding: {Encoding.EncodingName}")]
	public sealed class CsvEngine : FileHelperEngine
	{
		public static DataTable CsvToDataTable(string filename, char delimiter)
		{
			return CsvEngine.CsvToDataTable(filename, "RecorMappingClass", delimiter, true);
		}

		public static DataTable CsvToDataTable(string filename, string classname, char delimiter)
		{
			return CsvEngine.CsvToDataTable(filename, classname, delimiter, true);
		}

		public static DataTable CsvToDataTable(string filename, string classname, char delimiter, bool hasHeader)
		{
			CsvOptions csvOptions = new CsvOptions(classname, delimiter, filename);
			if (!hasHeader)
			{
				csvOptions.HeaderLines = 0;
			}
			return CsvEngine.CsvToDataTable(filename, csvOptions);
		}

		public static DataTable CsvToDataTable(string filename, string classname, char delimiter, bool hasHeader, bool ignoreEmptyLines)
		{
			CsvOptions csvOptions = new CsvOptions(classname, delimiter, filename);
			if (!hasHeader)
			{
				csvOptions.HeaderLines = 0;
			}
			csvOptions.IgnoreEmptyLines = ignoreEmptyLines;
			return CsvEngine.CsvToDataTable(filename, csvOptions);
		}

		public static DataTable CsvToDataTable(string filename, CsvOptions options)
		{
			CsvEngine csvEngine = new CsvEngine(options);
			return csvEngine.ReadFileAsDT(filename);
		}

		public static void DataTableToCsv(DataTable dt, string filename)
		{
			CsvEngine.DataTableToCsv(dt, filename, new CsvOptions("Tempo1", ',', dt.Columns.Count));
		}

		public static void DataTableToCsv(DataTable dt, string filename, char delimiter)
		{
			CsvEngine.DataTableToCsv(dt, filename, new CsvOptions("Tempo1", delimiter, dt.Columns.Count));
		}

		public static void DataTableToCsv(DataTable dt, string filename, CsvOptions options)
		{
			using (StreamWriter streamWriter = new StreamWriter(filename, false, options.Encoding, 102400))
			{
				foreach (object obj in dt.Rows)
				{
					DataRow dataRow = (DataRow)obj;
					object[] itemArray = dataRow.ItemArray;
					for (int i = 0; i < itemArray.Length; i++)
					{
						if (i > 0)
						{
							streamWriter.Write(options.Delimiter);
						}
						streamWriter.Write(options.ValueToString(itemArray[i]));
					}
					streamWriter.Write(StringHelper.NewLine);
				}
				streamWriter.Close();
			}
		}

		public CsvEngine(string className, char delimiter, string sampleFile)
			: this(new CsvOptions(className, delimiter, sampleFile))
		{
		}

		public CsvEngine(string className, char delimiter, int numberOfFields)
			: this(new CsvOptions(className, delimiter, numberOfFields))
		{
		}

		public CsvEngine(CsvOptions options)
			: base(CsvEngine.GetMappingClass(options))
		{
		}

		private static Type GetMappingClass(CsvOptions options)
		{
			CsvClassBuilder csvClassBuilder = new CsvClassBuilder(options);
			return csvClassBuilder.CreateRecordClass();
		}
	}
}
