using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class BadImageFormatException : SystemException
	{
		public BadImageFormatException()
			: base(Locale.GetText("Format of the executable (.exe) or library (.dll) is invalid."))
		{
			base.HResult = -2147024885;
		}

		public BadImageFormatException(string message)
			: base(message)
		{
			base.HResult = -2147024885;
		}

		protected BadImageFormatException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.fileName = info.GetString("BadImageFormat_FileName");
			this.fusionLog = info.GetString("BadImageFormat_FusionLog");
		}

		public BadImageFormatException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2147024885;
		}

		public BadImageFormatException(string message, string fileName)
			: base(message)
		{
			this.fileName = fileName;
			base.HResult = -2147024885;
		}

		public BadImageFormatException(string message, string fileName, Exception inner)
			: base(message, inner)
		{
			this.fileName = fileName;
			base.HResult = -2147024885;
		}

		public override string Message
		{
			get
			{
				if (this.message == null)
				{
					return string.Format(CultureInfo.CurrentCulture, "Could not load file or assembly '{0}' or one of its dependencies. An attempt was made to load a program with an incorrect format.", new object[] { this.fileName });
				}
				return base.Message;
			}
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		[MonoTODO("Probably not entirely correct. fusionLog needs to be set somehow (we are probably missing internal constuctor)")]
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
			info.AddValue("BadImageFormat_FileName", this.fileName);
			info.AddValue("BadImageFormat_FusionLog", this.fusionLog);
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

		private const int Result = -2147024885;

		private string fileName;

		private string fusionLog;
	}
}
