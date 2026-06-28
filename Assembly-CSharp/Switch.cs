using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Switch : KMonoBehaviour, ISaveLoadableJson, IToggleHandler, IEffectDescriptor
{
	public event Action<bool> OnToggle;

	public bool IsSwitchedOn
	{
		get
		{
			return this.switchedOn;
		}
	}

	protected override void OnSpawn()
	{
		this.openToggleIndex = this.openSwitch.SetTarget(this);
		this.animController = base.GetComponent<KBatchedAnimController>();
		this.Subscribe(493375141, new EventSystem.EventHandler(this.OnRefreshUserMenu));
		this.UpdateCircuit();
	}

	protected override void OnCleanUp()
	{
		bool flag = this.switchedOn;
		this.switchedOn = true;
		this.UpdateCircuit();
		this.switchedOn = flag;
	}

	public void HandleToggle()
	{
		this.Toggle();
	}

	public bool IsHandlerOn()
	{
		return this.switchedOn;
	}

	public bool IsConnected()
	{
		int num = Grid.PosToCell(this.transform.position);
		GameObject gameObject = Grid.Objects[num, (int)this.objectLayer];
		return gameObject != null && gameObject.GetComponent<IDisconnectable>() != null;
	}

	public void UpdateCircuit()
	{
		int num = Grid.PosToCell(this.transform.position);
		GameObject gameObject = Grid.Objects[num, (int)this.objectLayer];
		IDisconnectable disconnectable = null;
		if (gameObject != null)
		{
			disconnectable = gameObject.GetComponent<IDisconnectable>();
		}
		bool flag = this.switchedOn;
		if (disconnectable != null)
		{
			if (this.switchedOn)
			{
				this.animController.Play("on", KAnim.PlayMode.Once, 1f, 0f);
				disconnectable.Connect();
			}
			else
			{
				this.animController.Play("off", KAnim.PlayMode.Once, 1f, 0f);
				disconnectable.Disconnect();
			}
		}
		else
		{
			this.switchedOn = false;
			this.animController.Play("off", KAnim.PlayMode.Once, 1f, 0f);
		}
		if (flag != this.switchedOn)
		{
			this.userMenu.Refresh();
		}
	}

	private void OnMinionToggle()
	{
		if (!DebugHandler.InstantBuildMode)
		{
			this.openSwitch.Toggle(this.openToggleIndex);
		}
		else
		{
			this.Toggle();
		}
	}

	protected virtual void Toggle()
	{
		this.switchedOn = !this.switchedOn;
		this.UpdateCircuit();
		this.userMenu.Refresh();
		if (this.OnToggle != null)
		{
			this.OnToggle(this.switchedOn);
		}
	}

	protected virtual void OnRefreshUserMenu(object data)
	{
		if (this.switchedOn)
		{
			this.userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_power", "Turn Off", new global::System.Action(this.OnMinionToggle), global::Action.ToggleEnabled, null, null, null, null, string.Empty));
		}
		else
		{
			this.userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_power", "Turn On", new global::System.Action(this.OnMinionToggle), global::Action.ToggleEnabled, null, null, null, null, string.Empty));
		}
	}

	public int DescriptionOrder { get; set; }

	public virtual List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, UI.BUILDINGEFFECTS.REQUIRESMANUALOPERATION), UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESMANUALOPERATION);
		list.Add(descriptor);
		return list;
	}

	public List<Descriptor> GetEffectDescriptions(BuildingDef def)
	{
		return null;
	}

	[Serialize]
	private bool switchedOn = true;

	[MyCmpAdd]
	protected UserMenu userMenu;

	[MyCmpAdd]
	private Toggleable openSwitch;

	[SerializeField]
	public ObjectLayer objectLayer;

	private KBatchedAnimController animController;

	private int openToggleIndex;
}
