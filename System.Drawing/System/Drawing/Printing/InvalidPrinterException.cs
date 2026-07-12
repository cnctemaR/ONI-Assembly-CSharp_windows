using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace System.Drawing.Printing
{
	[Serializable]
	public class InvalidPrinterException : SystemException
	{
		protected InvalidPrinterException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this._settings = (PrinterSettings)info.GetValue("settings", typeof(PrinterSettings));
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("settings", this._settings);
		}

		public InvalidPrinterException(PrinterSettings settings)
			: base(InvalidPrinterException.GenerateMessage(settings))
		{
			this._settings = settings;
		}

		private static string GenerateMessage(PrinterSettings settings)
		{
			if (settings.IsDefaultPrinter)
			{
				return SR.Format("No printers are installed.", Array.Empty<object>());
			}
			string text;
			try
			{
				text = SR.Format("Settings to access printer '{0}' are not valid.", new object[] { settings.PrinterName });
			}
			catch (SecurityException)
			{
				text = SR.Format("Settings to access printer '{0}' are not valid.", new object[] { SR.Format("(printer name protected due to security restrictions)", Array.Empty<object>()) });
			}
			return text;
		}

		private PrinterSettings _settings;
	}
}
