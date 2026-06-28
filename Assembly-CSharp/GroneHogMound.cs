using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class GroneHogMound : StateMachineComponent<GroneHogMound.StatesInstance>, ISaveLoadableJson
{
	protected override void OnPrefabInit()
	{
		this.RestoreHogsList();
		base.OnPrefabInit();
	}

	private void RestoreHogsList()
	{
		if (this.hogs == null)
		{
			this.hogs = new List<GameObject>();
		}
	}

	public void SpawnHog()
	{
		if (this.hogPrefabID == null)
		{
			return;
		}
		while (this.hogs.Count < this.maxHogs)
		{
			this.hogs.Add(null);
		}
		for (int i = 0; i < this.maxHogs; i++)
		{
			if (this.hogs[i] == null)
			{
				this.mHog = Scenario.SpawnPrefab(Grid.PosToCell(base.gameObject), 0, 0, this.hogPrefabID, Grid.SceneLayer.Use, Folder.Entities);
				this.mHog.SetActive(true);
				this.mHog.GetComponent<GroneHog>().SetMound(this);
				this.hogs[i] = this.mHog;
				return;
			}
		}
		for (int j = 0; j < this.maxHogs; j++)
		{
			if (this.hogs[j] != null && !this.hogs[j].activeSelf)
			{
				this.EnableHog(this.hogs[j]);
			}
		}
	}

	private void ReleaseAllHogs()
	{
		foreach (GameObject gameObject in this.hogs)
		{
			if (gameObject != null && !gameObject.activeSelf)
			{
				this.EnableHog(gameObject);
			}
		}
	}

	public void RestoreHog(GameObject restoredHog)
	{
		this.RestoreHogsList();
		for (int i = 0; i < this.hogs.Count; i++)
		{
			if (this.hogs[i] == null)
			{
				this.hogs[i] = restoredHog;
				return;
			}
		}
		this.hogs.Add(restoredHog);
	}

	private int activeHogs()
	{
		int num = 0;
		foreach (GameObject gameObject in this.hogs)
		{
			if (gameObject != null && gameObject.activeSelf)
			{
				num++;
			}
		}
		return num;
	}

	private void Cleanup()
	{
		Util.KDestroyGameObject(base.gameObject);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		Grid.Objects[Grid.PosToCell(base.gameObject), 3] = base.gameObject;
	}

	protected void EnableHog(GameObject hog)
	{
		hog.SetActive(true);
	}

	public bool HogEnter()
	{
		if (!this.collapsing)
		{
			base.smi.GoTo(base.smi.sm.harvestableStates.enter);
			return true;
		}
		return false;
	}

	private bool collapsing;

	public GameObject mHog;

	public string hogPrefabID = "GroneHog";

	private float RespawnTime = 5f;

	private int maxHogs = 3;

	private List<GameObject> hogs;

	public class StatesInstance : GameStateMachine<GroneHogMound.States, GroneHogMound.StatesInstance, GroneHogMound>.GameInstance
	{
		public StatesInstance(GroneHogMound smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<GroneHogMound.States, GroneHogMound.StatesInstance, GroneHogMound>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.harvestableStates.idle;
			this.harvestableStates.Update(delegate(GroneHogMound.StatesInstance smi)
			{
				if (!Grid.Solid[Grid.CellBelow(Grid.PosToCell(smi.transform.position))])
				{
					smi.GoTo(this.collapse);
				}
			}).EventTransition(GameHashes.Harvest, this.collapse, null);
			this.harvestableStates.idle.Enter(delegate(GroneHogMound.StatesInstance smi)
			{
				smi.Play("idle", KAnim.PlayMode.Once);
				smi.ScheduleGoTo(smi.master.RespawnTime, this.harvestableStates.respawn);
			}).EventTransition(GameHashes.Died, this.harvestableStates.idle_alt, null);
			this.harvestableStates.idle.GoTo(this.harvestableStates.idle);
			this.harvestableStates.enter.PlayAnim("enter", KAnim.PlayMode.Once, null).ScheduleGoTo(1f, this.harvestableStates.idle);
			this.harvestableStates.respawn.Enter(delegate(GroneHogMound.StatesInstance smi)
			{
				if (smi.master.activeHogs() < smi.master.maxHogs && !CreatureHelpers.CrewNearby(smi.transform, 7))
				{
					smi.Play("exit", KAnim.PlayMode.Once);
					smi.master.SpawnHog();
					smi.ScheduleGoTo(3f, this.harvestableStates.idle);
				}
				else
				{
					smi.GoTo(this.harvestableStates.idle);
				}
			});
			this.collapse.Enter(delegate(GroneHogMound.StatesInstance smi)
			{
				if (SelectTool.Instance.selected != null && SelectTool.Instance.selected.gameObject == smi.gameObject)
				{
					SelectTool.Instance.Select(null, false);
				}
				smi.master.collapsing = true;
				smi.Play("collapse", KAnim.PlayMode.Once);
				smi.master.ReleaseAllHogs();
				smi.Schedule(3f, delegate(object d)
				{
					smi.master.Cleanup();
				}, null);
			});
		}

		public GroneHogMound.States.HarvestableStates harvestableStates;

		public GameStateMachine<GroneHogMound.States, GroneHogMound.StatesInstance, GroneHogMound>.State collapse;

		public class HarvestableStates : GameStateMachine<GroneHogMound.States, GroneHogMound.StatesInstance, GroneHogMound>.State
		{
			public GameStateMachine<GroneHogMound.States, GroneHogMound.StatesInstance, GroneHogMound>.State idle;

			public GameStateMachine<GroneHogMound.States, GroneHogMound.StatesInstance, GroneHogMound>.State enter;

			public GameStateMachine<GroneHogMound.States, GroneHogMound.StatesInstance, GroneHogMound>.State idle_alt;

			public GameStateMachine<GroneHogMound.States, GroneHogMound.StatesInstance, GroneHogMound>.State respawn;
		}
	}
}
