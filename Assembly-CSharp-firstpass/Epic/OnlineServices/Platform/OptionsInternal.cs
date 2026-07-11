using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Platform
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct OptionsInternal : IDisposable
	{
		public int ApiVersion
		{
			get
			{
				int @default = Helper.GetDefault<int>();
				Helper.TryMarshalGet<int>(this.m_ApiVersion, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<int>(ref this.m_ApiVersion, value);
			}
		}

		public IntPtr Reserved
		{
			get
			{
				IntPtr @default = Helper.GetDefault<IntPtr>();
				Helper.TryMarshalGet(this.m_Reserved, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<IntPtr>(ref this.m_Reserved, value);
			}
		}

		public string ProductId
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_ProductId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_ProductId, value);
			}
		}

		public string SandboxId
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_SandboxId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_SandboxId, value);
			}
		}

		public ClientCredentialsInternal ClientCredentials
		{
			get
			{
				ClientCredentialsInternal @default = Helper.GetDefault<ClientCredentialsInternal>();
				Helper.TryMarshalGet<ClientCredentialsInternal>(this.m_ClientCredentials, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<ClientCredentialsInternal>(ref this.m_ClientCredentials, value);
			}
		}

		public bool IsServer
		{
			get
			{
				bool @default = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(this.m_IsServer, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_IsServer, value);
			}
		}

		public string EncryptionKey
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_EncryptionKey, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_EncryptionKey, value);
			}
		}

		public string OverrideCountryCode
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_OverrideCountryCode, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_OverrideCountryCode, value);
			}
		}

		public string OverrideLocaleCode
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_OverrideLocaleCode, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_OverrideLocaleCode, value);
			}
		}

		public string DeploymentId
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_DeploymentId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_DeploymentId, value);
			}
		}

		public PlatformFlags Flags
		{
			get
			{
				PlatformFlags @default = Helper.GetDefault<PlatformFlags>();
				Helper.TryMarshalGet<PlatformFlags>(this.m_Flags, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<PlatformFlags>(ref this.m_Flags, value);
			}
		}

		public string CacheDirectory
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_CacheDirectory, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_CacheDirectory, value);
			}
		}

		public uint TickBudgetInMilliseconds
		{
			get
			{
				uint @default = Helper.GetDefault<uint>();
				Helper.TryMarshalGet<uint>(this.m_TickBudgetInMilliseconds, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<uint>(ref this.m_TickBudgetInMilliseconds, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose<ClientCredentialsInternal>(ref this.m_ClientCredentials);
		}

		private int m_ApiVersion;

		private IntPtr m_Reserved;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ProductId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_SandboxId;

		private ClientCredentialsInternal m_ClientCredentials;

		private int m_IsServer;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_EncryptionKey;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_OverrideCountryCode;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_OverrideLocaleCode;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_DeploymentId;

		private PlatformFlags m_Flags;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_CacheDirectory;

		private uint m_TickBudgetInMilliseconds;
	}
}
