using System;
using KSerialization;
using STRINGS;

public class FactionAlignment : KMonoBehaviour
{
	[MyCmpAdd]
	public Health health { get; private set; }

	public AttackableBase attackable { get; private set; }

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.health = base.GetComponent<Health>();
		this.attackable = base.GetComponent<AttackableBase>();
		Components.FactionAlignments.Add(this);
		base.Subscribe<FactionAlignment>(493375141, FactionAlignment.OnRefreshUserMenuDelegate);
		base.Subscribe<FactionAlignment>(2127324410, FactionAlignment.SetPlayerTargetedFalseDelegate);
		if (this.alignmentActive)
		{
			FactionManager.Instance.GetFaction(this.Alignment).Members.Add(this);
		}
		base.Subscribe<FactionAlignment>(1623392196, FactionAlignment.OnDeathDelegate);
		this.UpdateStatusItem();
	}

	private void OnDeath(object data)
	{
		this.SetAlignmentActive(false);
	}

	public void SetAlignmentActive(bool active)
	{
		this.SetPlayerTargetable(active);
		this.alignmentActive = active;
		if (active)
		{
			FactionManager.Instance.GetFaction(this.Alignment).Members.Add(this);
			return;
		}
		FactionManager.Instance.GetFaction(this.Alignment).Members.Remove(this);
	}

	public bool IsAlignmentActive()
	{
		return FactionManager.Instance.GetFaction(this.Alignment).Members.Contains(this);
	}

	public void SetPlayerTargetable(bool state)
	{
		this.targetable = state;
		if (!state)
		{
			this.SetPlayerTargeted(false);
		}
	}

	public void SetPlayerTargeted(bool state)
	{
		this.targeted = state && this.targetable;
		this.UpdateStatusItem();
	}

	private void UpdateStatusItem()
	{
		if (this.targeted)
		{
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.OrderAttack, null);
			return;
		}
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.OrderAttack, false);
	}

	public void SwitchAlignment(FactionManager.FactionID newAlignment)
	{
		this.SetAlignmentActive(false);
		this.Alignment = newAlignment;
		this.SetAlignmentActive(true);
	}

	protected override void OnCleanUp()
	{
		Components.FactionAlignments.Remove(this);
		FactionManager.Instance.GetFaction(this.Alignment).Members.Remove(this);
		base.OnCleanUp();
	}

	private void OnRefreshUserMenu(object data)
	{
		if (this.Alignment == FactionManager.FactionID.Duplicant)
		{
			return;
		}
		if (!this.IsAlignmentActive())
		{
			return;
		}
		KIconButtonMenu.ButtonInfo buttonInfo = ((!this.targeted) ? new KIconButtonMenu.ButtonInfo("action_attack", UI.USERMENUACTIONS.ATTACK.NAME, delegate
		{
			this.SetPlayerTargeted(true);
		}, global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.ATTACK.TOOLTIP, true) : new KIconButtonMenu.ButtonInfo("action_attack", UI.USERMENUACTIONS.CANCELATTACK.NAME, delegate
		{
			this.SetPlayerTargeted(false);
		}, global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.CANCELATTACK.TOOLTIP, true));
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 1f);
	}

	[Serialize]
	private bool alignmentActive = true;

	public FactionManager.FactionID Alignment;

	[Serialize]
	public bool targeted;

	[Serialize]
	public bool targetable = true;

	private static readonly EventSystem.IntraObjectHandler<FactionAlignment> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<FactionAlignment>(delegate(FactionAlignment component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<FactionAlignment> OnDeathDelegate = new EventSystem.IntraObjectHandler<FactionAlignment>(delegate(FactionAlignment component, object data)
	{
		component.OnDeath(data);
	});

	private static readonly EventSystem.IntraObjectHandler<FactionAlignment> SetPlayerTargetedFalseDelegate = new EventSystem.IntraObjectHandler<FactionAlignment>(delegate(FactionAlignment component, object data)
	{
		component.SetPlayerTargeted(false);
	});
}
