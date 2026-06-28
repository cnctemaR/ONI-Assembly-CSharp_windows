using System;
using System.Collections.Generic;
using Klei;
using ProcGen;
using STRINGS;
using UnityEngine;

public class ExcavatorBomb : StateMachineComponent<ExcavatorBomb.StatesInstance>
{
	public float CountdownRemaining
	{
		get
		{
			return this.currentCountdown;
		}
	}

	public BuildingDef Def
	{
		get
		{
			return this.building.Def;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	private void PlayExplosion(Vector3 pos)
	{
		GameUtil.KInstantiate(EffectPrefabs.Instance.Explosion, pos, Grid.SceneLayer.Front, Folder.FX, null, 0);
	}

	private List<Vector2> DoCircularExplosion(float x, float y, float radius)
	{
		List<Vector2> circle = global::ProcGen.Util.GetCircle(new Vector2(x, y), (int)Mathf.Floor(radius));
		for (int i = 0; i < circle.Count; i++)
		{
			Vector3 vector = new Vector3(circle[i].x, circle[i].y, -Grid.CellSizeInMeters * 0.5f);
			int cell = Grid.PosToCell(vector);
			if (Grid.IsValidCell(cell) && Grid.Element[cell].id != SimHashes.Unobtanium)
			{
				this.PlayExplosion(vector);
				int num = -1;
				if (Grid.Solid[cell])
				{
					Element elem = Grid.Element[cell];
					float mass = Grid.Cell[cell].mass * 0.25f;
					global::System.Action action = delegate
					{
						if (elem.IsSolid)
						{
							Substance substance = elem.substance;
							substance.SpawnResource(Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore), mass, 300f, byte.MaxValue, 0, false, false);
						}
					};
					num = Game.Instance.callbackManager.Add(new Game.CallbackInfo(action, false)).index;
				}
				int cell2 = cell;
				SimHashes simHashes = SimHashes.CarbonDioxide;
				CellElementEvent excavator = CellEventLogger.Instance.Excavator;
				float num2 = 8f;
				float num3 = 1000f;
				int num4 = num;
				SimMessages.ReplaceElement(cell2, simHashes, excavator, num2, num3, byte.MaxValue, 0, num4);
			}
		}
		return circle;
	}

	private List<int> GetNeighbors(int cell)
	{
		List<int> list = new List<int>();
		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				if (Vector3.Distance(Grid.CellToPos(cell, (float)i, (float)j, 0f), this.startLocation) < this.maxRadius * this.maxRadius)
				{
					CellOffset cellOffset = new CellOffset(i, j);
					int num = Grid.OffsetCell(cell, cellOffset);
					if (!this.visitedPoints.ContainsKey(num))
					{
						if (Grid.IsValidCell(num))
						{
							if (Grid.Element[num].id != SimHashes.Unobtanium)
							{
								list.Add(num);
							}
						}
					}
				}
			}
		}
		return list;
	}

	private void DoShockwaveExplosion(float x, float y)
	{
		if (this.shockwavePoints == null)
		{
			this.startLocation = new Vector2(x, y);
			List<Vector2> list = this.DoCircularExplosion(x, y, 1f);
			this.visitedPoints = new Dictionary<int, ExcavatorBomb.visitedPoint>();
			this.shockwavePoints = new Dictionary<int, float>();
			for (int i = 0; i < list.Count; i++)
			{
				int num = Grid.PosToCell(list[i]);
				if (Grid.Element[num].id != SimHashes.Unobtanium && Grid.Element[num].id != SimHashes.Vacuum)
				{
					this.shockwavePoints[num] = this.totalEnergy;
				}
			}
			this.maxEnergy = this.totalEnergy;
			return;
		}
		Dictionary<int, float> dictionary = new Dictionary<int, float>(this.shockwavePoints);
		foreach (int num2 in this.shockwavePoints.Keys)
		{
			this.visitedPoints[num2] = default(ExcavatorBomb.visitedPoint);
		}
		this.shockwavePoints.Clear();
		this.maxEnergy = float.MinValue;
		foreach (int num3 in dictionary.Keys)
		{
			if (Grid.Element[num3].id != SimHashes.Unobtanium && Grid.Element[num3].id != SimHashes.Vacuum)
			{
				float mass = Grid.Cell[num3].mass;
				float num4 = 1f;
				float num5 = this.totalEnergy / num4;
				Element element = Grid.Element[num3];
				float num6 = 0.37037036f;
				float num7 = 0.7f;
				float num8 = 1f;
				float num9 = 2f;
				float num10 = 6f;
				float num11 = 0.5f;
				float num12 = 1f;
				float num13 = 1f;
				float num14 = 300f;
				float num15 = 0.1f;
				float num16 = 0.05f;
				float num17 = 0.2f;
				float num18 = dictionary[num3];
				float num19 = 0f;
				float num21;
				float num22;
				if (element.IsSolid)
				{
					float num20 = Mathf.Pow((float)element.hardness, num6);
					num19 = MathUtil.Clamp(num7, num8, MathUtil.ReRange(num20, num9, num10, num7, num8));
					num21 = MathUtil.Clamp(num11, num12, MathUtil.ReRange(mass, num13, num14, num11, num12));
					num22 = 1f - num21 * num19 + num15;
				}
				else if (element.IsLiquid)
				{
					num21 = MathUtil.Clamp(num11, num12, MathUtil.ReRange(mass, num13, num14, num11, num12));
					num22 = 1f - num21 + num16;
				}
				else
				{
					num21 = MathUtil.Clamp(num11, num12, MathUtil.ReRange(mass, num13 / 1000f, num14 / 1000f, num11, num12));
					num22 = 1f - num21 + num17;
				}
				num18 -= num22;
				this.visitedPoints[num3] = new ExcavatorBomb.visitedPoint(dictionary[num3], num18, num22, this.step, (!element.IsSolid) ? ((!element.IsLiquid) ? "gas" : "liquid") : "solid", element.name, num19, num21);
				this.maxEnergy = Mathf.Max(num18, this.maxEnergy);
				if (num18 > 0f)
				{
					KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("snore_fx_kanim", Grid.CellToPosCCC(num3, Grid.SceneLayer.FXFront), SceneOrganizer.Instance.GetFolder(Folder.FX).transform, false, Grid.SceneLayer.FXFront, false);
					kbatchedAnimController.destroyOnAnimComplete = true;
					kbatchedAnimController.Play("snore", KAnim.PlayMode.Once, 1f, 0f);
					if (element.IsSolid)
					{
						float num23 = num18 * 2f / this.totalEnergy;
						Output.Log(new object[]
						{
							"step",
							this.step,
							"\tprevEnergy",
							dictionary[num3],
							"\tenergy",
							num18,
							"\tDamageb",
							num23
						});
						float kilojoules = 10000f * dictionary[num3] / this.totalEnergy;
						float temperature = Grid.Temperature[num3];
						Element elem = Grid.Element[num3];
						int local_cell = num3;
						global::System.Action action = delegate
						{
							if (elem.IsSolid)
							{
								float num25 = temperature + SimUtil.EnergyFlowToTemperatureDelta(kilojoules, elem.specificHeatCapacity, mass);
								Substance substance = elem.substance;
								substance.SpawnResource(Grid.CellToPos(local_cell, CellAlignment.RandomInternal, Grid.SceneLayer.Ore), mass * 0.25f, num25, byte.MaxValue, 0, false, false);
							}
						};
						HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(action, false));
						if (!WorldDamage.Instance.ApplyDamage(num3, num23, -1, handle.index))
						{
							SimMessages.ModifyEnergy(num3, kilojoules, SimMessages.EnergySourceID.Excavator);
						}
					}
					List<int> neighbors = this.GetNeighbors(num3);
					foreach (int num24 in neighbors)
					{
						this.shockwavePoints[num24] = num18;
					}
				}
			}
		}
		this.step++;
	}

	private bool Explode(float dt)
	{
		this.nextExplosion -= dt;
		if (this.nextExplosion < 0f)
		{
			Collider componentInChildren = base.GetComponentInChildren<Collider>();
			Bounds bounds;
			if (componentInChildren != null)
			{
				bounds = componentInChildren.bounds;
			}
			else
			{
				bounds = base.GetComponentInChildren<Collider2D>().bounds;
			}
			float x = bounds.center.x;
			float y = bounds.center.y;
			ExcavatorBomb.ExplosionType explosionType = this.type;
			if (explosionType != ExcavatorBomb.ExplosionType.CircularFilled)
			{
				if (explosionType == ExcavatorBomb.ExplosionType.Shockwave)
				{
					this.DoShockwaveExplosion(x, y);
				}
			}
			else
			{
				this.currentRadius += 1f;
				this.DoCircularExplosion(x, y, this.currentRadius);
			}
			this.nextExplosion = 0.001f;
		}
		if (this.currentRadius > this.maxRadius || this.maxEnergy <= 0f)
		{
			int num = Grid.PosToCell(this);
			foreach (CellOffset cellOffset in this.Def.PlacementOffsets)
			{
				int num2 = Grid.OffsetCell(num, cellOffset);
				if (Grid.Element[num2].id != SimHashes.Unobtanium)
				{
					Vector3 vector = Grid.CellToPosCCC(num2, Grid.SceneLayer.Building);
					vector.z = -Grid.CellSizeInMeters * 0.5f;
					this.PlayExplosion(vector);
				}
			}
			base.PlaySound3D(Sounds.Instance.BlowUp_GenericMigrated);
			Vector3 vector2 = Grid.CellToPosCCC(num, Grid.SceneLayer.Move);
			GameUtil.CreateExplosion(vector2);
			global::Util.KDestroyGameObject(base.gameObject);
			return true;
		}
		return false;
	}

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	public float CountdownTime = 10f;

	private float currentCountdown;

	public float minSafeDistance = 5f;

	public float maxRadius = 5f;

	public float currentRadius;

	public float nextExplosion = 0.01f;

	public bool detectMinion = true;

	public ExcavatorBomb.ExplosionType type;

	public Dictionary<int, ExcavatorBomb.visitedPoint> visitedPoints;

	private Dictionary<int, float> shockwavePoints;

	private Vector3 startLocation;

	private int step;

	private float maxEnergy = float.MinValue;

	private float totalEnergy = 1f;

	public class StatesInstance : GameStateMachine<ExcavatorBomb.States, ExcavatorBomb.StatesInstance, ExcavatorBomb, object>.GameInstance
	{
		public StatesInstance(ExcavatorBomb master)
			: base(master)
		{
		}

		public bool DupeInDanger()
		{
			if (!base.smi.master.detectMinion)
			{
				return false;
			}
			float num = float.MaxValue;
			for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
			{
				float num2 = Vector3.Distance(Components.LiveMinionIdentities[i].gameObject.transform.position, base.transform.position);
				num = Mathf.Min(num, num2);
			}
			return num < base.master.maxRadius + base.master.minSafeDistance;
		}
	}

	public class States : GameStateMachine<ExcavatorBomb.States, ExcavatorBomb.StatesInstance, ExcavatorBomb>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.InitializeStates(out default_state);
			default_state = this.idle;
			this.statusItemUnarmed = new StatusItem("Unarmed", BUILDING.STATUSITEMS.EXCAVATOR_BOMB.UNARMED.NAME, BUILDING.STATUSITEMS.EXCAVATOR_BOMB.UNARMED.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 30718);
			this.statusItemArmed = new StatusItem("Armed", BUILDING.STATUSITEMS.EXCAVATOR_BOMB.ARMED.NAME, BUILDING.STATUSITEMS.EXCAVATOR_BOMB.ARMED.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 30718);
			this.statusItemCountdown = new StatusItem("Countdown", BUILDING.STATUSITEMS.EXCAVATOR_BOMB.COUNTDOWN.NAME, BUILDING.STATUSITEMS.EXCAVATOR_BOMB.COUNTDOWN.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 30718);
			this.statusItemCountdown.resolveStringCallback = delegate(string str, object data)
			{
				ExcavatorBomb.StatesInstance statesInstance = (ExcavatorBomb.StatesInstance)data;
				return string.Format(str, GameUtil.GetFormattedTime(statesInstance.master.CountdownRemaining));
			};
			this.statusItemDupeDanger = new StatusItem("DupeDanger", BUILDING.STATUSITEMS.EXCAVATOR_BOMB.DUPE_DANGER.NAME, BUILDING.STATUSITEMS.EXCAVATOR_BOMB.DUPE_DANGER.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 30718);
			this.statusItemExpoding = new StatusItem("Exploding", BUILDING.STATUSITEMS.EXCAVATOR_BOMB.EXPLODING.NAME, BUILDING.STATUSITEMS.EXCAVATOR_BOMB.EXPLODING.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, 30718);
			this.idle.PlayAnim("off", KAnim.PlayMode.Loop).ToggleMainStatusItem(this.statusItemUnarmed).GoTo(this.armed);
			this.armed.PlayAnim("on", KAnim.PlayMode.Loop).ToggleMainStatusItem(this.statusItemArmed).GoTo(this.dupe_danger);
			this.dupe_danger.PlayAnim("working_post", KAnim.PlayMode.Loop).ToggleMainStatusItem(this.statusItemDupeDanger).Transition(this.countdown, (ExcavatorBomb.StatesInstance smi) => !smi.DupeInDanger());
			this.countdown.PlayAnim("working", KAnim.PlayMode.Loop).ToggleMainStatusItem(this.statusItemCountdown).Transition(this.dupe_danger, (ExcavatorBomb.StatesInstance smi) => smi.DupeInDanger())
				.Enter(delegate(ExcavatorBomb.StatesInstance smi)
				{
					smi.master.currentCountdown = smi.master.CountdownTime;
				})
				.Update(delegate(ExcavatorBomb.StatesInstance smi)
				{
					smi.master.currentCountdown -= smi.deltatime;
					if (smi.master.currentCountdown <= 0f)
					{
						smi.GoTo(this.exploding);
					}
				});
			this.exploding.ToggleMainStatusItem(this.statusItemExpoding).Update(delegate(ExcavatorBomb.StatesInstance smi)
			{
				bool flag = smi.master.Explode(smi.deltatime);
				if (flag)
				{
					smi.GoTo(this.defunct);
				}
			});
		}

		public GameStateMachine<ExcavatorBomb.States, ExcavatorBomb.StatesInstance, ExcavatorBomb, object>.State idle;

		public GameStateMachine<ExcavatorBomb.States, ExcavatorBomb.StatesInstance, ExcavatorBomb, object>.State armed;

		public GameStateMachine<ExcavatorBomb.States, ExcavatorBomb.StatesInstance, ExcavatorBomb, object>.State countdown;

		public GameStateMachine<ExcavatorBomb.States, ExcavatorBomb.StatesInstance, ExcavatorBomb, object>.State dupe_danger;

		public GameStateMachine<ExcavatorBomb.States, ExcavatorBomb.StatesInstance, ExcavatorBomb, object>.State exploding;

		public GameStateMachine<ExcavatorBomb.States, ExcavatorBomb.StatesInstance, ExcavatorBomb, object>.State defunct;

		private StatusItem statusItemUnarmed;

		private StatusItem statusItemArmed;

		private StatusItem statusItemCountdown;

		private StatusItem statusItemDupeDanger;

		private StatusItem statusItemExpoding;
	}

	public enum ExplosionType
	{
		CircularFilled,
		CircularCenterOnly,
		Shockwave
	}

	public struct visitedPoint
	{
		public visitedPoint(float energyIn, float energyOut, float energySpent, int step, string state, string element, float hm, float mm)
		{
			this.energyIn = energyIn;
			this.energyOut = energyOut;
			this.energySpent = energySpent;
			this.step = step;
			this.state = state;
			this.element = element;
			this.hm = hm;
			this.mm = mm;
		}

		public float energyIn;

		public float energyOut;

		public float energySpent;

		public int step;

		public string state;

		public string element;

		public float hm;

		public float mm;
	}
}
