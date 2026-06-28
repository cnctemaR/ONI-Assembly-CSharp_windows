using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using FileHelpers.Events;

namespace FileHelpers
{
	[DebuggerDisplay("FileHelperAsyncEngine for type: {RecordType.Name}. ErrorMode: {ErrorManager.ErrorMode.ToString()}. Encoding: {Encoding.EncodingName}")]
	public class FileHelperAsyncEngine<T> : EventEngineBase<T>, IFileHelperAsyncEngine<T>, IEnumerable<T>, IEnumerable, IDisposable where T : class
	{
		public FileHelperAsyncEngine()
			: base(typeof(T))
		{
		}

		protected FileHelperAsyncEngine(Type recordType)
			: base(recordType)
		{
		}

		public FileHelperAsyncEngine(Encoding encoding)
			: base(typeof(T), encoding)
		{
		}

		protected FileHelperAsyncEngine(Type recordType, Encoding encoding)
			: base(recordType, encoding)
		{
		}

		public T LastRecord
		{
			get
			{
				return this.mLastRecord;
			}
		}

		public object[] LastRecordValues
		{
			get
			{
				return this.mLastRecordValues;
			}
		}

		public object this[int fieldIndex]
		{
			get
			{
				if (this.mLastRecordValues == null)
				{
					throw new BadUsageException("You must be reading something to access this property. Try calling BeginReadFile first.");
				}
				return this.mLastRecordValues[fieldIndex];
			}
			set
			{
				if (this.mAsyncWriter == null)
				{
					throw new BadUsageException("You must be writing something to set a record value. Try calling BeginWriteFile first.");
				}
				if (this.mLastRecordValues == null)
				{
					this.mLastRecordValues = new object[base.RecordInfo.FieldCount];
				}
				if (value == null)
				{
					if (base.RecordInfo.Fields[fieldIndex].FieldType.IsValueType)
					{
						throw new BadUsageException("You can't assign null to a value type.");
					}
					this.mLastRecordValues[fieldIndex] = null;
					return;
				}
				else
				{
					if (!base.RecordInfo.Fields[fieldIndex].FieldType.IsInstanceOfType(value))
					{
						throw new BadUsageException(string.Format("Invalid type: {0}. Expected: {1}", value.GetType().Name, base.RecordInfo.Fields[fieldIndex].FieldType.Name));
					}
					this.mLastRecordValues[fieldIndex] = value;
					return;
				}
			}
		}

		public object this[string fieldName]
		{
			get
			{
				if (this.mLastRecordValues == null)
				{
					throw new BadUsageException("You must be reading something to access this property. Try calling BeginReadFile first.");
				}
				int fieldIndex = base.RecordInfo.GetFieldIndex(fieldName);
				return this.mLastRecordValues[fieldIndex];
			}
			set
			{
				int fieldIndex = base.RecordInfo.GetFieldIndex(fieldName);
				this[fieldIndex] = value;
			}
		}

		public IDisposable BeginReadStream(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader", "The TextReader can't be null.");
			}
			if (this.mAsyncWriter != null)
			{
				throw new BadUsageException("You can't start to read while you are writing.");
			}
			NewLineDelimitedRecordReader newLineDelimitedRecordReader = new NewLineDelimitedRecordReader(reader);
			base.ResetFields();
			this.mHeaderText = string.Empty;
			this.mFooterText = string.Empty;
			if (base.RecordInfo.IgnoreFirst > 0)
			{
				for (int i = 0; i < base.RecordInfo.IgnoreFirst; i++)
				{
					string text = newLineDelimitedRecordReader.ReadRecordString();
					this.mLineNumber++;
					if (text == null)
					{
						break;
					}
					this.mHeaderText = this.mHeaderText + text + StringHelper.NewLine;
				}
			}
			this.mAsyncReader = new ForwardReader(newLineDelimitedRecordReader, base.RecordInfo.IgnoreLast, this.mLineNumber)
			{
				DiscardForward = true
			};
			this.State = FileHelperAsyncEngine<T>.EngineState.Reading;
			this.mStreamInfo = new StreamInfoProvider(reader);
			this.mCurrentRecord = 0;
			if (base.MustNotifyProgress)
			{
				base.OnProgress(new ProgressEventArgs(0, -1, this.mStreamInfo.Position, this.mStreamInfo.TotalBytes));
			}
			return this;
		}

