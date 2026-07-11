using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public abstract class TextReader : MarshalByRefObject, IDisposable
	{
		public virtual void Close()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public virtual int Peek()
		{
			return -1;
		}

		public virtual int Read()
		{
			return -1;
		}

		public virtual int Read([In] [Out] char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("Buffer cannot be null."));
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("Non-negative number required."));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("Non-negative number required."));
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
			}
			int num = 0;
			do
			{
				int num2 = this.Read();
				if (num2 == -1)
				{
					break;
				}
				buffer[index + num++] = (char)num2;
			}
			while (num < count);
			return num;
		}

		public virtual string ReadToEnd()
		{
			char[] array = new char[4096];
			StringBuilder stringBuilder = new StringBuilder(4096);
			int num;
			while ((num = this.Read(array, 0, array.Length)) != 0)
			{
				stringBuilder.Append(array, 0, num);
			}
			return stringBuilder.ToString();
		}

		public virtual int ReadBlock([In] [Out] char[] buffer, int index, int count)
		{
			int num = 0;
			int num2;
			do
			{
				num += (num2 = this.Read(buffer, index + num, count - num));
			}
			while (num2 > 0 && num < count);
			return num;
		}

		public virtual string ReadLine()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num;
			for (;;)
			{
				num = this.Read();
				if (num == -1)
				{
					goto IL_0043;
				}
				if (num == 13 || num == 10)
				{
					break;
				}
				stringBuilder.Append((char)num);
			}
			if (num == 13 && this.Peek() == 10)
			{
				this.Read();
			}
			return stringBuilder.ToString();
			IL_0043:
			if (stringBuilder.Length > 0)
			{
				return stringBuilder.ToString();
			}
			return null;
		}

		[ComVisible(false)]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public virtual Task<string> ReadLineAsync()
		{
			return Task<string>.Factory.StartNew(TextReader._ReadLineDelegate, this, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
		}

		[ComVisible(false)]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public virtual async Task<string> ReadToEndAsync()
		{
			char[] chars = new char[4096];
			StringBuilder sb = new StringBuilder(4096);
			for (;;)
			{
				int num = await this.ReadAsyncInternal(chars, 0, chars.Length).ConfigureAwait(false);
				int len;
				if ((len = num) == 0)
				{
					break;
				}
				sb.Append(chars, 0, len);
			}
			return sb.ToString();
		}

		[ComVisible(false)]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public virtual Task<int> ReadAsync(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("Buffer cannot be null."));
			}
			if (index < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", Environment.GetResourceString("Non-negative number required."));
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
			}
			return this.ReadAsyncInternal(buffer, index, count);
		}

		internal virtual Task<int> ReadAsyncInternal(char[] buffer, int index, int count)
		{
			Tuple<TextReader, char[], int, int> tuple = new Tuple<TextReader, char[], int, int>(this, buffer, index, count);
			return Task<int>.Factory.StartNew(TextReader._ReadDelegate, tuple, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
		}

		[ComVisible(false)]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public virtual Task<int> ReadBlockAsync(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("Buffer cannot be null."));
			}
			if (index < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", Environment.GetResourceString("Non-negative number required."));
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
			}
			return this.ReadBlockAsyncInternal(buffer, index, count);
		}

		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		private async Task<int> ReadBlockAsyncInternal(char[] buffer, int index, int count)
		{
			int i = 0;
			int num;
			do
			{
				num = await this.ReadAsyncInternal(buffer, index + i, count - i).ConfigureAwait(false);
				i += num;
			}
			while (num > 0 && i < count);
			return i;
		}

		[HostProtection(SecurityAction.LinkDemand, Synchronization = true)]
		public static TextReader Synchronized(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader is TextReader.SyncTextReader)
			{
				return reader;
			}
			return new TextReader.SyncTextReader(reader);
		}

		[NonSerialized]
		private static Func<object, string> _ReadLineDelegate = (object state) => ((TextReader)state).ReadLine();

		[NonSerialized]
		private static Func<object, int> _ReadDelegate = delegate(object state)
		{
			Tuple<TextReader, char[], int, int> tuple = (Tuple<TextReader, char[], int, int>)state;
			return tuple.Item1.Read(tuple.Item2, tuple.Item3, tuple.Item4);
		};

		public static readonly TextReader Null = new TextReader.NullTextReader();

		[Serializable]
		private sealed class NullTextReader : TextReader
		{
			public override int Read(char[] buffer, int index, int count)
			{
				return 0;
			}

			public override string ReadLine()
			{
				return null;
			}
		}

		[Serializable]
		internal sealed class SyncTextReader : TextReader
		{
			internal SyncTextReader(TextReader t)
			{
				this._in = t;
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Close()
			{
				this._in.Close();
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					((IDisposable)this._in).Dispose();
				}
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override int Peek()
			{
				return this._in.Peek();
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override int Read()
			{
				return this._in.Read();
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override int Read([In] [Out] char[] buffer, int index, int count)
			{
				return this._in.Read(buffer, index, count);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override int ReadBlock([In] [Out] char[] buffer, int index, int count)
			{
				return this._in.ReadBlock(buffer, index, count);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override string ReadLine()
			{
				return this._in.ReadLine();
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override string ReadToEnd()
			{
				return this._in.ReadToEnd();
			}

			[ComVisible(false)]
			[MethodImpl(MethodImplOptions.Synchronized)]
			public override Task<string> ReadLineAsync()
			{
				return Task.FromResult<string>(this.ReadLine());
			}

			[ComVisible(false)]
			[MethodImpl(MethodImplOptions.Synchronized)]
			public override Task<string> ReadToEndAsync()
			{
				return Task.FromResult<string>(this.ReadToEnd());
			}

			[ComVisible(false)]
			[MethodImpl(MethodImplOptions.Synchronized)]
			public override Task<int> ReadBlockAsync(char[] buffer, int index, int count)
			{
				if (buffer == null)
				{
					throw new ArgumentNullException("buffer", Environment.GetResourceString("Buffer cannot be null."));
				}
				if (index < 0 || count < 0)
				{
					throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", Environment.GetResourceString("Non-negative number required."));
				}
				if (buffer.Length - index < count)
				{
					throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
				}
				return Task.FromResult<int>(this.ReadBlock(buffer, index, count));
			}

			[ComVisible(false)]
			[MethodImpl(MethodImplOptions.Synchronized)]
			public override Task<int> ReadAsync(char[] buffer, int index, int count)
			{
				if (buffer == null)
				{
					throw new ArgumentNullException("buffer", Environment.GetResourceString("Buffer cannot be null."));
				}
				if (index < 0 || count < 0)
				{
					throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", Environment.GetResourceString("Non-negative number required."));
				}
				if (buffer.Length - index < count)
				{
					throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
				}
				return Task.FromResult<int>(this.Read(buffer, index, count));
			}

			internal TextReader _in;
		}
	}
}
