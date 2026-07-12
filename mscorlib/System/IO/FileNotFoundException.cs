using System;
using System.Runtime.Serialization;
using System.Security;

namespace System.IO
{
	[Serializable]
	public class FileNotFoundException : IOException
	{
		public FileNotFoundException()
			: base("Unable to find the specified file.")
		{
			base.HResult = -2147024894;
		}

		public FileNotFoundException(string message)
			: base(message)
		{
			base.HResult = -2147024894;
		}

		public FileNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2147024894;
		}

		public FileNotFoundException(string message, string fileName)
			: base(message)
		{
			base.HResult = -2147024894;
			this.FileName = fileName;
		}

		public FileNotFoundException(string message, string fileName, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2147024894;
			this.FileName = fileName;
		}

		public override string Message
		{
			get
			{
				this.SetMessageField();
				return this._message;
			}
		}

		private void SetMessageField()
		{
			if (this._message == null)
			{
				if (this.FileName == null && base.HResult == -2146233088)
				{
					this._message = "Unable to find the specified file.";
					return;
				}
				if (this.FileName != null)
				{
					this._message = FileLoadException.FormatFileLoadExceptionMessage(this.FileName, base.HResult);
				}
			}
		}

		public string FileName { get; }

		public string FusionLog { get; }

		public override string ToString()
		{
			string text = base.GetType().ToString() + ": " + this.Message;
			if (this.FileName != null && this.FileName.Length != 0)
			{
				text = text + Environment.NewLine + SR.Format("File name: '{0}'", this.FileName);
			}
			if (base.InnerException != null)
			{
				text = text + " ---> " + base.InnerException.ToString();
			}
			if (this.StackTrace != null)
			{
				text = text + Environment.NewLine + this.StackTrace;
			}
			if (this.FusionLog != null)
			{
				if (text == null)
				{
					text = " ";
				}
				text += Environment.NewLine;
				text += Environment.NewLine;
				text += this.FusionLog;
			}
			return text;
		}

		protected FileNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.FileName = info.GetString("FileNotFound_FileName");
			this.FusionLog = info.GetString("FileNotFound_FusionLog");
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("FileNotFound_FileName", this.FileName, typeof(string));
			info.AddValue("FileNotFound_FusionLog", this.FusionLog, typeof(string));
		}
	}
}
