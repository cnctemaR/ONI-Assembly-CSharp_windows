using System;
using System.Runtime.Serialization;
using System.Security;

namespace System.IO
{
	[Serializable]
	public class FileLoadException : IOException
	{
		public FileLoadException()
			: base("Could not load the specified file.")
		{
			base.HResult = -2146232799;
		}

		public FileLoadException(string message)
			: base(message)
		{
			base.HResult = -2146232799;
		}

		public FileLoadException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146232799;
		}

		public FileLoadException(string message, string fileName)
			: base(message)
		{
			base.HResult = -2146232799;
			this.FileName = fileName;
		}

		public FileLoadException(string message, string fileName, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146232799;
			this.FileName = fileName;
		}

		public override string Message
		{
			get
			{
				if (this._message == null)
				{
					this._message = FileLoadException.FormatFileLoadExceptionMessage(this.FileName, base.HResult);
				}
				return this._message;
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

		protected FileLoadException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.FileName = info.GetString("FileLoad_FileName");
			this.FusionLog = info.GetString("FileLoad_FusionLog");
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("FileLoad_FileName", this.FileName, typeof(string));
			info.AddValue("FileLoad_FusionLog", this.FusionLog, typeof(string));
		}

		internal static string FormatFileLoadExceptionMessage(string fileName, int hResult)
		{
			if (fileName != null)
			{
				return SR.Format("Could not load the file '{0}'.", fileName);
			}
			return "Could not load the specified file.";
		}
	}
}
