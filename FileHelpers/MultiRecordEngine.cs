using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using FileHelpers.Events;

namespace FileHelpers
{
	[DebuggerDisplay("MultiRecordEngine for types: {ListTypes()}. ErrorMode: {ErrorManager.ErrorMode.ToString()}. Encoding: {Encoding.EncodingName}")]
	public sealed class MultiRecordEngine : EventEngineBase<object>, IEnumerable, IDisposable
	{
		private string ListTypes()
		{
			string text = string.Empty;
			bool flag = true;
			foreach (object obj in this.mRecordInfoHash.Keys)
			{
				Type type = (Type)obj;
				if (flag)
				{
					flag = false;
				}
				else
				{
					text += ", ";
				}
				text += type.Name;
			}
			return text;
		}

		public RecordTypeSelector RecordSelector
		{
			get
			{
				return this.mRecordSelector;
			}
			set
			{
				this.mRecordSelector = value;
			}
		}

		public MultiRecordEngine(params Type[] recordTypes)
			: this(null, recordTypes)
		{
		}

		public MultiRecordEngine(RecordTypeSelector recordSelector, params Type[] recordTypes)
			: base(MultiRecordEngine.GetFirstType(recordTypes))
		{
			this.mTypes = recordTypes;
			this.mMultiRecordInfo = new IRecordInfo[this.mTypes.Length];
			this.mRecordInfoHash = new Hashtable(this.mTypes.Length);
			for (int i = 0; i < this.mTypes.Length; i++)
			{
				if (this.mTypes[i] == null)
				{
					throw new BadUsageException("The type at index " + i.ToString() + " is null.");
				}
				if (this.mRecordInfoHash.Contains(this.mTypes[i]))
				{
					throw new BadUsageException("The type '" + this.mTypes[i].Name + " is already in the engine. You can't pass the same type twice to the constructor.");
				}
				this.mMultiRecordInfo[i] = FileHelpers.RecordInfo.Resolve(this.mTypes[i]);
				this.mRecordInfoHash.Add(this.mTypes[i], this.mMultiRecordInfo[i]);
			}
			this.mRecordSelector = recordSelector;
		}

		public object[] ReadFile(string fileName)
		{
			object[] array;
			using (StreamReader streamReader = new StreamReader(fileName, this.mEncoding, true, 102400))
			{
				array = this.ReadStream(streamReader);
			}
			return array;
		}

		public object[] ReadStream(TextReader reader)
		{
			return this.ReadStream(new NewLineDelimitedRecordReader(reader));
		}

