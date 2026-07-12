using System;

public class FossilMineWorkable : ComplexFabricatorWorkable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.shouldShowSkillPerkStatusItem = false;
	}

	public void SetShouldShowSkillPerkStatusItem(bool shouldItBeShown)
	{
		this.shouldShowSkillPerkStatusItem = shouldItBeShown;
		if (this.skillsUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(this.skillsUpdateHandle);
			this.skillsUpdateHandle = -1;
		}
		if (this.shouldShowSkillPerkStatusItem && !string.IsNullOrEmpty(this.requiredSkillPerk))
		{
			this.skillsUpdateHandle = Game.Instance.Subscribe(-1523247426, new Action<object>(this.UpdateStatusItem));
		}
		this.UpdateStatusItem(null);
	}
}
