using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace System.CodeDom.Compiler
{
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[Serializable]
	public class TempFileCollection : IDisposable, ICollection, IEnumerable
	{
		public TempFileCollection()
			: this(string.Empty, false)
		{
		}

		public TempFileCollection(string tempDir)
			: this(tempDir, false)
		{
		}

		public TempFileCollection(string tempDir, bool keepFiles)
		{
			this.filehash = new Hashtable();
			this.tempdir = ((tempDir != null) ? tempDir : string.Empty);
			this.keepfiles = keepFiles;
		}

		int ICollection.Count
		{
			get
			{
				return this.filehash.Count;
			}
		}

		void ICollection.CopyTo(Array array, int start)
		{
			this.filehash.Keys.CopyTo(array, start);
		}

		object ICollection.SyncRoot
		{
			get
			{
				return null;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.filehash.Keys.GetEnumerator();
		}

		public string BasePath
		{
			get
			{
				if (this.basepath == null)
				{
					if (this.rnd == null)
					{
						this.rnd = new Random();
					}
					string text = this.tempdir;
					if (text.Length == 0)
					{
						text = this.GetOwnTempDir();
					}
					FileStream fileStream = null;
					do
					{
						int num = this.rnd.Next();
						this.basepath = Path.Combine(text, (num + 1).ToString("x"));
						string text2 = this.basepath + ".tmp";
						try
						{
							fileStream = new FileStream(text2, FileMode.CreateNew);
						}
						catch (IOException)
						{
							fileStream = null;
						}
						catch
						{
							throw;
						}
					}
					while (fileStream == null);
					fileStream.Close();
					if (SecurityManager.SecurityEnabled)
					{
						new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this.basepath).Demand();
					}
				}
				return this.basepath;
			}
		}

		private string GetOwnTempDir()
		{
			if (this.ownTempDir != null)
			{
				return this.ownTempDir;
			}
			string tempPath = Path.GetTempPath();
			int num = -1;
			bool flag = false;
			switch (Environment.OSVersion.Platform)
			{
			case PlatformID.Win32S:
			case PlatformID.Win32Windows:
			case PlatformID.Win32NT:
			case PlatformID.WinCE:
				flag = true;
				num = 0;
				break;
			}
			for (;;)
			{
				int num2 = this.rnd.Next();
				this.ownTempDir = Path.Combine(tempPath, (num2 + 1).ToString("x"));
				if (!Directory.Exists(this.ownTempDir))
				{
					if (flag)
					{
						Directory.CreateDirectory(this.ownTempDir);
					}
					else
					{
						num = TempFileCollection.mkdir(this.ownTempDir, 448U);
					}
					if (num != 0 && !Directory.Exists(this.ownTempDir))
					{
						break;
					}
				}
				if (num == 0)
				{
					goto Block_7;
				}
			}
			throw new IOException();
			Block_7:
			return this.ownTempDir;
		}

		public int Count
		{
			get
			{
				return this.filehash.Count;
			}
		}

		public bool KeepFiles
		{
			get
			{
				return this.keepfiles;
			}
			set
			{
				this.keepfiles = value;
			}
		}

		public string TempDir
		{
			get
			{
				return this.tempdir;
			}
		}

		public string AddExtension(string fileExtension)
		{
			return this.AddExtension(fileExtension, this.keepfiles);
		}

		public string AddExtension(string fileExtension, bool keepFile)
		{
			string text = this.BasePath + "." + fileExtension;
			this.AddFile(text, keepFile);
			return text;
		}

		public void AddFile(string fileName, bool keepFile)
		{
			this.filehash.Add(fileName, keepFile);
		}

		public void CopyTo(string[] fileNames, int start)
		{
			this.filehash.Keys.CopyTo(fileNames, start);
		}

		public void Delete()
		{
			bool flag = true;
			string[] array = new string[this.filehash.Count];
			this.filehash.Keys.CopyTo(array, 0);
			foreach (string text in array)
			{
				if (!(bool)this.filehash[text])
				{
					File.Delete(text);
					this.filehash.Remove(text);
				}
				else
				{
					flag = false;
				}
			}
			if (this.basepath != null)
			{
				string text2 = this.basepath + ".tmp";
				File.Delete(text2);
				this.basepath = null;
			}
			if (flag && this.ownTempDir != null)
			{
				Directory.Delete(this.ownTempDir, true);
				this.ownTempDir = null;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.filehash.Keys.GetEnumerator();
		}

		protected virtual void Dispose(bool disposing)
		{
			this.Delete();
			if (disposing)
			{
				GC.SuppressFinalize(true);
			}
		}

		~TempFileCollection()
		{
			this.Dispose(false);
		}

		[DllImport("libc")]
		private static extern int mkdir(string olpath, uint mode);

		private Hashtable filehash;

		private string tempdir;

		private bool keepfiles;

		private string basepath;

		private Random rnd;

		private string ownTempDir;
	}
}
