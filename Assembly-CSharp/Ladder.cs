using System;

[SkipSaveFileSerialization]
public class Ladder : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Grid.HasLadder[Grid.PosToCell(this)] = true;
		Components.Ladders.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal, null);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Grid.HasLadder[Grid.PosToCell(this)] = false;
		Components.Ladders.Remove(this);
	}
}
