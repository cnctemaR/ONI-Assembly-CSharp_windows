using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Platform
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct InitializeOptionsInternal : IDisposable
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

		public AllocateMemoryFunc AllocateMemoryFunction
		{
			get
			{
				AllocateMemoryFunc @default = Helper.GetDefault<AllocateMemoryFunc>();
				Helper.TryMarshalGet<AllocateMemoryFunc>(this.m_AllocateMemoryFunction, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<AllocateMemoryFunc>(ref this.m_AllocateMemoryFunction, value);
			}
		}

		public ReallocateMemoryFunc ReallocateMemoryFunction
		{
			get
			{
				ReallocateMemoryFunc @default = Helper.GetDefault<ReallocateMemoryFunc>();
				Helper.TryMarshalGet<ReallocateMemoryFunc>(this.m_ReallocateMemoryFunction, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<ReallocateMemoryFunc>(ref this.m_ReallocateMemoryFunction, value);
			}
		}

		public ReleaseMemoryFunc ReleaseMemoryFunction
		{
			get
			{
				ReleaseMemoryFunc @default = Helper.GetDefault<ReleaseMemoryFunc>();
				Helper.TryMarshalGet<ReleaseMemoryFunc>(this.m_ReleaseMemoryFunction, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<ReleaseMemoryFunc>(ref this.m_ReleaseMemoryFunction, value);
			}
		}

		public string ProductName
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_ProductName, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_ProductName, value);
			}
		}

		public string ProductVersion
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_ProductVersion, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_ProductVersion, value);
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

		public IntPtr SystemInitializeOptions
		{
			get
			{
				IntPtr @default = Helper.GetDefault<IntPtr>();
				Helper.TryMarshalGet(this.m_SystemInitializeOptions, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<IntPtr>(ref this.m_SystemInitializeOptions, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		private AllocateMemoryFunc m_AllocateMemoryFunction;

		private ReallocateMemoryFunc m_ReallocateMemoryFunction;

		private ReleaseMemoryFunc m_ReleaseMemoryFunction;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ProductName;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ProductVersion;

		private IntPtr m_Reserved;

		private IntPtr m_SystemInitializeOptions;
	}
}
