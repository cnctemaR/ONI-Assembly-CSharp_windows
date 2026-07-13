using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[Obsolete("This type is deprecated and will be removed in a future release.", false)]
	[NativeConditional("ENABLE_CLUSTERINPUT")]
	[NativeHeader("Modules/ClusterInput/ClusterInput.h")]
	public class ClusterInput
	{
		public unsafe static float GetAxis(string name)
		{
			float axis_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				axis_Injected = ClusterInput.GetAxis_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return axis_Injected;
		}

		public unsafe static bool GetButton(string name)
		{
			bool button_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				button_Injected = ClusterInput.GetButton_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return button_Injected;
		}

		[NativeConditional("ENABLE_CLUSTERINPUT", "Vector3f(0.0f, 0.0f, 0.0f)")]
		public unsafe static Vector3 GetTrackerPosition(string name)
		{
			Vector3 vector2;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Vector3 vector;
				ClusterInput.GetTrackerPosition_Injected(ref managedSpanWrapper, out vector);
			}
			finally
			{
				char* ptr = null;
				Vector3 vector;
				vector2 = vector;
			}
			return vector2;
		}

		[NativeConditional("ENABLE_CLUSTERINPUT", "Quartenion::identity")]
		public unsafe static Quaternion GetTrackerRotation(string name)
		{
			Quaternion quaternion2;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Quaternion quaternion;
				ClusterInput.GetTrackerRotation_Injected(ref managedSpanWrapper, out quaternion);
			}
			finally
			{
				char* ptr = null;
				Quaternion quaternion;
				quaternion2 = quaternion;
			}
			return quaternion2;
		}

		public unsafe static void SetAxis(string name, float value)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ClusterInput.SetAxis_Injected(ref managedSpanWrapper, value);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public unsafe static void SetButton(string name, bool value)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ClusterInput.SetButton_Injected(ref managedSpanWrapper, value);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public unsafe static void SetTrackerPosition(string name, Vector3 value)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ClusterInput.SetTrackerPosition_Injected(ref managedSpanWrapper, ref value);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public unsafe static void SetTrackerRotation(string name, Quaternion value)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ClusterInput.SetTrackerRotation_Injected(ref managedSpanWrapper, ref value);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public unsafe static bool AddInput(string name, string deviceName, string serverUrl, int index, ClusterInputType type)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(deviceName, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = deviceName.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper3;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(serverUrl, ref managedSpanWrapper3))
				{
					ReadOnlySpan<char> readOnlySpan3 = serverUrl.AsSpan();
					fixed (char* ptr3 = readOnlySpan3.GetPinnableReference())
					{
						managedSpanWrapper3 = new ManagedSpanWrapper((void*)ptr3, readOnlySpan3.Length);
					}
				}
				flag = ClusterInput.AddInput_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, ref managedSpanWrapper3, index, type);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
				char* ptr3 = null;
			}
			return flag;
		}

		public unsafe static bool EditInput(string name, string deviceName, string serverUrl, int index, ClusterInputType type)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(deviceName, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = deviceName.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper3;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(serverUrl, ref managedSpanWrapper3))
				{
					ReadOnlySpan<char> readOnlySpan3 = serverUrl.AsSpan();
					fixed (char* ptr3 = readOnlySpan3.GetPinnableReference())
					{
						managedSpanWrapper3 = new ManagedSpanWrapper((void*)ptr3, readOnlySpan3.Length);
					}
				}
				flag = ClusterInput.EditInput_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, ref managedSpanWrapper3, index, type);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
				char* ptr3 = null;
			}
			return flag;
		}

		public unsafe static bool CheckConnectionToServer(string name)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = ClusterInput.CheckConnectionToServer_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetAxis_Injected(ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetButton_Injected(ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTrackerPosition_Injected(ref ManagedSpanWrapper name, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTrackerRotation_Injected(ref ManagedSpanWrapper name, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAxis_Injected(ref ManagedSpanWrapper name, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetButton_Injected(ref ManagedSpanWrapper name, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTrackerPosition_Injected(ref ManagedSpanWrapper name, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTrackerRotation_Injected(ref ManagedSpanWrapper name, [In] ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddInput_Injected(ref ManagedSpanWrapper name, ref ManagedSpanWrapper deviceName, ref ManagedSpanWrapper serverUrl, int index, ClusterInputType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool EditInput_Injected(ref ManagedSpanWrapper name, ref ManagedSpanWrapper deviceName, ref ManagedSpanWrapper serverUrl, int index, ClusterInputType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CheckConnectionToServer_Injected(ref ManagedSpanWrapper name);
	}
}
