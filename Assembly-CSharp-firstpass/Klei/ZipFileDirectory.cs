using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Ionic.Zip;

namespace Klei
{
	public class ZipFileDirectory : IFileDirectory
	{
		public string GetID()
		{
			return this.id;
		}

		public ZipFileDirectory(string id, ZipFile zipfile, string mount_point = "", bool isModded = false)
		{
			this.id = id;
			this.isModded = isModded;
			this.mountPoint = FileSystem.Normalize(mount_point);
			this.zipfile = zipfile;
		}

		public ZipFileDirectory(string id, Stream zip_data_stream, string mount_point = "", bool isModded = false)
			: this(id, ZipFile.Read(zip_data_stream), mount_point, isModded)
		{
		}

		public string MountPoint
		{
			get
			{
				return this.mountPoint;
			}
		}

		public string GetRoot()
		{
			return this.MountPoint;
		}

		public byte[] ReadBytes(string filename)
		{
			if (this.mountPoint.Length > 0)
			{
				filename = filename.Substring(this.mountPoint.Length);
			}
			ZipEntry zipEntry = this.zipfile[filename];
			if (zipEntry == null)
			{
				return null;
			}
			MemoryStream memoryStream = new MemoryStream();
			zipEntry.Extract(memoryStream);
			return memoryStream.ToArray();
		}

		public void GetFiles(Regex re, string path, ICollection<string> result)
		{
			if (this.zipfile.Count <= 0)
			{
				return;
			}
			foreach (ZipEntry zipEntry in this.zipfile.Entries)
			{
				if (!zipEntry.IsDirectory)
				{
					string text = FileSystem.Normalize(Path.Combine(this.mountPoint, zipEntry.FileName));
					if (re.IsMatch(text))
					{
						result.Add(text);
					}
				}
			}
		}

		public bool FileExists(string path)
		{
			if (this.mountPoint.Length > 0)
			{
				if (this.mountPoint.Length > path.Length)
				{
					Debug.LogError("Tried finding an invalid path inside a matching mount point!\n" + path + "\n" + this.mountPoint);
				}
				path = path.Substring(this.mountPoint.Length);
			}
			return this.zipfile.ContainsEntry(path);
		}

		public FileHandle FindFileHandle(string path)
		{
			if (this.FileExists(path))
			{
				if (this.mountPoint.Length > 0)
				{
					if (this.mountPoint.Length > path.Length)
					{
						Debug.LogError("Tried finding an invalid path inside a matching mount point!\n" + path + "\n" + this.mountPoint);
					}
					path = path.Substring(this.mountPoint.Length);
				}
				return new FileHandle
				{
					full_path = FileSystem.Normalize(Path.Combine(this.mountPoint, path)),
					source = this
				};
			}
			return default(FileHandle);
		}

		public bool IsModded()
		{
			return this.isModded;
		}

		private string id;

		private string mountPoint;

		private ZipFile zipfile;

		private bool isModded;
	}
}
