using System;
using System.IO;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Writer
{
	public abstract class AbstractWriter
	{
		public string Filename
		{
			get
			{
				return this._filename;
			}
			set
			{
				this._filename = value;
			}
		}

		public abstract void WriteFile();

		protected void OpenFile()
		{
			if (this._writer != null)
			{
				return;
			}
			if (File.Exists(this._filename))
			{
				try
				{
					File.Delete(this._filename);
				}
				catch (Exception ex)
				{
					throw new IOException("Unable to delete destination file", ex);
				}
			}
			BufferedStream bufferedStream;
			try
			{
				bufferedStream = new BufferedStream(new FileStream(this._filename, FileMode.Create));
			}
			catch (Exception ex2)
			{
				throw new IOException("Unable to create destination file", ex2);
			}
			this._writer = new BinaryWriter(bufferedStream);
		}

		protected void CloseFile()
		{
			try
			{
				this._writer.Flush();
				this._writer.Close();
				this._writer = null;
			}
			catch (Exception ex)
			{
				throw new IOException("Unable to release stream", ex);
			}
		}

		protected string _filename;

		protected BinaryWriter _writer;
	}
}
