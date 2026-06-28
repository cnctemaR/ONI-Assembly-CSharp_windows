using System;
using Klei;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class DemoTimer : MonoBehaviour
{
	private void Start()
	{
		DemoTimer.Instance = this;
		if (GenericGameSettings.instance != null)
		{
			if (GenericGameSettings.instance.demoMode)
			{
				this.duration = (float)GenericGameSettings.instance.demoTime;
				this.labelText.gameObject.SetActive(GenericGameSettings.instance.showDemoTimer);
				this.clockImage.gameObject.SetActive(GenericGameSettings.instance.showDemoTimer);
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
		this.duration = (float)GenericGameSettings.instance.demoTime;
		this.fadeOutScreen = Util.KInstantiateUI(this.Prefab_FadeOutScreen, GameScreenManager.Instance.ssOverlayCanvas.gameObject, false);
		Image component = this.fadeOutScreen.GetComponent<Image>();
		component.raycastTarget = false;
		this.fadeOutColor = component.color;
		this.fadeOutColor.a = 0f;
		this.fadeOutScreen.GetComponent<Image>().color = this.fadeOutColor;
	}

	private void Update()
	{
		if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.BackQuote))
		{
			this.CountdownActive = !this.CountdownActive;
			this.UpdateLabel();
		}
		if (!this.demoOver && this.CountdownActive)
		{
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
	}

	private void UpdateLabel()
	{
		int num = Mathf.RoundToInt(this.duration - this.elapsed);
		int num2 = Mathf.FloorToInt((float)(num / 60));
		int num3 = num % 60;
		this.labelText.text = string.Concat(new string[]
		{
			UI.DEMOOVERSCREEN.TIMEREMAINING,
			" ",
			num2.ToString("00"),
			":",
			num3.ToString("00")
		});
		if (!this.CountdownActive)
		{
			this.labelText.text = UI.DEMOOVERSCREEN.TIMERINACTIVE;
		}
	}

	public void EndDemo()
	{
		if (!this.demoOver)
		{
			this.demoOver = true;
			GameObject gameObject = Util.KInstantiateUI(this.Prefab_DemoOverScreen, GameScreenManager.Instance.ssOverlayCanvas.gameObject, false);
			gameObject.GetComponent<DemoOverScreen>().Show(true);
		}
	}

	public static DemoTimer Instance;

	public LocText labelText;

	public Image clockImage;

	public GameObject Prefab_DemoOverScreen;

	public GameObject Prefab_FadeOutScreen;

	private float duration;

	private float elapsed = 0f;

	private bool demoOver = false;

	private float beginTime = -1f;

	public bool CountdownActive = false;

	private GameObject fadeOutScreen;

	private Color fadeOutColor = default(Color);
}
