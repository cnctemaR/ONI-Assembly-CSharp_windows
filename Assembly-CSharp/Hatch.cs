using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class Hatch : StateMachineComponent<Hatch.StatesInstance>
{
	private float hungerEatThreshold
	{
		get
		{
			return 600f;
		}
	}

	private float maxHunger
	{
		get
		{
			return 900f;
		}
	}

	private float unitsPerFeeding
	{
		get
		{
			return 50f;
		}
	}

	private float foodUnitsPerFeeding
	{
		get
		{
			return 0.5f;
		}
	}

	private float minPoopSize
	{
		get
		{
			return 100f;
		}
	}

	private float maxPoopSize
	{
		get
		{
			return 125f;
		}
	}

	private float DigHardnessLimit
	{
		get
		{
			return 20f;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		base.Subscribe(2127324410, new Action<object>(this.OnRefreshUserMenu));
		base.Subscribe(229718515, new Action<object>(this.OnThreatned));
		base.Subscribe(-21431934, new Action<object>(this.ClearThreat));
	}

	private void FindAndMoveToFood()
	{
		if (this.isHungry(base.smi))
		{
			Pickupable eatTarget = base.smi.master.GetEatTarget();
			if (eatTarget != null)
			{
				base.smi.sm.eatMoveTarget.Set(eatTarget.gameObject, base.smi);
				base.smi.GoTo(base.smi.sm.alive.grounded.eatStates.moveToFood);
			}
		}
	}

	public void Poop()
	{
		float num = Mathf.Min(base.smi.sm.consumedMass.Get(base.smi), this.maxPoopSize);
		base.smi.master.emitter.ForceEmit(num, byte.MaxValue, 0, base.GetComponent<PrimaryElement>().Temperature);
		base.smi.sm.consumedMass.Delta(-num, base.smi);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.ForgetDigPlacer();
		if (this.digPlacerChangedMonitor != null)
		{
			this.digPlacerChangedMonitor.Release();
		}
		if (this.solidCellMonitor != null)
		{
			this.solidCellMonitor.Release();
		}
	}

	private Pickupable GetEatTarget()
	{
		Navigator component = base.GetComponent<Navigator>();
		int num = int.MaxValue;
		Pickupable pickupable = null;
		int num2 = 0;
		int num3 = 0;
		Grid.CellToXY(Grid.PosToCell(base.gameObject.transform.position), out num2, out num3);
		int num4 = 8;
		List<ScenePartitionerEntry> list = GameScenePartitioner.Instance.ReserveList();
		GameScenePartitioner.Instance.GatherEntries(num2 - num4, num3 - num4, num4 * 2, num4 * 2, GameScenePartitioner.Instance.pickupablesLayer, list);
		for (int i = 0; i < list.Count; i++)
		{
			ScenePartitionerEntry scenePartitionerEntry = list[i];
			Pickupable pickupable2 = scenePartitionerEntry.obj as Pickupable;
			if (!(pickupable2 == null))
			{
				if (!(null == pickupable2.GetComponent<ElementChunk>()) || !(null == pickupable2.GetComponent<Edible>()))
				{
					if (!(pickupable2.GetComponent<MinionIdentity>() != null))
					{
						int num5 = Grid.PosToCell(pickupable2);
						int navigationCost = component.GetNavigationCost(num5);
						if (navigationCost != PathProber.InvalidCost)
						{
							if (navigationCost < num)
							{
								if (pickupable2.GetComponent<PrimaryElement>().ElementID != this.emitter.outputElement.elementHash)
								{
									num = navigationCost;
									pickupable = pickupable2;
								}
							}
						}
					}
				}
			}
		}
		GameScenePartitioner.Instance.ReleaseList(list);
		return pickupable;
	}

	public bool isHungry(Hatch.StatesInstance smi)
	{
		return smi.sm.hungerLevel.Get(smi) > this.hungerEatThreshold && smi.sm.timeSinceLastMeal.Get(smi) > this.minimumTimeBetweenMeals;
	}

	private void OnThreatned(object threat)
	{
		this.mainThreat = (GameObject)threat;
	}

	private void ClearThreat(object data)
	{
		this.mainThreat = null;
	}

	private bool EmergeIsClear()
	{
		int num = Grid.PosToCell(base.gameObject);
		bool flag;
		if (!Grid.IsValidCell(num) || !Grid.IsValidCell(Grid.CellAbove(num)))
		{
			flag = false;
		}
		else
		{
			int num2 = Grid.CellAbove(num);
			flag = !Grid.Solid[num2] && !Grid.IsSubstantialLiquid(Grid.CellAbove(num), 0.9f);
		}
		return flag;
	}

	private bool ShouldBurrow()
	{
		return GameClock.Instance.GetCurrentDayAsPercentage() < 0.875f && this.CanBurrowInto(Grid.CellBelow(Grid.PosToCell(base.gameObject)));
	}

	private bool CanBurrowInto(int cell)
	{
		return Grid.IsValidCell(cell) && Grid.Solid[cell] && !Grid.IsSubstantialLiquid(Grid.CellAbove(cell), 0.35f) && !(Grid.Objects[cell, 1] != null) && (float)Grid.Element[cell].hardness <= this.DigHardnessLimit && !Grid.Foundation[cell];
	}

	private GameObject EdibleOnCell(int cell)
	{
		GameObject gameObject;
		if (base.smi.sm.eatMoveTarget.Get(base.smi) != null && Grid.PosToCell(base.smi.sm.eatMoveTarget.Get(base.smi)) == cell)
		{
			gameObject = base.smi.sm.eatMoveTarget.Get(base.smi);
		}
		else
		{
			GameObject gameObject2 = Grid.Objects[cell, 3];
			if (gameObject2 != null && gameObject2.GetComponent<Pickupable>().storage == null)
			{
				if (gameObject2.HasTag(GameTags.Ore) || gameObject2.HasTag(GameTags.Edible) || gameObject2.HasTag(GameTags.BuildableRaw) || gameObject2.HasTag(GameTags.Solid))
				{
					PrimaryElement component = gameObject2.GetComponent<PrimaryElement>();
					if (component != null)
					{
						if (component.ElementID != this.emitter.outputElement.elementHash)
						{
							return gameObject2;
						}
					}
				}
			}
			gameObject = null;
		}
		return gameObject;
	}

	private void RemoveMassFromEdible(GameObject edibleObject)
	{
		Edible component = edibleObject.GetComponent<Edible>();
		this.latestMealName = edibleObject.GetProperName();
		PrimaryElement component2 = edibleObject.GetComponent<PrimaryElement>();
		this.latestMealElement = ElementLoader.FindElementByHash(component2.ElementID);
		if (component)
		{
			float num = Mathf.Min(this.foodUnitsPerFeeding, component.Units);
			float num2 = num * (this.unitsPerFeeding / this.foodUnitsPerFeeding);
			base.smi.sm.hungerLevel.Set(Mathf.Max(0f, base.smi.sm.hungerLevel.Get(base.smi) - this.hungerSatiationScale * num2), base.smi);
			component.Units -= num;
			base.smi.sm.consumedMass.Delta(num * component2.MassPerUnit, base.smi);
			if (component.Units <= 0f)
			{
				Util.KDestroyGameObject(edibleObject);
			}
		}
		else if (component2)
		{
			base.smi.sm.hungerLevel.Set(Mathf.Max(0f, base.smi.sm.hungerLevel.Get(base.smi) - this.hungerSatiationScale * this.RemoveUnits(component2, this.unitsPerFeeding)), base.smi);
			if (component2.Units <= 0f)
			{
				Util.KDestroyGameObject(edibleObject);
			}
		}
	}

	public float RemoveUnits(PrimaryElement primary_element, float other_mass)
	{
		float num = Mathf.Min(this.unitsPerFeeding, primary_element.Units);
		primary_element.Units -= num;
		base.smi.sm.consumedMass.Delta(primary_element.MassPerUnit * num, base.smi);
		return num;
	}

	private void OnRefreshUserMenu(object data)
	{
		if (base.smi.IsInsideState(base.smi.sm.alive.hide.loop))
		{
			if (base.smi.sm.DigPlacer.Get(base.smi) == null)
			{
				UserMenu userMenu = this.userMenu;
				string text = "action_uproot";
				string text2 = UI.USERMENUACTIONS.DIG.NAME;
				global::System.Action action = new global::System.Action(this.OnPressDig);
				string text3 = UI.USERMENUACTIONS.DIG.TOOLTIP;
				userMenu.AddButton(new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true), 1f);
			}
			else
			{
				UserMenu userMenu2 = this.userMenu;
				string text3 = "action_uproot";
				string text2 = UI.USERMENUACTIONS.CANCELDIG.NAME;
				global::System.Action action = new global::System.Action(this.OnPressCancelDig);
				string text = UI.USERMENUACTIONS.DIG.TOOLTIP_OFF;
				userMenu2.AddButton(new KIconButtonMenu.ButtonInfo(text3, text2, action, global::Action.NumActions, null, null, null, text, true), 1f);
			}
		}
	}

	private GameObject FindExistingDigPlacer()
	{
		GameObject gameObject = Grid.Objects[Grid.PosToCell(base.gameObject), 7];
		GameObject gameObject2;
		if (gameObject == null)
		{
			gameObject2 = null;
		}
		else if (this.prevDigPlacer == gameObject)
		{
			gameObject2 = null;
		}
		else
		{
			gameObject2 = gameObject;
		}
		return gameObject2;
	}

	private void OnPressDig()
	{
		GameObject gameObject = this.FindExistingDigPlacer();
		if (gameObject != null)
		{
			global::Debug.LogWarning("User menu trying to create dig placer on hatch while it already has one. This should not be possible", null);
		}
		else
		{
			DigTool.PlaceDig(Grid.PosToCell(base.gameObject), 0);
		}
	}

	private void OnPressCancelDig()
	{
		if (base.smi.sm.DigPlacer.Get(base.smi) != null)
		{
			base.smi.sm.DigPlacer.Get(base.smi).Trigger(2127324410, null);
		}
		this.ForgetDigPlacer();
	}

	private void OnDuplicantDigBurrow(object dupeObject)
	{
		if (!(base.gameObject == null))
		{
			FactionAlignment component = ((GameObject)dupeObject).GetComponent<FactionAlignment>();
			ThreatMonitor.Instance smi = base.gameObject.GetSMI<ThreatMonitor.Instance>();
			if (smi != null)
			{
				smi.OnOffended(component);
			}
			if (base.smi.sm.DigPlacer.Get(base.smi) != null)
			{
				base.smi.master.ForgetDigPlacer();
			}
		}
	}

	private void ForgetDigPlacer()
	{
		if (this.currentDigPlacer != null)
		{
			this.currentDigPlacer.Unsubscribe(963113026, new Action<object>(this.OnDuplicantDigBurrow));
		}
		this.prevDigPlacer = base.smi.sm.DigPlacer.Get(base.smi);
		if (base.smi.sm.DigPlacer.Get(base.smi) != null)
		{
			base.smi.sm.DigPlacer.Get(base.smi).Unsubscribe(963113026, new Action<object>(this.OnDuplicantDigBurrow));
		}
		base.smi.sm.DigPlacer.Set(null, base.smi);
	}

	private void SetDigPlacer(GameObject newDigPlacer)
	{
		this.ForgetDigPlacer();
		base.smi.sm.DigPlacer.Set(newDigPlacer, base.smi);
		newDigPlacer.Subscribe(963113026, new Action<object>(this.OnDuplicantDigBurrow));
		this.currentDigPlacer = newDigPlacer;
	}

	private void ListenForDigPlacerChanged()
	{
		this.StopListeningForDigPlacerChanged();
		Vector2 vector = Grid.PosToXY(base.smi.master.transform.position);
		base.smi.master.digPlacerChangedMonitor = GameScenePartitioner.Instance.Add("DigPlacerChanged", base.smi.master.gameObject, new Extents((int)vector.x, (int)vector.y, 1, 1), GameScenePartitioner.Instance.objectLayers[7], new Action<object>(base.smi.master.DigPlacerChanged));
	}

	private void StopListeningForDigPlacerChanged()
	{
		if (base.smi.master.digPlacerChangedMonitor != null)
		{
			base.smi.master.digPlacerChangedMonitor.Release();
		}
	}

	private void DigPlacerChanged(object data)
	{
		GameObject gameObject = this.FindExistingDigPlacer();
		if (base.smi.sm.DigPlacer.Get(base.smi) == null)
		{
			if (gameObject != null)
			{
				this.SetDigPlacer(gameObject);
			}
		}
		else if (data == null)
		{
			if (base.smi.sm.DigPlacer.Get(base.smi) != null)
			{
				this.ForgetDigPlacer();
			}
		}
		this.userMenu.Refresh();
	}

	private void InheritExistingDigPlacer()
	{
		GameObject gameObject = base.smi.master.FindExistingDigPlacer();
		if (gameObject != null)
		{
			base.smi.master.SetDigPlacer(gameObject);
		}
	}

	[MyCmpAdd]
	private KBatchedAnimController anim;

	[MyCmpAdd]
	private ElementEmitter emitter;

	[MyCmpAdd]
	private SimpleMover mover;

	[MyCmpAdd]
	private Navigator nav;

	[MyCmpGet]
	private UserMenu userMenu;

	[MyCmpAdd]
	private BoxCollider2D mCollider;

	[MyCmpAdd]
	private Weapon weapon;

	private float hungerSatiationScale = 1f;

	private float eatTime = 5f;

	[SerializeField]
	private GameObject mainThreat;

	public Element latestMealElement;

	public string latestMealName = "";

	public float minimumTimeBetweenMeals = 30f;

	public static float minimumAwakeTime = 24f;

	public bool alive = true;

	private GameObject prevDigPlacer;

	private GameObject currentDigPlacer;

	public GameScenePartitionerEntry solidCellMonitor;

	public GameScenePartitionerEntry digPlacerChangedMonitor;

	public class StatesInstance : GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.GameInstance
	{
		public StatesInstance(Hatch smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.newGame;
			base.serializable = true;
			this.awakeTime.defaultValue = Hatch.minimumAwakeTime + 1f;
			this.newGame.Enter(delegate(Hatch.StatesInstance smi)
			{
				bool flag = false;
				int num = Grid.PosToCell(smi.master.gameObject);
				if (Grid.Solid[num])
				{
					int num2 = 0;
					while (!flag && num2 < 20)
					{
						num = Grid.CellBelow(num);
						if (smi.master.CanBurrowInto(num))
						{
							flag = true;
							smi.master.mover.TeleportToTarget(Grid.CellToPos(num, 0.5f, 0f, 0f), 0f);
							smi.GoTo(this.alive.hide.loop);
						}
						num2++;
					}
				}
				else
				{
					flag = true;
					smi.master.mover.TeleportToTarget(Grid.CellToPos(num, 0.5f, 0f, 0f), 0f);
					smi.GoTo(this.alive.grounded.idle);
				}
				if (!flag)
				{
					Util.KDestroyGameObject(smi.master.gameObject);
				}
			});
			this.alive.TagTransition(GameTags.Dead, this.death, false).EventTransition(GameHashes.Died, this.death, null).EventTransition(GameHashes.TooColdFatal, this.death, null)
				.EventTransition(GameHashes.TooHotFatal, this.death, null)
				.EventTransition(GameHashes.Drowned, this.death, null)
				.ToggleStateMachine((Hatch.StatesInstance smi) => new ThreatMonitor.Instance(smi.master))
				.Enter(delegate(Hatch.StatesInstance smi)
				{
					smi.master.GetSMI<ThreatMonitor.Instance>().sm.FleeThresholdState = Health.HealthState.Dead;
					this.mover.Set(smi.master, smi);
				})
				.Update(delegate(Hatch.StatesInstance smi)
				{
					this.hungerLevel.Set(Mathf.Min(smi.master.maxHunger, this.hungerLevel.Get(smi) + smi.deltatime), smi);
					this.timeSinceLastMeal.Delta(smi.deltatime, smi);
				});
			this.alive.grounded.DefaultState(this.alive.grounded.idle).EventTransition(GameHashes.Drowning, this.alive.grounded.distressed.Drowning, null).TagTransition(GameTags.Trapped, this.alive.grounded.caged, false)
				.Enter(delegate(Hatch.StatesInstance smi)
				{
					int num3 = Grid.PosToCell(smi.transform.position + Vector3.down);
					if (Grid.IsValidCell(num3) && !Grid.Solid[num3])
					{
						smi.GoTo(this.alive.fall);
					}
				})
				.Update(delegate(Hatch.StatesInstance smi)
				{
					this.awakeTime.Set(this.awakeTime.Get(smi) + smi.deltatime, smi);
					int num4 = Grid.PosToCell(smi.transform.position + Vector3.down);
					if (Grid.IsValidCell(num4) && !Grid.Solid[num4])
					{
						smi.GoTo(this.alive.fall);
					}
				});
			this.alive.grounded.caged.DefaultState(this.alive.grounded.caged.idle).TagTransition(GameTags.Trapped, this.alive.grounded, true);
			this.alive.grounded.caged.idle.PlayAnim("idle_loop", KAnim.PlayMode.Loop).ScheduleGoTo(3f, this.alive.grounded.caged.funny_idle);
			this.alive.grounded.caged.funny_idle.PlayAnim("harvest", KAnim.PlayMode.Once).OnAnimQueueComplete(this.alive.grounded.caged.idle);
			this.alive.grounded.idle.DefaultState(this.alive.grounded.idle.idle).ToggleMainStatusItem(Db.Get().CreatureStatusItems.Idle).ToggleSchedulePeriodic("HatchLooksForFood", 1f, delegate(Hatch.StatesInstance smi)
			{
				smi.master.FindAndMoveToFood();
			})
				.EventTransition(GameHashes.Threatned, this.alive.grounded.attackStates, null)
				.Update(delegate(Hatch.StatesInstance smi)
				{
					if (this.awakeTime.Get(smi) > Hatch.minimumAwakeTime && smi.master.ShouldBurrow())
					{
						smi.GoTo(this.alive.hide);
					}
				});
			this.alive.grounded.idle.idle.PlayAnim("idle_loop", KAnim.PlayMode.Loop).ScheduleGoTo(3f, this.alive.grounded.idle.move).Enter(delegate(Hatch.StatesInstance smi)
			{
				if (smi.master.isHungry(smi) && smi.master.EdibleOnCell(Grid.PosToCell(smi.transform.position)) != null)
				{
					smi.GoTo(this.alive.grounded.eatStates);
				}
				else if (smi.sm.consumedMass.Get(smi) >= smi.master.minPoopSize)
				{
					smi.GoTo(this.alive.grounded.idle.poop);
				}
				if (smi.master.isHungry(smi) && this.eatMoveTarget.Get(smi) != null && Grid.PosToCell(smi.master.gameObject) == Grid.PosToCell(this.eatMoveTarget.Get(smi).gameObject))
				{
					smi.GoTo(this.alive.grounded.eatStates);
				}
			});
			this.alive.grounded.idle.poop.PlayAnim("harvest").OnAnimQueueComplete(this.alive.grounded.idle).Enter(delegate(Hatch.StatesInstance smi)
			{
				smi.master.Poop();
			});
			this.alive.grounded.idle.move.InitializeStates(this.alive.grounded.idle);
			this.alive.grounded.distressed.DrowningEmerge.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Emerging).Enter(delegate(Hatch.StatesInstance smi)
			{
				if (smi.sm.DigPlacer.Get(smi) != null)
				{
					smi.master.ForgetDigPlacer();
				}
				smi.master.mover.TeleportToTarget(Grid.CellToPos(Grid.CellAbove(Grid.PosToCell(smi.master.transform.position)), 0.5f, 0f, 0f), 0f);
			}).QueueAnim("emerge", false, null)
				.OnAnimQueueComplete(this.alive.grounded.distressed.Drowning)
				.Exit(delegate(Hatch.StatesInstance smi)
				{
					smi.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().CreatureStatusItems.Emerging, false);
				});
			this.alive.grounded.distressed.Drowning.PlayAnim("harvest", KAnim.PlayMode.Loop).Enter(delegate(Hatch.StatesInstance smi)
			{
				smi.master.nav.Stop(false);
				DrowningMonitor component = smi.master.GetComponent<DrowningMonitor>();
				if (component != null && !component.Drowning)
				{
					smi.GoTo(this.alive.grounded.idle.move);
				}
			}).EventTransition(GameHashes.EnteredBreathableArea, this.alive.grounded.idle.move, null);
			this.alive.grounded.eatStates.DefaultState(this.alive.grounded.eatStates.eat_pre).EventTransition(GameHashes.Threatned, this.alive.grounded.attackStates, null);
			this.alive.grounded.eatStates.moveToFood.InitializeStates(this.mover, this.eatMoveTarget, this.alive.grounded.idle, this.alive.grounded.idle, Grid.DefaultOffset, null);
			this.alive.grounded.eatStates.eat_pre.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Eating).Enter(delegate(Hatch.StatesInstance smi)
			{
				smi.Play("eat_pre", KAnim.PlayMode.Once);
				smi.Queue("eat_loop", KAnim.PlayMode.Loop);
				smi.Schedule(0.5f, delegate(object d)
				{
					GameObject gameObject = smi.master.EdibleOnCell(Grid.PosToCell(smi.master));
					if (gameObject == null)
					{
						if (this.eatMoveTarget.Get(smi) != null && Grid.PosToCell(smi.master.gameObject) == Grid.PosToCell(this.eatMoveTarget.Get(smi).gameObject))
						{
							gameObject = this.eatMoveTarget.Get(smi);
						}
					}
					if (gameObject != null)
					{
						PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, MISC.POPFX.RESOURCE_EATEN, smi.transform, 1.5f, false);
						smi.master.RemoveMassFromEdible(gameObject);
					}
					smi.GoTo(this.alive.grounded.eatStates.eat);
				}, null);
			});
			this.alive.grounded.eatStates.eat.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Eating).Enter(delegate(Hatch.StatesInstance smi)
			{
				this.timeSinceLastMeal.Set(0f, smi);
				smi.ScheduleGoTo(smi.master.eatTime, this.alive.grounded.eatStates.eat_pst);
			});
			this.alive.grounded.eatStates.eat_pst.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Eating).Enter(delegate(Hatch.StatesInstance smi)
			{
				smi.Queue("eat_pst", KAnim.PlayMode.Once);
			}).EventTransition(GameHashes.AnimQueueComplete, this.alive.grounded.idle, (Hatch.StatesInstance smi) => smi.timeinstate > 0f);
			this.alive.grounded.attackStates.DefaultState(this.alive.grounded.attackStates.plan_attack).EventTransition(GameHashes.SafeFromThreats, this.alive.grounded.idle, null);
			this.alive.grounded.attackStates.plan_attack.Enter(delegate(Hatch.StatesInstance smi)
			{
				if (!CreatureHelpers.WillEngageNonEssentialTargets(smi.gameObject, Health.HealthState.Dead))
				{
					smi.GoTo(this.alive.grounded.attackStates.flee);
				}
				else
				{
					this.threatMoveTarget.Set(smi.master.mainThreat, smi);
					smi.GoTo(this.alive.grounded.attackStates.approachtarget);
				}
			});
			this.alive.grounded.attackStates.approachtarget.InitializeStates(this.mover, this.threatMoveTarget, this.alive.grounded.attackStates.regular, this.alive.grounded.attackStates.flee, new CellOffset[]
			{
				new CellOffset(0, 0),
				new CellOffset(1, 0),
				new CellOffset(-1, 0),
				new CellOffset(1, 1),
				new CellOffset(-1, 1)
			}, null);
			this.alive.grounded.attackStates.regular.OnAnimQueueComplete(this.alive.grounded.attackStates.plan_attack).Enter(delegate(Hatch.StatesInstance smi)
			{
				smi.Play("eat_pre", KAnim.PlayMode.Once);
				smi.Queue("eat_pst", KAnim.PlayMode.Once);
				smi.Schedule(0.5f, delegate
				{
					smi.master.weapon.AttackTarget(smi.master.mainThreat);
				}, null);
			});
			this.alive.grounded.attackStates.flee.InitializeStates(this.mover, this.alive.grounded.idle);
			this.alive.grounded.emerge.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Emerging).Enter(delegate(Hatch.StatesInstance smi)
			{
				if (smi.sm.DigPlacer.Get(smi) != null)
				{
					smi.master.ForgetDigPlacer();
				}
				smi.master.mover.TeleportToTarget(Grid.CellToPos(Grid.CellAbove(Grid.PosToCell(smi.master.transform.position)), 0.5f, 0f, 0f), 0f);
			}).QueueAnim("emerge", false, null)
				.OnAnimQueueComplete(this.alive.grounded.idle)
				.Exit(delegate(Hatch.StatesInstance smi)
				{
					smi.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().CreatureStatusItems.Emerging, false);
				});
			this.alive.hide.DefaultState(this.alive.hide.pre).EventTransition(GameHashes.Drowning, this.alive.grounded.distressed.DrowningEmerge, null).Enter(delegate(Hatch.StatesInstance smi)
			{
				smi.master.userMenu.Refresh();
				smi.master.GetComponent<FactionAlignment>().ToggleAlignmentActive(false);
				smi.master.mainThreat = null;
				this.threatMoveTarget.Set(null, smi);
				int num5 = Grid.PosToCell(smi.master.transform.position);
				if (Grid.IsValidCell(num5))
				{
					smi.master.solidCellMonitor = GameScenePartitioner.Instance.Add("Hatch.Hide", smi.master.gameObject, num5, GameScenePartitioner.Instance.solidChangedLayer, delegate(object data)
					{
						if (!Grid.Solid[Grid.PosToCell(smi.master.transform.position)])
						{
							smi.GoTo(this.alive.dormant.pre);
						}
					});
				}
			})
				.Exit(delegate(Hatch.StatesInstance smi)
				{
					smi.master.StopListeningForDigPlacerChanged();
					smi.master.solidCellMonitor.Release();
					smi.master.GetComponent<FactionAlignment>().ToggleAlignmentActive(true);
				});
			this.alive.hide.pre.PlayAnim("hide").EventTransition(GameHashes.AnimQueueComplete, this.alive.hide.loop, null).ToggleMainStatusItem(Db.Get().CreatureStatusItems.Burrowing)
				.Enter(delegate(Hatch.StatesInstance smi)
				{
					smi.master.userMenu.Refresh();
					smi.master.GetComponent<Navigator>().Stop(false);
				})
				.Exit(delegate(Hatch.StatesInstance smi)
				{
					smi.master.mover.TeleportToTarget(Grid.CellToPos(Grid.CellBelow(Grid.PosToCell(smi.master.transform.position)), 0.5f, 0f, 0f), 0f);
				});
			this.alive.hide.loop.PlayAnim("idle_mound", KAnim.PlayMode.Loop).ToggleMainStatusItem(Db.Get().CreatureStatusItems.Burrowed).Enter(delegate(Hatch.StatesInstance smi)
			{
				smi.master.InheritExistingDigPlacer();
				smi.master.ListenForDigPlacerChanged();
				smi.master.mCollider.size = new Vector2(1f, 1.5f);
				smi.master.mCollider.offset = new Vector2(0f, 0.75f);
			})
				.EventTransition(GameHashes.Nighttime, (Hatch.StatesInstance smi) => GameClock.Instance, this.alive.grounded.emerge, (Hatch.StatesInstance smi) => GameClock.Instance.IsNighttime() && smi.master.EmergeIsClear())
				.Update(delegate(Hatch.StatesInstance smi)
				{
					int num6 = Grid.PosToCell(smi.transform.position);
					if (Grid.IsValidCell(num6) && !Grid.Solid[num6])
					{
						smi.GoTo(this.alive.dormant.pre);
					}
				})
				.Exit(delegate(Hatch.StatesInstance smi)
				{
					smi.master.userMenu.Refresh();
					this.awakeTime.Set(0f, smi);
					smi.master.mCollider.size = new Vector2(1f, 1f);
					smi.master.mCollider.offset = new Vector2(0f, 0.5f);
				});
			this.alive.dormant.Update(delegate(Hatch.StatesInstance smi)
			{
				int num7 = Grid.PosToCell(smi.transform.position + Vector3.down);
				if (Grid.IsValidCell(num7) && !Grid.Solid[num7])
				{
					smi.GoTo(this.alive.fall);
				}
			});
			this.alive.dormant.pre.PlayAnim("dormant_pre").EventTransition(GameHashes.Nighttime, (Hatch.StatesInstance smi) => GameClock.Instance, this.alive.dormant.pst, null).OnAnimQueueComplete(this.alive.dormant.pst)
				.Enter(delegate(Hatch.StatesInstance smi)
				{
					smi.master.userMenu.Refresh();
				});
			this.alive.dormant.pst.PlayAnim("dormant_pst").OnAnimQueueComplete(this.alive.grounded.attackStates);
			this.alive.fall.PlayAnim("fall", KAnim.PlayMode.Loop).ToggleGravity(this.alive.grounded.idle);
			this.death.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead).Enter(delegate(Hatch.StatesInstance smi)
			{
				smi.master.alive = false;
				Butcherable component2 = smi.master.GetComponent<Butcherable>();
				if (component2)
				{
					component2.OnButcherComplete();
				}
				smi.Play("death", KAnim.PlayMode.Once);
				smi.Schedule(2f, delegate(object d)
				{
					Util.KDestroyGameObject(smi.gameObject);
				}, null);
			});
		}

		public StateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.TargetParameter eatMoveTarget;

		public StateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.TargetParameter threatMoveTarget;

		public StateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.TargetParameter mover;

		public StateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.FloatParameter awakeTime;

		public StateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.FloatParameter hungerLevel;

		public StateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.FloatParameter consumedMass;

		public StateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.FloatParameter timeSinceLastMeal;

		public StateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.ObjectParameter<ThreatMonitor> threatMonitor;

		public StateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.ObjectParameter<GameObject> DigPlacer;

		public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State newGame;

		public Hatch.States.AliveStates alive;

		public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State death;

		public class AliveStates : GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State
		{
			public Hatch.States.GroundedState grounded;

			public Hatch.States.AliveStates.DormantState dormant;

			public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.PLPState hide;

			public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State fall;

			public class DormantState : GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State
			{
				public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State pre;

				public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State pst;
			}
		}

		public class GroundedState : GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State
		{
			public Hatch.States.IdleStates idle;

			public Hatch.States.EatStates eatStates;

			public Hatch.States.AttackStates attackStates;

			public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State emerge;

			public Hatch.States.DistressStates distressed;

			public Hatch.States.CagedStates caged;
		}

		public class EatStates : GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State
		{
			public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.ApproachSubState<Pickupable> moveToFood;

			public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State eat_pre;

			public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State eat;

			public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State eat_pst;
		}

		public class IdleStates : GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State
		{
			public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State idle;

			public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State poop;

			public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.IdleMoveSubState move;
		}

		public class AttackStates : GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State
		{
			public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State plan_attack;

			public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.ApproachSubState<AttackableBase> approachtarget;

			public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State regular;

			public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.CreatureFleeSubState<Approachable> flee;
		}

		public class DistressStates : GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State
		{
			public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State DrowningEmerge;

			public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State Drowning;
		}

		public class CagedStates : GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State
		{
			public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State idle;

			public GameStateMachine<Hatch.States, Hatch.StatesInstance, Hatch, object>.State funny_idle;
		}
	}
}
