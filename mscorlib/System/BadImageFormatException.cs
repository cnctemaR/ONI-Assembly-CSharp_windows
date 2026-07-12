using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[Serializable]
	public class BadImageFormatException : SystemException
	{
		public BadImageFormatException()
			: base("Format of the executable (.exe) or library (.dll) is invalid.")
		{
			base.HResult = -2147024885;
		}

		public BadImageFormatException(string message)
			: base(message)
		{
			base.HResult = -2147024885;
		}

		public BadImageFormatException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2147024885;
		}

		public BadImageFormatException(string message, string fileName)
			: base(message)
		{
			base.HResult = -2147024885;
			this._fileName = fileName;
		}

		public BadImageFormatException(string message, string fileName, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2147024885;
			this._fileName = fileName;
		}

		protected BadImageFormatException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this._fileName = info.GetString("BadImageFormat_FileName");
			this._fusionLog = info.GetString("BadImageFormat_FusionLog");
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("BadImageFormat_FileName", this._fileName, typeof(string));
			info.AddValue("BadImageFormat_FusionLog", this._fusionLog, typeof(string));
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
				if (this._fileName == null && base.HResult == -2146233088)
				{
					this._message = "Format of the executable (.exe) or library (.dll) is invalid.";
					return;
				}
				this._message = FileLoadException.FormatFileLoadExceptionMessage(this._fileName, base.HResult);
			}
		}

		public string FileName
		{
			get
			{
				return this._fileName;
			}
		}

		public override string ToString()
		{
			string text = base.GetType().ToString() + ": " + this.Message;
			if (this._fileName != null && this._fileName.Length != 0)
			{
				text = text + Environment.NewLine + SR.Format("File name: '{0}'", this._fileName);
			}
			if (base.InnerException != null)
			{
				text = text + " ---> " + base.InnerException.ToString();
			}
			if (this.StackTrace != null)
			{
				text = text + Environment.NewLine + this.StackTrace;
			}
			if (this._fusionLog != null)
			{
				if (text == null)
				{
					text = " ";
				}
				text += Environment.NewLine;
				text += Environment.NewLine;
				text += this._fusionLog;
			}
			return text;
		}

		public string FusionLog
		{
			get
			{
				return this._fusionLog;
			}
		}

		private string _fileName;

		private string _fusionLog;
	}
}
