using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.IO
{
	[ComVisible(true)]
	[PermissionSet(SecurityAction.InheritanceDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Unrestricted=\"true\"/>\n</PermissionSet>\n")]
	[Serializable]
	public abstract class FileSystemInfo : MarshalByRefObject, ISerializable
	{
		protected FileSystemInfo()
		{
			this.valid = false;
			this.FullPath = null;
		}

		protected FileSystemInfo(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this.FullPath = info.GetString("FullPath");
			this.OriginalPath = info.GetString("OriginalPath");
		}

		[ComVisible(false)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("OriginalPath", this.OriginalPath, typeof(string));
			info.AddValue("FullPath", this.FullPath, typeof(string));
		}

		public abstract bool Exists { get; }

		public abstract string Name { get; }

		public virtual string FullName
		{
			get
			{
				return this.FullPath;
			}
		}

		public string Extension
		{
			get
			{
				return Path.GetExtension(this.Name);
			}
		}

		public FileAttributes Attributes
		{
			get
			{
				this.Refresh(false);
				return this.stat.Attributes;
			}
			set
			{
				MonoIOError monoIOError;
				if (!MonoIO.SetFileAttributes(this.FullName, value, out monoIOError))
				{
					throw MonoIO.GetException(this.FullName, monoIOError);
				}
				this.Refresh(true);
			}
		}

		public DateTime CreationTime
		{
			get
			{
				this.Refresh(false);
				return DateTime.FromFileTime(this.stat.CreationTime);
			}
			set
			{
				long num = value.ToFileTime();
				MonoIOError monoIOError;
				if (!MonoIO.SetFileTime(this.FullName, num, -1L, -1L, out monoIOError))
				{
					throw MonoIO.GetException(this.FullName, monoIOError);
				}
				this.Refresh(true);
			}
		}

		[ComVisible(false)]
		public DateTime CreationTimeUtc
		{
			get
			{
				return this.CreationTime.ToUniversalTime();
			}
			set
			{
				this.CreationTime = value.ToLocalTime();
			}
		}

		public DateTime LastAccessTime
		{
			get
			{
				this.Refresh(false);
				return DateTime.FromFileTime(this.stat.LastAccessTime);
			}
			set
			{
				long num = value.ToFileTime();
				MonoIOError monoIOError;
				if (!MonoIO.SetFileTime(this.FullName, -1L, num, -1L, out monoIOError))
				{
					throw MonoIO.GetException(this.FullName, monoIOError);
				}
				this.Refresh(true);
			}
		}

		[ComVisible(false)]
		public DateTime LastAccessTimeUtc
		{
			get
			{
				this.Refresh(false);
				return this.LastAccessTime.ToUniversalTime();
			}
			set
			{
				this.LastAccessTime = value.ToLocalTime();
			}
		}

		public DateTime LastWriteTime
		{
			get
			{
				this.Refresh(false);
				return DateTime.FromFileTime(this.stat.LastWriteTime);
			}
			set
			{
				long num = value.ToFileTime();
				MonoIOError monoIOError;
				if (!MonoIO.SetFileTime(this.FullName, -1L, -1L, num, out monoIOError))
				{
					throw MonoIO.GetException(this.FullName, monoIOError);
				}
				this.Refresh(true);
			}
		}

		[ComVisible(false)]
		public DateTime LastWriteTimeUtc
		{
			get
			{
				this.Refresh(false);
				return this.LastWriteTime.ToUniversalTime();
			}
			set
			{
				this.LastWriteTime = value.ToLocalTime();
			}
		}

		public abstract void Delete();

		public void Refresh()
		{
			this.Refresh(true);
		}

		internal void Refresh(bool force)
		{
			if (this.valid && !force)
			{
				return;
			}
			MonoIOError monoIOError;
			MonoIO.GetFileStat(this.FullName, out this.stat, out monoIOError);
			this.valid = true;
			this.InternalRefresh();
		}

		internal virtual void InternalRefresh()
		{
		}

		internal void CheckPath(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("An empty file name is not valid.");
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Illegal characters in path.");
			}
		}

		protected string FullPath;

		protected string OriginalPath;

		internal MonoIOStat stat;

		internal bool valid;
	}
}
