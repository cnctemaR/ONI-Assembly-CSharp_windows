using System;
using STRINGS;
using UnityEngine;

public class SelfDestructButtonSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		this.Refresh();
		this.button.onClick += this.TriggerDestruct;
	}

	public override int GetSideScreenSortOrder()
	{
		return -150;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<CraftModuleInterface>() != null && target.HasTag(GameTags.RocketInSpace);
	}

	public override void SetTarget(GameObject target)
	{
		this.craftInterface = target.GetComponent<CraftModuleInterface>();
		this.acknowledgeWarnings = false;
		this.craftInterface.Subscribe<SelfDestructButtonSideScreen>(-1582839653, SelfDestructButtonSideScreen.TagsChangedDelegate);
		this.Refresh();
	}

	public override void ClearTarget()
	{
		if (this.craftInterface != null)
		{
			this.craftInterface.Unsubscribe<SelfDestructButtonSideScreen>(-1582839653, SelfDestructButtonSideScreen.TagsChangedDelegate, false);
			this.craftInterface = null;
		}
	}

	private void OnTagsChanged(object data)
	{
		if (((TagChangedEventData)data).tag == GameTags.RocketStranded)
		{
			this.Refresh();
		}
	}

	private void TriggerDestruct()
	{
		if (this.acknowledgeWarnings)
		{
			this.craftInterface.gameObject.Trigger(-1061799784, null);
			this.acknowledgeWarnings = false;
		}
		else
		{
			this.acknowledgeWarnings = true;
		}
		this.Refresh();
	}

	private void Refresh()
	{
		if (this.craftInterface == null)
		{
			return;
		}
		this.statusText.text = UI.UISIDESCREENS.SELFDESTRUCTSIDESCREEN.MESSAGE_TEXT;
		if (this.acknowledgeWarnings)
		{
			this.button.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.SELFDESTRUCTSIDESCREEN.BUTTON_TEXT_CONFIRM;
			this.button.GetComponentInChildren<ToolTip>().toolTip = UI.UISIDESCREENS.SELFDESTRUCTSIDESCREEN.BUTTON_TOOLTIP_CONFIRM;
			return;
		}
		this.button.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.SELFDESTRUCTSIDESCREEN.BUTTON_TEXT;
		this.button.GetComponentInChildren<ToolTip>().toolTip = UI.UISIDESCREENS.SELFDESTRUCTSIDESCREEN.BUTTON_TOOLTIP;
	}

	public KButton button;

	public LocText statusText;

	private CraftModuleInterface craftInterface;

	private bool acknowledgeWarnings;

	private static readonly EventSystem.IntraObjectHandler<SelfDestructButtonSideScreen> TagsChangedDelegate = new EventSystem.IntraObjectHandler<SelfDestructButtonSideScreen>(delegate(SelfDestructButtonSideScreen cmp, object data)
	{
		cmp.OnTagsChanged(data);
	});
}
