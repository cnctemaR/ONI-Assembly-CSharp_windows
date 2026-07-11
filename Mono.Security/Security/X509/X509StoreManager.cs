using System;
using System.Collections;
using System.IO;

namespace Mono.Security.X509
{
	public sealed class X509StoreManager
	{
		private X509StoreManager()
		{
		}

		internal static string CurrentUserPath
		{
			get
			{
				if (X509StoreManager._userPath == null)
				{
					X509StoreManager._userPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".mono");
					X509StoreManager._userPath = Path.Combine(X509StoreManager._userPath, "certs");
				}
				return X509StoreManager._userPath;
			}
		}

		internal static string LocalMachinePath
		{
			get
			{
				if (X509StoreManager._localMachinePath == null)
				{
					X509StoreManager._localMachinePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), ".mono");
					X509StoreManager._localMachinePath = Path.Combine(X509StoreManager._localMachinePath, "certs");
				}
				return X509StoreManager._localMachinePath;
			}
		}

		internal static string NewCurrentUserPath
		{
			get
			{
				if (X509StoreManager._newUserPath == null)
				{
					X509StoreManager._newUserPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".mono");
					X509StoreManager._newUserPath = Path.Combine(X509StoreManager._newUserPath, "new-certs");
				}
				return X509StoreManager._newUserPath;
			}
		}

		internal static string NewLocalMachinePath
		{
			get
			{
				if (X509StoreManager._newLocalMachinePath == null)
				{
					X509StoreManager._newLocalMachinePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), ".mono");
					X509StoreManager._newLocalMachinePath = Path.Combine(X509StoreManager._newLocalMachinePath, "new-certs");
				}
				return X509StoreManager._newLocalMachinePath;
			}
		}

		public static X509Stores CurrentUser
		{
			get
			{
				if (X509StoreManager._userStore == null)
				{
					X509StoreManager._userStore = new X509Stores(X509StoreManager.CurrentUserPath, false);
				}
				return X509StoreManager._userStore;
			}
		}

		public static X509Stores LocalMachine
		{
			get
			{
				if (X509StoreManager._machineStore == null)
				{
					X509StoreManager._machineStore = new X509Stores(X509StoreManager.LocalMachinePath, false);
				}
				return X509StoreManager._machineStore;
			}
		}

		public static X509Stores NewCurrentUser
		{
			get
			{
				if (X509StoreManager._newUserStore == null)
				{
					X509StoreManager._newUserStore = new X509Stores(X509StoreManager.NewCurrentUserPath, true);
				}
				return X509StoreManager._newUserStore;
			}
		}

		public static X509Stores NewLocalMachine
		{
			get
			{
				if (X509StoreManager._newMachineStore == null)
				{
					X509StoreManager._newMachineStore = new X509Stores(X509StoreManager.NewLocalMachinePath, true);
				}
				return X509StoreManager._newMachineStore;
			}
		}

		public static X509CertificateCollection IntermediateCACertificates
		{
			get
			{
				X509CertificateCollection x509CertificateCollection = new X509CertificateCollection();
				x509CertificateCollection.AddRange(X509StoreManager.CurrentUser.IntermediateCA.Certificates);
				x509CertificateCollection.AddRange(X509StoreManager.LocalMachine.IntermediateCA.Certificates);
				return x509CertificateCollection;
			}
		}

		public static ArrayList IntermediateCACrls
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				arrayList.AddRange(X509StoreManager.CurrentUser.IntermediateCA.Crls);
				arrayList.AddRange(X509StoreManager.LocalMachine.IntermediateCA.Crls);
				return arrayList;
			}
		}

		public static X509CertificateCollection TrustedRootCertificates
		{
			get
			{
				X509CertificateCollection x509CertificateCollection = new X509CertificateCollection();
				x509CertificateCollection.AddRange(X509StoreManager.CurrentUser.TrustedRoot.Certificates);
				x509CertificateCollection.AddRange(X509StoreManager.LocalMachine.TrustedRoot.Certificates);
				return x509CertificateCollection;
			}
		}

		public static ArrayList TrustedRootCACrls
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				arrayList.AddRange(X509StoreManager.CurrentUser.TrustedRoot.Crls);
				arrayList.AddRange(X509StoreManager.LocalMachine.TrustedRoot.Crls);
				return arrayList;
			}
		}

		public static X509CertificateCollection UntrustedCertificates
		{
			get
			{
				X509CertificateCollection x509CertificateCollection = new X509CertificateCollection();
				x509CertificateCollection.AddRange(X509StoreManager.CurrentUser.Untrusted.Certificates);
				x509CertificateCollection.AddRange(X509StoreManager.LocalMachine.Untrusted.Certificates);
				return x509CertificateCollection;
			}
		}

		private static string _userPath;

		private static string _localMachinePath;

		private static string _newUserPath;

		private static string _newLocalMachinePath;

		private static X509Stores _userStore;

		private static X509Stores _machineStore;

		private static X509Stores _newUserStore;

		private static X509Stores _newMachineStore;
	}
}
