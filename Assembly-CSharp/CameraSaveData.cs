using System;
using UnityEngine;

public static class CameraSaveData
{
	public static void Load(FastReader reader)
	{
		CameraSaveData.position = reader.ReadVector3();
		CameraSaveData.localScale = reader.ReadVector3();
		CameraSaveData.rotation = reader.ReadQuaternion();
		CameraSaveData.orthographicsSize = reader.ReadSingle();
		CameraSaveData.valid = true;
	}

	public static bool valid;

	public static Vector3 position;

	public static Vector3 localScale;

	public static Quaternion rotation;

	public static float orthographicsSize;
}
