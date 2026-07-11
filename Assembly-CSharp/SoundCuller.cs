using System;
using UnityEngine;

public struct SoundCuller
{
	public bool IsAudible(Vector2 pos)
	{
		return this.min.LessEqual(pos) && pos.LessEqual(this.max);
	}

	public bool IsAudibleNoCameraScaling(Vector2 pos, float falloff_distance_sq)
	{
		return (pos.x - this.cameraPos.x) * (pos.x - this.cameraPos.x) + (pos.y - this.cameraPos.y) * (pos.y - this.cameraPos.y) < falloff_distance_sq;
	}

	public bool IsAudible(Vector2 pos, float falloff_distance_sq)
	{
		pos = this.GetVerticallyScaledPosition(pos, false);
		return this.IsAudibleNoCameraScaling(pos, falloff_distance_sq);
	}

	public bool IsAudible(Vector2 pos, string sound_path)
	{
		return !string.IsNullOrEmpty(sound_path) && this.IsAudible(pos, KFMOD.GetSoundEventDescription(sound_path).falloffDistanceSq);
	}

	public Vector3 GetVerticallyScaledPosition(Vector3 pos, bool objectIsSelectedAndVisible = false)
	{
		float num = 1f;
		float num2;
		if (pos.y > this.max.y)
		{
			num2 = Mathf.Abs(pos.y - this.max.y);
		}
		else if (pos.y < this.min.y)
		{
			num2 = Mathf.Abs(pos.y - this.min.y);
			num = -1f;
		}
		else
		{
			num2 = 0f;
		}
		float extraYRange = TuningData<SoundCuller.Tuning>.Get().extraYRange;
		num2 = ((num2 < extraYRange) ? num2 : extraYRange);
		float num3 = num2 * num2 / (4f * this.zoomScaler);
		num3 *= num;
		Vector3 vector = new Vector3(pos.x, pos.y + num3, 0f);
		if (objectIsSelectedAndVisible)
		{
			vector.z = pos.z;
		}
		return vector;
	}

	public static SoundCuller CreateCuller()
	{
		SoundCuller soundCuller = default(SoundCuller);
		Camera main = Camera.main;
		Vector3 vector = main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.transform.GetPosition().z));
		Vector3 vector2 = main.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.main.transform.GetPosition().z));
		soundCuller.min = new Vector3(vector2.x, vector2.y, 0f);
		soundCuller.max = new Vector3(vector.x, vector.y, 0f);
		soundCuller.cameraPos = main.transform.GetPosition();
		Audio audio = Audio.Get();
		float num = CameraController.Instance.cameras[0].orthographicSize / (audio.listenerReferenceZ - audio.listenerMinZ);
		if (num <= 0f)
		{
			num = 2f;
		}
		else
		{
			num = 1f;
		}
		soundCuller.zoomScaler = num;
		return soundCuller;
	}

	private Vector2 min;

	private Vector2 max;

	private Vector2 cameraPos;

	private float zoomScaler;

	public class Tuning : TuningData<SoundCuller.Tuning>
	{
		public float extraYRange;
	}
}
