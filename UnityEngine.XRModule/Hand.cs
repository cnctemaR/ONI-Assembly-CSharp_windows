using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[StaticAccessor("XRInputDevices::Get()", StaticAccessorType.Dot)]
	[NativeHeader("Modules/XR/Subsystems/Input/Public/XRInputDevices.h")]
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[NativeHeader("XRScriptingClasses.h")]
	[NativeConditional("ENABLE_VR")]
	[RequiredByNativeCode]
	public struct Hand : IEquatable<Hand>
	{
		internal ulong deviceId
		{
			get
			{
				return this.m_DeviceId;
			}
		}

		internal uint featureIndex
		{
			get
			{
				return this.m_FeatureIndex;
			}
		}

		public bool TryGetRootBone(out Bone boneOut)
		{
			return Hand.Hand_TryGetRootBone(this, out boneOut);
		}

		private static bool Hand_TryGetRootBone(Hand hand, out Bone boneOut)
		{
			return Hand.Hand_TryGetRootBone_Injected(ref hand, out boneOut);
		}

		public bool TryGetFingerBones(HandFinger finger, List<Bone> bonesOut)
		{
			bool flag = bonesOut == null;
			if (flag)
			{
				throw new ArgumentNullException("bonesOut");
			}
			return Hand.Hand_TryGetFingerBonesAsList(this, finger, bonesOut);
		}

		private unsafe static bool Hand_TryGetFingerBonesAsList(Hand hand, HandFinger finger, [NotNull] List<Bone> bonesOut)
		{
			if (bonesOut == null)
			{
				ThrowHelper.ThrowArgumentNullException(bonesOut, "bonesOut");
			}
			bool flag;
			try
			{
				fixed (Bone[] array = NoAllocHelpers.ExtractArrayFromList<Bone>(bonesOut))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, bonesOut.Count);
					flag = Hand.Hand_TryGetFingerBonesAsList_Injected(ref hand, finger, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<Bone>(bonesOut);
			}
			return flag;
		}

		public override bool Equals(object obj)
		{
			bool flag = !(obj is Hand);
			return !flag && this.Equals((Hand)obj);
		}

		public bool Equals(Hand other)
		{
			return this.deviceId == other.deviceId && this.featureIndex == other.featureIndex;
		}

		public override int GetHashCode()
		{
			return this.deviceId.GetHashCode() ^ (this.featureIndex.GetHashCode() << 1);
		}

		public static bool operator ==(Hand a, Hand b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Hand a, Hand b)
		{
			return !(a == b);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Hand_TryGetRootBone_Injected([In] ref Hand hand, out Bone boneOut);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Hand_TryGetFingerBonesAsList_Injected([In] ref Hand hand, HandFinger finger, ref BlittableListWrapper bonesOut);

		private ulong m_DeviceId;

		private uint m_FeatureIndex;
	}
}
