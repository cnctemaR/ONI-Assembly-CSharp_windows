using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.XR.WSA
{
	[StaticAccessor("HolographicEmulation::HolographicEmulationManager::Get()", StaticAccessorType.Dot)]
	[NativeHeader("Runtime/VR/HoloLens/HolographicEmulation/HolographicEmulationManager.h")]
	[NativeConditional("ENABLE_HOLOLENS_MODULE")]
	internal class HolographicAutomation
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Initialize();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Shutdown();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void LoadRoom(string id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetEmulationMode(EmulationMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetGestureHand(GestureHand hand);

		[NativeName("ResetEmulationState")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Reset();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void PerformGesture(GestureHand hand, SimulatedGesture gesture);

		[NativeConditional("ENABLE_HOLOLENS_MODULE", StubReturnStatement = "Vector3f::zero")]
		internal static Vector3 GetBodyPosition()
		{
			Vector3 vector;
			HolographicAutomation.GetBodyPosition_Injected(out vector);
			return vector;
		}

		internal static void SetBodyPosition(Vector3 position)
		{
			HolographicAutomation.SetBodyPosition_Injected(ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float GetBodyRotation();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetBodyRotation(float degrees);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float GetBodyHeight();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetBodyHeight(float degrees);

		[NativeConditional("ENABLE_HOLOLENS_MODULE", StubReturnStatement = "Vector3f::zero")]
		internal static Vector3 GetHeadRotation()
		{
			Vector3 vector;
			HolographicAutomation.GetHeadRotation_Injected(out vector);
			return vector;
		}

		internal static void SetHeadRotation(Vector3 degrees)
		{
			HolographicAutomation.SetHeadRotation_Injected(ref degrees);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float GetHeadDiameter();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetHeadDiameter(float degrees);

		[NativeConditional("ENABLE_HOLOLENS_MODULE", StubReturnStatement = "Vector3f::zero")]
		internal static Vector3 GetHandPosition(GestureHand hand)
		{
			Vector3 vector;
			HolographicAutomation.GetHandPosition_Injected(hand, out vector);
			return vector;
		}

		internal static void SetHandPosition(GestureHand hand, Vector3 position)
		{
			HolographicAutomation.SetHandPosition_Injected(hand, ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetHandActivated(GestureHand hand);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetHandActivated(GestureHand hand, bool activated);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetHandVisible(GestureHand hand);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void EnsureHandVisible(GestureHand hand);

		public static SimulatedBody simulatedBody
		{
			get
			{
				return HolographicAutomation.s_Body;
			}
		}

		public static SimulatedHead simulatedHead
		{
			get
			{
				return HolographicAutomation.s_Head;
			}
		}

		public static SimulatedHand simulatedLeftHand
		{
			get
			{
				return HolographicAutomation.s_LeftHand;
			}
		}

		public static SimulatedHand simulatedRightHand
		{
			get
			{
				return HolographicAutomation.s_RightHand;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBodyPosition_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBodyPosition_Injected(ref Vector3 position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetHeadRotation_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetHeadRotation_Injected(ref Vector3 degrees);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetHandPosition_Injected(GestureHand hand, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetHandPosition_Injected(GestureHand hand, ref Vector3 position);

		private static SimulatedBody s_Body = new SimulatedBody();

		private static SimulatedHead s_Head = new SimulatedHead();

		private static SimulatedHand s_LeftHand = new SimulatedHand(GestureHand.Left);

		private static SimulatedHand s_RightHand = new SimulatedHand(GestureHand.Right);
	}
}
