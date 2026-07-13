using System;
using System.Collections;
using Klei;
using UnityEngine;
using UnityEngine.Scripting;

public class KProfilerBegin : MonoBehaviour
{
	private void Start()
	{
		base.StartCoroutine(this.FrameTicker());
	}

	private IEnumerator FrameTicker()
	{
		for (;;)
		{
			yield return this.wait;
			this.TickScriptedProfile();
		}
		yield break;
	}

	private void TickScriptedProfile()
	{
		GenericGameSettings.ScriptedProfile scriptedProfile = GenericGameSettings.instance.scriptedProfile;
		if (!string.IsNullOrEmpty((scriptedProfile != null) ? scriptedProfile.saveGame : null))
		{
			if (!this.scriptedProfileBegun && Time.timeSinceLevelLoad >= GenericGameSettings.instance.scriptedProfile.startWaitTime)
			{
				if (GenericGameSettings.instance.scriptedProfile.disableGC)
				{
					GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
				}
				global::System.Action onStartCapture = KProfilerBegin.OnStartCapture;
				if (onStartCapture != null)
				{
					onStartCapture();
				}
				this.scriptedProfileBegun = true;
				return;
			}
			if (this.scriptedProfileBegun)
			{
				this.framesElapsed++;
				if (this.framesElapsed >= GenericGameSettings.instance.scriptedProfile.frameCount)
				{
					global::System.Action onStopCapture = KProfilerBegin.OnStopCapture;
					if (onStopCapture != null)
					{
						onStopCapture();
					}
					App.Quit();
				}
			}
		}
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
	}

	private void OnApplicationQuit()
	{
	}

	public static int begin_counter;

	private WaitForEndOfFrame wait = new WaitForEndOfFrame();

	private bool scriptedProfileBegun;

	private int framesElapsed;

	public static global::System.Action OnStartCapture;

	public static global::System.Action OnStopCapture;
}
