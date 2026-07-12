using System;

namespace Mono
{
	internal class SystemDependencyProvider : ISystemDependencyProvider
	{
		public static SystemDependencyProvider Instance
		{
			get
			{
				SystemDependencyProvider.Initialize();
				return SystemDependencyProvider.instance;
			}
		}

		internal static void Initialize()
		{
			object obj = SystemDependencyProvider.syncRoot;
			lock (obj)
			{
				if (SystemDependencyProvider.instance == null)
				{
					SystemDependencyProvider.instance = new SystemDependencyProvider();
				}
			}
		}

		ISystemCertificateProvider ISystemDependencyProvider.CertificateProvider
		{
			get
			{
				return this.CertificateProvider;
			}
		}

		public SystemCertificateProvider CertificateProvider { get; }

		public X509PalImpl X509Pal
		{
			get
			{
				return this.CertificateProvider.X509Pal;
			}
		}

		private SystemDependencyProvider()
		{
			this.CertificateProvider = new SystemCertificateProvider();
			DependencyInjector.Register(this);
		}

		private static SystemDependencyProvider instance;

		private static object syncRoot = new object();
	}
}
