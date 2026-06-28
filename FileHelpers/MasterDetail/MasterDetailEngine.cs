using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using FileHelpers.Events;

namespace FileHelpers.MasterDetail
{
	public class MasterDetailEngine<TMaster, TDetail> : EngineBase where TMaster : class where TDetail : class
	{
		public MasterDetailEngine()
			: this(null)
		{
		}

		public MasterDetailEngine(MasterDetailSelector recordSelector)
			: this(typeof(TMaster), typeof(TDetail), recordSelector)
		{
		}

		internal MasterDetailEngine(Type masterType, Type detailType, MasterDetailSelector recordSelector)
			: base(detailType)
		{
			this.mMasterType = masterType;
			this.mMasterInfo = FileHelpers.RecordInfo.Resolve(this.mMasterType);
			this.mRecordSelector = recordSelector;
		}

		public MasterDetailEngine(CommonSelector action, string selector)
			: this(typeof(TMaster), typeof(TDetail), action, selector)
		{
		}

		internal MasterDetailEngine(Type masterType, Type detailType, CommonSelector action, string selector)
			: base(detailType)
		{
			this.mMasterType = masterType;
			this.mMasterInfo = FileHelpers.RecordInfo.Resolve(this.mMasterType);
			MasterDetailEngine<object, object>.CommonSelectorInternal commonSelectorInternal = new MasterDetailEngine<object, object>.CommonSelectorInternal(action, selector, this.mMasterInfo.IgnoreEmptyLines || base.RecordInfo.IgnoreEmptyLines);
			this.mRecordSelector = new MasterDetailSelector(commonSelectorInternal.CommonSelectorMethod);
		}

		public Type MasterType
		{
			get
			{
				return this.mMasterType;
			}
		}

		public MasterDetailSelector RecordSelector
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

		public MasterDetails<TMaster, TDetail>[] ReadFile(string fileName)
		{
			MasterDetails<TMaster, TDetail>[] array2;
			using (StreamReader streamReader = new StreamReader(fileName, this.mEncoding, true, 102400))
			{
				MasterDetails<TMaster, TDetail>[] array = this.ReadStream(streamReader);
				streamReader.Close();
				array2 = array;
			}
			return array2;
		}

