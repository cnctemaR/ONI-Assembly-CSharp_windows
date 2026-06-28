using System;
using System.Collections.Generic;
using UnityEngine;

public class ImageToggleStateThrobber : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		List<ImageToggleState> list = new List<ImageToggleState>(this.targetImageToggleStates);
		this.targetImageToggleStates = list.ToArray();
	}

	public void OnEnable()
	{
		this.t = 0f;
	}

	public void OnDisable()
	{
		foreach (ImageToggleState imageToggleState in this.targetImageToggleStates)
		{
			imageToggleState.ResetColor();
		}
	}

	public void Update()
	{
		float num = ((!this.useScaledTime) ? Time.unscaledDeltaTime : Time.deltaTime);
		this.t = (this.t + num) % this.period;
		float num2 = Mathf.Cos(this.t / this.period * 2f * 3.1415927f) * 0.5f + 0.5f;
		foreach (ImageToggleState imageToggleState in this.targetImageToggleStates)
		{
			Color color = this.ColorForState(imageToggleState, this.state1);
			Color color2 = this.ColorForState(imageToggleState, this.state2);
			Color color3 = Color.Lerp(color, color2, num2);
			imageToggleState.TargetImage.color = color3;
		}
	}

	private Color ColorForState(ImageToggleState its, ImageToggleState.State state)
	{
		Color color;
		switch (state)
		{
		case ImageToggleState.State.Disabled:
			color = its.DisabledColour;
			break;
		case ImageToggleState.State.Inactive:
			color = its.InactiveColour;
			break;
		default:
			color = its.ActiveColour;
			break;
		case ImageToggleState.State.DisabledActive:
			color = its.DisabledActiveColour;
			break;
		}
		return color;
	}

	public ImageToggleState[] targetImageToggleStates;

	public ImageToggleState.State state1;

	public ImageToggleState.State state2;

	public float period = 2f;

	public bool useScaledTime = false;

	private float t = 0f;
}
