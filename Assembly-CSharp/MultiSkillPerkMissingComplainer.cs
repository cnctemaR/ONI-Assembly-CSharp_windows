using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MultiSkillPerkMissingComplainer")]
public class MultiSkillPerkMissingComplainer : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.selectable = base.GetComponent<KSelectable>();
		if (this.requiredSkillPerks != null && this.requiredSkillPerks.Length > 1)
		{
			this.skillUpdateHandle = Game.Instance.Subscribe(-1523247426, new Action<object>(this.UpdateStatusItem));
		}
		else
		{
			global::Debug.LogWarning("Use SkillPerkMissingComplainer on " + base.gameObject.name + " if multiple skill perks are not required. This should only be used if a single dupe requires more than one perk.");
		}
		this.UpdateStatusItem(null);
	}

	protected override void OnCleanUp()
	{
		if (this.skillUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(this.skillUpdateHandle);
		}
		base.OnCleanUp();
	}

	protected virtual void UpdateStatusItem(object data = null)
	{
		if (this.selectable == null)
		{
			return;
		}
		if (this.requiredSkillPerks == null)
		{
			return;
		}
		bool flag = MinionResume.AnyMinionHasAllPerks(this.requiredSkillPerks, this.GetMyWorldId());
		if (!flag && this.workStatusItemHandle == Guid.Empty)
		{
			this.workStatusItemHandle = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.ColonyLacksDupeWithMultiSkillPerk, this.requiredSkillPerks);
			return;
		}
		if (flag && this.workStatusItemHandle != Guid.Empty)
		{
			this.selectable.RemoveStatusItem(this.workStatusItemHandle, false);
			this.workStatusItemHandle = Guid.Empty;
		}
	}

	public string[] requiredSkillPerks;

	private KSelectable selectable;

	private int skillUpdateHandle = -1;

	private Guid workStatusItemHandle;
}
