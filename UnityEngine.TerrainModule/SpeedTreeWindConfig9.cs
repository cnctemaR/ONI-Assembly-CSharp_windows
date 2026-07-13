using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	internal struct SpeedTreeWindConfig9
	{
		public SpeedTreeWindConfig9()
		{
			this.strengthResponse = 5f;
			this.directionResponse = 2.5f;
			this.gustFrequency = 0f;
			this.gustStrengthMin = 0.5f;
			this.gustStrengthMax = 1f;
			this.gustDurationMin = 1f;
			this.gustDurationMax = 4f;
			this.gustRiseScalar = 1f;
			this.gustFallScalar = 1f;
			this.branch1StretchLimit = 1f;
			this.branch2StretchLimit = 1f;
			this.sharedHeightStart = 0f;
			this.independenceShared = 0f;
			this.independenceBranch1 = 0f;
			this.independenceBranch2 = 0f;
			this.independenceRipple = 0f;
			this.shimmerRipple = 0f;
			this.windIndependence = 0f;
			this.treeExtentX = 0f;
			this.treeExtentY = 0f;
			this.treeExtentZ = 0f;
			this.doShared = 0;
			this.doBranch1 = 0;
			this.doBranch2 = 0;
			this.doRipple = 0;
			this.doShimmer = 0;
			this.lodFade = 0;
			this.importScale = 1f;
		}

		public readonly bool IsWindEnabled
		{
			get
			{
				return this.doShared != 0 || this.doBranch1 != 0 || this.doBranch2 != 0 || this.doRipple != 0;
			}
		}

		public static byte[] Serialize(SpeedTreeWindConfig9 config)
		{
			int num = Marshal.SizeOf<SpeedTreeWindConfig9>(config);
			byte[] array = new byte[num];
			GCHandle gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			try
			{
				IntPtr intPtr = gchandle.AddrOfPinnedObject();
				Marshal.StructureToPtr<SpeedTreeWindConfig9>(config, intPtr, false);
			}
			finally
			{
				gchandle.Free();
			}
			return array;
		}

		public float strengthResponse;

		public float directionResponse;

		public float gustFrequency;

		public float gustStrengthMin;

		public float gustStrengthMax;

		public float gustDurationMin;

		public float gustDurationMax;

		public float gustRiseScalar;

		public float gustFallScalar;

		public float branch1StretchLimit;

		public float branch2StretchLimit;

		public float sharedHeightStart;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<bendShared>e__FixedBuffer bendShared;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<oscillationShared>e__FixedBuffer oscillationShared;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<speedShared>e__FixedBuffer speedShared;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<turbulenceShared>e__FixedBuffer turbulenceShared;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<flexibilityShared>e__FixedBuffer flexibilityShared;

		public float independenceShared;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<bendBranch1>e__FixedBuffer bendBranch1;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<oscillationBranch1>e__FixedBuffer oscillationBranch1;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<speedBranch1>e__FixedBuffer speedBranch1;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<turbulenceBranch1>e__FixedBuffer turbulenceBranch1;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<flexibilityBranch1>e__FixedBuffer flexibilityBranch1;

		public float independenceBranch1;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<bendBranch2>e__FixedBuffer bendBranch2;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<oscillationBranch2>e__FixedBuffer oscillationBranch2;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<speedBranch2>e__FixedBuffer speedBranch2;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<turbulenceBranch2>e__FixedBuffer turbulenceBranch2;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<flexibilityBranch2>e__FixedBuffer flexibilityBranch2;

		public float independenceBranch2;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<planarRipple>e__FixedBuffer planarRipple;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<directionalRipple>e__FixedBuffer directionalRipple;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<speedRipple>e__FixedBuffer speedRipple;

		[FixedBuffer(typeof(float), 20)]
		public SpeedTreeWindConfig9.<flexibilityRipple>e__FixedBuffer flexibilityRipple;

		public float independenceRipple;

		public float shimmerRipple;

		public float treeExtentX;

		public float treeExtentY;

		public float treeExtentZ;

		public float windIndependence;

		public int doShared;

		public int doBranch1;

		public int doBranch2;

		public int doRipple;

		public int doShimmer;

		public int lodFade;

		public float importScale;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <bendBranch1>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <bendBranch2>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <bendShared>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <directionalRipple>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <flexibilityBranch1>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <flexibilityBranch2>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <flexibilityRipple>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <flexibilityShared>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <oscillationBranch1>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <oscillationBranch2>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <oscillationShared>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <planarRipple>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <speedBranch1>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <speedBranch2>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <speedRipple>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <speedShared>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <turbulenceBranch1>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <turbulenceBranch2>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <turbulenceShared>e__FixedBuffer
		{
			public float FixedElementField;
		}
	}
}
