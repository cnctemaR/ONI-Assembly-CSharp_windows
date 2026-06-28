using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class DiskArchiveStorage : BaseArchiveStorage
	{
		public DiskArchiveStorage(ZipFile file, FileUpdateMode updateMode)
			: base(updateMode)
		{
			if (file.Name == null)
			{
				throw new ZipException("Cant handle non file archives");
			}
			this.fileName_ = file.Name;
		}

		public DiskArchiveStorage(ZipFile file)
			: this(file, FileUpdateMode.Safe)
		{
		}

		public override Stream GetTemporaryOutput()
		{
			if (this.temporaryStream_ != null)
			{
				return this.temporaryStream_;
			}
			string tempFileName = Path.GetTempFileName();
			this.temporaryStream_ = File.Open(tempFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
			return this.temporaryStream_;
		}

		public override Stream ConvertTemporaryToFinal()
		{
			Stream temporaryOutput = this.GetTemporaryOutput();
			if (temporaryOutput == null || !(temporaryOutput is FileStream))
			{
				throw new ZipException("No temporary stream has been created");
			}
			Stream stream = null;
			string name = ((FileStream)temporaryOutput).Name;
			string tempFileName = DiskArchiveStorage.GetTempFileName(this.fileName_, false);
			bool flag = false;
			try
			{
				temporaryOutput.Close();
				File.Move(this.fileName_, tempFileName);
				File.Move(name, this.fileName_);
				flag = true;
				File.Delete(tempFileName);
				stream = File.Open(this.fileName_, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			catch (Exception)
			{
				stream = null;
				if (!flag)
				{
					File.Move(tempFileName, this.fileName_);
					File.Delete(tempFileName);
				}
				throw;
			}
			return stream;
		}

		public override Stream MakeTemporaryCopy(Stream stream)
		{
			stream.Close();
			string tempFileName = DiskArchiveStorage.GetTempFileName(this.fileName_, true);
			File.Copy(this.fileName_, tempFileName, true);
			if (this.temporaryStream_ != null)
			{
				this.temporaryStream_.Close();
			}
			this.temporaryStream_ = new FileStream(tempFileName, FileMode.Open, FileAccess.ReadWrite);
			return this.temporaryStream_;
		}

		public override Stream OpenForDirectUpdate(Stream stream)
		{
			Stream stream2;
			if (stream == null || !stream.CanWrite)
			{
				if (stream != null)
				{
					stream.Close();
				}
				stream2 = new FileStream(this.fileName_, FileMode.Open, FileAccess.ReadWrite);
			}
			else
			{
				stream2 = stream;
			}
			return stream2;
		}

		public override void Dispose()
		{
			if (this.temporaryStream_ != null)
			{
				this.temporaryStream_.Close();
			}
		}

		private static string GetTempFileName(string original, bool makeTempFile)
		{
			string text = null;
			if (original == null)
			{
				text = Path.GetTempFileName();
			}
			else
			{
				int num = 0;
				int num2 = DateTime.Now.Second;
				while (text == null)
				{
					num++;
					string text2 = string.Format("{0}.{1}{2}.tmp", original, num2, num);
					if (!File.Exists(text2))
					{
						if (makeTempFile)
						{
							try
							{
								using (File.Create(text2))
								{
								}
								text = text2;
							}
							catch
							{
								num2 = DateTime.Now.Second;
							}
						}
						else
						{
							text = text2;
						}
					}
				}
			}
			return text;
		}

		private Stream temporaryStream_;

		private string fileName_;
	}
}
