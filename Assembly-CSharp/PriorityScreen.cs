using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class PriorityScreen : KScreen
{
	public void InstantiateButtons(Action<PriorityScreen.PriorityClass, int> on_click, string tooltip_string_key_root, bool playSelectionSound = true)
	{
		for (int i = 1; i <= 9; i++)
		{
			int idx = i;
			PrioritySetting priority = new PrioritySetting(PriorityScreen.PriorityClass.basic, idx);
			PriorityButton priorityButton = global::Util.KInstantiateUI<PriorityButton>(this.buttonPrefab_basic.gameObject, base.gameObject, false);
			this.buttons_basic.Add(priorityButton);
			priorityButton.toggle.onClick += delegate
			{
				if (playSelectionSound)
				{
					this.PlayPriorityConfirmSound(priority);
				}
				on_click(PriorityScreen.PriorityClass.basic, idx);
			};
			priorityButton.text.text = i.ToString();
			priorityButton.priority = priority;
			string text = string.Format(Strings.Get(tooltip_string_key_root + ".BASIC"), i);
			priorityButton.tooltip.SetSimpleTooltip(text);
		}
		this.buttonPrefab_basic.gameObject.SetActive(false);
		this.buttonPrefab_high.gameObject.SetActive(false);
		this.buttonPrefab_emergency.gameObject.SetActive(false);
	}

	private void RefreshButton(PriorityButton b, PriorityScreen.PriorityClass priorityClass, int priority, bool play_sound)
	{
		if (priorityClass == b.priority.priority_class && priority == b.priority.priority_value)
		{
			b.toggle.Select();
			b.toggle.isOn = true;
			if (play_sound)
			{
				b.toggle.soundPlayer.Play(0);
			}
		}
		else
		{
			b.toggle.isOn = false;
		}
	}

	public void SetScreenPriority(PriorityScreen.PriorityClass priorityClass, int priority, bool play_sound = false)
	{
		if (this.priorityClass != priorityClass || this.priority != priority)
		{
			this.buttons_basic.ForEach(delegate(PriorityButton b)
			{
				this.RefreshButton(b, priorityClass, priority, play_sound);
			});
			this.buttons_high.ForEach(delegate(PriorityButton b)
			{
				this.RefreshButton(b, priorityClass, priority, play_sound);
			});
			this.buttons_emergency.ForEach(delegate(PriorityButton b)
			{
				this.RefreshButton(b, priorityClass, priority, play_sound);
			});
			this.priorityClass = priorityClass;
			this.priority = priority;
		}
	}

	public PrioritySetting GetScreenPriority()
	{
		return new PrioritySetting(this.priorityClass, this.priority);
	}

	public void PlayPriorityConfirmSound(PrioritySetting priority)
	{
		EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("Priority_Tool_Confirm", false), Vector3.zero);
		if (eventInstance != null)
		{
			float num = 0f;
			num += (float)priority.priority_value;
			eventInstance.setParameterValue("priority", num);
			KFMOD.EndOneShot(eventInstance);
		}
	}

	[SerializeField]
	protected PriorityButton buttonPrefab_basic;

	[SerializeField]
	protected PriorityButton buttonPrefab_high;

	[SerializeField]
	protected PriorityButton buttonPrefab_emergency;

	[SerializeField]
	protected GameObject spacerPrefab;

	protected List<PriorityButton> buttons_basic = new List<PriorityButton>();

	protected List<PriorityButton> buttons_high = new List<PriorityButton>();

	protected List<PriorityButton> buttons_emergency = new List<PriorityButton>();

	private int priority;

	private PriorityScreen.PriorityClass priorityClass = PriorityScreen.PriorityClass.basic;

	public enum PriorityClass
	{
		basic,
		high,
		emergency
	}
}
