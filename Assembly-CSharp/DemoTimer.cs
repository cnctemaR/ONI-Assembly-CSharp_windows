using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class DemoTimer : MonoBehaviour
{
	private void Start()
	{
		DemoTimer.Instance = this;
		this.fadeOutScreen = Util.KInstantiateUI(this.Prefab_FadeOutScreen, GameScreenManager.Instance.ssOverlayCanvas.gameObject, false);
		Image component = this.fadeOutScreen.GetComponent<Image>();
		component.raycastTarget = false;
		this.fadeOutColor = component.color;
		this.fadeOutColor.a = 0f;
		this.fadeOutScreen.GetComponent<Image>().color = this.fadeOutColor;
		base.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (this.demoOver || !this.CountdownActive)
		{
			return;
		}
		if (this.beginTime == -1f)
		{
			this.beginTime = Time.unscaledTime;
		}
		this.elapsed = Mathf.Clamp(0f, Time.unscaledTime - this.beginTime, this.duration);
		if (this.elapsed + 5f >= this.duration)
		{
			float num = (this.duration - this.elapsed) / 5f;
			this.fadeOutColor.a = Mathf.Min(1f, 1f - Mathf.Sqrt(num));
			this.fadeOutScreen.GetComponent<Image>().color = this.fadeOutColor;
		}
		if (this.elapsed >= this.duration)
		{
			this.EndDemo();
		}
		this.UpdateLabel();
	}

	private void UpdateLabel()
	{
		int num = Mathf.RoundToInt(this.duration - this.elapsed);
		int num2 = Mathf.FloorToInt((float)(num / 60));
		int num3 = num % 60;
		this.labelText.text = string.Concat(new string[]
		{
			UI.DEMOOVERSCREEN.TIMEREMAINING.key.ToString(),
			" ",
			num2.ToString("00"),
			":",
			num3.ToString("00")
		});
	}

	private void EndDemo()
	{
		this.demoOver = true;
		GameObject gameObject = Util.KInstantiateUI(this.Prefab_DemoOverScreen, GameScreenManager.Instance.ssOverlayCanvas.gameObject, false);
		gameObject.GetComponent<DemoOverScreen>().Show(true);
	}

	public static DemoTimer Instance;

	public LocText labelText;

	public GameObject Prefab_DemoOverScreen;

	public GameObject Prefab_FadeOutScreen;

	private float duration = 900f;

	private float elapsed;

	private bool demoOver;

	private float beginTime = -1f;

	public bool CountdownActive;

	private GameObject fadeOutScreen;

	private Color fadeOutColor = default(Color);
}
