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

	public static Vector2I SetInitialCamera()
	{
		Vector2I baseStartPos = SaveLoader.Instance.cachedGSD.baseStartPos;
		int num = Grid.OffsetCell(0, baseStartPos.x, baseStartPos.y);
		Vector3 vector = Grid.CellToPosCCC(Grid.OffsetCell(num, 0, -2), Grid.SceneLayer.Background);
		vector.z = CameraController.Instance.defaultDepth;
		CameraController.Instance.SetMaxOrthographicSize(40f);
		CameraController.Instance.SnapTo(vector);
		CameraController.Instance.SetTargetPos(vector, 20f, false);
		CameraController.Instance.SetOrthographicsSize(40f);
		CameraSaveData.valid = false;
		return baseStartPos;
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
		Game.Instance.ResetTime();
		this.Final();
	}

	public void SetStartingMinionStats(MinionStartingStats[] stats)
	{
		this.minionStartingStats = stats;
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
			for (int i = 0; i < array.Length; i++)
			{
				if (e.TryConsume(array[i]))
				{
					break;
				}
			}
		}
	}

	private void Final()
	{
		SpeedControlScreen.Instance.Unpause(false);
		Telepad telepad = global::UnityEngine.Object.FindObjectOfType<Telepad>();
		if (telepad)
		{
			this.SpawnMinions(Grid.PosToCell(telepad.gameObject));
		}
		Game.Instance.baseAlreadyCreated = true;
		Game.Instance.StartDelayedInitialSave();
		this.Deactivate();
	}

	private void SpawnMinions(int headquartersCell)
	{
		if (headquartersCell == -1)
		{
			global::Debug.LogWarning("No headquarters in saved base template. Cannot place minions. Confirm there is a headquarters saved to the base template, or consider creating a new one.", null);
			return;
		}
		int num;
		int num2;
		Grid.CellToXY(headquartersCell, out num, out num2);
		if (Grid.WidthInCells < 64)
		{
			return;
		}
		int baseLeft = WorldGen.BaseLeft;
		int baseRight = WorldGen.BaseRight;
		Effect a_new_hope = Db.Get().effects.Get("AnewHope");
		for (int i = 0; i < this.minionStartingStats.Length; i++)
		{
			int num3 = num + i % (baseRight - baseLeft) + 1;
			int num4 = num2;
			int num5 = Grid.XYToCell(num3, num4);
			GameObject gameObject = Util.KInstantiate(EntityPrefabs.Instance.MinionPrefab, SceneOrganizer.Instance.GetFolder(Folder.Minions), null);
			gameObject.transform.localPosition = Grid.CellToPosCBC(num5, Grid.SceneLayer.Move);
			this.minionStartingStats[i].Apply(gameObject);
			GameScheduler.Instance.Schedule("ANewHope", 3f + 0.5f * (float)i, delegate(object m)
			{
				((GameObject)m).GetComponent<Effects>().Add(a_new_hope, true);
			}, gameObject, null);
		}
	}

	public static NewBaseScreen Instance;

	[SerializeField]
	private CanvasGroup[] disabledUIElements;

	[EventRef]
	public string ScanSoundMigrated;

	[EventRef]
	public string BuildBaseSoundMigrated;

	private MinionStartingStats[] minionStartingStats;
}
