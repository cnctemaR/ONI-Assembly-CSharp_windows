using System;
using System.Collections.Generic;
using FMOD.Studio;
using STRINGS;
using UnityEngine;

public class PriorityScreen : KScreen
{
	public void InstantiateButtons(Action<PrioritySetting> on_click, bool playSelectionSound = true)
	{
		this.onClick = on_click;
		for (int i = 1; i <= 9; i++)
		{
			int num = i;
			PriorityButton priorityButton = global::Util.KInstantiateUI<PriorityButton>(this.buttonPrefab_basic.gameObject, this.buttonPrefab_basic.transform.parent.gameObject, false);
			this.buttons_basic.Add(priorityButton);
			priorityButton.playSelectionSound = playSelectionSound;
			priorityButton.onClick = this.onClick;
			priorityButton.text.text = num.ToString();
			priorityButton.priority = new PrioritySetting(PriorityScreen.PriorityClass.basic, num);
			priorityButton.tooltip.SetSimpleTooltip(string.Format(UI.PRIORITYSCREEN.BASIC, num));
		}
		this.buttonPrefab_basic.gameObject.SetActive(false);
		this.EmergencyContainer.SetActive(false);
		this.button_emergency.gameObject.SetActive(false);
		this.button_toggleHigh.gameObject.SetActive(false);
		this.PriorityMenuContainer.SetActive(true);
		this.button_priorityMenu.gameObject.SetActive(true);
		this.button_priorityMenu.onClick += this.PriorityButtonClicked;
		this.button_priorityMenu.GetComponent<ToolTip>().SetSimpleTooltip(UI.PRIORITYSCREEN.OPEN_JOBS_SCREEN);
		this.diagram.SetActive(false);
		this.SetScreenPriority(new PrioritySetting(PriorityScreen.PriorityClass.basic, 5), false);
	}

	private void OnClick(PrioritySetting priority)
	{
		if (this.onClick != null)
		{
			this.onClick(priority);
		}
	}

	public void ShowDiagram(bool show)
	{
		this.diagram.SetActive(show);
	}

	public void ResetPriority()
	{
		this.SetScreenPriority(new PrioritySetting(PriorityScreen.PriorityClass.basic, 5), false);
	}

	public void PriorityButtonClicked()
	{
		ManagementMenu.Instance.TogglePriorities();
	}

	private void RefreshButton(PriorityButton b, PrioritySetting priority, bool play_sound)
	{
		if (b.priority == priority)
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

	public void SetScreenPriority(PrioritySetting priority, bool play_sound = false)
	{
		if (this.lastSelectedPriority == priority)
		{
			return;
		}
		this.lastSelectedPriority = priority;
		if (priority.priority_class == PriorityScreen.PriorityClass.high)
		{
			this.button_toggleHigh.isOn = true;
		}
		else if (priority.priority_class == PriorityScreen.PriorityClass.basic)
		{
			this.button_toggleHigh.isOn = false;
		}
		for (int i = 0; i < this.buttons_basic.Count; i++)
		{
			this.buttons_basic[i].priority = new PrioritySetting((!this.button_toggleHigh.isOn) ? PriorityScreen.PriorityClass.basic : PriorityScreen.PriorityClass.high, i + 1);
			this.buttons_basic[i].tooltip.SetSimpleTooltip(string.Format((!this.button_toggleHigh.isOn) ? UI.PRIORITYSCREEN.BASIC : UI.PRIORITYSCREEN.HIGH, i + 1));
			this.RefreshButton(this.buttons_basic[i], this.lastSelectedPriority, play_sound);
		}
		this.RefreshButton(this.button_emergency, this.lastSelectedPriority, play_sound);
	}

	public PrioritySetting GetLastSelectedPriority()
	{
		return this.lastSelectedPriority;
	}

	public static void PlayPriorityConfirmSound(PrioritySetting priority)
	{
		EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("Priority_Tool_Confirm", false), Vector3.zero);
		if (eventInstance.isValid())
		{
			float num = 0f;
			if (priority.priority_class >= PriorityScreen.PriorityClass.high)
			{
				num += 10f;
			}
			if (priority.priority_class >= PriorityScreen.PriorityClass.emergency)
			{
				num += 9f;
			}
			num += (float)priority.priority_value;
			eventInstance.setParameterValue("priority", num);
			KFMOD.EndOneShot(eventInstance);
		}
	}

	[SerializeField]
	protected PriorityButton buttonPrefab_basic;

	[SerializeField]
	protected GameObject EmergencyContainer;

	[SerializeField]
	protected PriorityButton button_emergency;

	[SerializeField]
	protected GameObject PriorityMenuContainer;

	[SerializeField]
	protected KButton button_priorityMenu;

	[SerializeField]
	protected KToggle button_toggleHigh;

	[SerializeField]
	protected GameObject diagram;

	protected List<PriorityButton> buttons_basic = new List<PriorityButton>();

	protected List<PriorityButton> buttons_emergency = new List<PriorityButton>();

	private PrioritySetting priority;

	private PrioritySetting lastSelectedPriority = new PrioritySetting(PriorityScreen.PriorityClass.basic, -1);

	private Action<PrioritySetting> onClick;

	public enum PriorityClass
	{
		idle = -1,
		basic,
		high,
		emergency
	}
}
