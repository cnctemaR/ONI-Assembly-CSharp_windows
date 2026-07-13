using System;
using KSerialization;
using TUNING;
using UnityEngine;

public class StickerBomber : GameStateMachine<StickerBomber, StickerBomber.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.neutral;
		this.root.TagTransition(GameTags.Dead, null, false);
		this.neutral.TagTransition(GameTags.Overjoyed, this.overjoyed, false).Exit(delegate(StickerBomber.Instance smi)
		{
			smi.nextStickerBomb = GameClock.Instance.GetTime() + TRAITS.JOY_REACTIONS.STICKER_BOMBER.TIME_PER_STICKER_BOMB;
		});
		this.overjoyed.TagTransition(GameTags.Overjoyed, this.neutral, true).DefaultState(this.overjoyed.idle).ToggleStatusItem(Db.Get().DuplicantStatusItems.JoyResponse_StickerBombing, null);
		this.overjoyed.idle.Transition(this.overjoyed.place_stickers, (StickerBomber.Instance smi) => GameClock.Instance.GetTime() >= smi.nextStickerBomb, UpdateRate.SIM_200ms);
		this.overjoyed.place_stickers.Exit(delegate(StickerBomber.Instance smi)
		{
			smi.nextStickerBomb = GameClock.Instance.GetTime() + TRAITS.JOY_REACTIONS.STICKER_BOMBER.TIME_PER_STICKER_BOMB;
		}).ToggleReactable((StickerBomber.Instance smi) => smi.CreateReactable()).OnSignal(this.doneStickerBomb, this.overjoyed.idle);
	}

	public StateMachine<StickerBomber, StickerBomber.Instance, IStateMachineTarget, object>.Signal doneStickerBomb;

	public GameStateMachine<StickerBomber, StickerBomber.Instance, IStateMachineTarget, object>.State neutral;

	public StickerBomber.OverjoyedStates overjoyed;

	public class OverjoyedStates : GameStateMachine<StickerBomber, StickerBomber.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<StickerBomber, StickerBomber.Instance, IStateMachineTarget, object>.State idle;

		public GameStateMachine<StickerBomber, StickerBomber.Instance, IStateMachineTarget, object>.State place_stickers;
	}

	public new class Instance : GameStateMachine<StickerBomber, StickerBomber.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public Reactable CreateReactable()
		{
			return new StickerBomber.Instance.StickerBombReactable(base.master.gameObject, base.smi);
		}

		[Serialize]
		public float nextStickerBomb;

		private class StickerBombReactable : Reactable
		{
			public StickerBombReactable(GameObject gameObject, StickerBomber.Instance stickerBomber)
				: base(gameObject, "StickerBombReactable", Db.Get().ChoreTypes.Build, 2, 1, false, 0f, 0f, float.PositiveInfinity, 0f, ObjectLayer.NumLayers)
			{
				this.preventChoreInterruption = true;
				this.stickerBomber = stickerBomber;
			}

			public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
			{
				if (this.reactor != null)
				{
					return false;
				}
				if (new_reactor == null)
				{
					return false;
				}
				if (this.gameObject != new_reactor)
				{
					return false;
				}
				Navigator component = new_reactor.GetComponent<Navigator>();
				return !(component == null) && component.CurrentNavType != NavType.Tube && component.CurrentNavType != NavType.Ladder && component.CurrentNavType != NavType.Pole;
			}

			protected override void InternalBegin()
			{
				this.stickersToPlace = global::UnityEngine.Random.Range(4, 6);
				this.STICKER_PLACE_TIMER = this.TIME_PER_STICKER_PLACED;
				this.placementCell = this.FindPlacementCell();
				if (this.placementCell == 0)
				{
					base.End();
					return;
				}
				this.kbac = this.reactor.GetComponent<KBatchedAnimController>();
				this.kbac.AddAnimOverrides(this.animset, 0f);
				this.kbac.Play(this.pre_anim, KAnim.PlayMode.Once, 1f, 0f);
				this.kbac.Queue(this.loop_anim, KAnim.PlayMode.Loop, 1f, 0f);
			}

			public override void Update(float dt)
			{
				this.STICKER_PLACE_TIMER -= dt;
				if (this.STICKER_PLACE_TIMER <= 0f)
				{
					this.PlaceSticker();
					this.STICKER_PLACE_TIMER = this.TIME_PER_STICKER_PLACED;
				}
				if (this.stickersPlaced >= this.stickersToPlace)
				{
					this.kbac.Play(this.pst_anim, KAnim.PlayMode.Once, 1f, 0f);
					base.End();
				}
			}

			protected override void InternalEnd()
			{
				if (this.kbac != null)
				{
					this.kbac.RemoveAnimOverrides(this.animset);
					this.kbac = null;
				}
				this.stickerBomber.sm.doneStickerBomb.Trigger(this.stickerBomber);
				this.stickersPlaced = 0;
			}

			private int FindPlacementCell()
			{
				int num = Grid.PosToCell(this.reactor.transform.GetPosition() + Vector3.up);
				HashSetPool<int, PathFinder>.PooledHashSet pooledHashSet = HashSetPool<int, PathFinder>.Allocate();
				ListPool<int, PathFinder>.PooledList pooledList = ListPool<int, PathFinder>.Allocate();
				QueuePool<GameUtil.FloodFillInfo, Comet>.PooledQueue pooledQueue = QueuePool<GameUtil.FloodFillInfo, Comet>.Allocate();
				pooledQueue.Enqueue(new GameUtil.FloodFillInfo
				{
					cell = num,
					depth = 0
				});
				GameUtil.FloodFillConditional(pooledQueue, this.canPlaceStickerCb, pooledHashSet, pooledList, 2);
				pooledHashSet.Recycle();
				pooledQueue.Recycle();
				int num2 = ((pooledList.Count > 0) ? pooledList.GetRandom<int>() : 0);
				pooledList.Recycle();
				return num2;
			}

			private void PlaceSticker()
			{
				this.stickersPlaced++;
				Vector3 vector = Grid.CellToPos(this.placementCell);
				int i = 10;
				while (i > 0)
				{
					i--;
					Vector3 vector2 = vector + new Vector3(global::UnityEngine.Random.Range(-this.tile_random_range, this.tile_random_range), global::UnityEngine.Random.Range(-this.tile_random_range, this.tile_random_range), -2.5f);
					if (StickerBomb.CanPlaceSticker(StickerBomb.BuildCellOffsets(vector2)))
					{
						GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("StickerBomb".ToTag()), vector2, Quaternion.Euler(0f, 0f, global::UnityEngine.Random.Range(-this.tile_random_rotation, this.tile_random_rotation)), null, null, true, 0);
						StickerBomb component = gameObject.GetComponent<StickerBomb>();
						string stickerType = this.reactor.GetComponent<MinionIdentity>().stickerType;
						component.SetStickerType(stickerType);
						gameObject.SetActive(true);
						i = 0;
					}
				}
			}

			protected override void InternalCleanup()
			{
			}

			private int stickersToPlace;

			private int stickersPlaced;

			private int placementCell;

			private float tile_random_range = 1f;

			private float tile_random_rotation = 90f;

			private float TIME_PER_STICKER_PLACED = 0.66f;

			private float STICKER_PLACE_TIMER;

			private KBatchedAnimController kbac;

			private KAnimFile animset = Assets.GetAnim("anim_stickers_kanim");

			private HashedString pre_anim = "working_pre";

			private HashedString loop_anim = "working_loop";

			private HashedString pst_anim = "working_pst";

			private StickerBomber.Instance stickerBomber;

			private Func<int, bool> canPlaceStickerCb = (int cell) => !Grid.Solid[cell] && (!Grid.IsValidCell(Grid.CellLeft(cell)) || !Grid.Solid[Grid.CellLeft(cell)]) && (!Grid.IsValidCell(Grid.CellRight(cell)) || !Grid.Solid[Grid.CellRight(cell)]) && (!Grid.IsValidCell(Grid.OffsetCell(cell, 0, 1)) || !Grid.Solid[Grid.OffsetCell(cell, 0, 1)]) && (!Grid.IsValidCell(Grid.OffsetCell(cell, 0, -1)) || !Grid.Solid[Grid.OffsetCell(cell, 0, -1)]) && !Grid.IsCellOpenToSpace(cell);
		}
	}
}
