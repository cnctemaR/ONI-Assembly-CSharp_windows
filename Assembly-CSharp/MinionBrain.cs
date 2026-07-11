using System;
using ProcGen;
using UnityEngine;

public class MinionBrain : Brain
{
	public bool IsCellClear(int cell)
	{
		GameObject gameObject = Grid.Objects[cell, 0];
		bool flag = gameObject != null && base.gameObject != gameObject && !gameObject.GetComponent<Navigator>().IsMoving();
		return (gameObject == null && !Grid.Reserved[cell]) || !flag;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Navigator.SetAbilities(new MinionPathFinderAbilities(this.Navigator));
		base.Subscribe(-1697596308, new Action<object>(this.AnimTrackStoredItem));
		base.Subscribe(-975551167, new Action<object>(this.OnUnstableGroundImpact));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Storage component = base.GetComponent<Storage>();
		foreach (GameObject gameObject in component.items)
		{
			this.AddAnimTracker(gameObject);
		}
		Game.Instance.Subscribe(-107300940, new Action<object>(this.OnResearchComplete));
	}

	private void AnimTrackStoredItem(object data)
	{
		Storage component = base.GetComponent<Storage>();
		GameObject gameObject = (GameObject)data;
		this.RemoveTracker(gameObject);
		if (component.items.Contains(gameObject))
		{
			this.AddAnimTracker(gameObject);
		}
	}

	private void AddAnimTracker(GameObject go)
	{
		KAnimControllerBase component = go.GetComponent<KAnimControllerBase>();
		if (component == null)
		{
			return;
		}
		if (component.AnimFiles != null && component.AnimFiles.Length > 0 && component.AnimFiles[0] != null && component.GetComponent<Pickupable>().trackOnPickup)
		{
			KBatchedAnimTracker kbatchedAnimTracker = go.AddComponent<KBatchedAnimTracker>();
			kbatchedAnimTracker.useTargetPoint = false;
			kbatchedAnimTracker.fadeOut = false;
			kbatchedAnimTracker.symbol = new HashedString("snapTo_chest");
			kbatchedAnimTracker.forceAlwaysVisible = true;
		}
	}

	private void RemoveTracker(GameObject go)
	{
		KBatchedAnimTracker component = go.GetComponent<KBatchedAnimTracker>();
		if (component != null)
		{
			global::UnityEngine.Object.Destroy(component);
		}
	}

	public override void UpdateBrain()
	{
		base.UpdateBrain();
		if (Game.Instance == null)
		{
			return;
		}
		if (!Game.Instance.savedInfo.discoveredSurface)
		{
			int num = Grid.PosToCell(base.gameObject);
			SubWorld.ZoneType subWorldZoneType = global::World.Instance.zoneRenderData.GetSubWorldZoneType(num);
			if (subWorldZoneType == SubWorld.ZoneType.Space)
			{
				Game.Instance.savedInfo.discoveredSurface = true;
				Vector3 position = base.gameObject.transform.GetPosition();
				DiscoveredSpaceMessage discoveredSpaceMessage = new DiscoveredSpaceMessage(position);
				Messenger.Instance.QueueMessage(discoveredSpaceMessage);
			}
		}
	}

	private void RegisterReactEmotePair(string kanim_file_name, float max_trigger_time)
	{
		if (base.gameObject == null)
		{
			return;
		}
		ReactionMonitor.Instance smi = base.gameObject.GetSMI<ReactionMonitor.Instance>();
		if (smi != null)
		{
			EmoteChore emoteChore = new EmoteChore(base.gameObject.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteIdle, kanim_file_name, new HashedString[] { "react" }, null);
			SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(base.gameObject, "GolfClap_React", Db.Get().ChoreTypes.Cough, kanim_file_name, max_trigger_time, 0f, float.PositiveInfinity);
			emoteChore.PairReactable(selfEmoteReactable);
			selfEmoteReactable.AddStep(new EmoteReactable.EmoteStep
			{
				anim = "react"
			});
			selfEmoteReactable.PairEmote(emoteChore);
			smi.AddOneshotReactable(selfEmoteReactable);
		}
	}

	private void OnResearchComplete(object data)
	{
		this.RegisterReactEmotePair("anim_react_research_complete_kanim", 3f);
	}

	private void OnUnstableGroundImpact(object data)
	{
		this.RegisterReactEmotePair("anim_react_shock_kanim", 1f);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Game.Instance.Unsubscribe(-107300940, new Action<object>(this.OnResearchComplete));
	}

	[MyCmpReq]
	public Navigator Navigator;

	[MyCmpGet]
	public OxygenBreather OxygenBreather;
}
