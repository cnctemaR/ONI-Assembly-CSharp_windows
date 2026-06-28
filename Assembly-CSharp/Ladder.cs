using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class Ladder : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Grid.HasLadder[Grid.PosToCell(this)] = true;
		Components.Ladders.Add(this);
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal, null);
		this.RefreshAnim();
		this.RefreshAnimBelow();
	}

	private void RefreshAnim()
	{
		int num = Grid.PosToCell(this);
		int num2 = Grid.CellAbove(num);
		GameObject gameObject = Grid.Objects[num2, 1];
		bool flag = false;
		if (gameObject != null && gameObject.GetComponent<Ladder>() != null)
		{
			flag = true;
		}
		if (flag)
		{
			base.GetComponent<KAnimControllerBase>().Play("off", KAnim.PlayMode.Once, 1f, 0f);
		}
		else
		{
			base.GetComponent<KAnimControllerBase>().Play("cap", KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	private void RefreshAnimBelow()
	{
		int num = Grid.PosToCell(this);
		int num2 = Grid.CellBelow(num);
		GameObject gameObject = Grid.Objects[num2, 1];
		if (gameObject != null)
		{
			Ladder component = gameObject.GetComponent<Ladder>();
			if (component != null)
			{
				component.RefreshAnim();
			}
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Grid.HasLadder[Grid.PosToCell(this)] = false;
		Components.Ladders.Remove(this);
		this.RefreshAnimBelow();
	}
}
