using System;
using UnityEngine;

public class Thought : Resource
{
	public Thought(string id, ResourceSet parent, Sprite icon, string mode_icon, string sound_name, string bubble, string speech_prefix, LocString hover_text, bool show_immediately = false, float show_time = 4f)
		: base(id, parent, null)
	{
		this.sprite = icon;
		if (mode_icon != null)
		{
			this.modeSprite = Assets.GetSprite(mode_icon);
		}
		this.bubbleSprite = Assets.GetSprite(bubble);
		this.sound = sound_name;
		this.speechPrefix = speech_prefix;
		this.hoverText = hover_text;
		this.showImmediately = show_immediately;
		this.showTime = show_time;
	}

	public Thought(string id, ResourceSet parent, string icon, string mode_icon, string sound_name, string bubble, string speech_prefix, LocString hover_text, bool show_immediately = false, float show_time = 4f)
		: base(id, parent, null)
	{
		this.sprite = Assets.GetSprite(icon);
		if (mode_icon != null)
		{
			this.modeSprite = Assets.GetSprite(mode_icon);
		}
		this.bubbleSprite = Assets.GetSprite(bubble);
		this.sound = sound_name;
		this.speechPrefix = speech_prefix;
		this.hoverText = hover_text;
		this.showImmediately = show_immediately;
		this.showTime = show_time;
	}

	public void PlayAsSpeech(SpeechMonitor.Instance speechMonitorInstance)
	{
		speechMonitorInstance.PlaySpeech(this.speechPrefix, this.sound);
	}

	public int priority;

	public Sprite sprite;

	public Sprite modeSprite;

	public string sound;

	public Sprite bubbleSprite;

	public string speechPrefix;

	public LocString hoverText;

	public bool showImmediately;

	public float showTime;
}
