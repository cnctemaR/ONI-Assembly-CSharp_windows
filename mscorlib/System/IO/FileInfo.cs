using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.AccessControl;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public sealed class FileInfo : FileSystemInfo
	{
		public FileInfo(string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			base.CheckPath(fileName);
			this.OriginalPath = fileName;
			this.FullPath = Path.GetFullPath(fileName);
		}

		private FileInfo(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		internal override void InternalRefresh()
		{
			this.exists = File.Exists(this.FullPath);
		}

		public override bool Exists
		{
			get
			{
				base.Refresh(false);
				return this.stat.Attributes != MonoIO.InvalidFileAttributes && (this.stat.Attributes & FileAttributes.Directory) == (FileAttributes)0 && this.exists;
			}
		}

		public override string Name
		{
			get
			{
				return Path.GetFileName(this.FullPath);
			}
		}

		public bool IsReadOnly
		{
			get
			{
				if (!this.Exists)
				{
					throw new FileNotFoundException("Could not find file \"" + this.OriginalPath + "\".", this.OriginalPath);
				}
				return (this.stat.Attributes & FileAttributes.ReadOnly) != (FileAttributes)0;
			}
			set
			{
				if (!this.Exists)
				{
					throw new FileNotFoundException("Could not find file \"" + this.OriginalPath + "\".", this.OriginalPath);
				}
				FileAttributes fileAttributes = File.GetAttributes(this.FullPath);
				if (value)
				{
					fileAttributes |= FileAttributes.ReadOnly;
				}
				else
				{
					fileAttributes &= ~FileAttributes.ReadOnly;
				}
				File.SetAttributes(this.FullPath, fileAttributes);
			}
		}

		[ComVisible(false)]
		[MonoLimitation("File encryption isn't supported (even on NTFS).")]
		public void Encrypt()
		{
			throw new NotSupportedException(Locale.GetText("File encryption isn't supported on any file system."));
		}

		[MonoLimitation("File encryption isn't supported (even on NTFS).")]
		[ComVisible(false)]
		public void Decrypt()
		{
			throw new NotSupportedException(Locale.GetText("File encryption isn't supported on any file system."));
		}

		public long Length
		{
			get
			{
				if (!this.Exists)
				{
					throw new FileNotFoundException("Could not find file \"" + this.OriginalPath + "\".", this.OriginalPath);
				}
				return this.stat.Length;
			}
		}

		public string DirectoryName
		{
			get
			{
				return Path.GetDirectoryName(this.FullPath);
			}
		}

		public DirectoryInfo Directory
		{
			get
			{
				return new DirectoryInfo(this.DirectoryName);
			}
		}

		public StreamReader OpenText()
		{
			return new StreamReader(this.Open(FileMode.Open, FileAccess.Read));
		}

		public StreamWriter CreateText()
		{
			return new StreamWriter(this.Open(FileMode.Create, FileAccess.Write));
		}

		public StreamWriter AppendText()
		{
			return new StreamWriter(this.Open(FileMode.Append, FileAccess.Write));
		}

		public FileStream Create()
		{
			return File.Create(this.FullPath);
		}

		public FileStream OpenRead()
		{
			return this.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		public FileStream OpenWrite()
		{
			return this.Open(FileMode.OpenOrCreate, FileAccess.Write);
		}

		public FileStream Open(FileMode mode)
		{
			return this.Open(mode, FileAccess.ReadWrite);
		}

		public FileStream Open(FileMode mode, FileAccess access)
		{
			return this.Open(mode, access, FileShare.None);
		}

		public FileStream Open(FileMode mode, FileAccess access, FileShare share)
		{
			return new FileStream(this.FullPath, mode, access, share);
		}

		public override void Delete()
		{
			MonoIOError monoIOError;
			if (!MonoIO.Exists(this.FullPath, out monoIOError))
			{
				return;
			}
			if (MonoIO.ExistsDirectory(this.FullPath, out monoIOError))
			{
				throw new UnauthorizedAccessException("Access to the path \"" + this.FullPath + "\" is denied.");
			}
			if (!MonoIO.DeleteFile(this.FullPath, out monoIOError))
			{
				throw MonoIO.GetException(this.OriginalPath, monoIOError);
			}
		}

		public void MoveTo(string destFileName)
		{
			if (destFileName == null)
			{
				throw new ArgumentNullException("destFileName");
			}
			if (destFileName == this.Name || destFileName == this.FullName)
			{
				return;
			}
			if (!File.Exists(this.FullPath))
			{
				throw new FileNotFoundException();
			}
			File.Move(this.FullPath, destFileName);
			this.FullPath = Path.GetFullPath(destFileName);
		}

		public FileInfo CopyTo(string destFileName)
		{
			return this.CopyTo(destFileName, false);
		}

		public FileInfo CopyTo(string destFileName, bool overwrite)
		{
			if (destFileName == null)
			{
				throw new ArgumentNullException("destFileName");
			}
			if (destFileName.Length == 0)
			{
				throw new ArgumentException("An empty file name is not valid.", "destFileName");
			}
			string fullPath = Path.GetFullPath(destFileName);
			if (overwrite && File.Exists(fullPath))
			{
				File.Delete(fullPath);
			}
			File.Copy(this.FullPath, fullPath);
			return new FileInfo(fullPath);
		}

		public override string ToString()
		{
			return this.OriginalPath;
		}

		public FileSecurity GetAccessControl()
		{
			throw new NotImplementedException();
		}

		public FileSecurity GetAccessControl(AccessControlSections includeSections)
		{
			throw new NotImplementedException();
		}

		[ComVisible(false)]
		public FileInfo Replace(string destinationFileName, string destinationBackupFileName)
		{
			if (!this.Exists)
			{
				throw new FileNotFoundException();
			}
			if (destinationFileName == null)
			{
				throw new ArgumentNullException("destinationFileName");
			}
			if (destinationFileName.Length == 0)
			{
				throw new ArgumentException("An empty file name is not valid.", "destinationFileName");
			}
			string fullPath = Path.GetFullPath(destinationFileName);
			if (!File.Exists(fullPath))
			{
				throw new FileNotFoundException();
			}
			FileAttributes attributes = File.GetAttributes(fullPath);
			if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
			{
				throw new UnauthorizedAccessException();
			}
			if (destinationBackupFileName != null)
			{
				if (destinationBackupFileName.Length == 0)
				{
					throw new ArgumentException("An empty file name is not valid.", "destinationBackupFileName");
				}
				File.Copy(fullPath, Path.GetFullPath(destinationBackupFileName), true);
			}
			File.Copy(this.FullPath, fullPath, true);
			File.Delete(this.FullPath);
			return new FileInfo(fullPath);
		}

		[ComVisible(false)]
		public FileInfo Replace(string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
		{
			throw new NotImplementedException();
		}

		public void SetAccessControl(FileSecurity fileSecurity)
		{
			throw new NotImplementedException();
		}

		private bool exists;
	}
}