		public object[] ReadStream(IRecordReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader", "The reader of the Stream can\u00b4t be null");
			}
			if (this.mRecordSelector == null)
			{
				throw new BadUsageException("The Recordselector can\u00b4t be null, please pass a not null Selector in the constructor.");
			}
			base.ResetFields();
			this.mHeaderText = string.Empty;
			this.mFooterText = string.Empty;
			ArrayList arrayList = new ArrayList();
			using (ForwardReader forwardReader = new ForwardReader(reader, this.mMultiRecordInfo[0].IgnoreLast))
			{
				forwardReader.DiscardForward = true;
				this.mLineNumber = 1;
				string text = forwardReader.ReadNextLine();
				string text2 = text;
				if (base.MustNotifyProgress)
				{
					base.OnProgress(new ProgressEventArgs(0, -1));
				}
				int num = 0;
				if (this.mMultiRecordInfo[0].IgnoreFirst > 0)
				{
					int num2 = 0;
					while (num2 < this.mMultiRecordInfo[0].IgnoreFirst && text2 != null)
					{
						this.mHeaderText = this.mHeaderText + text2 + StringHelper.NewLine;
						text2 = forwardReader.ReadNextLine();
						this.mLineNumber++;
						num2++;
					}
				}
				bool flag = false;
				LineInfo lineInfo = new LineInfo(text2)
				{
					mReader = forwardReader
				};
				while (text2 != null)
				{
					try
					{
						this.mTotalRecords++;
						num++;
						lineInfo.ReLoad(text2);
						bool flag2 = false;
						Type type = null;
						try
						{
							type = this.mRecordSelector(this, text2);
						}
						catch (Exception ex)
						{
							throw new Exception("Selector failed to process correctly", ex);
						}
						if (type != null)
						{
							RecordInfo recordInfo = (RecordInfo)this.mRecordInfoHash[type];
							if (recordInfo == null)
							{
								throw new BadUsageException("A record is of type '" + type.Name + "' which this engine is not configured to handle. Try adding this type to the constructor.");
							}
							object obj = recordInfo.Operations.CreateRecordHandler();
							if (base.MustNotifyProgress)
							{
								base.OnProgress(new ProgressEventArgs(num, -1));
							}
							BeforeReadEventArgs<object> beforeReadEventArgs = null;
							if (base.MustNotifyRead)
							{
								beforeReadEventArgs = new BeforeReadEventArgs<object>(this, obj, text2, base.LineNumber);
								flag2 = base.OnBeforeReadRecord(beforeReadEventArgs);
								if (beforeReadEventArgs.RecordLineChanged)
								{
									lineInfo.ReLoad(beforeReadEventArgs.RecordLine);
								}
							}
							if (!flag2)
							{
								object[] array = new object[recordInfo.FieldCount];
								if (recordInfo.Operations.StringToRecord(obj, lineInfo, array))
								{
									if (base.MustNotifyRead)
									{
										flag2 = base.OnAfterReadRecord(text2, obj, beforeReadEventArgs.RecordLineChanged, base.LineNumber);
									}
									if (!flag2)
									{
										arrayList.Add(obj);
									}
								}
							}
						}
					}
					catch (Exception ex2)
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
								mLineNumber = forwardReader.LineNumber,
								mExceptionInfo = ex2,
								mRecordString = text
							};
							this.mErrorManager.AddError(errorInfo);
							break;
						}
						}
					}
					finally
					{
						if (!flag)
						{
							text2 = forwardReader.ReadNextLine();
							text = text2;
							this.mLineNumber = forwardReader.LineNumber;
						}
					}
				}
				if (this.mMultiRecordInfo[0].IgnoreLast > 0)
				{
					this.mFooterText = forwardReader.RemainingText;
				}
			}
			return arrayList.ToArray();
		}

		public object[] ReadString(string source)
		{
			InternalStringReader internalStringReader = new InternalStringReader(source);
			object[] array = this.ReadStream(internalStringReader);
			internalStringReader.Close();
			return array;
		}

		public void WriteFile(string fileName, IEnumerable records)
		{
			this.WriteFile(fileName, records, -1);
		}

		public void WriteFile(string fileName, IEnumerable records, int maxRecords)
		{
			using (StreamWriter streamWriter = new StreamWriter(fileName, false, this.mEncoding, 102400))
			{
				this.WriteStream(streamWriter, records, maxRecords);
				streamWriter.Close();
			}
		}

		public void WriteStream(TextWriter writer, IEnumerable records)
		{
			this.WriteStream(writer, records, -1);
		}

		public void WriteStream(TextWriter writer, IEnumerable records, int maxRecords)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer", "The writer of the Stream can be null");
			}
			if (records == null)
			{
				throw new ArgumentNullException("records", "The records can be null. Try with an empty array.");
			}
			base.ResetFields();
			if (!string.IsNullOrEmpty(this.mHeaderText))
			{
				if (this.mHeaderText.EndsWith(StringHelper.NewLine))
				{
					writer.Write(this.mHeaderText);
				}
				else
				{
					writer.WriteLine(this.mHeaderText);
				}
			}
			string text = null;
			int num = maxRecords;
			if (records is IList)
			{
				num = Math.Min((num < 0) ? int.MaxValue : num, ((IList)records).Count);
			}
			if (base.MustNotifyProgress)
			{
				base.OnProgress(new ProgressEventArgs(0, num));
			}
			int num2 = 0;
			foreach (object obj in records)
			{
				if (num2 == maxRecords)
				{
					break;
				}
				try
				{
					if (obj == null)
					{
						throw new BadUsageException("The record at index " + num2.ToString() + " is null.");
					}
					bool flag = false;
					if (base.MustNotifyProgress)
					{
						base.OnProgress(new ProgressEventArgs(num2 + 1, num));
					}
					if (base.MustNotifyWrite)
					{
						flag = base.OnBeforeWriteRecord(obj, base.LineNumber);
					}
					IRecordInfo recordInfo = (IRecordInfo)this.mRecordInfoHash[obj.GetType()];
					if (recordInfo == null)
					{
						throw new BadUsageException(string.Concat(new string[]
						{
							"The record at index ",
							num2.ToString(),
							" is of type '",
							obj.GetType().Name,
							"' and the engine dont handle this type. You can add it to the constructor."
						}));
					}
					if (!flag)
					{
						text = recordInfo.Operations.RecordToString(obj);
						if (base.MustNotifyWrite)
						{
							text = base.OnAfterWriteRecord(text, obj);
						}
						writer.WriteLine(text);
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
				num2++;
			}
			this.mTotalRecords = num2;
			if (!string.IsNullOrEmpty(this.mFooterText))
			{
				if (this.mFooterText.EndsWith(StringHelper.NewLine))
				{
					writer.Write(this.mFooterText);
					return;
				}
				writer.WriteLine(this.mFooterText);
			}
		}

		public string WriteString(IEnumerable records)
		{
			return this.WriteString(records, -1);
		}

		public string WriteString(IEnumerable records, int maxRecords)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringWriter stringWriter = new StringWriter(stringBuilder);
			this.WriteStream(stringWriter, records, maxRecords);
			string text = stringWriter.ToString();
			stringWriter.Close();
			return text;
		}

		public void AppendToFile(string fileName, object record)
		{
			this.AppendToFile(fileName, new object[] { record });
		}

		public void AppendToFile(string fileName, IEnumerable records)
		{
			using (TextWriter textWriter = StreamHelper.CreateFileAppender(fileName, this.mEncoding, true, false, 102400))
			{
				this.mHeaderText = string.Empty;
				this.mFooterText = string.Empty;
				this.WriteStream(textWriter, records);
				textWriter.Close();
			}
		}

		private static Type GetFirstType(Type[] types)
		{
			if (types == null)
			{
				throw new BadUsageException("A null Type[] is not valid for the MultiRecordEngine.");
			}
			if (types.Length == 0)
			{
				throw new BadUsageException("An empty Type[] is not valid for the MultiRecordEngine.");
			}
			if (types.Length == 1)
			{
				throw new BadUsageException("You only provide one type to the engine constructor. You need 2 or more types, for one type you can use the FileHelperEngine.");
			}
			return types[0];
		}

		public object LastRecord
		{
			get
			{
				return this.mLastRecord;
			}
		}

		public void BeginReadStream(TextReader reader)
		{
			this.BeginReadStream(new NewLineDelimitedRecordReader(reader));
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void BeginReadStream(IRecordReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("The TextReader can\u00b4t be null.");
			}
			base.ResetFields();
			this.mHeaderText = string.Empty;
			this.mFooterText = string.Empty;
			if (base.RecordInfo.IgnoreFirst > 0)
			{
				for (int i = 0; i < base.RecordInfo.IgnoreFirst; i++)
				{
					string text = reader.ReadRecordString();
					this.mLineNumber++;
					if (text == null)
					{
						break;
					}
					this.mHeaderText = this.mHeaderText + text + StringHelper.NewLine;
				}
			}
			this.mAsyncReader = new ForwardReader(reader, base.RecordInfo.IgnoreLast, this.mLineNumber)
			{
				DiscardForward = true
			};
		}

		public void BeginReadFile(string fileName)
		{
			this.BeginReadStream(new StreamReader(fileName, this.mEncoding, true, 102400));
		}

		public void BeginReadString(string sourceData)
		{
			if (sourceData == null)
			{
				sourceData = string.Empty;
			}
			this.BeginReadStream(new InternalStringReader(sourceData));
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
			try
			{
				if (this.mAsyncReader != null)
				{
					this.mAsyncReader.Close();
				}
				this.mAsyncReader = null;
			}
			catch
			{
			}
			try
			{
				if (this.mAsyncWriter != null)
				{
					if (!string.IsNullOrEmpty(this.mFooterText))
					{
						if (this.mFooterText.EndsWith(StringHelper.NewLine))
						{
							this.mAsyncWriter.Write(this.mFooterText);
						}
						else
						{
							this.mAsyncWriter.WriteLine(this.mFooterText);
						}
					}
					this.mAsyncWriter.Close();
					this.mAsyncWriter = null;
				}
			}
			catch
			{
			}
		}

		public object ReadNext()
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
			this.mLastRecord = null;
			LineInfo lineInfo = new LineInfo(text)
			{
				mReader = this.mAsyncReader
			};
			while (text != null)
			{
				try
				{
					try
					{
						this.mTotalRecords++;
						Type type = this.mRecordSelector(this, text);
						lineInfo.ReLoad(text);
						if (type != null)
						{
							RecordInfo recordInfo = (RecordInfo)this.mRecordInfoHash[type];
							if (recordInfo == null)
							{
								throw new BadUsageException("A record is of type '" + type.Name + "' which this engine is not configured to handle. Try adding this type to the constructor.");
							}
							object[] array = new object[recordInfo.FieldCount];
							this.mLastRecord = recordInfo.Operations.StringToRecord(lineInfo, array);
							if (base.MustNotifyRead)
							{
								base.OnAfterReadRecord(text, this.mLastRecord, false, base.LineNumber);
							}
							if (this.mLastRecord != null)
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
			this.mLastRecord = null;
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

		public object[] ReadNexts(int numberOfRecords)
		{
			if (this.mAsyncReader == null)
			{
				throw new BadUsageException("Before call ReadNext you must call BeginReadFile or BeginReadStream.");
			}
			ArrayList arrayList = new ArrayList(numberOfRecords);
			for (int i = 0; i < numberOfRecords; i++)
			{
				this.ReadNextRecord();
				if (this.mLastRecord == null)
				{
					break;
				}
				arrayList.Add(this.mLastRecord);
			}
			return arrayList.ToArray();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (this.mAsyncReader == null)
			{
				throw new FileHelpersException("You must call BeginRead before use the engine in a foreach loop.");
			}
			return new MultiRecordEngine.AsyncEnumerator(this);
		}

		void IDisposable.Dispose()
		{
			this.Close();
			GC.SuppressFinalize(this);
		}

		~MultiRecordEngine()
		{
			this.Close();
		}

		public void WriteNext(object record)
		{
			if (this.mAsyncWriter == null)
			{
				throw new BadUsageException("Before call WriteNext you must call BeginWriteFile or BeginWriteStream.");
			}
			if (record == null)
			{
				throw new BadUsageException("The record to write can\u00b4t be null.");
			}
			this.WriteRecord(record);
		}

		public void WriteNexts(IEnumerable records)
		{
			if (this.mAsyncWriter == null)
			{
				throw new BadUsageException("Before call WriteNext you must call BeginWriteFile or BeginWriteStream.");
			}
			if (records == null)
			{
				throw new ArgumentNullException("The record to write can\u00b4t be null.");
			}
			int num = 0;
			foreach (object obj in records)
			{
				num++;
				if (obj == null)
				{
					throw new BadUsageException("The record at index " + num.ToString() + " is null.");
				}
				this.WriteRecord(obj);
			}
		}

		private void WriteRecord(object record)
		{
			string text = null;
			try
			{
				this.mLineNumber++;
				this.mTotalRecords++;
				IRecordInfo recordInfo = (IRecordInfo)this.mRecordInfoHash[record.GetType()];
				if (recordInfo == null)
				{
					throw new BadUsageException("A record is of type '" + record.GetType().Name + "' and the engine dont handle this type. You can add it to the constructor.");
				}
				text = recordInfo.Operations.RecordToString(record);
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
		}

		public void BeginWriteStream(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentException("The TextWriter can\u00b4t be null.", "writer");
			}
			base.ResetFields();
			this.mAsyncWriter = writer;
			this.WriteHeader();
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

		public void BeginWriteFile(string fileName)
		{
			this.BeginWriteStream(new StreamWriter(fileName, false, this.mEncoding, 102400));
		}

		public void BeginAppendToFile(string fileName)
		{
			this.mAsyncWriter = StreamHelper.CreateFileAppender(fileName, this.mEncoding, false, true, 102400);
			this.mHeaderText = string.Empty;
			this.mFooterText = string.Empty;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly IRecordInfo[] mMultiRecordInfo;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Hashtable mRecordInfoHash;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private RecordTypeSelector mRecordSelector;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Type[] mTypes;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ForwardReader mAsyncReader;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TextWriter mAsyncWriter;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private object mLastRecord;

		private class AsyncEnumerator : IEnumerator
		{
			public AsyncEnumerator(MultiRecordEngine engine)
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

			private readonly MultiRecordEngine mEngine;
		}
	}
}
