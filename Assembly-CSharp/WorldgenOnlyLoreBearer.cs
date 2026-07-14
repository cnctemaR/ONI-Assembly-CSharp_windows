using System;
using KSerialization;

public class WorldgenOnlyLoreBearer : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.Subscribe(1119167081, new Action<object>(this.OnNewGameSpawn));
	}

	private void UpdateLore()
	{
		this.loreBearer.hideLore = !this.hasLore;
	}

	protected override void OnSpawn()
	{
		this.UpdateLore();
	}

	private void OnNewGameSpawn(object obj)
	{
		this.hasLore = true;
		this.UpdateLore();
	}

	[MyCmpReq]
	private LoreBearer loreBearer;

	[Serialize]
	public bool hasLore;
}
