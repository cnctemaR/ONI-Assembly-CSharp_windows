using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Database;
using Klei.AI;
using KSerialization;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Artable")]
public class Artable : Workable
{
	public string CurrentStage
	{
		get
		{
			return this.currentStage;
		}
	}

	protected Artable()
	{
		this.faceTargetWhenWorking = true;
		if (string.IsNullOrEmpty(this.requiredSkillPerk))
		{
			this.requiredSkillPerk = Db.Get().SkillPerks.CanArt.Id;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Arting;
		this.attributeConverter = Db.Get().AttributeConverters.ArtSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Art.Id;
		this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		base.SetWorkTime(80f);
	}

	protected override void OnSpawn()
	{
		base.GetComponent<KPrefabID>().PrefabID();
		if (string.IsNullOrEmpty(this.currentStage) || this.currentStage == "Default")
		{
			this.SetDefault();
		}
		else
		{
			this.SetStage(this.currentStage, true);
		}
		this.shouldShowSkillPerkStatusItem = false;
		base.OnSpawn();
	}

	[OnDeserialized]
	public void OnDeserialized()
	{
		if (Db.GetArtableStages().TryGet(this.currentStage) == null && this.currentStage != "Default")
		{
			string text = string.Format("{0}_{1}", base.GetComponent<KPrefabID>().PrefabID().ToString(), this.currentStage);
			if (Db.GetArtableStages().TryGet(text) == null)
			{
				global::Debug.LogWarning("Failed up to update " + this.currentStage + " to ArtableStages");
				this.currentStage = "Default";
				return;
			}
			this.currentStage = text;
		}
	}

	protected override void OnCompleteWork(WorkerBase worker)
	{
		if (string.IsNullOrEmpty(this.userChosenTargetStage))
		{
			Db db = Db.Get();
			Tag tag = base.GetComponent<KPrefabID>().PrefabID();
			List<ArtableStage> prefabStages = Db.GetArtableStages().GetPrefabStages(tag);
			ArtableStatusItem artist_skill = db.ArtableStatuses.LookingUgly;
			MinionResume component = worker.GetComponent<MinionResume>();
			if (component != null)
			{
				if (component.HasPerk(db.SkillPerks.CanArtGreat.Id))
				{
					artist_skill = db.ArtableStatuses.LookingGreat;
				}
				else if (component.HasPerk(db.SkillPerks.CanArtOkay.Id))
				{
					artist_skill = db.ArtableStatuses.LookingOkay;
				}
			}
			prefabStages.RemoveAll((ArtableStage stage) => stage.statusItem.StatusType > artist_skill.StatusType || stage.statusItem.StatusType == ArtableStatuses.ArtableStatusType.AwaitingArting);
			prefabStages.Sort((ArtableStage x, ArtableStage y) => y.statusItem.StatusType.CompareTo(x.statusItem.StatusType));
			ArtableStatuses.ArtableStatusType highest_type = prefabStages[0].statusItem.StatusType;
			prefabStages.RemoveAll((ArtableStage stage) => stage.statusItem.StatusType < highest_type);
			prefabStages.RemoveAll((ArtableStage stage) => !stage.IsUnlocked());
			prefabStages.Shuffle<ArtableStage>();
			this.SetStage(prefabStages[0].id, false);
			if (prefabStages[0].cheerOnComplete)
			{
				new EmoteChore(worker.GetComponent<ChoreProvider>(), db.ChoreTypes.EmoteHighPriority, db.Emotes.Minion.Cheer, 1, null);
			}
			else
			{
				new EmoteChore(worker.GetComponent<ChoreProvider>(), db.ChoreTypes.EmoteHighPriority, db.Emotes.Minion.Disappointed, 1, null);
			}
		}
		else
		{
			this.SetStage(this.userChosenTargetStage, false);
			this.userChosenTargetStage = null;
		}
		this.shouldShowSkillPerkStatusItem = false;
		this.UpdateStatusItem(null);
		Prioritizable.RemoveRef(base.gameObject);
	}

	public void SetDefault()
	{
		this.currentStage = "Default";
		base.GetComponent<KBatchedAnimController>().SwapAnims(base.GetComponent<Building>().Def.AnimFiles);
		base.GetComponent<KAnimControllerBase>().Play(this.defaultAnimName, KAnim.PlayMode.Once, 1f, 0f);
		KSelectable component = base.GetComponent<KSelectable>();
		BuildingDef def = base.GetComponent<Building>().Def;
		component.SetName(def.Name);
		component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().ArtableStatuses.AwaitingArting, this);
		this.GetAttributes().Remove(this.artQualityDecorModifier);
		this.shouldShowSkillPerkStatusItem = false;
		this.UpdateStatusItem(null);
		if (this.currentStage == "Default")
		{
			this.shouldShowSkillPerkStatusItem = true;
			Prioritizable.AddRef(base.gameObject);
			this.chore = new WorkChore<Artable>(Db.Get().ChoreTypes.Art, this, null, true, null, null, null, true, null, false, this.onlyWorkableWhenOperational, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			this.chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, this.requiredSkillPerk);
		}
		this.RefreshDecorTag();
		base.Trigger(111068960, this.currentStage);
	}

	public virtual void SetStage(string stage_id, bool skip_effect)
	{
		ArtableStage artableStage = Db.GetArtableStages().Get(stage_id);
		if (artableStage == null)
		{
			global::Debug.LogError("Missing stage: " + stage_id);
			return;
		}
		this.currentStage = artableStage.id;
		base.GetComponent<KBatchedAnimController>().SwapAnims(new KAnimFile[] { Assets.GetAnim(artableStage.animFile) });
		base.GetComponent<KAnimControllerBase>().Play(artableStage.anim, KAnim.PlayMode.Once, 1f, 0f);
		this.GetAttributes().Remove(this.artQualityDecorModifier);
		if (artableStage.decor != 0)
		{
			this.artQualityDecorModifier = new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, (float)artableStage.decor, "Art Quality", false, false, true);
			this.GetAttributes().Add(this.artQualityDecorModifier);
		}
		KSelectable component = base.GetComponent<KSelectable>();
		component.SetName(artableStage.Name);
		component.SetStatusItem(Db.Get().StatusItemCategories.Main, artableStage.statusItem, this);
		base.gameObject.GetComponent<BuildingComplete>().SetDescriptionFlavour(artableStage.Description);
		this.shouldShowSkillPerkStatusItem = false;
		this.UpdateStatusItem(null);
		this.RefreshDecorTag();
		base.Trigger(111068960, this.currentStage);
	}

	public void SetUserChosenTargetState(string stageID)
	{
		this.SetDefault();
		this.userChosenTargetStage = stageID;
	}

	public void RefreshDecorTag()
	{
		KPrefabID component = base.GetComponent<KPrefabID>();
		bool flag = component.HasTag(GameTags.Decoration);
		bool flag2 = this.CurrentStage != null && this.currentStage != "Default";
		if (flag2)
		{
			component.AddTag(GameTags.Decoration, false);
		}
		else
		{
			component.RemoveTag(GameTags.Decoration);
		}
		if (flag2 != flag)
		{
			Game.Instance.roomProber.TriggerBuildingChangedEvent(Grid.PosToCell(base.gameObject), component);
		}
	}

	[Serialize]
	private string currentStage;

	[Serialize]
	private string userChosenTargetStage;

	private AttributeModifier artQualityDecorModifier;

	public const string defaultArtworkId = "Default";

	public string defaultAnimName;

	public bool onlyWorkableWhenOperational = true;

	private WorkChore<Artable> chore;
}
