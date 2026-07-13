using System;
using System.Collections.Generic;
using FMOD.Studio;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class RocketInteriorSectionSideScreen : SideScreenContent
{
	public override int GetSideScreenSortOrder()
	{
		return 105;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.button.onClick += this.ClickViewInterior;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		global::UnityEngine.Object component = target.GetComponent<RocketModuleCluster>();
		ClustercraftInteriorDoor component2 = target.GetComponent<ClustercraftInteriorDoor>();
		RocketControlStation component3 = target.GetComponent<RocketControlStation>();
		Clustercraft component4 = target.GetComponent<Clustercraft>();
		return component != null || component3 != null || component2 != null || component4 != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			global::Debug.LogError("Invalid gameObject received");
			return;
		}
		RocketModuleCluster component = new_target.GetComponent<RocketModuleCluster>();
		RocketControlStation component2 = new_target.GetComponent<RocketControlStation>();
		ClustercraftInteriorDoor component3 = new_target.GetComponent<ClustercraftInteriorDoor>();
		Clustercraft component4 = new_target.GetComponent<Clustercraft>();
		this.IsInterior = component4 == null && component == null && (component2 != null || component3 != null);
		if (component != null)
		{
			this.moduleInterface = component.CraftInterface;
		}
		else if (component4 != null)
		{
			this.moduleInterface = component4.ModuleInterface;
		}
		else
		{
			this.moduleInterface = new_target.GetMyWorld().GetComponent<Clustercraft>().ModuleInterface;
		}
		this.moduleInterface.Unsubscribe(1512695988, new Action<object>(this.OnRocketModuleCountChanged));
		this.moduleInterface.Subscribe(1512695988, new Action<object>(this.OnRocketModuleCountChanged));
		this.Refresh();
	}

	public override void ClearTarget()
	{
		if (this.moduleInterface != null)
		{
			this.moduleInterface.Unsubscribe(1512695988, new Action<object>(this.OnRocketModuleCountChanged));
		}
		base.ClearTarget();
	}

	private void OnRocketModuleCountChanged(object o)
	{
		this.Refresh();
	}

	public void Refresh()
	{
		PassengerRocketModule passengerModule = this.GetPassengerModule();
		ClustercraftExteriorDoor clustercraftExteriorDoor = ((passengerModule == null) ? null : passengerModule.GetComponent<ClustercraftExteriorDoor>());
		bool flag = clustercraftExteriorDoor != null && clustercraftExteriorDoor.GetMyWorld() != null;
		this.button.isInteractable = passengerModule != null && (!this.IsInterior || flag);
		this.buttonLabel.SetText(this.IsInterior ? UI.UISIDESCREENS.ROCKETVIEWINTERIORSECTION.BUTTONVIEWEXTERIOR.LABEL : UI.UISIDESCREENS.ROCKETVIEWINTERIORSECTION.BUTTONVIEWINTERIOR.LABEL);
		this.tooltip.SetSimpleTooltip((passengerModule != null) ? (this.IsInterior ? (flag ? UI.UISIDESCREENS.ROCKETVIEWINTERIORSECTION.BUTTONVIEWEXTERIOR.DESC.text : UI.UISIDESCREENS.ROCKETVIEWINTERIORSECTION.BUTTONVIEWEXTERIOR.INVALID.text) : UI.UISIDESCREENS.ROCKETVIEWINTERIORSECTION.BUTTONVIEWINTERIOR.DESC.text) : UI.UISIDESCREENS.ROCKETVIEWINTERIORSECTION.BUTTONVIEWINTERIOR.INVALID.text);
		Sprite sprite;
		if (passengerModule != null)
		{
			sprite = Assets.GetSprite(this.IsInterior ? "rocket_small_habitat_open_out" : "rocket_small_habitat_open");
		}
		else
		{
			sprite = Assets.GetSprite("rocket_no_habitat_module");
		}
		this.interiorModuleIcon.sprite = sprite;
		this.interiorModuleIcon.color = Color.white;
	}

	private PassengerRocketModule GetPassengerModule()
	{
		if (!(this.moduleInterface == null))
		{
			return this.moduleInterface.GetPassengerModule();
		}
		return null;
	}

	private void ClickViewInterior()
	{
		PassengerRocketModule passengerModule = this.GetPassengerModule();
		if (passengerModule == null)
		{
			this.Refresh();
			return;
		}
		ClustercraftExteriorDoor component = passengerModule.GetComponent<ClustercraftExteriorDoor>();
		WorldContainer targetWorld = component.GetTargetWorld();
		WorldContainer myWorld = component.GetMyWorld();
		if (ClusterManager.Instance.activeWorld == targetWorld)
		{
			if (myWorld != null && myWorld.id != 255)
			{
				AudioMixer.instance.Stop(passengerModule.interiorReverbSnapshot, STOP_MODE.ALLOWFADEOUT);
				AudioMixer.instance.PauseSpaceVisibleSnapshot(false);
				ClusterManager.Instance.SetActiveWorld(myWorld.id);
				SelectTool.Instance.Select(passengerModule.GetComponent<KSelectable>(), false);
			}
		}
		else
		{
			AudioMixer.instance.Start(passengerModule.interiorReverbSnapshot);
			AudioMixer.instance.PauseSpaceVisibleSnapshot(true);
			ClusterManager.Instance.SetActiveWorld(targetWorld.id);
			bool flag = false;
			if (Components.RocketControlStations != null)
			{
				List<RocketControlStation> worldItems = Components.RocketControlStations.GetWorldItems(targetWorld.id, false);
				if (worldItems != null && worldItems.Count > 0)
				{
					RocketControlStation rocketControlStation = worldItems[0];
					SelectTool.Instance.Select(rocketControlStation.GetComponent<KSelectable>(), false);
					flag = true;
				}
			}
			if (!flag)
			{
				ClustercraftInteriorDoor interiorDoor = component.GetInteriorDoor();
				if (interiorDoor != null)
				{
					SelectTool.Instance.Select(interiorDoor.GetComponent<KSelectable>(), false);
				}
			}
		}
		DetailsScreen.Instance.ClearSecondarySideScreen();
		ManagementMenu.Instance.CloseClusterMap();
		this.Refresh();
	}

	public Image interiorModuleIcon;

	public KButton button;

	public LocText buttonLabel;

	public ToolTip tooltip;

	private CraftModuleInterface moduleInterface;

	public const string NOT_APPLICABLE_ICON_NAME = "rocket_no_habitat_module";

	public const string HABITAT_MODULE_SEE_INTERIOR_ICON_NAME = "rocket_small_habitat_open";

	public const string HABITAT_MODULE_SEE_EXTERIOR_ICON_NAME = "rocket_small_habitat_open_out";

	public Color noPassengerModuleImageColor = new Color(0.84313726f, 0.23529412f, 0.23529412f, 1f);

	private bool IsInterior;
}
