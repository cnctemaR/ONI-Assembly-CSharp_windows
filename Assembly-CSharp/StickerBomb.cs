using System;
using System.Collections.Generic;
using KSerialization;
using TUNING;
using UnityEngine;

public class StickerBomb : StateMachineComponent<StickerBomb.StatesInstance>
{
	protected override void OnSpawn()
	{
		this.cellOffsets = StickerBomb.BuildCellOffsets(base.transform.GetPosition());
		base.smi.destroyTime = GameClock.Instance.GetTime() + TRAITS.JOY_REACTIONS.STICKER_BOMBER.STICKER_DURATION;
		base.smi.StartSM();
		Extents extents = base.GetComponent<OccupyArea>().GetExtents();
		Extents extents2 = new Extents(extents.x - 1, extents.y - 1, extents.width + 2, extents.height + 2);
		this.partitionerEntry = GameScenePartitioner.Instance.Add("StickerBomb.OnSpawn", base.gameObject, extents2, GameScenePartitioner.Instance.objectLayers[2], new Action<object>(this.OnFoundationCellChanged));
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		base.OnCleanUp();
	}

	private void OnFoundationCellChanged(object data)
	{
		if (!StickerBomb.CanPlaceSticker(this.cellOffsets))
		{
			Util.KDestroyGameObject(base.gameObject);
		}
	}

	public static List<int> BuildCellOffsets(Vector3 position)
	{
		List<int> list = new List<int>();
		bool flag = position.x % 1f < 0.5f;
		bool flag2 = position.y % 1f > 0.5f;
		int num = Grid.PosToCell(position);
		list.Add(num);
		if (flag)
		{
			list.Add(Grid.CellLeft(num));
			if (flag2)
			{
				list.Add(Grid.CellAbove(num));
				list.Add(Grid.CellUpLeft(num));
			}
			else
			{
				list.Add(Grid.CellBelow(num));
				list.Add(Grid.CellDownLeft(num));
			}
		}
		else
		{
			list.Add(Grid.CellRight(num));
			if (flag2)
			{
				list.Add(Grid.CellAbove(num));
				list.Add(Grid.CellUpRight(num));
			}
			else
			{
				list.Add(Grid.CellBelow(num));
				list.Add(Grid.CellDownRight(num));
			}
		}
		return list;
	}

	public static bool CanPlaceSticker(List<int> offsets)
	{
		using (List<int>.Enumerator enumerator = offsets.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (Grid.IsCellOpenToSpace(enumerator.Current))
				{
					return false;
				}
			}
		}
		return true;
	}

	public void SetStickerType(string newStickerType)
	{
		if (newStickerType == null)
		{
			newStickerType = "sticker";
		}
		this.stickerType = string.Format("{0}_{1}", newStickerType, TRAITS.JOY_REACTIONS.STICKER_BOMBER.STICKER_ANIMS.GetRandom<string>());
	}

	[Serialize]
	public string stickerType;

	private HandleVector<int>.Handle partitionerEntry;

	private List<int> cellOffsets;

	public class StatesInstance : GameStateMachine<StickerBomb.States, StickerBomb.StatesInstance, StickerBomb, object>.GameInstance
	{
		public StatesInstance(StickerBomb master)
			: base(master)
		{
		}

		public string GetStickerAnim(string type)
		{
			return string.Format("{0}_{1}", type, base.master.stickerType);
		}

		[Serialize]
		public float destroyTime;
	}

	public class States : GameStateMachine<StickerBomb.States, StickerBomb.StatesInstance, StickerBomb>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			base.serializable = true;
			this.root.Transition(this.destroy, (StickerBomb.StatesInstance smi) => GameClock.Instance.GetTime() >= smi.destroyTime, UpdateRate.SIM_200ms).DefaultState(this.idle);
			this.idle.PlayAnim((StickerBomb.StatesInstance smi) => smi.GetStickerAnim("idle"), KAnim.PlayMode.Once).ScheduleGoTo((StickerBomb.StatesInstance smi) => (float)global::UnityEngine.Random.Range(20, 30), this.sparkle);
			this.sparkle.PlayAnim((StickerBomb.StatesInstance smi) => smi.GetStickerAnim("sparkle"), KAnim.PlayMode.Once).OnAnimQueueComplete(this.idle);
			this.destroy.Enter(delegate(StickerBomb.StatesInstance smi)
			{
				Util.KDestroyGameObject(smi.master);
			});
		}

		public GameStateMachine<StickerBomb.States, StickerBomb.StatesInstance, StickerBomb, object>.State destroy;

		public GameStateMachine<StickerBomb.States, StickerBomb.StatesInstance, StickerBomb, object>.State sparkle;

		public GameStateMachine<StickerBomb.States, StickerBomb.StatesInstance, StickerBomb, object>.State idle;
	}
}
