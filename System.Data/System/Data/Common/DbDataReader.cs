using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Common
{
	public abstract class DbDataReader : MarshalByRefObject, IDataReader, IDisposable, IDataRecord, IEnumerable, IAsyncDisposable
	{
		public abstract int Depth { get; }

		public abstract int FieldCount { get; }

		public abstract bool HasRows { get; }

		public abstract bool IsClosed { get; }

		public abstract int RecordsAffected { get; }

		public virtual int VisibleFieldCount
		{
			get
			{
				return this.FieldCount;
			}
		}

		public abstract object this[int ordinal] { get; }

		public abstract object this[string name] { get; }

		public virtual void Close()
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
		}

		public abstract string GetDataTypeName(int ordinal);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract IEnumerator GetEnumerator();

		public abstract Type GetFieldType(int ordinal);

		public abstract string GetName(int ordinal);

		public abstract int GetOrdinal(string name);

		public virtual DataTable GetSchemaTable()
		{
			throw new NotSupportedException();
		}

		public abstract bool GetBoolean(int ordinal);

		public abstract byte GetByte(int ordinal);

		public abstract long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length);

		public abstract char GetChar(int ordinal);

		public abstract long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public DbDataReader GetData(int ordinal)
		{
			return this.GetDbDataReader(ordinal);
		}

		IDataReader IDataRecord.GetData(int ordinal)
		{
			return this.GetDbDataReader(ordinal);
		}

		protected virtual DbDataReader GetDbDataReader(int ordinal)
		{
			throw ADP.NotSupported();
		}

		public abstract DateTime GetDateTime(int ordinal);

		public abstract decimal GetDecimal(int ordinal);

		public abstract double GetDouble(int ordinal);

		public abstract float GetFloat(int ordinal);

		public abstract Guid GetGuid(int ordinal);

		public abstract short GetInt16(int ordinal);

		public abstract int GetInt32(int ordinal);

		public abstract long GetInt64(int ordinal);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual Type GetProviderSpecificFieldType(int ordinal)
		{
			return this.GetFieldType(ordinal);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual object GetProviderSpecificValue(int ordinal)
		{
			return this.GetValue(ordinal);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual int GetProviderSpecificValues(object[] values)
		{
			return this.GetValues(values);
		}

		public abstract string GetString(int ordinal);

		public virtual Stream GetStream(int ordinal)
		{
			Stream stream;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				long num = 0L;
				byte[] array = new byte[4096];
				long bytes;
				do
				{
					bytes = this.GetBytes(ordinal, num, array, 0, array.Length);
					memoryStream.Write(array, 0, (int)bytes);
					num += bytes;
				}
				while (bytes > 0L);
				stream = new MemoryStream(memoryStream.ToArray(), false);
			}
			return stream;
		}

		public virtual TextReader GetTextReader(int ordinal)
		{
			if (this.IsDBNull(ordinal))
			{
				return new StringReader(string.Empty);
			}
			return new StringReader(this.GetString(ordinal));
		}

		public abstract object GetValue(int ordinal);

		public virtual T GetFieldValue<T>(int ordinal)
		{
			return (T)((object)this.GetValue(ordinal));
		}

		public Task<T> GetFieldValueAsync<T>(int ordinal)
		{
			return this.GetFieldValueAsync<T>(ordinal, CancellationToken.None);
		}

		public virtual Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return ADP.CreatedTaskWithCancellation<T>();
			}
			Task<T> task;
			try
			{
				task = Task.FromResult<T>(this.GetFieldValue<T>(ordinal));
			}
			catch (Exception ex)
			{
				task = Task.FromException<T>(ex);
			}
			return task;
		}

		public abstract int GetValues(object[] values);

		public abstract bool IsDBNull(int ordinal);

		public Task<bool> IsDBNullAsync(int ordinal)
		{
			return this.IsDBNullAsync(ordinal, CancellationToken.None);
		}

		public virtual Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return ADP.CreatedTaskWithCancellation<bool>();
			}
			Task<bool> task;
			try
			{
				task = (this.IsDBNull(ordinal) ? ADP.TrueTask : ADP.FalseTask);
			}
			catch (Exception ex)
			{
				task = Task.FromException<bool>(ex);
			}
			return task;
		}

		public abstract bool NextResult();

		public abstract bool Read();

		public Task<bool> ReadAsync()
		{
			return this.ReadAsync(CancellationToken.None);
		}

		public virtual Task<bool> ReadAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return ADP.CreatedTaskWithCancellation<bool>();
			}
			Task<bool> task;
			try
			{
				task = (this.Read() ? ADP.TrueTask : ADP.FalseTask);
			}
			catch (Exception ex)
			{
				task = Task.FromException<bool>(ex);
			}
			return task;
		}

		public Task<bool> NextResultAsync()
		{
			return this.NextResultAsync(CancellationToken.None);
		}

		public virtual Task<bool> NextResultAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return ADP.CreatedTaskWithCancellation<bool>();
			}
			Task<bool> task;
			try
			{
				task = (this.NextResult() ? ADP.TrueTask : ADP.FalseTask);
			}
			catch (Exception ex)
			{
				task = Task.FromException<bool>(ex);
			}
			return task;
		}

		public virtual Task CloseAsync()
		{
			Task task;
			try
			{
				this.Close();
				task = Task.CompletedTask;
			}
			catch (Exception ex)
			{
				task = Task.FromException(ex);
			}
			return task;
		}

		public virtual ValueTask DisposeAsync()
		{
			this.Dispose();
			return default(ValueTask);
		}
	}
}