		public MasterDetails<TMaster, TDetail>[] ReadStream(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader", "The reader of the Stream can't be null");
			}
			if (this.RecordSelector == null)
			{
				throw new BadUsageException("The RecordSelector can't be null on read operations.");
			}
			NewLineDelimitedRecordReader newLineDelimitedRecordReader = new NewLineDelimitedRecordReader(reader);
			base.ResetFields();
			this.mHeaderText = string.Empty;
			this.mFooterText = string.Empty;
			ArrayList arrayList = new ArrayList();
			using (ForwardReader forwardReader = new ForwardReader(newLineDelimitedRecordReader, this.mMasterInfo.IgnoreLast))
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
				if (this.mMasterInfo.IgnoreFirst > 0)
				{
					int num2 = 0;
					while (num2 < this.mMasterInfo.IgnoreFirst && text2 != null)
					{
						this.mHeaderText = this.mHeaderText + text2 + StringHelper.NewLine;
						text2 = forwardReader.ReadNextLine();
						this.mLineNumber++;
						num2++;
					}
				}
				bool flag = false;
				MasterDetails<TMaster, TDetail> masterDetails = null;
				ArrayList arrayList2 = new ArrayList();
				LineInfo lineInfo = new LineInfo(text2)
				{
					mReader = forwardReader
				};
				object[] array = new object[this.mMasterInfo.FieldCount];
				object[] array2 = new object[base.RecordInfo.FieldCount];
				while (text2 != null)
				{
					try
					{
						num++;
						lineInfo.ReLoad(text2);
						if (base.MustNotifyProgress)
						{
							base.OnProgress(new ProgressEventArgs(num, -1));
						}
						RecordAction recordAction = RecordAction.Skip;
						try
						{
							recordAction = this.RecordSelector(text2);
						}
						catch (Exception ex)
						{
							throw new Exception("Supplied Record selector failed to process record", ex);
						}
						switch (recordAction)
						{
						case RecordAction.Master:
						{
							if (masterDetails != null)
							{
								masterDetails.Details = (TDetail[])arrayList2.ToArray(typeof(TDetail));
								arrayList.Add(masterDetails);
							}
							this.mTotalRecords++;
							masterDetails = new MasterDetails<TMaster, TDetail>();
							arrayList2.Clear();
							TMaster tmaster = (TMaster)((object)this.mMasterInfo.Operations.StringToRecord(lineInfo, array));
							if (tmaster != null)
							{
								masterDetails.Master = tmaster;
							}
							break;
						}
						case RecordAction.Detail:
						{
							TDetail tdetail = (TDetail)((object)base.RecordInfo.Operations.StringToRecord(lineInfo, array2));
							if (tdetail != null)
							{
								arrayList2.Add(tdetail);
							}
							break;
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
								mLineNumber = this.mLineNumber,
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
				if (masterDetails != null)
				{
					masterDetails.Details = (TDetail[])arrayList2.ToArray(typeof(TDetail));
					arrayList.Add(masterDetails);
				}
				if (this.mMasterInfo.IgnoreLast > 0)
				{
					this.mFooterText = forwardReader.RemainingText;
				}
			}
			return (MasterDetails<TMaster, TDetail>[])arrayList.ToArray(typeof(MasterDetails<TMaster, TDetail>));
		}

		public MasterDetails<TMaster, TDetail>[] ReadString(string source)
		{
			StringReader stringReader = new StringReader(source);
			MasterDetails<TMaster, TDetail>[] array = this.ReadStream(stringReader);
			stringReader.Close();
			return array;
		}

		public void WriteFile(string fileName, IEnumerable<MasterDetails<TMaster, TDetail>> records)
		{
			this.WriteFile(fileName, records, -1);
		}

		public void WriteFile(string fileName, IEnumerable<MasterDetails<TMaster, TDetail>> records, int maxRecords)
		{
			using (StreamWriter streamWriter = new StreamWriter(fileName, false, this.mEncoding, 102400))
			{
				this.WriteStream(streamWriter, records, maxRecords);
				streamWriter.Close();
			}
		}

		public void WriteStream(TextWriter writer, IEnumerable<MasterDetails<TMaster, TDetail>> records)
		{
			this.WriteStream(writer, records, -1);
		}

		public void WriteStream(TextWriter writer, IEnumerable<MasterDetails<TMaster, TDetail>> records, int maxRecords)
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
			foreach (MasterDetails<TMaster, TDetail> masterDetails in records)
			{
				if (num2 == maxRecords)
				{
					break;
				}
				try
				{
					if (masterDetails == null)
					{
						throw new BadUsageException("The record at index " + num2.ToString() + " is null.");
					}
					if (base.MustNotifyProgress)
					{
						base.OnProgress(new ProgressEventArgs(num2 + 1, num));
					}
					text = this.mMasterInfo.Operations.RecordToString(masterDetails.Master);
					writer.WriteLine(text);
					if (masterDetails.Details != null)
					{
						for (int i = 0; i < masterDetails.Details.Length; i++)
						{
							text = base.RecordInfo.Operations.RecordToString(masterDetails.Details[i]);
							writer.WriteLine(text);
						}
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

		public string WriteString(IEnumerable<MasterDetails<TMaster, TDetail>> records)
		{
			return this.WriteString(records, -1);
		}

		public string WriteString(IEnumerable<MasterDetails<TMaster, TDetail>> records, int maxRecords)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringWriter stringWriter = new StringWriter(stringBuilder);
			this.WriteStream(stringWriter, records, maxRecords);
			string text = stringWriter.ToString();
			stringWriter.Close();
			return text;
		}

		public void AppendToFile(string fileName, MasterDetails<TMaster, TDetail> record)
		{
			this.AppendToFile(fileName, new MasterDetails<TMaster, TDetail>[] { record });
		}

		public void AppendToFile(string fileName, IEnumerable<MasterDetails<TMaster, TDetail>> records)
		{
			using (TextWriter textWriter = StreamHelper.CreateFileAppender(fileName, this.mEncoding, true, false, 102400))
			{
				this.mHeaderText = string.Empty;
				this.mFooterText = string.Empty;
				this.WriteStream(textWriter, records);
				textWriter.Close();
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly IRecordInfo mMasterInfo;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private MasterDetailSelector mRecordSelector;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Type mMasterType;

		internal class CommonSelectorInternal
		{
			internal CommonSelectorInternal(CommonSelector action, string selector, bool ignoreEmpty)
			{
				this.mAction = action;
				this.mSelector = selector;
				this.mIgnoreEmpty = ignoreEmpty;
			}

			internal RecordAction CommonSelectorMethod(string recordString)
			{
				if (this.mIgnoreEmpty && recordString == string.Empty)
				{
					return RecordAction.Skip;
				}
				switch (this.mAction)
				{
				case CommonSelector.MasterIfContains:
					if (recordString.IndexOf(this.mSelector) >= 0)
					{
						return RecordAction.Master;
					}
					return RecordAction.Detail;
				case CommonSelector.MasterIfBegins:
					if (recordString.StartsWith(this.mSelector))
					{
						return RecordAction.Master;
					}
					return RecordAction.Detail;
				case CommonSelector.MasterIfEnds:
					if (recordString.EndsWith(this.mSelector))
					{
						return RecordAction.Master;
					}
					return RecordAction.Detail;
				case CommonSelector.MasterIfEnclosed:
					if (recordString.StartsWith(this.mSelector) && recordString.EndsWith(this.mSelector))
					{
						return RecordAction.Master;
					}
					return RecordAction.Detail;
				case CommonSelector.DetailIfContains:
					if (recordString.IndexOf(this.mSelector) >= 0)
					{
						return RecordAction.Detail;
					}
					return RecordAction.Master;
				case CommonSelector.DetailIfBegins:
					if (recordString.StartsWith(this.mSelector))
					{
						return RecordAction.Detail;
					}
					return RecordAction.Master;
				case CommonSelector.DetailIfEnds:
					if (recordString.EndsWith(this.mSelector))
					{
						return RecordAction.Detail;
					}
					return RecordAction.Master;
				case CommonSelector.DetailIfEnclosed:
					if (recordString.StartsWith(this.mSelector) && recordString.EndsWith(this.mSelector))
					{
						return RecordAction.Detail;
					}
					return RecordAction.Master;
				default:
					return RecordAction.Skip;
				}
			}

			private readonly CommonSelector mAction;

			private readonly string mSelector;

			private readonly bool mIgnoreEmpty;
		}
	}
}
