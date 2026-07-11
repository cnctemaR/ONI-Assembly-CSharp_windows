using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Ionic.Zip;

namespace Klei
{
	public class ZipFileSystem : IFileSystem
	{
		public ZipFileSystem(string id, ZipFile zipfile, string mount_point = "")
		{
			this.id = id;
			this.mountPoint = FSUtil.Normalize(mount_point);
			this.zipfile = zipfile;
		}

		public ZipFileSystem(string id, Stream zip_data_stream, string mount_point = "")
			: this(id, ZipFile.Read(zip_data_stream), mount_point)
		{
		}

		public string GetID()
		{
			return this.id;
		}

		public string MountPoint
		{
			get
			{
				return this.mountPoint;
			}
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
					string text = FSUtil.Normalize(Path.Combine(this.mountPoint, zipEntry.FileName));
					if (re.IsMatch(text))
					{
						result.Add(text);
					}
				}
			}
		}

		private string id;

		private string mountPoint;

		private ZipFile zipfile;
	}
}
