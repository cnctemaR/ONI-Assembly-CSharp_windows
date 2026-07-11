using System;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KSlider : Slider
{
	public event global::System.Action onReleaseHandle;

	public event global::System.Action onDrag;

	public event global::System.Action onPointerDown;

	public event global::System.Action onMove;

	private new void Awake()
	{
		this.currentSounds = new string[KSlider.DefaultSounds.Length];
		for (int i = 0; i < KSlider.DefaultSounds.Length; i++)
		{
			this.currentSounds[i] = KSlider.DefaultSounds[i];
		}
		this.lastMoveTime = Time.unscaledTime;
		this.lastMoveValue = -1f;
		this.tooltip = base.handleRect.gameObject.GetComponent<ToolTip>();
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		this.PlayEndSound();
		if (this.tooltip != null)
		{
			this.tooltip.enabled = true;
			this.tooltip.OnPointerEnter(eventData);
		}
		if (this.onReleaseHandle != null)
		{
			this.onReleaseHandle();
		}
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		this.PlayStartSound();
		if (this.value != this.lastMoveValue)
		{
			this.PlayMoveSound(KSlider.MoveSource.MouseClick);
		}
		if (this.tooltip != null)
		{
			this.tooltip.enabled = false;
		}
		if (this.onPointerDown != null)
		{
			this.onPointerDown();
		}
	}

	public override void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);
		this.PlayMoveSound(KSlider.MoveSource.MouseDrag);
		if (this.onDrag != null)
		{
			this.onDrag();
		}
	}

	public override void OnMove(AxisEventData eventData)
	{
		base.OnMove(eventData);
		this.PlayMoveSound(KSlider.MoveSource.Keyboard);
		if (this.onMove != null)
		{
			this.onMove();
		}
	}

	public void ClearReleaseHandleEvent()
	{
		this.onReleaseHandle = null;
	}

	public void SetTooltipText(string tooltipText)
	{
		if (this.tooltip != null)
		{
			this.tooltip.SetSimpleTooltip(tooltipText);
		}
	}

	public void PlayStartSound()
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		if (!this.playSounds)
		{
			return;
		}
		string text = this.currentSounds[0];
		if (text != null && text.Length > 0)
		{
			KFMOD.PlayUISound(text);
		}
	}

	public void PlayMoveSound(KSlider.MoveSource moveSource)
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		if (!this.playSounds)
		{
			return;
		}
		float num = Time.unscaledTime - this.lastMoveTime;
		if (num < this.movePlayRate)
		{
			return;
		}
		if (moveSource != KSlider.MoveSource.MouseDrag)
		{
			this.playedBoundaryBump = false;
		}
		float num2 = Mathf.InverseLerp(base.minValue, base.maxValue, this.value);
		string text = null;
		if (num2 == 1f && this.lastMoveValue == 1f)
		{
			if (!this.playedBoundaryBump)
			{
				text = this.currentSounds[4];
				this.playedBoundaryBump = true;
			}
		}
		else if (num2 == 0f && this.lastMoveValue == 0f)
		{
			if (!this.playedBoundaryBump)
			{
				text = this.currentSounds[3];
				this.playedBoundaryBump = true;
			}
		}
		else if (num2 >= 0f && num2 <= 1f)
		{
			text = this.currentSounds[1];
			this.playedBoundaryBump = false;
		}
		if (text != null && text.Length > 0)
		{
			this.lastMoveTime = Time.unscaledTime;
			this.lastMoveValue = num2;
			EventInstance eventInstance = KFMOD.BeginOneShot(text, Vector3.zero, 1f);
			eventInstance.setParameterByName("sliderValue", num2, false);
			eventInstance.setParameterByName("timeSinceLast", num, false);
			KFMOD.EndOneShot(eventInstance);
		}
	}

	public void PlayEndSound()
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		if (!this.playSounds)
		{
			return;
		}
		string text = this.currentSounds[2];
		if (text != null && text.Length > 0)
		{
			EventInstance eventInstance = KFMOD.BeginOneShot(text, Vector3.zero, 1f);
			eventInstance.setParameterByName("sliderValue", this.value, false);
			KFMOD.EndOneShot(eventInstance);
		}
	}

	public AnimationCurve sliderWeightCurve;

	public static string[] DefaultSounds = new string[5];

	private string[] currentSounds;

	private bool playSounds = true;

	private float lastMoveTime;

	private float movePlayRate = 0.025f;

	private float lastMoveValue;

	public bool playedBoundaryBump;

	private ToolTip tooltip;

	public enum SoundType
	{
		Start,
		Move,
		End,
		BoundaryLow,
		BoundaryHigh,
		Num
	}

	public enum MoveSource
	{
		Keyboard,
		MouseDrag,
		MouseClick,
		Num
	}
}
