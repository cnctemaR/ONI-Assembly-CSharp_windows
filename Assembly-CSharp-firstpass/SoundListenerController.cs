using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SoundListenerController : MonoBehaviour
{
	public static SoundListenerController Instance { get; private set; }

	private void Awake()
	{
		SoundListenerController.Instance = this;
	}

	private void OnDestroy()
	{
		SoundListenerController.Instance = null;
	}

	private void Start()
	{
		RuntimeManager.StudioSystem.getVCA("vca:/Looping", out this.loopingVCA);
	}

	public void SetLoopingVolume(float volume)
	{
		if (this.loopingVCA != null)
		{
			this.loopingVCA.setFaderLevel(volume);
		}
	}

	private void Update()
	{
		Audio audio = Audio.Get();
		Vector3 position = Camera.main.transform.position;
		float num = (Camera.main.orthographicSize - audio.listenerMinOrthographicSize) / (audio.listenerReferenceOrthographicSize - audio.listenerMinOrthographicSize);
		num = Mathf.Max(num, 0f);
		float num2 = -audio.listenerMinZ - num * (audio.listenerReferenceZ - audio.listenerMinZ);
		position.z = num2;
		base.transform.SetPosition(position);
	}

	private VCA loopingVCA;
}
