using System;
using System.IO;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixConnection
	{
		public UnixConnection(HostConnectionPool pool, ReusableUnixClient client)
		{
			this._pool = pool;
			this._client = client;
			this._stream = new BufferedStream(client.GetStream());
			this._controlTime = DateTime.Now;
			this._buffer = new byte[UnixMessageIO.DefaultStreamBufferSize];
		}

		public Stream Stream
		{
			get
			{
				return this._stream;
			}
		}

		public DateTime ControlTime
		{
			get
			{
				return this._controlTime;
			}
			set
			{
				this._controlTime = value;
			}
		}

		public bool IsAlive
		{
			get
			{
				return this._client.IsAlive;
			}
		}

		public byte[] Buffer
		{
			get
			{
				return this._buffer;
			}
		}

		public void Release()
		{
			this._pool.ReleaseConnection(this);
		}

		public void Close()
		{
			this._client.Close();
		}

		private DateTime _controlTime;

		private Stream _stream;

		private ReusableUnixClient _client;

		private HostConnectionPool _pool;

		private byte[] _buffer;
	}
}
