using System;
using STRINGS;

public class ClustercraftInteriorDoor : KMonoBehaviour, ISidescreenButtonControl
{
	public string SidescreenButtonText
	{
		get
		{
			return UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWEXTERIOR.LABEL;
		}
	}

	public string SidescreenButtonTooltip
	{
		get
		{
			return this.SidescreenButtonInteractable() ? UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWEXTERIOR.LABEL : UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWEXTERIOR.INVALID;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.ClusterCraftInteriorDoors.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.ClusterCraftInteriorDoors.Remove(this);
		base.OnCleanUp();
	}

	public bool SidescreenEnabled()
	{
		return true;
	}

	public bool SidescreenButtonInteractable()
	{
		WorldContainer myWorld = base.gameObject.GetMyWorld();
		return myWorld.ParentWorldId != 255 && myWorld.ParentWorldId != myWorld.id;
	}

	public void OnSidescreenButtonPressed()
	{
		ClusterManager.Instance.SetActiveWorld(base.gameObject.GetMyWorld().ParentWorldId);
	}

	public int ButtonSideScreenSortOrder()
	{
		return 20;
	}

	public void SetButtonTextOverride(ButtonMenuTextOverride text)
	{
		throw new NotImplementedException();
	}

	public int HorizontalGroupID()
	{
		return -1;
	}
}
