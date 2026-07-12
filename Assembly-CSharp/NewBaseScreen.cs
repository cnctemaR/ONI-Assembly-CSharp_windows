using System;
using FMODUnity;
using Klei.AI;
using ProcGenGame;
using UnityEngine;

public class NewBaseScreen : KScreen
{
	public override float GetSortKey()
	{
		return 1f;
	}

	protected override void OnPrefabInit()
	{
		NewBaseScreen.Instance = this;
		base.OnPrefabInit();
		TimeOfDay.Instance.SetScale(0f);
	}

	protected override void OnForcedCleanUp()
	{
		NewBaseScreen.Instance = null;
		base.OnForcedCleanUp();
	}

	public static Vector2I SetInitialCamera()
	{
		Vector2I vector2I = SaveLoader.Instance.cachedGSD.baseStartPos;
		vector2I += ClusterManager.Instance.GetStartWorld().WorldOffset;
		Vector3 vector = Grid.CellToPosCCC(Grid.OffsetCell(Grid.OffsetCell(0, vector2I.x, vector2I.y), 0, -2), Grid.SceneLayer.Background);
		CameraController.Instance.SetMaxOrthographicSize(40f);
		CameraController.Instance.SnapTo(vector);
		CameraController.Instance.SetTargetPos(vector, 20f, false);
		CameraController.Instance.OrthographicSize = 40f;
		CameraSaveData.valid = false;
		return vector2I;
	}

	protected override void OnActivate()
	{
		if (this.disabledUIElements != null)
		{
			foreach (CanvasGroup canvasGroup in this.disabledUIElements)
			{
				if (canvasGroup != null)
				{
					canvasGroup.interactable = false;
				}
			}
		}
		NewBaseScreen.SetInitialCamera();
		if (SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Unpause(false);
		}
		this.Final();
	}

	public void Init(Cluster clusterLayout, ITelepadDeliverable[] startingMinionStats)
	{
		this.m_clusterLayout = clusterLayout;
		this.m_minionStartingStats = startingMinionStats;
	}

	protected override void OnDeactivate()
	{
		Game.Instance.Trigger(-122303817, null);
		if (this.disabledUIElements != null)
		{
			foreach (CanvasGroup canvasGroup in this.disabledUIElements)
			{
				if (canvasGroup != null)
				{
					canvasGroup.interactable = true;
				}
			}
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		global::Action[] array = new global::Action[]
		{
			global::Action.SpeedUp,
			global::Action.SlowDown,
			global::Action.TogglePause,
			global::Action.CycleSpeed
		};
		if (!e.Consumed)
		{
			int num = 0;
			while (num < array.Length && !e.TryConsume(array[num]))
			{
				num++;
			}
		}
	}

	private void Final()
	{
		SpeedControlScreen.Instance.Unpause(false);
		GameObject telepad = GameUtil.GetTelepad(ClusterManager.Instance.GetStartWorld().id);
		if (telepad)
		{
			this.SpawnMinions(Grid.PosToCell(telepad));
		}
		Game.Instance.baseAlreadyCreated = true;
		this.Deactivate();
	}

	private void SpawnMinions(int headquartersCell)
	{
		if (headquartersCell == -1)
		{
			global::Debug.LogWarning("No headquarters in saved base template. Cannot place minions. Confirm there is a headquarters saved to the base template, or consider creating a new one.");
			return;
		}
		int num;
		int num2;
		Grid.CellToXY(headquartersCell, out num, out num2);
		if (Grid.WidthInCells < 64)
		{
			return;
		}
		int baseLeft = this.m_clusterLayout.currentWorld.BaseLeft;
		int baseRight = this.m_clusterLayout.currentWorld.BaseRight;
		Effect a_new_hope = Db.Get().effects.Get("AnewHope");
		Action<object> <>9__0;
		for (int i = 0; i < this.m_minionStartingStats.Length; i++)
		{
			int num3 = num + i % (baseRight - baseLeft) + 1;
			int num4 = num2;
			int num5 = Grid.XYToCell(num3, num4);
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID), null, null);
			Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
			gameObject.transform.SetLocalPosition(Grid.CellToPosCBC(num5, Grid.SceneLayer.Move));
			gameObject.SetActive(true);
			((MinionStartingStats)this.m_minionStartingStats[i]).Apply(gameObject);
			GameScheduler instance = GameScheduler.Instance;
			string text = "ANewHope";
			float num6 = 3f + 0.5f * (float)i;
			Action<object> action;
			if ((action = <>9__0) == null)
			{
				action = (<>9__0 = delegate(object m)
				{
					GameObject gameObject2 = m as GameObject;
					if (gameObject2 == null)
					{
						return;
					}
					gameObject2.GetComponent<Effects>().Add(a_new_hope, true);
				});
			}
			instance.Schedule(text, num6, action, gameObject, null);
		}
		ClusterManager.Instance.activeWorld.SetDupeVisited();
	}

	public static NewBaseScreen Instance;

	[SerializeField]
	private CanvasGroup[] disabledUIElements;

	public EventReference ScanSoundMigrated;

	public EventReference BuildBaseSoundMigrated;

	private ITelepadDeliverable[] m_minionStartingStats;

	private Cluster m_clusterLayout;
}
