using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Plugins/ImageToggleStateThrobber")]
public class ImageToggleStateThrobber : KMonoBehaviour
{
	public void OnEnable()
	{
		this.t = 0f;
	}

	protected override void OnDisable()
	{
		ImageToggleState[] array = this.targetImageToggleStates;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ResetColor();
		}
	}

	public void Update()
	{
		float num = (this.useScaledTime ? Time.deltaTime : Time.unscaledDeltaTime);
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
		switch (state)
		{
		case ImageToggleState.State.Disabled:
			return its.DisabledColour;
		case ImageToggleState.State.Inactive:
			return its.InactiveColour;
		default:
			return its.ActiveColour;
		case ImageToggleState.State.DisabledActive:
			return its.DisabledActiveColour;
		}
	}

	public ImageToggleState[] targetImageToggleStates;

	public ImageToggleState.State state1;

	public ImageToggleState.State state2;

	public float period = 2f;

	public bool useScaledTime;

	private float t;
}
