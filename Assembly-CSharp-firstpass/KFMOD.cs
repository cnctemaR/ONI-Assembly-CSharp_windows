using System;
using System.Runtime.InteropServices;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class KFMOD : KMonoBehaviour
{
	public static void PlayOneShot(string path, Vector3 position)
	{
		Vector3 vector = new Vector3(position.x, position.y, 0f);
		RuntimeManager.PlayOneShot(path, vector);
	}

	public static void PlayOneShot(Guid guid, [Optional] Vector3 position)
	{
		Vector3 vector = new Vector3(position.x, position.y, 0f);
		RuntimeManager.PlayOneShot(guid, vector);
	}

	public static void PlayOneShot(string sound)
	{
		RuntimeManager.PlayOneShot(sound, default(Vector3));
	}

	public static EventInstance BeginOneShot(string ev, Vector3 position)
	{
		if (ev == null)
		{
			return null;
		}
		if (App.IsExiting)
		{
			return null;
		}
		EventInstance eventInstance = RuntimeManager.CreateInstance(ev);
		if (eventInstance == null)
		{
			return null;
		}
		Vector3 vector = new Vector3(position.x, position.y, 0f);
		return KFMOD.BeginOneShot(eventInstance, vector);
	}

	public static EventInstance BeginOneShot(EventInstance instance, Vector3 position)
	{
		Vector3 vector = new Vector3(position.x, position.y, 0f);
		ATTRIBUTES_3D attributes_3D = vector.To3DAttributes();
		instance.set3DAttributes(attributes_3D);
		instance.setVolume(1f);
		return instance;
	}

	public static bool EndOneShot(EventInstance instance)
	{
		if (instance != null)
		{
			instance.start();
			instance.release();
			return true;
		}
		return false;
	}

	public static EventInstance CreateInstance(string path)
	{
		return RuntimeManager.CreateInstance(path);
	}

	public static Vector3 GetInstancePosition(EventInstance instance)
	{
		ATTRIBUTES_3D attributes_3D;
		instance.get3DAttributes(out attributes_3D);
		Vector3 vector = new Vector3(attributes_3D.position.x, attributes_3D.position.y, attributes_3D.position.z);
		return vector;
	}

	public static Vector3 GetZFlattenedPosition(Vector3 pos)
	{
		Vector3 vector = new Vector3(pos.x, pos.y, 0f);
		return vector;
	}

	public static KFMOD.AudioDevice currentDevice;

	public struct AudioDevice
	{
		public int fmod_id;

		public string name;

		public Guid guid;

		public int systemRate;

		public SPEAKERMODE speakerMode;

		public int speakerModeChannels;

		public bool selected;
	}
}
