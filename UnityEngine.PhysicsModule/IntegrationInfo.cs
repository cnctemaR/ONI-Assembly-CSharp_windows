using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	[StructLayout(LayoutKind.Explicit)]
	public struct IntegrationInfo
	{
		public readonly uint id
		{
			get
			{
				return this.m_Id;
			}
		}

		public unsafe string name
		{
			get
			{
				fixed (byte* ptr = &this.m_Name.FixedElementField)
				{
					byte* ptr2 = ptr;
					return Marshal.PtrToStringAnsi(new IntPtr((void*)ptr2));
				}
			}
		}

		public unsafe string description
		{
			get
			{
				fixed (byte* ptr = &this.m_Desc.FixedElementField)
				{
					byte* ptr2 = ptr;
					return Marshal.PtrToStringAnsi(new IntPtr((void*)ptr2));
				}
			}
		}

		public bool isFallback
		{
			get
			{
				return this.id == 3737844653U;
			}
		}

		internal bool isExperimental
		{
			get
			{
				return this.m_IntegrationVersion.FixedElementField < 1;
			}
		}

		internal const uint k_InvalidID = 0U;

		internal const uint k_FallbackIntegrationId = 3737844653U;

		[FieldOffset(0)]
		private readonly uint m_Id;

		[FixedBuffer(typeof(ushort), 3)]
		[FieldOffset(4)]
		private IntegrationInfo.<m_IntegrationVersion>e__FixedBuffer m_IntegrationVersion;

		[FixedBuffer(typeof(ushort), 3)]
		[FieldOffset(10)]
		private IntegrationInfo.<m_SdkVersion>e__FixedBuffer m_SdkVersion;

		[FieldOffset(16)]
		private readonly IntegrationInfo.SupportedUnityFeatures m_Features;

		[FixedBuffer(typeof(byte), 16)]
		[FieldOffset(20)]
		private IntegrationInfo.<m_Name>e__FixedBuffer m_Name;

		[FixedBuffer(typeof(byte), 220)]
		[FieldOffset(36)]
		private IntegrationInfo.<m_Desc>e__FixedBuffer m_Desc;

		[Flags]
		internal enum SupportedUnityFeatures
		{
			None = 0,
			DynamicsSupport = 2,
			SDKVisualDebuggerSupport = 4,
			ArticulationSupport = 8,
			ImmediateModeSupport = 16,
			VehicleSupport = 32,
			CharacterControllerSupport = 64
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 220)]
		public struct <m_Desc>e__FixedBuffer
		{
			public byte FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 6)]
		public struct <m_IntegrationVersion>e__FixedBuffer
		{
			public ushort FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		public struct <m_Name>e__FixedBuffer
		{
			public byte FixedElementField;
		}

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 6)]
		public struct <m_SdkVersion>e__FixedBuffer
		{
			public ushort FixedElementField;
		}
	}
}
