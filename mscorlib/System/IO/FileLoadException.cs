using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public class FileLoadException : IOException
	{
		public FileLoadException()
			: base(Locale.GetText("I/O Error"))
		{
			base.HResult = -2147024894;
			this.msg = Locale.GetText("I/O Error");
		}

		public FileLoadException(string message)
			: base(message)
		{
			base.HResult = -2147024894;
			this.msg = message;
		}

		public FileLoadException(string message, string fileName)
			: base(message)
		{
			base.HResult = -2147024894;
			this.msg = message;
			this.fileName = fileName;
		}

		public FileLoadException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2147024894;
			this.msg = message;
		}

		public FileLoadException(string message, string fileName, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2147024894;
			this.msg = message;
			this.fileName = fileName;
		}

		protected FileLoadException(SerializationInfo info, StreamingContext context)
		{
			this.fileName = info.GetString("FileLoad_FileName");
			this.fusionLog = info.GetString("FileLoad_FusionLog");
		}

		public override string Message
		{
			get
			{
				return this.msg;
			}
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

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("FileLoad_FileName", this.fileName);
			info.AddValue("FileLoad_FusionLog", this.fusionLog);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(this.GetType().FullName);
			stringBuilder.AppendFormat(": {0}", this.msg);
			if (this.fileName != null)
			{
				stringBuilder.AppendFormat(" : {0}", this.fileName);
			}
			if (this.InnerException != null)
			{
				stringBuilder.AppendFormat(" ----> {0}", this.InnerException);
			}
			if (this.StackTrace != null)
			{
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(this.StackTrace);
			}
			return stringBuilder.ToString();
		}

		private const int Result = -2147024894;

		private string msg;

		private string fileName;

		private string fusionLog;
	}
}
