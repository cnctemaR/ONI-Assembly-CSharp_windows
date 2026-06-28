using System;
using System.Collections;
using System.IO;

namespace System.ComponentModel.Design.Serialization
{
	public abstract class SerializationStore : IDisposable
	{
		void IDisposable.Dispose()
		{
			this.Dispose(true);
		}

		public abstract ICollection Errors { get; }

		public abstract void Close();

		public abstract void Save(Stream stream);

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
		}
	}
}
