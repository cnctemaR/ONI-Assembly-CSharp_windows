using System;
using System.Security.Permissions;

namespace System.Security.Cryptography.X509Certificates
{
	public static class X509Certificate2UI
	{
		[MonoTODO]
		public static void DisplayCertificate(X509Certificate2 certificate)
		{
			X509Certificate2UI.DisplayCertificate(certificate, IntPtr.Zero);
		}

		[MonoTODO]
		[UIPermission(SecurityAction.Demand, Window = UIPermissionWindow.SafeTopLevelWindows)]
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		public static void DisplayCertificate(X509Certificate2 certificate, IntPtr hwndParent)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			certificate.GetRawCertData();
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static X509Certificate2Collection SelectFromCollection(X509Certificate2Collection certificates, string title, string message, X509SelectionFlag selectionFlag)
		{
			return X509Certificate2UI.SelectFromCollection(certificates, title, message, selectionFlag, IntPtr.Zero);
		}

		[MonoTODO]
		[UIPermission(SecurityAction.Demand, Window = UIPermissionWindow.SafeTopLevelWindows)]
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		public static X509Certificate2Collection SelectFromCollection(X509Certificate2Collection certificates, string title, string message, X509SelectionFlag selectionFlag, IntPtr hwndParent)
		{
			if (certificates == null)
			{
				throw new ArgumentNullException("certificates");
			}
			if (selectionFlag < X509SelectionFlag.SingleSelection || selectionFlag > X509SelectionFlag.MultiSelection)
			{
				throw new ArgumentException("selectionFlag");
			}
			throw new NotImplementedException();
		}
	}
}
