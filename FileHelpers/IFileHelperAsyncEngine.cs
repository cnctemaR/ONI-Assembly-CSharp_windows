using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FileHelpers.Events;
using FileHelpers.Options;

namespace FileHelpers
{
	public interface IFileHelperAsyncEngine<T> : IEnumerable<T>, IEnumerable, IDisposable where T : class
	{
		T LastRecord { get; }

		object[] LastRecordValues { get; }

		RecordOptions Options { get; }

		int LineNumber { get; }

		int TotalRecords { get; }

		Type RecordType { get; }

		string HeaderText { get; set; }

		string FooterText { get; set; }

		Encoding Encoding { get; set; }

		ErrorManager ErrorManager { get; }

		ErrorMode ErrorMode { get; set; }

		IDisposable BeginReadStream(TextReader reader);

		IDisposable BeginReadFile(string fileName);

		IDisposable BeginReadString(string sourceData);

		T ReadNext();

		T[] ReadToEnd();

		T[] ReadNexts(int numberOfRecords);

		void Flush();

		void Close();

		IDisposable BeginWriteStream(TextWriter writer);

		IDisposable BeginWriteFile(string fileName);

		IDisposable BeginAppendToFile(string fileName);

		void WriteNext(T record);

		void WriteNexts(IEnumerable<T> records);

		void WriteNextValues();

		event BeforeReadHandler<T> BeforeReadRecord;

		event AfterReadHandler<T> AfterReadRecord;

		event BeforeWriteHandler<T> BeforeWriteRecord;

		event AfterWriteHandler<T> AfterWriteRecord;

		event EventHandler<ProgressEventArgs> Progress;
	}
}
