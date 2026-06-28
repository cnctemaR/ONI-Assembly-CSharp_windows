using System;
using System.Runtime.InteropServices;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class KFMOD
{
	public static void PlayOneShot(string path, Vector3 position)
	{
		RuntimeManager.PlayOneShot(path, position);
		KFMODDebugger.instance.Log(string.Concat(new object[] { "PlayOneShot: ", path, " at ", position }));
	}

	public static void PlayOneShot(Guid guid, [Optional] Vector3 position)
	{
		RuntimeManager.PlayOneShot(guid, position);
		KFMODDebugger.instance.Log(string.Concat(new object[] { "PlayOneShot: ", guid, " at ", position }));
	}

	public static void PlayOneShot(string sound)
	{
		RuntimeManager.PlayOneShot(sound, default(Vector3));
		KFMODDebugger.instance.Log("PlayOneShot: " + sound);
	}

	public static EventInstance BeginOneShot(string ev, Vector3 pos)
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
			KFMODDebugger.instance.Log("Could not find event: " + ev);
			return null;
		}
		KFMODDebugger.instance.Log(string.Concat(new object[] { "BeginOneShot: ", ev, " at ", pos }));
		return KFMOD.BeginOneShot(eventInstance, pos);
	}

	public static EventInstance BeginOneShot(EventInstance instance, Vector3 pos)
	{
		ATTRIBUTES_3D attributes_3D = pos.To3DAttributes();
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
		KFMODDebugger.instance.Log("CreateInstance: " + path);
		return RuntimeManager.CreateInstance(path);
	}

	public static Vector3 GetInstancePosition(EventInstance instance)
	{
		ATTRIBUTES_3D attributes_3D;
		instance.get3DAttributes(out attributes_3D);
		Vector3 vector = new Vector3(attributes_3D.position.x, attributes_3D.position.y, attributes_3D.position.z);
		return vector;
	}
}
