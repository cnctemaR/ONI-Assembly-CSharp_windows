using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[SerializationConfig(MemberSerialization.OptIn)]
public class BuildingHP : Workable
{
	public int HitPoints
	{
		get
		{
			return this.hitpoints;
		}
	}

	public void SetHitPoints(int hp)
	{
		this.hitpoints = hp;
	}

	public int MaxHitPoints
	{
		get
		{
			return this.building.Def.HitPoints;
		}
	}

	public BuildingHP.DamageSourceInfo GetDamageSourceInfo()
	{
		return this.damageSourceInfo;
	}

	protected override void OnLoadLevel()
	{
		this.smi = null;
		base.OnLoadLevel();
	}

	public void DoDamage(int damage)
	{
		if (!this.invincible)
		{
			damage = Math.Max(0, damage);
			this.hitpoints = Math.Max(0, this.hitpoints - damage);
			base.Trigger(-1964935036, this);
		}
	}

	public void Repair(int repair_amount)
	{
		if (this.hitpoints + repair_amount < this.hitpoints)
		{
			this.hitpoints = this.building.Def.HitPoints;
		}
		else
		{
			this.hitpoints = Math.Min(this.hitpoints + repair_amount, this.building.Def.HitPoints);
		}
		base.Trigger(-1699355994, null);
		if (this.hitpoints >= this.building.Def.HitPoints)
		{
			base.Trigger(-1735440190, this);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.SetWorkTime(10f);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.smi = new BuildingHP.SMInstance(this);
		this.smi.StartSM();
		base.Subscribe(-794517298, new Action<object>(this.OnDoBuildingDamage));
		if (this.destroyOnDamaged)
		{
			base.Subscribe(774203113, new Action<object>(this.DestroyOnDamaged));
		}
		if (this.hitpoints <= 0)
		{
			base.Trigger(774203113, this);
		}
	}

	private void DestroyOnDamaged(object data)
	{
		Util.KDestroyGameObject(base.gameObject);
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		Workable.AnimInfo anim = base.GetAnim(worker);
		anim.smi = new MultitoolController.Instance(this, worker, "build", EffectPrefabs.Instance.BuildEffect);
		return anim;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		AttributeInstance attributeInstance = Db.Get().Attributes.Machinery.Lookup(worker);
		int num = (int)attributeInstance.GetTotalValue();
		int num2 = 10 + Math.Max(0, num * 10);
		this.Repair(num2);
	}

	private void OnDoBuildingDamage(object data)
	{
		this.damageSourceInfo = (BuildingHP.DamageSourceInfo)data;
		this.DoDamage(this.damageSourceInfo.damage);
		this.DoDamagePopFX(this.damageSourceInfo);
	}

	private void DoDamagePopFX(BuildingHP.DamageSourceInfo info)
	{
		if (info.popString != null && Time.time > this.lastPopTime + this.minDamagePopInterval)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, info.popString, base.gameObject.transform, 1.5f, false);
			this.lastPopTime = Time.time;
		}
	}

	public bool IsBroken
	{
		get
		{
			return this.hitpoints == 0;
		}
	}

	public bool NeedsRepairs
	{
		get
		{
			return this.HitPoints < this.building.Def.HitPoints;
		}
	}

	[Serialize]
	[SerializeField]
	private int hitpoints;

	[Serialize]
	private BuildingHP.DamageSourceInfo damageSourceInfo;

	public static List<Meter> kbacQueryList = new List<Meter>();

	public bool destroyOnDamaged = false;

	public bool invincible = false;

	[MyCmpGet]
	private Building building;

	private BuildingHP.SMInstance smi;

	private float minDamagePopInterval = 4f;

	private float lastPopTime = 0f;

	public struct DamageSourceInfo
	{
		public override string ToString()
		{
			return this.source;
		}

		public int damage;

		public string source;

		public string popString;
	}

	public class SMInstance : GameStateMachine<BuildingHP.States, BuildingHP.SMInstance, BuildingHP, object>.GameInstance
	{
		public SMInstance(BuildingHP master)
			: base(master)
		{
		}

		public Notification CreateBrokenMachineNotification()
		{
			return new Notification(MISC.NOTIFICATIONS.BROKENMACHINE.NAME, NotificationType.BadMinor, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.BROKENMACHINE.TOOLTIP + notificationList.ReduceMessages(false), "/t• " + base.master.damageSourceInfo.source, false, 0f, null, null, null);
		}

		public void ShowProgressBar(bool show)
		{
			if (show && Grid.Visible[Grid.PosToCell(base.gameObject)] > 0)
			{
				this.CreateProgressBar();
			}
			else if (this.progressBar != null)
			{
				this.progressBar.gameObject.DeleteObject();
				this.progressBar = null;
			}
		}

		public void UpdateMeter()
		{
			if (this.progressBar == null)
			{
				this.ShowProgressBar(true);
			}
			this.progressBar.Update();
		}

		private float HealthPercent()
		{
			return (float)base.smi.master.HitPoints / (float)base.smi.master.building.Def.HitPoints;
		}

		private void CreateProgressBar()
		{
			if (!(this.progressBar != null))
			{
				this.progressBar = Util.KInstantiateUI<ProgressBar>(ProgressBarsConfig.Instance.progressBarPrefab, null, false);
				this.progressBar.transform.SetParent(GameScreenManager.Instance.worldSpaceCanvas.transform);
				this.progressBar.name = base.smi.master.name + "." + base.smi.master.GetType().Name + " ProgressBar";
				this.progressBar.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("ProgressBar");
				this.progressBar.SetUpdateFunc(new Func<float>(this.HealthPercent));
				this.progressBar.barColor = ProgressBarsConfig.Instance.GetBarColor("HealthBar");
				CanvasGroup component = this.progressBar.GetComponent<CanvasGroup>();
				component.interactable = false;
				component.blocksRaycasts = false;
				this.progressBar.Update();
				float num = 0.15f;
				Vector3 vector = base.gameObject.transform.position + Vector3.down * num;
				vector.z += 0.05f;
				vector -= Vector3.right * 0.5f * (float)(base.smi.master.building.Def.WidthInCells % 2);
				this.progressBar.transform.SetPosition(vector);
			}
		}

		private static string ToolTipResolver(List<Notification> notificationList, object data)
		{
			string text = "";
			for (int i = 0; i < notificationList.Count; i++)
			{
				Notification notification = notificationList[i];
				text += string.Format(BUILDINGS.DAMAGESOURCES.NOTIFICATION_TOOLTIP, notification.NotifierName, (string)notification.tooltipData);
				if (i < notificationList.Count - 1)
				{
					text += "\n";
				}
			}
			return text;
		}

		public void ShowDamagedEffect()
		{
			BuildingDef def = base.master.building.Def;
			if (def.RequiresPowerInput || def.RequiresPowerOutput || def.GeneratorWattageRating > 0f)
			{
				int num = Grid.PosToCell(base.master);
				int num2 = Grid.OffsetCell(num, def.WidthInCells - 1, def.HeightInCells - 1);
				Game.Instance.SpawnFX(SpawnFXHashes.BuildingSpark, num2, 0f);
			}
		}

		public FXAnim.Instance InstantiateSmokeDamageFX()
		{
			BuildingDef def = base.master.GetComponent<BuildingComplete>().Def;
			Vector3 vector = new Vector3((float)(def.WidthInCells - 1), (float)(def.HeightInCells - 1), 0f);
			return new FXAnim.Instance(base.smi.master, "smoke_damage_kanim", "idle", KAnim.PlayMode.Loop, vector, Lighting.Instance.Settings.SmokeDamageTint);
		}

		public void SetCrackOverlayValue(float value)
		{
			KBatchedAnimController component = base.master.GetComponent<KBatchedAnimController>();
			if (!(component == null))
			{
				component.SetBlendValue(value);
				BuildingHP.kbacQueryList.Clear();
				base.master.GetComponentsInChildren<Meter>(BuildingHP.kbacQueryList);
				for (int i = 0; i < BuildingHP.kbacQueryList.Count; i++)
				{
					Meter meter = BuildingHP.kbacQueryList[i];
					KBatchedAnimController component2 = meter.GetComponent<KBatchedAnimController>();
					component2.SetBlendValue(value);
				}
			}
		}

		private ProgressBar progressBar = null;
	}

	public class States : GameStateMachine<BuildingHP.States, BuildingHP.SMInstance, BuildingHP>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = true;
			default_state = this.healthy;
			this.healthy.DefaultState(this.healthy.imperfect).EventTransition(GameHashes.BuildingReceivedDamage, this.damaged, (BuildingHP.SMInstance smi) => smi.master.HitPoints <= 0);
			this.healthy.imperfect.Enter(delegate(BuildingHP.SMInstance smi)
			{
				smi.ShowProgressBar(true);
			}).DefaultState(this.healthy.imperfect.playEffect).EventTransition(GameHashes.BuildingPartiallyRepaired, this.healthy.perfect, (BuildingHP.SMInstance smi) => smi.master.HitPoints == smi.master.building.Def.HitPoints)
				.EventHandler(GameHashes.BuildingPartiallyRepaired, delegate(BuildingHP.SMInstance smi)
				{
					smi.UpdateMeter();
				})
				.Exit(delegate(BuildingHP.SMInstance smi)
				{
					smi.ShowProgressBar(false);
				});
			this.healthy.imperfect.playEffect.Transition(this.healthy.imperfect.waiting, (BuildingHP.SMInstance smi) => true);
			this.healthy.imperfect.waiting.ScheduleGoTo((BuildingHP.SMInstance smi) => global::UnityEngine.Random.Range(15f, 30f), this.healthy.imperfect.playEffect);
			this.healthy.perfect.EventTransition(GameHashes.BuildingReceivedDamage, this.healthy.imperfect, (BuildingHP.SMInstance smi) => smi.master.HitPoints < smi.master.building.Def.HitPoints);
			this.damaged.Enter(delegate(BuildingHP.SMInstance smi)
			{
				Operational component = smi.GetComponent<Operational>();
				if (component != null)
				{
					component.SetFlag(BuildingHP.States.healthyFlag, false);
				}
				smi.ShowProgressBar(true);
				smi.master.Trigger(774203113, smi.master);
				smi.SetCrackOverlayValue(1f);
			}).ToggleNotification((BuildingHP.SMInstance smi) => smi.CreateBrokenMachineNotification()).ToggleStatusItem(Db.Get().BuildingStatusItems.Broken, null)
				.EventTransition(GameHashes.BuildingPartiallyRepaired, this.healthy.perfect, (BuildingHP.SMInstance smi) => smi.master.HitPoints == smi.master.building.Def.HitPoints)
				.EventHandler(GameHashes.BuildingPartiallyRepaired, delegate(BuildingHP.SMInstance smi)
				{
					smi.UpdateMeter();
				})
				.Exit(delegate(BuildingHP.SMInstance smi)
				{
					Operational component2 = smi.GetComponent<Operational>();
					if (component2 != null)
					{
						component2.SetFlag(BuildingHP.States.healthyFlag, true);
					}
					smi.ShowProgressBar(false);
					smi.SetCrackOverlayValue(0f);
				});
		}

		private Chore CreateRepairChore(BuildingHP.SMInstance smi)
		{
			return new WorkChore<BuildingHP>(Db.Get().ChoreTypes.Repair, smi.master, null, true, null, null, null, true, null, false, default(Tag), null, false, true, true, PriorityScreen.PriorityClass.basic, int.MaxValue);
		}

		private static Operational.Flag healthyFlag = new Operational.Flag("healthy", Operational.Flag.Type.Functional);

		public GameStateMachine<BuildingHP.States, BuildingHP.SMInstance, BuildingHP, object>.State damaged;

		public BuildingHP.States.Healthy healthy;

		public class Healthy : GameStateMachine<BuildingHP.States, BuildingHP.SMInstance, BuildingHP, object>.State
		{
			public BuildingHP.States.ImperfectStates imperfect;

			public GameStateMachine<BuildingHP.States, BuildingHP.SMInstance, BuildingHP, object>.State perfect;
		}

		public class ImperfectStates : GameStateMachine<BuildingHP.States, BuildingHP.SMInstance, BuildingHP, object>.State
		{
			public GameStateMachine<BuildingHP.States, BuildingHP.SMInstance, BuildingHP, object>.State playEffect;

			public GameStateMachine<BuildingHP.States, BuildingHP.SMInstance, BuildingHP, object>.State waiting;
		}
	}
}
