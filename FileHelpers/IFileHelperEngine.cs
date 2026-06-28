using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using FileHelpers.Events;
using FileHelpers.Options;

namespace FileHelpers
{
	public interface IFileHelperEngine<T> where T : class
	{
		T[] ReadFile(string fileName);

		T[] ReadFile(string fileName, int maxRecords);

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		T[] ReadStream(TextReader reader);

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		T[] ReadStream(TextReader reader, int maxRecords);

		T[] ReadString(string source);

		T[] ReadString(string source, int maxRecords);

		void WriteFile(string fileName, IEnumerable<T> records);

		void WriteFile(string fileName, IEnumerable<T> records, int maxRecords);

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		void WriteStream(TextWriter writer, IEnumerable<T> records);

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		void WriteStream(TextWriter writer, IEnumerable<T> records, int maxRecords);

		string WriteString(IEnumerable<T> records);

		string WriteString(IEnumerable<T> records, int maxRecords);

		void AppendToFile(string fileName, T record);

		void AppendToFile(string fileName, IEnumerable<T> records);

		DataTable ReadFileAsDT(string fileName);

		DataTable ReadFileAsDT(string fileName, int maxRecords);

		DataTable ReadStringAsDT(string source);

		DataTable ReadStringAsDT(string source, int maxRecords);

		DataTable ReadStreamAsDT(TextReader reader);

		DataTable ReadStreamAsDT(TextReader reader, int maxRecords);

		event BeforeReadHandler<T> BeforeReadRecord;

		event AfterReadHandler<T> AfterReadRecord;

		event BeforeWriteHandler<T> BeforeWriteRecord;

		event AfterWriteHandler<T> AfterWriteRecord;

		RecordOptions Options { get; }

		int LineNumber { get; }

		int TotalRecords { get; }

		Type RecordType { get; }

		string HeaderText { get; set; }

		string FooterText { get; set; }

		Encoding Encoding { get; set; }

		ErrorManager ErrorManager { get; }

		ErrorMode ErrorMode { get; set; }

		event EventHandler<ProgressEventArgs> Progress;
	}
}
