using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using TUNING;
using UnityEngine;

public class Artable : Workable
{
	protected Artable()
	{
		this.faceTargetWhenWorking = true;
		this.statuses = new Dictionary<Artable.Status, StatusItem>();
	}

	public string CurrentStage
	{
		get
		{
			return this.currentStage;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.statuses[Artable.Status.Ready] = new StatusItem("AwaitingArting", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
		this.statuses[Artable.Status.Ugly] = new StatusItem("LookingUgly", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
		this.statuses[Artable.Status.Okay] = new StatusItem("LookingOkay", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
		this.statuses[Artable.Status.Great] = new StatusItem("LookingGreat", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Arting;
		this.attributeConverter = Db.Get().AttributeConverters.ArtSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Art.Id;
		this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		this.requiredSkillPerk = Db.Get().SkillPerks.CanArt.Id;
		base.SetWorkTime(80f);
	}

	protected override void OnSpawn()
	{
		if (string.IsNullOrEmpty(this.currentStage))
		{
			this.currentStage = "Default";
		}
		this.SetStage(this.currentStage, true);
		this.shouldShowSkillPerkStatusItem = false;
		if (this.currentStage == "Default")
		{
			this.shouldShowSkillPerkStatusItem = true;
			Prioritizable.AddRef(base.gameObject);
			this.chore = new WorkChore<Artable>(Db.Get().ChoreTypes.Art, this, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			this.chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, this.requiredSkillPerk);
		}
		base.OnSpawn();
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Artable.Status artist_skill = Artable.Status.Ugly;
		MinionResume component = worker.GetComponent<MinionResume>();
		if (component != null)
		{
			if (component.HasPerk(Db.Get().SkillPerks.CanArtGreat.Id))
			{
				artist_skill = Artable.Status.Great;
			}
			else if (component.HasPerk(Db.Get().SkillPerks.CanArtOkay.Id))
			{
				artist_skill = Artable.Status.Okay;
			}
		}
		List<Artable.Stage> potential_stages = new List<Artable.Stage>();
		this.stages.ForEach(delegate(Artable.Stage item)
		{
			potential_stages.Add(item);
		});
		potential_stages.RemoveAll((Artable.Stage x) => x.statusItem > artist_skill || x.id == "Default");
		potential_stages.Sort((Artable.Stage x, Artable.Stage y) => y.statusItem.CompareTo(x.statusItem));
		Artable.Status highest_status = potential_stages[0].statusItem;
		potential_stages.RemoveAll((Artable.Stage x) => x.statusItem < highest_status);
		potential_stages.Shuffle<Artable.Stage>();
		this.SetStage(potential_stages[0].id, false);
		if (potential_stages[0].cheerOnComplete)
		{
			new EmoteChore(worker.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_cheer_kanim", new HashedString[] { "cheer_pre", "cheer_loop", "cheer_pst" }, null);
		}
		else
		{
			new EmoteChore(worker.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_disappointed_kanim", new HashedString[] { "disappointed_pre", "disappointed_loop", "disappointed_pst" }, null);
		}
		this.shouldShowSkillPerkStatusItem = false;
		this.UpdateStatusItem(null);
		Prioritizable.RemoveRef(base.gameObject);
	}

	public virtual void SetStage(string stage_id, bool skip_effect)
	{
		Artable.Stage stage = null;
		for (int i = 0; i < this.stages.Count; i++)
		{
			if (this.stages[i].id == stage_id)
			{
				stage = this.stages[i];
				break;
			}
		}
		if (stage == null)
		{
			global::Debug.LogError("Missing stage: " + stage_id);
		}
		else
		{
			this.currentStage = stage.id;
			base.GetComponent<KAnimControllerBase>().Play(stage.anim, KAnim.PlayMode.Once, 1f, 0f);
			if (stage.decor != 0)
			{
				AttributeModifier attributeModifier = new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, (float)stage.decor, "Art Quality", false, false, true);
				this.GetAttributes().Add(attributeModifier);
			}
			KSelectable component = base.GetComponent<KSelectable>();
			component.SetName(stage.name);
			component.SetStatusItem(Db.Get().StatusItemCategories.Main, this.statuses[stage.statusItem], this);
			this.shouldShowSkillPerkStatusItem = false;
			this.UpdateStatusItem(null);
		}
	}

	private Dictionary<Artable.Status, StatusItem> statuses;

	[SerializeField]
	public List<Artable.Stage> stages = new List<Artable.Stage>();

	[Serialize]
	private string currentStage;

	private WorkChore<Artable> chore;

	[Serializable]
	public class Stage
	{
		public Stage(string id, string name, string anim, int decor_value, bool cheer_on_complete, Artable.Status status_item)
		{
			this.id = id;
			this.name = name;
			this.anim = anim;
			this.decor = decor_value;
			this.cheerOnComplete = cheer_on_complete;
			this.statusItem = status_item;
		}

		public string id;

		public string name;

		public string anim;

		public int decor;

		public bool cheerOnComplete;

		public Artable.Status statusItem;
	}

	public enum Status
	{
		Ready,
		Ugly,
		Okay,
		Great
	}
}
