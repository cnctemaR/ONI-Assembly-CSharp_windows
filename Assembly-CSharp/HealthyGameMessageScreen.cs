using System;
using UnityEngine;

public class HealthyGameMessageScreen : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.confirmButton.onClick += delegate
		{
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
		}
		else
		{
			float num = Mathf.Min(Time.unscaledDeltaTime, 0.033333335f);
			float num2 = Time.unscaledTime - this.spawnTime;
			if (num2 < this.totalTime - this.fadeTime)
			{
				this.canvasGroup.alpha = this.canvasGroup.alpha + num * (1f / this.fadeTime);
			}
			else if (num2 >= this.totalTime + 0.75f)
			{
				global::UnityEngine.Object.Destroy(base.gameObject);
			}
			else if (num2 >= this.totalTime - this.fadeTime)
			{
				this.canvasGroup.alpha = this.canvasGroup.alpha - num * (1f / this.fadeTime);
			}
		}
	}

	public KButton confirmButton;

	public CanvasGroup canvasGroup;

	private float spawnTime;

	private float totalTime = 10f;

	private float fadeTime = 1.5f;

	private bool isFirstUpdate = true;
}
