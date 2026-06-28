using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace FileHelpers
{
	[DebuggerDisplay("FileTransformanEngine for types: {SourceType.Name} --> {DestinationType.Name}. Source Encoding: {SourceEncoding.EncodingName}. Destination Encoding: {DestinationEncoding.EncodingName}")]
	public sealed class FileTransformEngine<TSource, TDestination> where TSource : class, ITransformable<TDestination> where TDestination : class
	{
		public ErrorMode ErrorMode
		{
			get
			{
				return this.mErrorMode;
			}
			set
			{
				this.mErrorMode = value;
				this.mSourceErrorManager = new ErrorManager(value);
				this.mDestinationErrorManager = new ErrorManager(value);
			}
		}

		public ErrorManager SourceErrorManager
		{
			get
			{
				return this.mSourceErrorManager;
			}
		}

		public ErrorManager DestinationErrorManager
		{
			get
			{
				return this.mDestinationErrorManager;
			}
		}

		public TDestination[] TransformFile(string sourceFile, string destFile)
		{
			ExHelper.CheckNullParam(sourceFile, "sourceFile");
			ExHelper.CheckNullParam(destFile, "destFile");
			ExHelper.CheckDifferentsParams(sourceFile, "sourceFile", destFile, "destFile");
			return this.CoreTransformFile(sourceFile, destFile);
		}

		public int TransformFileFast(string sourceFile, string destFile)
		{
			ExHelper.CheckNullParam(sourceFile, "sourceFile");
			ExHelper.CheckNullParam(destFile, "destFile");
			ExHelper.CheckDifferentsParams(sourceFile, "sourceFile", destFile, "destFile");
			return this.CoreTransformAsync(new InternalStreamReader(sourceFile, this.SourceEncoding, true, 512000), new StreamWriter(destFile, false, this.DestinationEncoding, 512000));
		}

		public int TransformFileFast(TextReader sourceStream, string destFile)
		{
			ExHelper.CheckNullParam(sourceStream, "sourceStream");
			ExHelper.CheckNullParam(destFile, "destFile");
			return this.CoreTransformAsync(sourceStream, new StreamWriter(destFile, false, this.DestinationEncoding, 512000));
		}

		public int TransformFileFast(TextReader sourceStream, StreamWriter destStream)
		{
			ExHelper.CheckNullParam(sourceStream, "sourceStream");
			ExHelper.CheckNullParam(destStream, "destStream");
			return this.CoreTransformAsync(sourceStream, destStream);
		}

		public int TransformFileFast(string sourceFile, StreamWriter destStream)
		{
			ExHelper.CheckNullParam(sourceFile, "sourceFile");
			ExHelper.CheckNullParam(destStream, "destStream");
			return this.CoreTransformAsync(new InternalStreamReader(sourceFile, this.SourceEncoding, true, 512000), destStream);
		}

		public TDestination[] TransformRecords(TSource[] sourceRecords)
		{
			return this.CoreTransformRecords(sourceRecords);
		}

		public TDestination[] ReadAndTransformRecords(string sourceFile)
		{
			FileHelperAsyncEngine<TSource> fileHelperAsyncEngine = new FileHelperAsyncEngine<TSource>(this.mSourceEncoding)
			{
				ErrorMode = this.ErrorMode
			};
			this.mSourceErrorManager = fileHelperAsyncEngine.ErrorManager;
			this.mDestinationErrorManager = new ErrorManager(this.ErrorMode);
			List<TDestination> list = new List<TDestination>();
			fileHelperAsyncEngine.BeginReadFile(sourceFile);
			foreach (TSource tsource in ((IEnumerable<TSource>)fileHelperAsyncEngine))
			{
				list.Add(tsource.TransformTo());
			}
			fileHelperAsyncEngine.Close();
			return list.ToArray();
		}

		private TDestination[] CoreTransform(InternalStreamReader sourceFile, StreamWriter destFile)
		{
			FileHelperEngine<TSource> fileHelperEngine = new FileHelperEngine<TSource>(this.mSourceEncoding);
			FileHelperEngine<TDestination> fileHelperEngine2 = new FileHelperEngine<TDestination>(this.mDestinationEncoding);
			fileHelperEngine.ErrorMode = this.ErrorMode;
			fileHelperEngine2.ErrorManager.ErrorMode = this.ErrorMode;
			this.mSourceErrorManager = fileHelperEngine.ErrorManager;
			this.mDestinationErrorManager = fileHelperEngine2.ErrorManager;
			TSource[] array = fileHelperEngine.ReadStream(sourceFile);
			TDestination[] array2 = this.CoreTransformRecords(array);
			fileHelperEngine2.WriteStream(destFile, array2);
			return array2;
		}

		private TDestination[] CoreTransformRecords(TSource[] sourceRecords)
		{
			List<TDestination> list = new List<TDestination>(sourceRecords.Length);
			for (int i = 0; i < sourceRecords.Length; i++)
			{
				list.Add(sourceRecords[i].TransformTo());
			}
			return list.ToArray();
		}

		private TDestination[] CoreTransformFile(string sourceFile, string destFile)
		{
			TDestination[] array;
			using (InternalStreamReader internalStreamReader = new InternalStreamReader(sourceFile, this.mSourceEncoding, true, 1024000))
			{
				using (StreamWriter streamWriter = new StreamWriter(destFile, false, this.mDestinationEncoding, 1024000))
				{
					array = this.CoreTransform(internalStreamReader, streamWriter);
					streamWriter.Close();
				}
				internalStreamReader.Close();
			}
			return array;
		}

		private int CoreTransformAsync(TextReader sourceFile, StreamWriter destFile)
		{
			FileHelperAsyncEngine<TSource> fileHelperAsyncEngine = new FileHelperAsyncEngine<TSource>();
			FileHelperAsyncEngine<TDestination> fileHelperAsyncEngine2 = new FileHelperAsyncEngine<TDestination>();
			fileHelperAsyncEngine.ErrorMode = this.ErrorMode;
			fileHelperAsyncEngine2.ErrorMode = this.ErrorMode;
			this.mSourceErrorManager = fileHelperAsyncEngine.ErrorManager;
			this.mDestinationErrorManager = fileHelperAsyncEngine2.ErrorManager;
			fileHelperAsyncEngine.Encoding = this.mSourceEncoding;
			fileHelperAsyncEngine2.Encoding = this.mDestinationEncoding;
			fileHelperAsyncEngine.BeginReadStream(sourceFile);
			fileHelperAsyncEngine2.BeginWriteStream(destFile);
			foreach (TSource tsource in ((IEnumerable<TSource>)fileHelperAsyncEngine))
			{
				fileHelperAsyncEngine2.WriteNext(tsource.TransformTo());
			}
			fileHelperAsyncEngine.Close();
			fileHelperAsyncEngine2.Close();
			return fileHelperAsyncEngine.TotalRecords;
		}

		public Type SourceType
		{
			get
			{
				return typeof(TSource);
			}
		}

		public Type DestinationType
		{
			get
			{
				return typeof(TDestination);
			}
		}

		public Encoding SourceEncoding
		{
			get
			{
				return this.mSourceEncoding;
			}
			set
			{
				this.mSourceEncoding = value;
			}
		}

		public Encoding DestinationEncoding
		{
			get
			{
				return this.mDestinationEncoding;
			}
			set
			{
				this.mDestinationEncoding = value;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static object[] mEmptyArray = new object[0];

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Encoding mSourceEncoding = Encoding.Default;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Encoding mDestinationEncoding = Encoding.Default;

		private ErrorMode mErrorMode;

		private ErrorManager mSourceErrorManager = new ErrorManager();

		private ErrorManager mDestinationErrorManager = new ErrorManager();
	}
}