		public IDisposable BeginReadFile(string fileName)
		{
			this.BeginReadFile(fileName, 102400);
			return this;
		}

		public IDisposable BeginReadFile(string fileName, int bufferSize)
		{
			this.BeginReadStream(new InternalStreamReader(fileName, this.mEncoding, true, bufferSize));
			return this;
		}

		public IDisposable BeginReadString(string sourceData)
		{
			if (sourceData == null)
			{
				sourceData = string.Empty;
			}
			this.BeginReadStream(new InternalStringReader(sourceData));
			return this;
		}

		public T ReadNext()
		{
			if (this.mAsyncReader == null)
			{
				throw new BadUsageException("Before call ReadNext you must call BeginReadFile or BeginReadStream.");
			}
			this.ReadNextRecord();
			return this.mLastRecord;
		}

		private void ReadNextRecord()
		{
			string text = this.mAsyncReader.ReadNextLine();
			this.mLineNumber++;
			bool flag = false;
			this.mLastRecord = default(T);
			LineInfo lineInfo = new LineInfo(string.Empty)
			{
				mReader = this.mAsyncReader
			};
			if (this.mLastRecordValues == null)
			{
				this.mLastRecordValues = new object[base.RecordInfo.FieldCount];
			}
			while (text != null)
			{
				try
				{
					try
					{
						this.mTotalRecords++;
						this.mCurrentRecord++;
						lineInfo.ReLoad(text);
						bool flag2 = false;
						this.mLastRecord = (T)((object)base.RecordInfo.Operations.CreateRecordHandler());
						if (base.MustNotifyProgress)
						{
							base.OnProgress(new ProgressEventArgs(this.mCurrentRecord, -1, this.mStreamInfo.Position, this.mStreamInfo.TotalBytes));
						}
						BeforeReadEventArgs<T> beforeReadEventArgs = null;
						if (base.MustNotifyRead)
						{
							beforeReadEventArgs = new BeforeReadEventArgs<T>(this, this.mLastRecord, text, base.LineNumber);
							flag2 = base.OnBeforeReadRecord(beforeReadEventArgs);
							if (beforeReadEventArgs.RecordLineChanged)
							{
								lineInfo.ReLoad(beforeReadEventArgs.RecordLine);
							}
						}
						if (!flag2 && base.RecordInfo.Operations.StringToRecord(this.mLastRecord, lineInfo, this.mLastRecordValues))
						{
							if (base.MustNotifyRead)
							{
								flag2 = base.OnAfterReadRecord(text, this.mLastRecord, beforeReadEventArgs.RecordLineChanged, base.LineNumber);
							}
							if (!flag2)
							{
								flag = true;
								return;
							}
						}
					}
					catch (Exception ex)
					{
						switch (this.mErrorManager.ErrorMode)
						{
						case ErrorMode.ThrowException:
							flag = true;
							throw;
						case ErrorMode.SaveAndContinue:
						{
							ErrorInfo errorInfo = new ErrorInfo
							{
								mLineNumber = this.mAsyncReader.LineNumber,
								mExceptionInfo = ex,
								mRecordString = text
							};
							this.mErrorManager.AddError(errorInfo);
							break;
						}
						}
					}
					continue;
				}
				finally
				{
					if (!flag)
					{
						text = this.mAsyncReader.ReadNextLine();
						this.mLineNumber = this.mAsyncReader.LineNumber;
					}
				}
				break;
			}
			this.mLastRecordValues = null;
			this.mLastRecord = default(T);
			if (base.RecordInfo.IgnoreLast > 0)
			{
				this.mFooterText = this.mAsyncReader.RemainingText;
			}
			try
			{
				this.mAsyncReader.Close();
			}
			catch
			{
			}
		}

		public T[] ReadToEnd()
		{
			return this.ReadNexts(int.MaxValue);
		}

		public T[] ReadNexts(int numberOfRecords)
		{
			if (this.mAsyncReader == null)
			{
				throw new BadUsageException("Before call ReadNext you must call BeginReadFile or BeginReadStream.");
			}
			List<T> list = new List<T>(numberOfRecords);
			for (int i = 0; i < numberOfRecords; i++)
			{
				this.ReadNextRecord();
				if (this.mLastRecord == null)
				{
					break;
				}
				list.Add(this.mLastRecord);
			}
			return list.ToArray();
		}

