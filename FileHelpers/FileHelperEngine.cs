using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using FileHelpers.Events;

namespace FileHelpers
{
	[DebuggerDisplay("FileHelperEngine for type: {RecordType.Name}. ErrorMode: {ErrorManager.ErrorMode.ToString()}. Encoding: {Encoding.EncodingName}")]
	public class FileHelperEngine<T> : EventEngineBase<T>, IFileHelperEngine<T> where T : class
	{
		public FileHelperEngine()
			: this(Encoding.Default)
		{
		}

		public FileHelperEngine(Encoding encoding)
			: base(typeof(T), encoding)
		{
			this.mObjectEngine = typeof(T) == typeof(object);
		}

		protected FileHelperEngine(Type recordType, Encoding encoding)
			: base(recordType, encoding)
		{
			this.mObjectEngine = typeof(T) == typeof(object);
		}

		internal FileHelperEngine(RecordInfo ri)
			: base(ri)
		{
			this.mObjectEngine = typeof(T) == typeof(object);
		}

		public T[] ReadFile(string fileName)
		{
			return this.ReadFile(fileName, int.MaxValue);
		}

		public T[] ReadFile(string fileName, int maxRecords)
		{
			T[] array2;
			using (InternalStreamReader internalStreamReader = new InternalStreamReader(fileName, this.mEncoding, true, 102400))
			{
				T[] array = this.ReadStream(internalStreamReader, maxRecords);
				internalStreamReader.Close();
				array2 = array;
			}
			return array2;
		}

		public List<T> ReadFileAsList(string fileName)
		{
			return this.ReadFileAsList(fileName, int.MaxValue);
		}

