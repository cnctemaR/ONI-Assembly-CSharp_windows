using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Drawing.Printing
{
	[Serializable]
	public class InvalidPrinterException : SystemException
	{
		public InvalidPrinterException(PrinterSettings settings)
			: base(InvalidPrinterException.GetMessage(settings))
		{
		}

		protected InvalidPrinterException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
		}

		private static string GetMessage(PrinterSettings settings)
		{
			if (settings.PrinterName == null || settings.PrinterName == string.Empty)
			{
				return "No Printers Installed";
			}
			return string.Format("Tried to access printer '{0}' with invalid settings.", settings.PrinterName);
		}
	}
}
