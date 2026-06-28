using System;
using STRINGS;
using UnityEngine;

public class StructuralDamage : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.Subscribe(-184635526, new EventSystem.EventHandler(this.OnDoDamage));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		float damage = this.GetDamage();
		if (damage > 0f)
		{
			this.OnDoDamage(false);
		}
	}

	private void OnDoDamage(object data)
	{
		if ((bool)data)
		{
			this.notifier.Remove(this.structuralDamage);
			Util.KDestroyGameObject(base.gameObject);
			foreach (int num in this.building.PlacementCells)
			{
				GameUtil.KInstantiate(EffectPrefabs.Instance.ResourceMelted, Grid.CellToPosCCC(num, Grid.SceneLayer.TileMain), Grid.SceneLayer.TileMain, Folder.FX, null, 0);
			}
		}
	}

	private float GetDamage()
	{
		float num = 0f;
		foreach (int num2 in this.building.PlacementCells)
		{
			num = Mathf.Max(num, Grid.Damage[num2]);
		}
		return num;
	}

	protected override void OnCleanUp()
	{
		this.notifier.Remove(this.structuralDamage);
	}

	[MyCmpReq]
	private Building building;

	[MyCmpAdd]
	private Notifier notifier;

	private Notification structuralDamage = new Notification(MISC.NOTIFICATIONS.STRUCTURALDAMAGE.NAME, NotificationType.BadMinor, null, null, null, false, 0f, null, null, null);
}
