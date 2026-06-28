using System;
using System.Text;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public sealed class UnixSymbolicLinkInfo : UnixFileSystemInfo
	{
		public UnixSymbolicLinkInfo(string path)
			: base(path)
		{
		}

		internal UnixSymbolicLinkInfo(string path, Stat stat)
			: base(path, stat)
		{
		}

		public override string Name
		{
			get
			{
				return UnixPath.GetFileName(base.FullPath);
			}
		}

		[Obsolete("Use GetContents()")]
		public UnixFileSystemInfo Contents
		{
			get
			{
				return this.GetContents();
			}
		}

		public string ContentsPath
		{
			get
			{
				return this.ReadLink();
			}
		}

		public bool HasContents
		{
			get
			{
				return this.TryReadLink() != null;
			}
		}

		public UnixFileSystemInfo GetContents()
		{
			string text = this.ReadLink();
			return UnixFileSystemInfo.GetFileSystemEntry(UnixPath.Combine(UnixPath.GetDirectoryName(base.FullPath), new string[] { this.ContentsPath }));
		}

		public void CreateSymbolicLinkTo(string path)
		{
			int num = Syscall.symlink(path, this.FullName);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public void CreateSymbolicLinkTo(UnixFileSystemInfo path)
		{
			int num = Syscall.symlink(path.FullName, this.FullName);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public override void Delete()
		{
			int num = Syscall.unlink(base.FullPath);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
			base.Refresh();
		}

		public override void SetOwner(long owner, long group)
		{
			int num = Syscall.lchown(base.FullPath, Convert.ToUInt32(owner), Convert.ToUInt32(group));
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		protected override bool GetFileStatus(string path, out Stat stat)
		{
			return Syscall.lstat(path, out stat) == 0;
		}

		private string ReadLink()
		{
			string text = this.TryReadLink();
			if (text == null)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return text;
		}

		private string TryReadLink()
		{
			StringBuilder stringBuilder = new StringBuilder((int)base.Length + 1);
			int num = Syscall.readlink(base.FullPath, stringBuilder);
			if (num == -1)
			{
				return null;
			}
			return stringBuilder.ToString(0, num);
		}
	}
}