		public List<T> ReadFileAsList(string fileName, int maxRecords)
		{
			List<T> list2;
			using (InternalStreamReader internalStreamReader = new InternalStreamReader(fileName, this.mEncoding, true, 102400))
			{
				List<T> list = this.ReadStreamAsList(internalStreamReader, maxRecords);
				internalStreamReader.Close();
				list2 = list;
			}
			return list2;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public T[] ReadStream(TextReader reader)
		{
			return this.ReadStream(reader, int.MaxValue);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public T[] ReadStream(TextReader reader, int maxRecords)
		{
			IList list = this.ReadStreamAsList(reader, maxRecords, null);
			if (this.mObjectEngine)
			{
				return (T[])((ArrayList)list).ToArray(base.RecordInfo.RecordType);
			}
			return ((List<T>)list).ToArray();
		}

		private void ReadStream(TextReader reader, int maxRecords, DataTable dt)
		{
			this.ReadStreamAsList(reader, maxRecords, dt);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public List<T> ReadStreamAsList(TextReader reader, int maxRecords)
		{
			IList list = this.ReadStreamAsList(reader, maxRecords, null);
			if (this.mObjectEngine)
			{
				List<T> list2 = new List<T>(list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					list2.Add((T)((object)list[i]));
				}
				return list2;
			}
			return (List<T>)list;
		}

		private IList ReadStreamAsList(TextReader reader, int maxRecords, DataTable dt)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader", "The reader of the Stream can\u00b4t be null");
			}
			NewLineDelimitedRecordReader newLineDelimitedRecordReader = new NewLineDelimitedRecordReader(reader);
			base.ResetFields();
			this.mHeaderText = string.Empty;
			this.mFooterText = string.Empty;
			IList list;
			if (this.mObjectEngine)
			{
				list = new ArrayList();
			}
			else
			{
				list = new List<T>();
			}
			int num = 0;
			StreamInfoProvider streamInfoProvider = new StreamInfoProvider(reader);
			using (ForwardReader forwardReader = new ForwardReader(newLineDelimitedRecordReader, base.RecordInfo.IgnoreLast))
			{
				forwardReader.DiscardForward = true;
				this.mLineNumber = 1;
				string text = forwardReader.ReadNextLine();
				string text2 = text;
				if (base.MustNotifyProgress)
				{
					base.OnProgress(new ProgressEventArgs(0, -1, streamInfoProvider.Position, streamInfoProvider.TotalBytes));
				}
				if (base.RecordInfo.IgnoreFirst > 0)
				{
					int num2 = 0;
					while (num2 < base.RecordInfo.IgnoreFirst && text2 != null)
					{
						this.mHeaderText = this.mHeaderText + text2 + StringHelper.NewLine;
						text2 = forwardReader.ReadNextLine();
						this.mLineNumber++;
						num2++;
					}
				}
				bool flag = false;
				if (maxRecords < 0)
				{
					maxRecords = int.MaxValue;
				}
				LineInfo lineInfo = new LineInfo(text2)
				{
					mReader = forwardReader
				};
				object[] array = new object[base.RecordInfo.FieldCount];
				while (text2 != null && num < maxRecords)
				{
					text = text2;
					try
					{
						this.mTotalRecords++;
						num++;
						lineInfo.ReLoad(text2);
						bool flag2 = false;
						T t = (T)((object)base.RecordInfo.Operations.CreateRecordHandler());
						if (base.MustNotifyProgress)
						{
							base.OnProgress(new ProgressEventArgs(num, -1, streamInfoProvider.Position, streamInfoProvider.TotalBytes));
						}
						BeforeReadEventArgs<T> beforeReadEventArgs = null;
						if (base.MustNotifyRead)
						{
							beforeReadEventArgs = new BeforeReadEventArgs<T>(this, t, text2, base.LineNumber);
							flag2 = base.OnBeforeReadRecord(beforeReadEventArgs);
							if (beforeReadEventArgs.RecordLineChanged)
							{
								lineInfo.ReLoad(beforeReadEventArgs.RecordLine);
							}
						}
						if (!flag2 && base.RecordInfo.Operations.StringToRecord(t, lineInfo, array))
						{
							if (base.MustNotifyRead)
							{
								flag2 = base.OnAfterReadRecord(text2, t, beforeReadEventArgs.RecordLineChanged, base.LineNumber);
							}
							if (!flag2)
							{
								if (dt == null)
								{
									list.Add(t);
								}
								else
								{
									dt.Rows.Add(base.RecordInfo.Operations.RecordToValues(t));
								}
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
								mLineNumber = forwardReader.LineNumber,
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
						if (!flag)
						{
							text2 = forwardReader.ReadNextLine();
							this.mLineNumber++;
						}
					}
				}
				if (base.RecordInfo.IgnoreLast > 0)
				{
					this.mFooterText = forwardReader.RemainingText;
				}
			}
			return list;
		}

		public T[] ReadString(string source)
		{
			return this.ReadString(source, int.MaxValue);
		}

		public T[] ReadString(string source, int maxRecords)
		{
			if (source == null)
			{
				source = string.Empty;
			}
			T[] array2;
			using (InternalStringReader internalStringReader = new InternalStringReader(source))
			{
				T[] array = this.ReadStream(internalStringReader, maxRecords);
				internalStringReader.Close();
				array2 = array;
			}
			return array2;
		}

		public List<T> ReadStringAsList(string source)
		{
			return this.ReadStringAsList(source, int.MaxValue);
		}

		public List<T> ReadStringAsList(string source, int maxRecords)
		{
			if (source == null)
			{
				source = string.Empty;
			}
			List<T> list2;
			using (InternalStringReader internalStringReader = new InternalStringReader(source))
			{
				List<T> list = this.ReadStreamAsList(internalStringReader, maxRecords);
				internalStringReader.Close();
				list2 = list;
			}
			return list2;
		}

		public void WriteFile(string fileName, IEnumerable<T> records)
		{
			this.WriteFile(fileName, records, -1);
		}

		public void WriteFile(string fileName, IEnumerable<T> records, int maxRecords)
		{
			using (StreamWriter streamWriter = new StreamWriter(fileName, false, this.mEncoding, 102400))
			{
				this.WriteStream(streamWriter, records, maxRecords);
				streamWriter.Close();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void WriteStream(TextWriter writer, IEnumerable<T> records)
		{
			this.WriteStream(writer, records, -1);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void WriteStream(TextWriter writer, IEnumerable<T> records, int maxRecords)
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
			bool flag = true;
			foreach (T t in records)
			{
				if (num2 == maxRecords)
				{
					break;
				}
				this.mLineNumber++;
				try
				{
					if (t == null)
					{
						throw new BadUsageException(string.Format("The record at index {0} is null.", num2));
					}
					if (flag)
					{
						flag = false;
						if (!base.RecordInfo.RecordType.IsInstanceOfType(t))
						{
							throw new BadUsageException("This engine works with record of type " + base.RecordInfo.RecordType.Name + " and you use records of type " + t.GetType().Name);
						}
					}
					bool flag2 = false;
					if (base.MustNotifyProgress)
					{
						base.OnProgress(new ProgressEventArgs(num2 + 1, num));
					}
					if (base.MustNotifyWrite)
					{
						flag2 = base.OnBeforeWriteRecord(t, base.LineNumber);
					}
					if (!flag2)
					{
						text = base.RecordInfo.Operations.RecordToString(t);
						if (base.MustNotifyWrite)
						{
							text = base.OnAfterWriteRecord(text, t);
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

		public string WriteString(IEnumerable<T> records)
		{
			return this.WriteString(records, -1);
		}

		public string WriteString(IEnumerable<T> records, int maxRecords)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text2;
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				this.WriteStream(stringWriter, records, maxRecords);
				string text = stringWriter.ToString();
				text2 = text;
			}
			return text2;
		}

		public void AppendToFile(string fileName, T record)
		{
			this.AppendToFile(fileName, new T[] { record });
		}

		public void AppendToFile(string fileName, IEnumerable<T> records)
		{
			using (TextWriter textWriter = StreamHelper.CreateFileAppender(fileName, this.mEncoding, true, false, 102400))
			{
				this.mHeaderText = string.Empty;
				this.mFooterText = string.Empty;
				this.WriteStream(textWriter, records);
				textWriter.Close();
			}
		}

		public DataTable ReadFileAsDT(string fileName)
		{
			return this.ReadFileAsDT(fileName, -1);
		}

		public DataTable ReadFileAsDT(string fileName, int maxRecords)
		{
			DataTable dataTable2;
			using (InternalStreamReader internalStreamReader = new InternalStreamReader(fileName, this.mEncoding, true, 102400))
			{
				DataTable dataTable = this.ReadStreamAsDT(internalStreamReader, maxRecords);
				internalStreamReader.Close();
				dataTable2 = dataTable;
			}
			return dataTable2;
		}

		public DataTable ReadStringAsDT(string source)
		{
			return this.ReadStringAsDT(source, -1);
		}

		public DataTable ReadStringAsDT(string source, int maxRecords)
		{
			if (source == null)
			{
				source = string.Empty;
			}
			DataTable dataTable2;
			using (InternalStringReader internalStringReader = new InternalStringReader(source))
			{
				DataTable dataTable = this.ReadStreamAsDT(internalStringReader, maxRecords);
				internalStringReader.Close();
				dataTable2 = dataTable;
			}
			return dataTable2;
		}

		public DataTable ReadStreamAsDT(TextReader reader)
		{
			return this.ReadStreamAsDT(reader, -1);
		}

		public DataTable ReadStreamAsDT(TextReader reader, int maxRecords)
		{
			DataTable dataTable = base.RecordInfo.Operations.CreateEmptyDataTable();
			dataTable.BeginLoadData();
			this.ReadStream(reader, maxRecords, dataTable);
			dataTable.EndLoadData();
			return dataTable;
		}

		private readonly bool mObjectEngine;
	}
}
