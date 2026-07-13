using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

public class BeckoningMonitor : GameStateMachine<BeckoningMonitor, BeckoningMonitor.Instance, IStateMachineTarget, BeckoningMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.EventHandler(GameHashes.CaloriesConsumed, delegate(BeckoningMonitor.Instance smi, object data)
		{
			smi.OnCaloriesConsumed(data);
		}).ToggleBehaviour(GameTags.Creatures.WantsToBeckon, (BeckoningMonitor.Instance smi) => smi.IsReadyToBeckon(), null).Update(delegate(BeckoningMonitor.Instance smi, float dt)
		{
			smi.UpdateBlockedStatusItem();
		}, UpdateRate.SIM_1000ms, false);
	}

	[Serializable]
	public class SongChance
	{
		public Tag meteorID;

		public string singAnimPre;

		public string singAnimLoop;

		public string singAnimPst;

		public float weight;
	}

	public class Def : StateMachine.BaseDef
	{
		public override void Configure(GameObject prefab)
		{
			prefab.AddOrGet<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Beckoning.Id);
		}

		public List<BeckoningMonitor.SongChance> initialSongWeights;

		public float caloriesPerCycle;

		public string effectId = "MooWellFed";
	}

	public new class Instance : GameStateMachine<BeckoningMonitor, BeckoningMonitor.Instance, IStateMachineTarget, BeckoningMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, BeckoningMonitor.Def def)
			: base(master, def)
		{
			this.beckoning = Db.Get().Amounts.Beckoning.Lookup(base.gameObject);
			this.InitializSongChances();
		}

		private void InitializSongChances()
		{
			this.songChances = new List<BeckoningMonitor.SongChance>();
			if (base.def.initialSongWeights != null)
			{
				foreach (BeckoningMonitor.SongChance songChance in base.def.initialSongWeights)
				{
					this.songChances.Add(new BeckoningMonitor.SongChance
					{
						meteorID = songChance.meteorID,
						weight = songChance.weight,
						singAnimPre = songChance.singAnimPre,
						singAnimLoop = songChance.singAnimLoop,
						singAnimPst = songChance.singAnimPst
					});
					foreach (MooSongModifier mooSongModifier in Db.Get().MooSongModifiers.GetForTag(songChance.meteorID))
					{
						mooSongModifier.ApplyFunction(this, songChance.meteorID);
					}
				}
				this.NormalizeSongsChances();
			}
		}

		public void AddSongChance(Tag type, float addedPercentChance)
		{
			foreach (BeckoningMonitor.SongChance songChance in this.songChances)
			{
				if (songChance.meteorID == type)
				{
					float num = Mathf.Min(1f - songChance.weight, Mathf.Max(0f - songChance.weight, addedPercentChance));
					songChance.weight += num;
				}
			}
			this.NormalizeSongsChances();
			base.master.Trigger(1105317911, this.songChances);
		}

		public void NormalizeSongsChances()
		{
			float num = 0f;
			foreach (BeckoningMonitor.SongChance songChance in this.songChances)
			{
				num += songChance.weight;
			}
			foreach (BeckoningMonitor.SongChance songChance2 in this.songChances)
			{
				songChance2.weight /= num;
			}
		}

		private bool IsSpaceVisible()
		{
			int num = Grid.PosToCell(this);
			return Grid.IsValidCell(num) && Grid.ExposedToSunlight[num] > 0;
		}

		private bool IsBeckoningAvailable()
		{
			return base.smi.beckoning.value >= base.smi.beckoning.GetMax();
		}

		public bool IsReadyToBeckon()
		{
			return this.IsBeckoningAvailable() && this.IsSpaceVisible();
		}

		public void UpdateBlockedStatusItem()
		{
			bool flag = this.IsSpaceVisible();
			if (!flag && this.IsBeckoningAvailable() && this.beckoningBlockedHandle == Guid.Empty)
			{
				this.beckoningBlockedHandle = this.kselectable.AddStatusItem(Db.Get().CreatureStatusItems.BeckoningBlocked, null);
				return;
			}
			if (flag)
			{
				this.beckoningBlockedHandle = this.kselectable.RemoveStatusItem(this.beckoningBlockedHandle, false);
			}
		}

		public void OnCaloriesConsumed(object data)
		{
			CreatureCalorieMonitor.CaloriesConsumedEvent value = ((Boxed<CreatureCalorieMonitor.CaloriesConsumedEvent>)data).value;
			EffectInstance effectInstance = this.effects.Get(base.smi.def.effectId);
			if (effectInstance == null)
			{
				effectInstance = this.effects.Add(base.smi.def.effectId, true);
			}
			effectInstance.timeRemaining += value.calories / base.smi.def.caloriesPerCycle * 600f;
		}

		private AmountInstance beckoning;

		[Serialize]
		public List<BeckoningMonitor.SongChance> songChances;

		[MyCmpGet]
		private Effects effects;

		[MyCmpGet]
		public KSelectable kselectable;

		private Guid beckoningBlockedHandle;
	}
}
