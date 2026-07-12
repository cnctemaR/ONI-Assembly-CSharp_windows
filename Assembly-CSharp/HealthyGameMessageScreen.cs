using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/HealthyGameMessageScreen")]
public class HealthyGameMessageScreen : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.confirmButton.onClick += delegate
		{
			string @string = KPlayerPrefs.GetString("PlayShortOnLaunch", "");
			if (!string.IsNullOrEmpty(MainMenu.Instance.IntroShortName) && @string != MainMenu.Instance.IntroShortName)
			{
				VideoScreen component = KScreenManager.AddChild(FrontEndManager.Instance.gameObject, ScreenPrefabs.Instance.VideoScreen.gameObject).GetComponent<VideoScreen>();
				component.PlayVideo(Assets.GetVideo(MainMenu.Instance.IntroShortName), false, AudioMixerSnapshots.Get().MainMenuVideoPlayingSnapshot, false);
				component.OnStop = (global::System.Action)Delegate.Combine(component.OnStop, new global::System.Action(delegate
				{
					KPlayerPrefs.SetString("PlayShortOnLaunch", MainMenu.Instance.IntroShortName);
					global::UnityEngine.Object.Destroy(base.gameObject);
				}));
				return;
			}
			global::UnityEngine.Object.Destroy(base.gameObject);
		};
		this.confirmButton.gameObject.SetActive(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		if (this.isFirstUpdate)
		{
			this.isFirstUpdate = false;
			this.spawnTime = Time.unscaledTime;
			return;
		}
		float num = Mathf.Min(Time.unscaledDeltaTime, 0.033333335f);
		float num2 = Time.unscaledTime - this.spawnTime;
		if (num2 < this.totalTime - this.fadeTime)
		{
			this.canvasGroup.alpha = this.canvasGroup.alpha + num * (1f / this.fadeTime);
			return;
		}
		if (num2 >= this.totalTime + 0.75f)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (num2 >= this.totalTime - this.fadeTime)
		{
			this.canvasGroup.alpha = this.canvasGroup.alpha - num * (1f / this.fadeTime);
		}
	}

	public KButton confirmButton;

	public CanvasGroup canvasGroup;

	private float spawnTime;

	private float totalTime = 10f;

	private float fadeTime = 1.5f;

	private bool isFirstUpdate = true;
}
