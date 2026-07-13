using System;
using System.Collections.Generic;
using Klei;
using UnityEngine;

public class PopFXManager : KScreen
{
	public static void DestroyInstance()
	{
		PopFXManager.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		PopFXManager.Instance = this;
		this.Prefab_PopFxGroup = new GameObject("Prefab_PopFxGroup");
		this.Prefab_PopFxGroup.AddComponent<PopFxGroup>();
		this.Prefab_PopFxGroup.transform.SetParent(this.Prefab_PopFX.transform.parent);
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
			PopFxGroup popFxGroup = this.CreatePopFxGroup();
			PopFX popFX = this.CreatePopFX();
			this.Pool.Add(popFX);
			this.GroupPool.Add(popFxGroup);
		}
	}

	public bool Ready()
	{
		return this.ready;
	}

	public PopFX SpawnFX(Sprite mainIcon, string text, Transform target_transform, float lifetime = 1.5f, bool track_target = false)
	{
		return this.SpawnFX(mainIcon, text, target_transform, Vector3.zero, lifetime, track_target, false);
	}

	public PopFX SpawnFX(Sprite mainIcon, string text, Transform target_transform, Vector3 offset, float lifetime = 1.5f, bool track_target = false, bool force_spawn = false)
	{
		return this.SpawnFX(mainIcon, null, text, target_transform, offset, lifetime, true, track_target, force_spawn);
	}

	public PopFX SpawnFX(Sprite mainIcon, Sprite secondaryIcon, string text, Transform target_transform, Vector3 offset, float lifetime = 1.5f, bool selfAdjustPositionIfInGroup = true, bool track_target = false, bool force_spawn = false)
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
		int num = Grid.PosToCell(vector);
		if (!force_spawn && (!Grid.IsValidCell(num) || !Grid.IsVisible(num) || (CameraController.Instance != null && !CameraController.Instance.IsVisiblePosExtended(vector))))
		{
			return null;
		}
		PopFX orCreatePopFX = this.GetOrCreatePopFX(mainIcon, secondaryIcon, text, target_transform, offset, selfAdjustPositionIfInGroup, lifetime, track_target);
		PopFxGroup popFxGroup;
		if (!this.AliveGroups.TryGetValue(num, out popFxGroup) || popFxGroup == null)
		{
			if (this.GroupPool.Count > 0)
			{
				popFxGroup = this.GroupPool[0];
				this.GroupPool[0].gameObject.SetActive(true);
				this.GroupPool.RemoveAt(0);
			}
			else
			{
				popFxGroup = this.CreatePopFxGroup();
				popFxGroup.gameObject.SetActive(true);
			}
			this.AliveGroups.Add(num, popFxGroup);
		}
		popFxGroup.Enqueue(orCreatePopFX);
		popFxGroup.WakeUp(num);
		return orCreatePopFX;
	}

	private PopFX GetOrCreatePopFX(Sprite mainIcon, Sprite secondaryIcon, string text, Transform target_transform, Vector3 offset, bool selfAdjustPositionIfInGroup = true, float lifetime = 1.5f, bool track_target = false)
	{
		PopFX popFX;
		if (this.Pool.Count > 0)
		{
			popFX = this.Pool[0];
			this.Pool[0].Setup(mainIcon, secondaryIcon, text, target_transform, offset, selfAdjustPositionIfInGroup, lifetime, track_target);
			this.Pool.RemoveAt(0);
		}
		else
		{
			popFX = this.CreatePopFX();
			popFX.Setup(mainIcon, secondaryIcon, text, target_transform, offset, selfAdjustPositionIfInGroup, lifetime, track_target);
		}
		return popFX;
	}

	private PopFX CreatePopFX()
	{
		bool activeInHierarchy = this.Prefab_PopFX.gameObject.activeInHierarchy;
		GameObject gameObject = Util.KInstantiate(this.Prefab_PopFX, base.gameObject, "Pooled_PopFX");
		gameObject.transform.localScale = Vector3.one;
		return gameObject.GetComponent<PopFX>();
	}

	private PopFxGroup CreatePopFxGroup()
	{
		GameObject gameObject = Util.KInstantiate(this.Prefab_PopFxGroup, base.gameObject, "Pooled_PopFxGroup");
		gameObject.transform.localScale = Vector3.one;
		return gameObject.GetComponent<PopFxGroup>();
	}

	public void RecycleFX(PopFX fx)
	{
		this.Pool.Add(fx);
	}

	public void RecycleFxGroup(int key, PopFxGroup fx)
	{
		this.AliveGroups.Remove(key);
		this.GroupPool.Add(fx);
	}

	public static PopFXManager Instance;

	private GameObject Prefab_PopFxGroup;

	public GameObject Prefab_PopFX;

	public List<PopFX> Pool = new List<PopFX>();

	public List<PopFxGroup> GroupPool = new List<PopFxGroup>();

	public Dictionary<int, PopFxGroup> AliveGroups = new Dictionary<int, PopFxGroup>();

	public Sprite sprite_Plus;

	public Sprite sprite_Negative;

	public Sprite sprite_Resource;

	public Sprite sprite_Building;

	public Sprite sprite_Research;

	private bool ready;
}
