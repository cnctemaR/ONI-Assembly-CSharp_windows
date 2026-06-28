using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public class FileNotFoundException : IOException
	{
		public FileNotFoundException()
			: base(Locale.GetText("Unable to find the specified file."))
		{
			base.HResult = -2146232799;
		}

		public FileNotFoundException(string message)
			: base(message)
		{
			base.HResult = -2146232799;
		}

		public FileNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146232799;
		}

		public FileNotFoundException(string message, string fileName)
			: base(message)
		{
			base.HResult = -2146232799;
			this.fileName = fileName;
		}

		public FileNotFoundException(string message, string fileName, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146232799;
			this.fileName = fileName;
		}

		protected FileNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.fileName = info.GetString("FileNotFound_FileName");
			this.fusionLog = info.GetString("FileNotFound_FusionLog");
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		public string FusionLog
		{
			[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlEvidence, ControlPolicy\"/>\n</PermissionSet>\n")]
			get
			{
				return this.fusionLog;
			}
		}

		public override string Message
		{
			get
			{
				if (this.message == null && this.fileName != null)
				{
					return string.Format(CultureInfo.CurrentCulture, "Could not load file or assembly '{0}' or one of its dependencies. The system cannot find the file specified.", new object[] { this.fileName });
				}
				return this.message;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("FileNotFound_FileName", this.fileName);
			info.AddValue("FileNotFound_FusionLog", this.fusionLog);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(this.GetType().FullName);
			stringBuilder.AppendFormat(": {0}", this.Message);
			if (this.fileName != null && this.fileName.Length > 0)
			{
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.AppendFormat("File name: '{0}'", this.fileName);
			}
			if (this.InnerException != null)
			{
				stringBuilder.AppendFormat(" ---> {0}", this.InnerException);
			}
			if (this.StackTrace != null)
			{
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(this.StackTrace);
			}
			return stringBuilder.ToString();
		}

		private const int Result = -2146232799;

		private string fileName;

		private string fusionLog;
	}
}
