using System;
using UnityEngine;

public class UIShake : KMonoBehaviour, IRenderEveryTick
{
	public float Intensity
	{
		get
		{
			return this.intensity;
		}
	}

	public void RenderEveryTick(float dt)
	{
		if (this.intensity != 0f || this.lastIntensity != 0f)
		{
			this.lastIntensity = this.intensity;
			Vector2 vector = new Vector2(global::UnityEngine.Random.Range(-1f, 1f) * this.MaxOffsets.x * this.intensity, global::UnityEngine.Random.Range(-1f, 1f) * this.MaxOffsets.y * this.intensity);
			Vector2 vector2 = this.initialLocalPosition + vector;
			this.transform.anchoredPosition = vector2;
		}
	}

	public void SetIntensity(float intensity)
	{
		this.intensity = intensity;
	}

	protected override void OnPrefabInit()
	{
		this.transform = base.transform as RectTransform;
		this.initialLocalPosition = this.transform.anchoredPosition;
	}

	public Vector2 MaxOffsets = Vector2.one;

	private float lastIntensity;

	private float intensity;

	private Vector2 initialLocalPosition;

	private new RectTransform transform;
}
