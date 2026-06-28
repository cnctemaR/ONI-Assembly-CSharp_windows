using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Threading;
using Mono.Audio;

namespace System.Media
{
	[global::System.ComponentModel.ToolboxItem(false)]
	[Serializable]
	public class SoundPlayer : global::System.ComponentModel.Component, ISerializable
	{
		public SoundPlayer()
		{
			this.sound_location = string.Empty;
		}

		public SoundPlayer(Stream stream)
			: this()
		{
			this.audiostream = stream;
		}

		public SoundPlayer(string soundLocation)
			: this()
		{
			if (soundLocation == null)
			{
				throw new ArgumentNullException("soundLocation");
			}
			this.sound_location = soundLocation;
		}

		protected SoundPlayer(SerializationInfo serializationInfo, StreamingContext context)
			: this()
		{
			throw new NotImplementedException();
		}

		public event global::System.ComponentModel.AsyncCompletedEventHandler LoadCompleted;

		public event EventHandler SoundLocationChanged;

		public event EventHandler StreamChanged;

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
		}

		private void LoadFromStream(Stream s)
		{
			this.mstream = new MemoryStream();
			byte[] array = new byte[4096];
			int num;
			while ((num = s.Read(array, 0, 4096)) > 0)
			{
				this.mstream.Write(array, 0, num);
			}
			this.mstream.Position = 0L;
		}

		private void LoadFromUri(string location)
		{
			this.mstream = null;
			if (string.IsNullOrEmpty(location))
			{
				return;
			}
			Stream stream;
			if (File.Exists(location))
			{
				stream = new FileStream(location, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			else
			{
				global::System.Net.WebRequest webRequest = global::System.Net.WebRequest.Create(location);
				stream = webRequest.GetResponse().GetResponseStream();
			}
			using (stream)
			{
				this.LoadFromStream(stream);
			}
		}

		public void Load()
		{
			if (this.load_completed)
			{
				return;
			}
			if (this.audiostream != null)
			{
				this.LoadFromStream(this.audiostream);
			}
			else
			{
				this.LoadFromUri(this.sound_location);
			}
			this.adata = null;
			this.adev = null;
			this.load_completed = true;
			global::System.ComponentModel.AsyncCompletedEventArgs e = new global::System.ComponentModel.AsyncCompletedEventArgs(null, false, this);
			this.OnLoadCompleted(e);
			if (this.LoadCompleted != null)
			{
				this.LoadCompleted(this, e);
			}
			if (SoundPlayer.use_win32_player)
			{
				if (this.win32_player == null)
				{
					this.win32_player = new Mono.Audio.Win32SoundPlayer(this.mstream);
				}
				else
				{
					this.win32_player.Stream = this.mstream;
				}
			}
		}

		private void AsyncFinished(IAsyncResult ar)
		{
			ThreadStart threadStart = ar.AsyncState as ThreadStart;
			threadStart.EndInvoke(ar);
		}

		public void LoadAsync()
		{
			if (this.load_completed)
			{
				return;
			}
			ThreadStart threadStart = new ThreadStart(this.Load);
			threadStart.BeginInvoke(new AsyncCallback(this.AsyncFinished), threadStart);
		}

		protected virtual void OnLoadCompleted(global::System.ComponentModel.AsyncCompletedEventArgs e)
		{
		}

		protected virtual void OnSoundLocationChanged(EventArgs e)
		{
		}

		protected virtual void OnStreamChanged(EventArgs e)
		{
		}

		private void Start()
		{
			if (!SoundPlayer.use_win32_player)
			{
				this.stopped = false;
				if (this.adata != null)
				{
					this.adata.IsStopped = false;
				}
			}
			if (!this.load_completed)
			{
				this.Load();
			}
		}

		public void Play()
		{
			if (!SoundPlayer.use_win32_player)
			{
				ThreadStart threadStart = new ThreadStart(this.PlaySync);
				threadStart.BeginInvoke(new AsyncCallback(this.AsyncFinished), threadStart);
			}
			else
			{
				this.Start();
				if (this.mstream == null)
				{
					SystemSounds.Beep.Play();
					return;
				}
				this.win32_player.Play();
			}
		}

		private void PlayLoop()
		{
			this.Start();
			if (this.mstream == null)
			{
				SystemSounds.Beep.Play();
				return;
			}
			while (!this.stopped)
			{
				this.PlaySync();
			}
		}

		public void PlayLooping()
		{
			if (!SoundPlayer.use_win32_player)
			{
				ThreadStart threadStart = new ThreadStart(this.PlayLoop);
				threadStart.BeginInvoke(new AsyncCallback(this.AsyncFinished), threadStart);
			}
			else
			{
				this.Start();
				if (this.mstream == null)
				{
					SystemSounds.Beep.Play();
					return;
				}
				this.win32_player.PlayLooping();
			}
		}

		public void PlaySync()
		{
			this.Start();
			if (this.mstream == null)
			{
				SystemSounds.Beep.Play();
				return;
			}
			if (!SoundPlayer.use_win32_player)
			{
				try
				{
					if (this.adata == null)
					{
						this.adata = new Mono.Audio.WavData(this.mstream);
					}
					if (this.adev == null)
					{
						this.adev = Mono.Audio.AudioDevice.CreateDevice(null);
					}
					if (this.adata != null)
					{
						this.adata.Setup(this.adev);
						this.adata.Play(this.adev);
					}
				}
				catch
				{
				}
			}
			else
			{
				this.win32_player.PlaySync();
			}
		}

		public void Stop()
		{
			if (!SoundPlayer.use_win32_player)
			{
				this.stopped = true;
				if (this.adata != null)
				{
					this.adata.IsStopped = true;
				}
			}
			else
			{
				this.win32_player.Stop();
			}
		}

		public bool IsLoadCompleted
		{
			get
			{
				return this.load_completed;
			}
		}

		public int LoadTimeout
		{
			get
			{
				return this.load_timeout;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("timeout must be >= 0");
				}
				this.load_timeout = value;
			}
		}

		public string SoundLocation
		{
			get
			{
				return this.sound_location;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.sound_location = value;
				this.load_completed = false;
				this.OnSoundLocationChanged(EventArgs.Empty);
				if (this.SoundLocationChanged != null)
				{
					this.SoundLocationChanged(this, EventArgs.Empty);
				}
			}
		}

		public Stream Stream
		{
			get
			{
				return this.audiostream;
			}
			set
			{
				if (this.audiostream != value)
				{
					this.audiostream = value;
					this.load_completed = false;
					this.OnStreamChanged(EventArgs.Empty);
					if (this.StreamChanged != null)
					{
						this.StreamChanged(this, EventArgs.Empty);
					}
				}
			}
		}

		public object Tag
		{
			get
			{
				return this.tag;
			}
			set
			{
				this.tag = value;
			}
		}

		private string sound_location;

		private Stream audiostream;

		private object tag = string.Empty;

		private MemoryStream mstream;

		private bool load_completed;

		private int load_timeout = 10000;

		private Mono.Audio.AudioDevice adev;

		private Mono.Audio.AudioData adata;

		private bool stopped;

		private Mono.Audio.Win32SoundPlayer win32_player;

		private static readonly bool use_win32_player = Environment.OSVersion.Platform != PlatformID.Unix;
	}
}
