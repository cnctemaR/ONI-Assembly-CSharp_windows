using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace FMODUnityResonance
{
	public static class FmodResonanceAudio
	{
		public static void UpdateAudioRoom(FmodResonanceAudioRoom room, bool roomEnabled)
		{
			if (roomEnabled)
			{
				if (!FmodResonanceAudio.enabledRooms.Contains(room))
				{
					FmodResonanceAudio.enabledRooms.Add(room);
				}
			}
			else
			{
				FmodResonanceAudio.enabledRooms.Remove(room);
			}
			if (FmodResonanceAudio.enabledRooms.Count > 0)
			{
				FmodResonanceAudio.RoomProperties roomProperties = FmodResonanceAudio.GetRoomProperties(FmodResonanceAudio.enabledRooms[FmodResonanceAudio.enabledRooms.Count - 1]);
				IntPtr intPtr = Marshal.AllocHGlobal(FmodResonanceAudio.roomPropertiesSize);
				Marshal.StructureToPtr<FmodResonanceAudio.RoomProperties>(roomProperties, intPtr, false);
				FmodResonanceAudio.ListenerPlugin.setParameterData(FmodResonanceAudio.roomPropertiesIndex, FmodResonanceAudio.GetBytes(intPtr, FmodResonanceAudio.roomPropertiesSize));
				Marshal.FreeHGlobal(intPtr);
				return;
			}
			FmodResonanceAudio.ListenerPlugin.setParameterData(FmodResonanceAudio.roomPropertiesIndex, FmodResonanceAudio.GetBytes(IntPtr.Zero, 0));
		}

		public static bool IsListenerInsideRoom(FmodResonanceAudioRoom room)
		{
			VECTOR vector;
			RuntimeManager.CoreSystem.get3DListenerAttributes(0, out FmodResonanceAudio.listenerPositionFmod, out vector, out vector, out vector);
			Vector3 vector2 = new Vector3(FmodResonanceAudio.listenerPositionFmod.x, FmodResonanceAudio.listenerPositionFmod.y, FmodResonanceAudio.listenerPositionFmod.z) - room.transform.position;
			Quaternion quaternion = Quaternion.Inverse(room.transform.rotation);
			FmodResonanceAudio.bounds.size = Vector3.Scale(room.transform.lossyScale, room.size);
			return FmodResonanceAudio.bounds.Contains(quaternion * vector2);
		}

		private static DSP ListenerPlugin
		{
			get
			{
				if (!FmodResonanceAudio.listenerPlugin.hasHandle())
				{
					FmodResonanceAudio.listenerPlugin = FmodResonanceAudio.Initialize();
				}
				return FmodResonanceAudio.listenerPlugin;
			}
		}

		private static float ConvertAmplitudeFromDb(float db)
		{
			return Mathf.Pow(10f, 0.05f * db);
		}

		private static void ConvertAudioTransformFromUnity(ref Vector3 position, ref Quaternion rotation)
		{
			Matrix4x4 matrix4x = Matrix4x4.TRS(position, rotation, Vector3.one);
			matrix4x = FmodResonanceAudio.flipZ * matrix4x * FmodResonanceAudio.flipZ;
			position = matrix4x.GetColumn(3);
			rotation = Quaternion.LookRotation(matrix4x.GetColumn(2), matrix4x.GetColumn(1));
		}

		private static byte[] GetBytes(IntPtr ptr, int length)
		{
			if (ptr != IntPtr.Zero)
			{
				byte[] array = new byte[length];
				Marshal.Copy(ptr, array, 0, length);
				return array;
			}
			return new byte[1];
		}

		private static FmodResonanceAudio.RoomProperties GetRoomProperties(FmodResonanceAudioRoom room)
		{
			Vector3 position = room.transform.position;
			Quaternion rotation = room.transform.rotation;
			Vector3 vector = Vector3.Scale(room.transform.lossyScale, room.size);
			FmodResonanceAudio.ConvertAudioTransformFromUnity(ref position, ref rotation);
			FmodResonanceAudio.RoomProperties roomProperties;
			roomProperties.positionX = position.x;
			roomProperties.positionY = position.y;
			roomProperties.positionZ = position.z;
			roomProperties.rotationX = rotation.x;
			roomProperties.rotationY = rotation.y;
			roomProperties.rotationZ = rotation.z;
			roomProperties.rotationW = rotation.w;
			roomProperties.dimensionsX = vector.x;
			roomProperties.dimensionsY = vector.y;
			roomProperties.dimensionsZ = vector.z;
			roomProperties.materialLeft = room.leftWall;
			roomProperties.materialRight = room.rightWall;
			roomProperties.materialBottom = room.floor;
			roomProperties.materialTop = room.ceiling;
			roomProperties.materialFront = room.frontWall;
			roomProperties.materialBack = room.backWall;
			roomProperties.reverbGain = FmodResonanceAudio.ConvertAmplitudeFromDb(room.reverbGainDb);
			roomProperties.reverbTime = room.reverbTime;
			roomProperties.reverbBrightness = room.reverbBrightness;
			roomProperties.reflectionScalar = room.reflectivity;
			return roomProperties;
		}

		private static DSP Initialize()
		{
			int num = 0;
			DSP dsp = default(DSP);
			Bank[] array = null;
			RuntimeManager.StudioSystem.getBankCount(out num);
			RuntimeManager.StudioSystem.getBankList(out array);
			for (int i = 0; i < num; i++)
			{
				int num2 = 0;
				Bus[] array2 = null;
				array[i].getBusCount(out num2);
				array[i].getBusList(out array2);
				RuntimeManager.StudioSystem.flushCommands();
				for (int j = 0; j < num2; j++)
				{
					string text = null;
					array2[j].getPath(out text);
					RuntimeManager.StudioSystem.getBus(text, out array2[j]);
					RuntimeManager.StudioSystem.flushCommands();
					ChannelGroup channelGroup;
					array2[j].getChannelGroup(out channelGroup);
					RuntimeManager.StudioSystem.flushCommands();
					if (channelGroup.hasHandle())
					{
						int num3 = 0;
						channelGroup.getNumDSPs(out num3);
						for (int k = 0; k < num3; k++)
						{
							channelGroup.getDSP(k, out dsp);
							int num4 = 0;
							uint num5 = 0U;
							string text2;
							dsp.getInfo(out text2, out num5, out num4, out num4, out num4);
							if (text2.ToString().Equals(FmodResonanceAudio.listenerPluginName) && dsp.hasHandle())
							{
								return dsp;
							}
						}
					}
				}
			}
			global::UnityEngine.Debug.LogError(FmodResonanceAudio.listenerPluginName + " not found in the FMOD project.");
			return dsp;
		}

		public const float maxGainDb = 24f;

		public const float minGainDb = -24f;

		public const float maxReverbBrightness = 1f;

		public const float minReverbBrightness = -1f;

		public const float maxReverbTime = 3f;

		public const float maxReflectivity = 2f;

		private static readonly Matrix4x4 flipZ = Matrix4x4.Scale(new Vector3(1f, 1f, -1f));

		private static readonly string listenerPluginName = "Resonance Audio Listener";

		private static readonly int roomPropertiesSize = MarshalHelper.SizeOf(typeof(FmodResonanceAudio.RoomProperties));

		private static readonly int roomPropertiesIndex = 1;

		private static Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

		private static List<FmodResonanceAudioRoom> enabledRooms = new List<FmodResonanceAudioRoom>();

		private static VECTOR listenerPositionFmod = default(VECTOR);

		private static DSP listenerPlugin;

		private struct RoomProperties
		{
			public float positionX;

			public float positionY;

			public float positionZ;

			public float rotationX;

			public float rotationY;

			public float rotationZ;

			public float rotationW;

			public float dimensionsX;

			public float dimensionsY;

			public float dimensionsZ;

			public FmodResonanceAudioRoom.SurfaceMaterial materialLeft;

			public FmodResonanceAudioRoom.SurfaceMaterial materialRight;

			public FmodResonanceAudioRoom.SurfaceMaterial materialBottom;

			public FmodResonanceAudioRoom.SurfaceMaterial materialTop;

			public FmodResonanceAudioRoom.SurfaceMaterial materialFront;

			public FmodResonanceAudioRoom.SurfaceMaterial materialBack;

			public float reflectionScalar;

			public float reverbGain;

			public float reverbTime;

			public float reverbBrightness;
		}
	}
}
