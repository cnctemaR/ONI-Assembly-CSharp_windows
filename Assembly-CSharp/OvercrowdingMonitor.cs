using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class OvercrowdingMonitor : GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.Update(new Action<OvercrowdingMonitor.Instance, float>(OvercrowdingMonitor.UpdateState), UpdateRate.SIM_1000ms, true);
	}

	private static int FetchCreaturePersonalSpace(KPrefabID creature)
	{
		int spaceRequiredPerCreature;
		if (OvercrowdingMonitor.personalSpaces.TryGetValue(creature.PrefabTag, out spaceRequiredPerCreature))
		{
			return spaceRequiredPerCreature;
		}
		OvercrowdingMonitor.Instance smi = creature.GetSMI<OvercrowdingMonitor.Instance>();
		if (smi != null)
		{
			spaceRequiredPerCreature = smi.def.spaceRequiredPerCreature;
		}
		OvercrowdingMonitor.personalSpaces[creature.PrefabTag] = spaceRequiredPerCreature;
		return spaceRequiredPerCreature;
	}

	private static int FetchEggPersonalSpace(KPrefabID egg)
	{
		int spaceRequiredPerCreature;
		if (OvercrowdingMonitor.personalSpaces.TryGetValue(egg.PrefabTag, out spaceRequiredPerCreature))
		{
			return spaceRequiredPerCreature;
		}
		IncubationMonitor.Instance smi = egg.GetSMI<IncubationMonitor.Instance>();
		if (OvercrowdingMonitor.personalSpaces.TryGetValue(smi.def.spawnedCreature, out spaceRequiredPerCreature))
		{
			OvercrowdingMonitor.personalSpaces[egg.PrefabTag] = spaceRequiredPerCreature;
			return spaceRequiredPerCreature;
		}
		GameObject prefab = Assets.GetPrefab(smi.def.spawnedCreature);
		if (prefab != null)
		{
			spaceRequiredPerCreature = prefab.GetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature;
		}
		OvercrowdingMonitor.personalSpaces[egg.PrefabTag] = spaceRequiredPerCreature;
		OvercrowdingMonitor.personalSpaces[smi.def.spawnedCreature] = spaceRequiredPerCreature;
		return spaceRequiredPerCreature;
	}

	private static void UpdateState(OvercrowdingMonitor.Instance smi, float dt)
	{
		OvercrowdingMonitor.UpdateRegion(smi);
		if (smi.def.spaceRequiredPerCreature == 0)
		{
			return;
		}
		OvercrowdingMonitor.Occupancy occupancy = null;
		List<KPrefabID> list = null;
		List<KPrefabID> list2 = null;
		if (smi.IsFish)
		{
			if (smi.pond != null)
			{
				occupancy = smi.pond.occupancy;
				list = smi.pond.fishes;
				list2 = smi.pond.eggs;
			}
		}
		else if (smi.cavity != null)
		{
			occupancy = smi.cavity.occupancy;
			list = smi.cavity.creatures;
			list2 = smi.cavity.eggs;
		}
		if (occupancy != null && occupancy.dirty)
		{
			occupancy.Analyze(list, list2, !smi.IsFish);
		}
		if (smi.regionAnalysis.IsDirty)
		{
			smi.regionAnalysis.SetOccupancy(occupancy);
			smi.OnRegionAnalysisDirtied();
		}
		OvercrowdingMonitor.AlignTagsAndEffects(smi);
	}

	private static void AlignTagsAndEffects(OvercrowdingMonitor.Instance smi)
	{
		bool isConfined = smi.regionAnalysis.IsConfined;
		bool isOvercrowded = smi.regionAnalysis.IsOvercrowded;
		int num = smi.regionAnalysis.OvercrowdedModifier;
		bool isFutureOvercrowded = smi.regionAnalysis.IsFutureOvercrowded;
		num = (isOvercrowded ? num : 0);
		if (isOvercrowded)
		{
			smi.overcrowded.Modifier.SetValue((float)num);
		}
		bool flag = smi.kpid.HasTag(GameTags.Creatures.Overcrowded);
		bool flag2 = smi.kpid.HasTag(GameTags.Creatures.Expecting);
		bool flag3 = smi.kpid.HasTag(GameTags.Creatures.Confined);
		bool flag4 = smi.effects.HasEffect(smi.futureOvercrowded.Effect);
		if (flag != isOvercrowded)
		{
			smi.kpid.SetTag(GameTags.Creatures.Overcrowded, isOvercrowded);
		}
		bool flag5 = !smi.isBaby && !isFutureOvercrowded;
		if (flag2 != flag5)
		{
			smi.kpid.SetTag(GameTags.Creatures.Expecting, flag5);
		}
		if (flag3 != isConfined)
		{
			smi.kpid.SetTag(GameTags.Creatures.Confined, isConfined);
		}
		bool flag6 = isConfined;
		bool flag7 = isOvercrowded && !flag6;
		bool flag8 = isFutureOvercrowded && !flag6;
		if (flag6 != flag3)
		{
			smi.confined.instance = OvercrowdingMonitor.SetEffect(smi, smi.confined.Effect, flag6, smi.confined.Tooltip);
		}
		if (flag7 != flag)
		{
			smi.overcrowded.instance = OvercrowdingMonitor.SetEffect(smi, smi.overcrowded.Effect, flag7, smi.overcrowded.Tooltip);
		}
		if (flag8 != flag4)
		{
			smi.futureOvercrowded.instance = OvercrowdingMonitor.SetEffect(smi, smi.futureOvercrowded.Effect, flag8, smi.futureOvercrowded.Tooltip);
		}
	}

	private static EffectInstance SetEffect(OvercrowdingMonitor.Instance smi, Effect effect, bool set, Func<string, object, string> tooltip)
	{
		if (set)
		{
			return smi.effects.Add(effect, false, tooltip);
		}
		smi.effects.Remove(effect);
		return null;
	}

	private static bool FetchIsFish(KPrefabID creature)
	{
		bool flag;
		if (OvercrowdingMonitor.isFish.TryGetValue(creature.PrefabTag, out flag))
		{
			return flag;
		}
		flag = creature.GetDef<FishOvercrowdingMonitor.Def>() != null;
		OvercrowdingMonitor.isFish[creature.PrefabTag] = flag;
		return flag;
	}

	private static bool FetchIsFishEgg(KPrefabID egg)
	{
		bool flag;
		if (OvercrowdingMonitor.isFish.TryGetValue(egg.PrefabTag, out flag))
		{
			return flag;
		}
		IncubationMonitor.Instance smi = egg.GetSMI<IncubationMonitor.Instance>();
		if (OvercrowdingMonitor.isFish.TryGetValue(smi.def.spawnedCreature, out flag))
		{
			OvercrowdingMonitor.isFish[egg.PrefabTag] = flag;
			return flag;
		}
		GameObject prefab = Assets.GetPrefab(smi.def.spawnedCreature);
		if (prefab != null)
		{
			flag = prefab.GetDef<FishOvercrowdingMonitor.Def>() != null;
		}
		OvercrowdingMonitor.isFish[egg.PrefabTag] = flag;
		OvercrowdingMonitor.isFish[smi.def.spawnedCreature] = flag;
		return flag;
	}

	private static void UpdateRegion(OvercrowdingMonitor.Instance smi)
	{
		int num = Grid.PosToCell(smi);
		CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
		if (cavityForCell != smi.cavity)
		{
			if (smi.cavity != null)
			{
				smi.RemoveFromCavity();
				Game.Instance.roomProber.UpdateRoom(cavityForCell);
				smi.regionAnalysis.ForceDirty();
			}
			smi.cavity = cavityForCell;
			if (smi.cavity != null)
			{
				smi.AddToCavity();
				Game.Instance.roomProber.UpdateRoom(smi.cavity);
				smi.regionAnalysis.ForceDirty();
			}
		}
		if (smi.IsFish)
		{
			FishOvercrowingManager.Pond pond = FishOvercrowingManager.Instance.GetPond(num);
			if (pond != smi.pond)
			{
				if (smi.pond != null)
				{
					smi.regionAnalysis.ForceDirty();
				}
				smi.pond = pond;
				if (smi.pond != null)
				{
					smi.regionAnalysis.ForceDirty();
				}
			}
		}
	}

	public const float OVERCROWDED_FERTILITY_DEBUFF = -1f;

	public static readonly Tag[] CONFINEMENT_IMMUNITY_TAGS = new Tag[]
	{
		GameTags.Creatures.Burrowed,
		GameTags.Creatures.Digger
	};

	private static readonly Dictionary<Tag, int> personalSpaces = new Dictionary<Tag, int>();

	private static readonly Dictionary<Tag, bool> isFish = new Dictionary<Tag, bool>();

	public class Def : StateMachine.BaseDef
	{
		public int spaceRequiredPerCreature;
	}

	public class Occupancy
	{
		public Dictionary<Tag, int> CritterCounts { get; } = new Dictionary<Tag, int>();

		public int OccupiedCellCount { get; private set; }

		public int HatchedEggOccupiedCellCount { get; private set; }

		public int Generation { get; private set; }

		public void Analyze(List<KPrefabID> creatures, List<KPrefabID> eggs, bool excludeFish = false)
		{
			DebugUtil.DevAssert(this.dirty, "Only incur Analyze overhead when dirty", null);
			this.CritterCounts.EnsureCapacity(creatures.Count);
			this.CritterCounts.Clear();
			this.OccupiedCellCount = 0;
			this.HatchedEggOccupiedCellCount = 0;
			creatures.RemoveAll(Util.IsNullOrDestroyedPredicate);
			foreach (KPrefabID kprefabID in creatures)
			{
				if (!excludeFish || !OvercrowdingMonitor.FetchIsFish(kprefabID))
				{
					this.OccupiedCellCount += OvercrowdingMonitor.FetchCreaturePersonalSpace(kprefabID);
					int num;
					if (this.CritterCounts.TryGetValue(kprefabID.PrefabTag, out num))
					{
						this.CritterCounts[kprefabID.PrefabTag] = num + 1;
					}
					else
					{
						this.CritterCounts[kprefabID.PrefabTag] = 1;
					}
				}
			}
			eggs.RemoveAll(Util.IsNullOrDestroyedPredicate);
			foreach (KPrefabID kprefabID2 in eggs)
			{
				if (!excludeFish || !OvercrowdingMonitor.FetchIsFishEgg(kprefabID2))
				{
					this.HatchedEggOccupiedCellCount += OvercrowdingMonitor.FetchEggPersonalSpace(kprefabID2);
				}
			}
			int generation = this.Generation;
			this.Generation = generation + 1;
			this.dirty = false;
		}

		public bool dirty = true;
	}

	public struct RegionAnalysis
	{
		public readonly bool IsDirty
		{
			get
			{
				return this.occupancy == null || this.occupancyGeneration != this.occupancy.Generation;
			}
		}

		public readonly int CellCount
		{
			get
			{
				return this.smi.RegionSize;
			}
		}

		public readonly bool IsPond
		{
			get
			{
				return this.smi.IsInPond;
			}
		}

		public readonly Dictionary<Tag, int> CritterCounts
		{
			get
			{
				if (this.occupancy == null)
				{
					return null;
				}
				return this.occupancy.CritterCounts;
			}
		}

		public readonly int OccupiedCellCount
		{
			get
			{
				if (this.occupancy == null)
				{
					return 0;
				}
				return this.occupancy.OccupiedCellCount;
			}
		}

		public readonly int HatchedEggOccupiedCellCount
		{
			get
			{
				if (this.occupancy == null)
				{
					return 0;
				}
				return this.occupancy.HatchedEggOccupiedCellCount;
			}
		}

		public readonly bool IsOvercrowded
		{
			get
			{
				return !this.IsDegeneratePersonalSpace && this.UnoccupiedCellCount < 0;
			}
		}

		public readonly bool IsConfined
		{
			get
			{
				return !this.ConfinementImmunity && !this.IsDegeneratePersonalSpace && this.CellCount < this.PersonalSpace;
			}
		}

		public readonly bool IsFutureOvercrowded
		{
			get
			{
				return !this.IsDegeneratePersonalSpace && this.FutureUnoccupiedCellCount < 0 && this.HatchedEggOccupiedCellCount > 0;
			}
		}

		public readonly int OvercrowdedModifier
		{
			get
			{
				if (!this.IsOvercrowded)
				{
					return 0;
				}
				return -this.OverOccupiedCritterCount;
			}
		}

		public readonly bool IsDegenerate
		{
			get
			{
				return this.CellCount <= 0;
			}
		}

		private readonly int PersonalSpace
		{
			get
			{
				return this.smi.def.spaceRequiredPerCreature;
			}
		}

		private readonly bool ConfinementImmunity
		{
			get
			{
				return this.smi.kpid.HasAnyTags(OvercrowdingMonitor.CONFINEMENT_IMMUNITY_TAGS);
			}
		}

		private readonly int UnoccupiedCellCount
		{
			get
			{
				return this.CellCount - this.OccupiedCellCount;
			}
		}

		private readonly int OverOccupiedCellCount
		{
			get
			{
				return this.OccupiedCellCount - this.CellCount;
			}
		}

		private readonly int FutureUnoccupiedCellCount
		{
			get
			{
				return this.UnoccupiedCellCount - this.HatchedEggOccupiedCellCount;
			}
		}

		private readonly bool IsDegeneratePersonalSpace
		{
			get
			{
				return this.PersonalSpace == 0;
			}
		}

		private readonly int OverOccupiedCritterCount
		{
			get
			{
				return this.ComputeOverOccupiedCritterCount(this.PersonalSpace);
			}
		}

		public RegionAnalysis(OvercrowdingMonitor.Instance smi)
		{
			this.smi = smi;
			this.occupancy = null;
			this.occupancyGeneration = -1;
		}

		public void SetOccupancy(OvercrowdingMonitor.Occupancy occupancy)
		{
			this.occupancy = occupancy;
			this.occupancyGeneration = ((occupancy != null) ? occupancy.Generation : (-1));
		}

		public void ForceDirty()
		{
			this.occupancyGeneration = -1;
		}

		public readonly string Substitute(string s)
		{
			LocString locString = (this.IsPond ? ((this.CellCount == 0) ? CREATURES.MODIFIERS.OVERCROWDED.EXPLANATION_AQUATIC.NO_CELLS : ((this.CellCount == 1) ? CREATURES.MODIFIERS.OVERCROWDED.EXPLANATION_AQUATIC.SINGLE_CELL : CREATURES.MODIFIERS.OVERCROWDED.EXPLANATION_AQUATIC.MULTIPLE_CELLS)) : ((this.CellCount == 0) ? CREATURES.MODIFIERS.OVERCROWDED.EXPLANATION.NO_CELLS : ((this.CellCount == 1) ? CREATURES.MODIFIERS.OVERCROWDED.EXPLANATION.SINGLE_CELL : CREATURES.MODIFIERS.OVERCROWDED.EXPLANATION.MULTIPLE_CELLS)));
			StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
			stringBuilder.Append(s);
			stringBuilder.Replace("{explanation}", locString);
			stringBuilder.Replace("{contextCritterType}", this.smi.kpid.PrefabTag.ProperName());
			stringBuilder.Replace("{personalSpace}", this.PersonalSpace.ToString());
			stringBuilder.Replace("{cellCount}", this.CellCount.ToString());
			stringBuilder.Replace("{occupiedCellCount}", this.OccupiedCellCount.ToString());
			stringBuilder.Replace("{unoccupiedCellCount}", (this.IsOvercrowded ? 0 : this.UnoccupiedCellCount).ToString());
			stringBuilder.Replace("{overOccupiedCellCount}", (this.IsOvercrowded ? this.OverOccupiedCellCount : 0).ToString());
			stringBuilder.Replace("{bullets}", this.BuildCritterOccupancies());
			return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
		}

		private readonly int ComputeOverOccupiedCritterCount(int personalSpace)
		{
			if (personalSpace == 0)
			{
				return 0;
			}
			int num;
			return Math.DivRem(this.OverOccupiedCellCount, personalSpace, out num) + ((num == 0) ? 0 : 1);
		}

		private readonly string BuildCritterOccupancies()
		{
			if (this.CritterCounts == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
			ListPool<OvercrowdingMonitor.RegionAnalysis.CritterOccupancy, OvercrowdingMonitor.RegionAnalysis>.PooledList pooledList = ListPool<OvercrowdingMonitor.RegionAnalysis.CritterOccupancy, OvercrowdingMonitor.RegionAnalysis>.Allocate();
			pooledList.Capacity = this.CritterCounts.Count;
			foreach (Tag tag in this.CritterCounts.Keys)
			{
				int num = OvercrowdingMonitor.personalSpaces[tag];
				int num2 = this.ComputeOverOccupiedCritterCount(num);
				int num3 = this.CritterCounts[tag];
				bool flag = num2 <= num3;
				num2 = Math.Min(this.CritterCounts[tag], num2);
				pooledList.Add(new OvercrowdingMonitor.RegionAnalysis.CritterOccupancy
				{
					critterType = tag,
					overOccupancy = num2,
					canFix = flag
				});
			}
			Dictionary<Tag, int> capturedCritterCounts = this.CritterCounts;
			Dictionary<Tag, int> capturedPersonalSpaces = OvercrowdingMonitor.personalSpaces;
			pooledList.Sort(delegate(OvercrowdingMonitor.RegionAnalysis.CritterOccupancy a, OvercrowdingMonitor.RegionAnalysis.CritterOccupancy b)
			{
				int num7 = capturedCritterCounts[a.critterType] * capturedPersonalSpaces[a.critterType];
				return (capturedCritterCounts[b.critterType] * capturedPersonalSpaces[b.critterType]).CompareTo(num7);
			});
			foreach (OvercrowdingMonitor.RegionAnalysis.CritterOccupancy critterOccupancy in pooledList)
			{
				int num4 = this.CritterCounts[critterOccupancy.critterType];
				LocString locString = ((num4 == 1) ? (critterOccupancy.canFix ? CREATURES.MODIFIERS.OVERCROWDED.BULLET.CAN_FIX.SINGULAR : CREATURES.MODIFIERS.OVERCROWDED.BULLET.CANNOT_FIX.SINGULAR) : (critterOccupancy.canFix ? CREATURES.MODIFIERS.OVERCROWDED.BULLET.CAN_FIX.MULTIPLE : CREATURES.MODIFIERS.OVERCROWDED.BULLET.CANNOT_FIX.MULTIPLE));
				int num5 = OvercrowdingMonitor.personalSpaces[critterOccupancy.critterType];
				int num6 = this.OccupiedCellCount - critterOccupancy.overOccupancy * num5;
				StringBuilder stringBuilder2 = GlobalStringBuilderPool.Alloc();
				stringBuilder2.Append(locString);
				stringBuilder2.Replace("{critterType}", critterOccupancy.critterType.ProperName());
				stringBuilder2.Replace("{critterCount}", num4.ToString());
				stringBuilder2.Replace("{personalSpace}", num5.ToString());
				StringBuilder stringBuilder3 = stringBuilder2;
				string text = "{overOccupancy}";
				int overOccupancy = critterOccupancy.overOccupancy;
				stringBuilder3.Replace(text, overOccupancy.ToString());
				stringBuilder2.Replace("{cellCountWithFix}", num6.ToString());
				stringBuilder.AppendLine(GlobalStringBuilderPool.ReturnAndFree(stringBuilder2));
			}
			if (this.CritterCounts.Count > 0)
			{
				stringBuilder.Length--;
			}
			pooledList.Recycle();
			return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
		}

		private OvercrowdingMonitor.Occupancy occupancy;

		private int occupancyGeneration;

		private readonly OvercrowdingMonitor.Instance smi;

		private struct CritterOccupancy
		{
			public Tag critterType;

			public int overOccupancy;

			public bool canFix;
		}
	}

	public struct OvercrowdEffect
	{
		public Effect Effect { readonly get; private set; }

		public AttributeModifier Modifier { readonly get; private set; }

		public Func<string, object, string> Tooltip { readonly get; private set; }

		public string TooltipText { readonly get; private set; }

		public OvercrowdEffect(string id, string name, AttributeModifier modifier, Func<string> GenerateTooltip)
		{
			this.Effect = new Effect(id, name, string.Empty, 0f, true, false, true, null, -1f, 0f, null, "");
			this.instance = null;
			this.Modifier = modifier;
			this.Tooltip = null;
			this.Effect.Add(modifier);
			this.TooltipText = null;
			OvercrowdingMonitor.OvercrowdEffect capturedThis = this;
			this.Tooltip = delegate(string _tooltip, object untypedEffectInstance)
			{
				if (capturedThis.TooltipText == null)
				{
					EffectInstance effectInstance = (EffectInstance)untypedEffectInstance;
					capturedThis.TooltipText = effectInstance.ResolveTooltip(GenerateTooltip(), untypedEffectInstance);
				}
				return capturedThis.TooltipText;
			};
		}

		public OvercrowdEffect(string id, string name, AttributeModifier modifier, string tooltipFormat)
		{
			this = new OvercrowdingMonitor.OvercrowdEffect(id, name, modifier, () => tooltipFormat);
		}

		public void ClearTooltip()
		{
			this.TooltipText = null;
		}

		public EffectInstance instance;
	}

	public new class Instance : GameStateMachine<OvercrowdingMonitor, OvercrowdingMonitor.Instance, IStateMachineTarget, OvercrowdingMonitor.Def>.GameInstance
	{
		public int RegionSize
		{
			get
			{
				if (!this.IsFish)
				{
					if (this.cavity == null)
					{
						return 0;
					}
					return this.cavity.NumCells;
				}
				else
				{
					if (this.pond == null)
					{
						return 0;
					}
					return this.pond.cellCount;
				}
			}
		}

		public bool IsInPond
		{
			get
			{
				return this.pond != null;
			}
		}

		public bool IsFish
		{
			get
			{
				return OvercrowdingMonitor.FetchIsFish(this.kpid);
			}
		}

		public bool IsEgg
		{
			get
			{
				return this.kpid.HasTag(GameTags.Egg);
			}
		}

		public Instance(IStateMachineTarget master, OvercrowdingMonitor.Def def)
			: base(master, def)
		{
			BabyMonitor.Def def2 = master.gameObject.GetDef<BabyMonitor.Def>();
			this.isBaby = def2 != null;
			this.futureOvercrowded = new OvercrowdingMonitor.OvercrowdEffect("FutureOvercrowded", CREATURES.MODIFIERS.OVERCROWDED.CRAMPED.NAME, new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, -1f, CREATURES.MODIFIERS.OVERCROWDED.CRAMPED.NAME, true, false, true), new Func<string>(this.<.ctor>g__FutureOvercrowdedTooltip|18_0));
			this.overcrowded = new OvercrowdingMonitor.OvercrowdEffect("Overcrowded", CREATURES.MODIFIERS.OVERCROWDED.CROWDED.NAME, new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, 0f, CREATURES.MODIFIERS.OVERCROWDED.CROWDED.NAME, false, false, false), new Func<string>(this.<.ctor>g__OvercrowdedTooltip|18_1));
			this.confined = new OvercrowdingMonitor.OvercrowdEffect("Confined", CREATURES.MODIFIERS.OVERCROWDED.CONFINED.NAME, new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -10f, CREATURES.MODIFIERS.OVERCROWDED.CONFINED.NAME, false, false, false), new Func<string>(this.<.ctor>g__ConfinedTooltip|18_2));
			this.confined.Effect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, -1f, CREATURES.MODIFIERS.OVERCROWDED.CONFINED.NAME, true, false, true));
			this.onRoomUpdatedHandle = base.Subscribe(144050788, new Action<object>(this.OnRoomUpdated));
			this.regionAnalysis = new OvercrowdingMonitor.RegionAnalysis(this);
			OvercrowdingMonitor.UpdateState(this, 0f);
		}

		public void OnRegionAnalysisDirtied()
		{
			this.futureOvercrowded.ClearTooltip();
			this.overcrowded.ClearTooltip();
			this.confined.ClearTooltip();
		}

		public void AddToCavity()
		{
			if (this.IsEgg)
			{
				this.cavity.eggs.Add(this.kpid);
				if (OvercrowdingMonitor.FetchIsFishEgg(this.kpid))
				{
					this.cavity.fish_eggs.Add(this.kpid);
				}
			}
			else
			{
				this.cavity.creatures.Add(this.kpid);
				if (this.IsFish)
				{
					this.cavity.fishes.Add(this.kpid);
				}
			}
			this.cavity.occupancy.dirty = true;
		}

		public void RemoveFromCavity()
		{
			if (this.IsEgg)
			{
				this.cavity.RemoveFromCavity(this.kpid, this.cavity.eggs);
				if (OvercrowdingMonitor.FetchIsFishEgg(this.kpid))
				{
					this.cavity.RemoveFromCavity(this.kpid, this.cavity.fish_eggs);
				}
			}
			else
			{
				this.cavity.RemoveFromCavity(this.kpid, this.cavity.creatures);
				if (this.IsFish)
				{
					this.cavity.RemoveFromCavity(this.kpid, this.cavity.fishes);
				}
			}
			this.cavity.occupancy.dirty = true;
		}

		protected override void OnCleanUp()
		{
			base.Unsubscribe(ref this.onRoomUpdatedHandle);
			if (this.cavity == null)
			{
				return;
			}
			this.RemoveFromCavity();
		}

		public void OnRoomUpdated(object o)
		{
			if (o == null)
			{
				this.RoomRefreshUpdateCavity();
			}
		}

		public void RoomRefreshUpdateCavity()
		{
			OvercrowdingMonitor.UpdateState(this, 0f);
		}

		[CompilerGenerated]
		private string <.ctor>g__FutureOvercrowdedTooltip|18_0()
		{
			return this.regionAnalysis.Substitute(this.IsFish ? CREATURES.MODIFIERS.OVERCROWDED.CRAMPED.FISHTOOLTIP : CREATURES.MODIFIERS.OVERCROWDED.CRAMPED.TOOLTIP);
		}

		[CompilerGenerated]
		private string <.ctor>g__OvercrowdedTooltip|18_1()
		{
			return this.regionAnalysis.Substitute(this.IsFish ? CREATURES.MODIFIERS.OVERCROWDED.CROWDED.FISHTOOLTIP : CREATURES.MODIFIERS.OVERCROWDED.CROWDED.TOOLTIP);
		}

		[CompilerGenerated]
		private string <.ctor>g__ConfinedTooltip|18_2()
		{
			return this.regionAnalysis.Substitute(this.regionAnalysis.IsDegenerate ? CREATURES.MODIFIERS.OVERCROWDED.CONFINED.TOOLTIP_NO_SUBSTITUTIONS : CREATURES.MODIFIERS.OVERCROWDED.CONFINED.TOOLTIP);
		}

		public CavityInfo cavity;

		public FishOvercrowingManager.Pond pond;

		public bool isBaby;

		public OvercrowdingMonitor.OvercrowdEffect futureOvercrowded;

		public OvercrowdingMonitor.OvercrowdEffect overcrowded;

		public OvercrowdingMonitor.OvercrowdEffect confined;

		public OvercrowdingMonitor.RegionAnalysis regionAnalysis;

		[MyCmpReq]
		public KPrefabID kpid;

		[MyCmpReq]
		public Effects effects;

		private int onRoomUpdatedHandle = -1;
	}
}
