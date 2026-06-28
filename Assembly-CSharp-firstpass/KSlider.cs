using System;
using System.Diagnostics;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KSlider : Slider
{
	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event global::System.Action onReleaseHandle;

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event global::System.Action onDrag;

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event global::System.Action onPointerDown;

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event global::System.Action onMove;

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

	public void PlayStartSound()
	{
		if (KInputManager.isFocused)
		{
			if (this.playSounds)
			{
				string text = this.currentSounds[0];
				if (text != null && text.Length > 0)
				{
					KFMOD.PlayOneShot(text);
				}
			}
		}
	}

	public void PlayMoveSound(KSlider.MoveSource moveSource)
	{
		if (KInputManager.isFocused)
		{
			if (this.playSounds)
			{
				float num = Time.unscaledTime - this.lastMoveTime;
				if (num >= this.movePlayRate)
				{
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
						FMOD.Studio.EventInstance eventInstance = KFMOD.BeginOneShot(text, Vector3.zero);
						eventInstance.setParameterValue("sliderValue", num2);
						eventInstance.setParameterValue("timeSinceLast", num);
						KFMOD.EndOneShot(eventInstance);
					}
				}
			}
		}
	}

	public void PlayEndSound()
	{
		if (KInputManager.isFocused)
		{
			if (this.playSounds)
			{
				string text = this.currentSounds[2];
				if (text != null && text.Length > 0)
				{
					FMOD.Studio.EventInstance eventInstance = KFMOD.BeginOneShot(text, Vector3.zero);
					eventInstance.setParameterValue("sliderValue", this.value);
					KFMOD.EndOneShot(eventInstance);
				}
			}
		}
	}

	public static string[] DefaultSounds = new string[5];

	private string[] currentSounds = null;

	private bool playSounds = true;

	private float lastMoveTime;

	private float movePlayRate = 0.025f;

	private float lastMoveValue;

	public bool playedBoundaryBump = false;

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
