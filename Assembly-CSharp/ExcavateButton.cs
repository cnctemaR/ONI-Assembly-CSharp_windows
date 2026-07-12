using System;
using STRINGS;

public class ExcavateButton : KMonoBehaviour, ISidescreenButtonControl
{
	public string SidescreenButtonText
	{
		get
		{
			if (this.isMarkedForDig == null || !this.isMarkedForDig())
			{
				return CODEX.STORY_TRAITS.FOSSILHUNT.UISIDESCREENS.DIG_SITE_EXCAVATE_BUTTON;
			}
			return CODEX.STORY_TRAITS.FOSSILHUNT.UISIDESCREENS.DIG_SITE_CANCEL_EXCAVATION_BUTTON;
		}
	}

	public string SidescreenButtonTooltip
	{
		get
		{
			if (this.isMarkedForDig == null || !this.isMarkedForDig())
			{
				return CODEX.STORY_TRAITS.FOSSILHUNT.UISIDESCREENS.DIG_SITE_EXCAVATE_BUTTON_TOOLTIP;
			}
			return CODEX.STORY_TRAITS.FOSSILHUNT.UISIDESCREENS.DIG_SITE_CANCEL_EXCAVATION_BUTTON_TOOLTIP;
		}
	}

	public int HorizontalGroupID()
	{
		return -1;
	}

	public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
	{
		throw new NotImplementedException();
	}

	public bool SidescreenEnabled()
	{
		return true;
	}

	public bool SidescreenButtonInteractable()
	{
		return true;
	}

	public void OnSidescreenButtonPressed()
	{
		global::System.Action onButtonPressed = this.OnButtonPressed;
		if (onButtonPressed == null)
		{
			return;
		}
		onButtonPressed();
	}

	public int ButtonSideScreenSortOrder()
	{
		return 20;
	}

	public Func<bool> isMarkedForDig;

	public global::System.Action OnButtonPressed;
}