		public void Flush()
		{
			if (this.mAsyncWriter != null)
			{
				this.mAsyncWriter.Flush();
			}
		}

		public void Close()
		{
			lock (this)
			{
				this.State = FileHelperAsyncEngine<T>.EngineState.Closed;
				try
				{
					this.mLastRecordValues = null;
					this.mLastRecord = default(T);
					ForwardReader forwardReader = this.mAsyncReader;
					if (forwardReader != null)
					{
						forwardReader.Close();
						this.mAsyncReader = null;
					}
				}
				catch
				{
				}
				try
				{
					TextWriter textWriter = this.mAsyncWriter;
					if (textWriter != null)
					{
						if (!string.IsNullOrEmpty(this.mFooterText))
						{
							if (this.mFooterText.EndsWith(StringHelper.NewLine))
							{
								textWriter.Write(this.mFooterText);
							}
							else
							{
								textWriter.WriteLine(this.mFooterText);
							}
						}
						textWriter.Close();
						this.mAsyncWriter = null;
					}
				}
				catch
				{
				}
			}
		}

		public IDisposable BeginWriteStream(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentException("writer", "The TextWriter can't be null.");
			}
			if (this.mAsyncReader != null)
			{
				throw new BadUsageException("You can't start to write while you are reading.");
			}
			this.State = FileHelperAsyncEngine<T>.EngineState.Writing;
			base.ResetFields();
			this.mAsyncWriter = writer;
			this.WriteHeader();
			this.mStreamInfo = new StreamInfoProvider(this.mAsyncWriter);
			this.mCurrentRecord = 0;
			if (base.MustNotifyProgress)
			{
				base.OnProgress(new ProgressEventArgs(0, -1, this.mStreamInfo.Position, this.mStreamInfo.TotalBytes));
			}
			return this;
		}

		private void WriteHeader()
		{
			if (!string.IsNullOrEmpty(this.mHeaderText))
			{
				if (this.mHeaderText.EndsWith(StringHelper.NewLine))
				{
					this.mAsyncWriter.Write(this.mHeaderText);
					return;
				}
				this.mAsyncWriter.WriteLine(this.mHeaderText);
			}
		}

		public IDisposable BeginWriteFile(string fileName)
		{
			return this.BeginWriteFile(fileName, 102400);
		}

		public IDisposable BeginWriteFile(string fileName, int bufferSize)
		{
			this.BeginWriteStream(new StreamWriter(fileName, false, this.mEncoding, bufferSize));
			return this;
		}

		public IDisposable BeginAppendToFile(string fileName)
		{
			return this.BeginAppendToFile(fileName, 102400);
		}

		public IDisposable BeginAppendToFile(string fileName, int bufferSize)
		{
			if (this.mAsyncReader != null)
			{
				throw new BadUsageException("You can't start to write while you are reading.");
			}
			this.mAsyncWriter = StreamHelper.CreateFileAppender(fileName, this.mEncoding, false, true, bufferSize);
			this.mHeaderText = string.Empty;
			this.mFooterText = string.Empty;
			this.State = FileHelperAsyncEngine<T>.EngineState.Writing;
			this.mStreamInfo = new StreamInfoProvider(this.mAsyncWriter);
			this.mCurrentRecord = 0;
			if (base.MustNotifyProgress)
			{
				base.OnProgress(new ProgressEventArgs(0, -1, this.mStreamInfo.Position, this.mStreamInfo.TotalBytes));
			}
			return this;
		}

		public void WriteNext(T record)
		{
			if (this.mAsyncWriter == null)
			{
				throw new BadUsageException("Before call WriteNext you must call BeginWriteFile or BeginWriteStream.");
			}
			if (record == null)
			{
				throw new BadUsageException("The record to write can't be null.");
			}
			if (!base.RecordType.IsAssignableFrom(record.GetType()))
			{
				throw new BadUsageException("The record must be of type: " + base.RecordType.Name);
			}
			this.WriteRecord(record);
		}

		private void WriteRecord(T record)
		{
			string text = null;
			try
			{
				this.mLineNumber++;
				this.mTotalRecords++;
				this.mCurrentRecord++;
				bool flag = false;
				if (base.MustNotifyProgress)
				{
					base.OnProgress(new ProgressEventArgs(this.mCurrentRecord, -1, this.mStreamInfo.Position, this.mStreamInfo.TotalBytes));
				}
				if (base.MustNotifyWrite)
				{
					flag = base.OnBeforeWriteRecord(record, base.LineNumber);
				}
				if (!flag)
				{
					text = base.RecordInfo.Operations.RecordToString(record);
					if (base.MustNotifyWrite)
					{
						text = base.OnAfterWriteRecord(text, record);
					}
					this.mAsyncWriter.WriteLine(text);
				}
			}
			catch (Exception ex)
			{
				switch (this.mErrorManager.ErrorMode)
				{
				case ErrorMode.ThrowException:
					throw;
				case ErrorMode.SaveAndContinue:
				{
					ErrorInfo errorInfo = new ErrorInfo
					{
						mLineNumber = this.mLineNumber,
						mExceptionInfo = ex,
						mRecordString = text
					};
					this.mErrorManager.AddError(errorInfo);
					break;
				}
				}
			}
		}

		public void WriteNexts(IEnumerable<T> records)
		{
			if (this.mAsyncWriter == null)
			{
				throw new BadUsageException("Before call WriteNext you must call BeginWriteFile or BeginWriteStream.");
			}
			if (records == null)
			{
				throw new ArgumentNullException("records", "The record to write can't be null.");
			}
			bool flag = true;
			foreach (T t in records)
			{
				if (flag)
				{
					if (!base.RecordType.IsAssignableFrom(t.GetType()))
					{
						throw new BadUsageException("The record must be of type: " + base.RecordType.Name);
					}
					flag = false;
				}
				this.WriteRecord(t);
			}
		}

		public void WriteNextValues()
		{
			if (this.mAsyncWriter == null)
			{
				throw new BadUsageException("Before call WriteNext you must call BeginWriteFile or BeginWriteStream.");
			}
			if (this.mLastRecordValues == null)
			{
				throw new BadUsageException("You must set some values of the record before call this method, or use the overload that has a record as argument.");
			}
			string text = null;
			try
			{
				this.mLineNumber++;
				this.mTotalRecords++;
				text = base.RecordInfo.Operations.RecordValuesToString(this.mLastRecordValues);
				this.mAsyncWriter.WriteLine(text);
			}
			catch (Exception ex)
			{
				switch (this.mErrorManager.ErrorMode)
				{
				case ErrorMode.ThrowException:
					throw;
				case ErrorMode.SaveAndContinue:
				{
					ErrorInfo errorInfo = new ErrorInfo
					{
						mLineNumber = this.mLineNumber,
						mExceptionInfo = ex,
						mRecordString = text
					};
					this.mErrorManager.AddError(errorInfo);
					break;
				}
				}
			}
			finally
			{
				this.mLastRecordValues = null;
			}
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			if (this.mAsyncReader == null)
			{
				throw new FileHelpersException("You must call BeginRead before use the engine in a for each loop.");
			}
			return new FileHelperAsyncEngine<T>.AsyncEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (this.mAsyncReader == null)
			{
				throw new FileHelpersException("You must call BeginRead before use the engine in a for each loop.");
			}
			return new FileHelperAsyncEngine<T>.AsyncEnumerator(this);
		}

		void IDisposable.Dispose()
		{
			this.Close();
			GC.SuppressFinalize(this);
		}

		~FileHelperAsyncEngine()
		{
			this.Close();
		}

		private FileHelperAsyncEngine<T>.EngineState State { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ForwardReader mAsyncReader;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TextWriter mAsyncWriter;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private T mLastRecord;

		private object[] mLastRecordValues;

		private int mCurrentRecord;

		private StreamInfoProvider mStreamInfo;

		private class AsyncEnumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			T IEnumerator<T>.Current
			{
				get
				{
					return this.mEngine.mLastRecord;
				}
			}

			void IDisposable.Dispose()
			{
				this.mEngine.Close();
			}

			public AsyncEnumerator(FileHelperAsyncEngine<T> engine)
			{
				this.mEngine = engine;
			}

			public bool MoveNext()
			{
				if (this.mEngine.ReadNext() == null)
				{
					this.mEngine.Close();
					return false;
				}
				return true;
			}

			public object Current
			{
				get
				{
					return this.mEngine.mLastRecord;
				}
			}

			public void Reset()
			{
			}

			private readonly FileHelperAsyncEngine<T> mEngine;
		}

		private enum EngineState
		{
			Closed,
			Reading,
			Writing
		}
	}
}
