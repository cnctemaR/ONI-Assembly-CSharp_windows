using System;
using System.Collections.Generic;
using Klei;
using UnityEngine;

public class PopFXManager : KScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		PopFXManager.Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.ready = true;
		if (GenericGameSettings.instance.disablePopFx)
		{
			return;
		}
		for (int i = 0; i < 20; i++)
		{
			PopFX popFX = this.SpawnFX(this.sprite_Plus, string.Empty, null, Vector3.zero, 1.5f, false, true);
			popFX.Recycle();
		}
	}

	public bool Ready()
	{
		return this.ready;
	}

	public PopFX SpawnFX(Sprite icon, string text, Transform target_transform, Vector3 offset, float lifetime = 1.5f, bool track_target = false, bool force_spawn = false)
	{
		if (GenericGameSettings.instance.disablePopFx)
		{
			return null;
		}
		if (Game.IsQuitting())
		{
			return null;
		}
		Vector3 vector = offset;
		if (target_transform != null)
		{
			vector += target_transform.GetPosition();
		}
		if (!force_spawn)
		{
			int num = Grid.PosToCell(vector);
			if (!Grid.IsValidCell(num) || !Grid.IsVisible(num))
			{
				return null;
			}
		}
		PopFX popFX;
		if (this.Pool.Count > 0)
		{
			popFX = this.Pool[0];
			this.Pool[0].gameObject.SetActive(true);
			this.Pool[0].Spawn(icon, text, target_transform, offset, lifetime, track_target);
			this.Pool.RemoveAt(0);
		}
		else
		{
			GameObject gameObject = Util.KInstantiate(this.Prefab_PopFX, base.gameObject, "Pooled_PopFX");
			gameObject.transform.localScale = Vector3.one;
			popFX = gameObject.GetComponent<PopFX>();
			popFX.Spawn(icon, text, target_transform, offset, lifetime, track_target);
		}
		return popFX;
	}

	public PopFX SpawnFX(Sprite icon, string text, Transform target_transform, float lifetime = 1.5f, bool track_target = false)
	{
		return this.SpawnFX(icon, text, target_transform, Vector3.zero, lifetime, track_target, false);
	}

	public void RecycleFX(PopFX fx)
	{
		this.Pool.Add(fx);
	}

	public static PopFXManager Instance;

	public GameObject Prefab_PopFX;

	public List<PopFX> Pool = new List<PopFX>();

	public Sprite sprite_Plus;

	public Sprite sprite_Negative;

	public Sprite sprite_Resource;

	public Sprite sprite_Building;

	public Sprite sprite_Research;

	private bool ready;
}
