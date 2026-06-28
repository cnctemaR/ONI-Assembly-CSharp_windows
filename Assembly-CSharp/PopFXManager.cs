using System;
using System.Collections.Generic;
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
		for (int i = 0; i < 20; i++)
		{
			PopFX popFX = this.SpawnFX(this.sprite_Plus, "", null, Vector3.zero, 1.5f, false, true);
			popFX.Recycle();
		}
	}

	public bool Ready()
	{
		return this.ready;
	}

	public PopFX SpawnFX(Sprite icon, string text, Transform target_transform, Vector3 offset, float lifetime = 1.5f, bool track_target = false, bool force_spawn = false)
	{
		PopFX popFX;
		if (Game.IsQuitting())
		{
			popFX = null;
		}
		else
		{
			Vector3 vector = offset;
			if (target_transform != null)
			{
				vector += target_transform.position;
			}
			if (!force_spawn && Grid.Visible[Grid.PosToCell(vector)] == 0)
			{
				popFX = null;
			}
			else
			{
				PopFX popFX2;
				if (this.Pool.Count > 0)
				{
					popFX2 = this.Pool[0];
					this.Pool[0].gameObject.SetActive(true);
					this.Pool[0].Spawn(icon, text, target_transform, offset, lifetime, track_target);
					this.Pool.RemoveAt(0);
				}
				else
				{
					GameObject gameObject = Util.KInstantiate(this.Prefab_PopFX, base.gameObject, "Pooled_PopFX");
					gameObject.transform.localScale = Vector3.one;
					popFX2 = gameObject.GetComponent<PopFX>();
					popFX2.Spawn(icon, text, target_transform, offset, lifetime, track_target);
				}
				popFX = popFX2;
			}
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

	private bool ready = false;
}
