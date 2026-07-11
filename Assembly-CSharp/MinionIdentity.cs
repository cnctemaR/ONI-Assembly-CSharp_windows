using System;
using System.Collections.Generic;
using Klei.AI;
using Klei.CustomSettings;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MinionIdentity : KMonoBehaviour, ISaveLoadable, IAssignableIdentity, IListableOption, ISim1000ms
{
	[Serialize]
	public string genderStringKey { get; set; }

	[Serialize]
	public string nameStringKey { get; set; }

	public static void DestroyStatics()
	{
		MinionIdentity.maleNameList = null;
		MinionIdentity.femaleNameList = null;
	}

	protected override void OnPrefabInit()
	{
		if (this.name == null)
		{
			this.name = MinionIdentity.ChooseRandomName();
		}
		if (GameClock.Instance != null)
		{
			this.arrivalTime = (float)GameClock.Instance.GetCycle();
		}
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.OnUpdateBounds = (Action<Bounds>)Delegate.Combine(component.OnUpdateBounds, new Action<Bounds>(this.OnUpdateBounds));
		base.Subscribe(1623392196, new Action<object>(this.OnDied));
	}

	protected override void OnSpawn()
	{
		PathProber component = base.GetComponent<PathProber>();
		if (component != null)
		{
			component.SetGroupProber(MinionGroupProber.Get());
		}
		this.SetName(this.name);
		if (this.nameStringKey == null)
		{
			this.nameStringKey = this.name;
		}
		this.SetGender(this.gender);
		if (this.genderStringKey == null)
		{
			this.genderStringKey = "NB";
		}
		if (this.addToIdentityList)
		{
			Components.MinionIdentities.Add(this);
			if (!base.gameObject.HasTag(GameTags.Dead))
			{
				Components.LiveMinionIdentities.Add(this);
			}
			Game.Instance.assignmentManager.AddToAssignmentGroup("public", this);
		}
		Accessorizer component2 = base.gameObject.GetComponent<Accessorizer>();
		this.bodyData = default(KCompBuilder.BodyData);
		component2.GetBodySlots(ref this.bodyData);
		SymbolOverrideController component3 = base.GetComponent<SymbolOverrideController>();
		component3.AddSymbolOverride(Db.Get().AccessorySlots.HairAlways.targetSymbolId, component2.GetAccessory(Db.Get().AccessorySlots.Hair).symbol, 1);
		component3.AddSymbolOverride(Db.Get().AccessorySlots.HatHair.targetSymbolId, Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(component2.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol, 1);
		this.voiceId = "0";
		this.voiceId += (this.voiceIdx + 1).ToString();
		Prioritizable component4 = base.GetComponent<Prioritizable>();
		if (component4 != null)
		{
			component4.showIcon = false;
		}
		Pickupable component5 = base.GetComponent<Pickupable>();
		if (component5 != null)
		{
			component5.carryAnimOverride = Assets.GetAnim("anim_incapacitated_carrier_kanim");
		}
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting("ImmuneSystem");
		if (currentQualitySetting.id == "Compromised")
		{
			Db.Get().Amounts.ImmuneLevel.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, -0.025f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.COMPROMISED.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting.id == "Weak")
		{
			Db.Get().Amounts.ImmuneLevel.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, -0.008333334f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.WEAK.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting.id == "Strong")
		{
			Db.Get().Amounts.ImmuneLevel.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, 0.008333334f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting.id == "Invincible")
		{
			Db.Get().Amounts.ImmuneLevel.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, float.PositiveInfinity, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.INVINCIBLE.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		SettingLevel currentQualitySetting2 = CustomGameSettings.Instance.GetCurrentQualitySetting("Stress");
		if (currentQualitySetting2.id == "Doomed")
		{
			Db.Get().Amounts.Stress.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.033333335f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.DOOMED.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting2.id == "Pessimistic")
		{
			Db.Get().Amounts.Stress.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.016666668f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.PESSIMISTIC.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting2.id == "Optimistic")
		{
			Db.Get().Amounts.Stress.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, -0.016666668f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.OPTIMISTIC.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting2.id == "Indomitable")
		{
			Db.Get().Amounts.Stress.deltaAttribute.Lookup(this).Add(new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, float.NegativeInfinity, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.INDOMITABLE.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
	}

	public string GetProperName()
	{
		return base.gameObject.GetProperName();
	}

	public string GetVoiceId()
	{
		return this.voiceId;
	}

	public void SetName(string name)
	{
		this.name = name;
		this.selectable.SetName(name);
		base.gameObject.name = name;
		NameDisplayScreen.Instance.UpdateName(base.gameObject);
	}

	public void SetGender(string gender)
	{
		this.gender = gender;
		this.selectable.SetGender(gender);
	}

	public static string ChooseRandomName()
	{
		if (MinionIdentity.femaleNameList == null)
		{
			MinionIdentity.maleNameList = new MinionIdentity.NameList(Game.Instance.maleNamesFile);
			MinionIdentity.femaleNameList = new MinionIdentity.NameList(Game.Instance.femaleNamesFile);
		}
		if (global::UnityEngine.Random.value > 0.5f)
		{
			return MinionIdentity.maleNameList.Next();
		}
		return MinionIdentity.femaleNameList.Next();
	}

	protected override void OnCleanUp()
	{
		Game.Instance.assignmentManager.RemoveFromAllGroups(this);
		Components.MinionIdentities.Remove(this);
		Components.LiveMinionIdentities.Remove(this);
	}

	private void OnUpdateBounds(Bounds bounds)
	{
		KBoxCollider2D component = base.GetComponent<KBoxCollider2D>();
		component.offset = bounds.center;
		component.size = bounds.extents;
	}

	private void OnDied(object data)
	{
		Ownables component = base.GetComponent<Ownables>();
		component.UnassignAll();
		Equipment component2 = base.GetComponent<Equipment>();
		component2.UnequipAll();
		Components.LiveMinionIdentities.Remove(this);
	}

	public List<Ownables> GetOwners()
	{
		return new List<Ownables> { base.GetComponent<Ownables>() };
	}

	public Ownables GetSoleOwner()
	{
		return base.GetComponent<Ownables>();
	}

	public void Sim1000ms(float dt)
	{
		if (this == null)
		{
			return;
		}
		if (!base.GetComponent<Navigator>().IsMoving())
		{
			return;
		}
		Chore currentChore = base.GetComponent<ChoreDriver>().GetCurrentChore();
		if (currentChore != null)
		{
			ReportManager.Instance.ReportValue(ReportManager.ReportType.TravelTime, dt, currentChore.choreType.Name, currentChore.driver.GetProperName());
			if (currentChore is FetchAreaChore)
			{
				MinionResume component = base.GetComponent<MinionResume>();
				if (component != null)
				{
					component.AddExperienceIfRole("Hauler", dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
					component.AddExperienceIfRole(MaterialsManager.ID, dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
					component.AddExperienceIfRole(Handyman.ID, dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
				}
			}
		}
	}

	[MyCmpReq]
	private KSelectable selectable;

	public int femaleVoiceCount;

	public int maleVoiceCount;

	[Serialize]
	private new string name;

	[Serialize]
	public string gender;

	[Serialize]
	[ReadOnly]
	public float arrivalTime;

	[Serialize]
	public int voiceIdx;

	[Serialize]
	public KCompBuilder.BodyData bodyData;

	public float timeLastSpoke;

	private string voiceId;

	private KAnimHashedString overrideExpression;

	private KAnimHashedString expression;

	public bool addToIdentityList = true;

	private static MinionIdentity.NameList maleNameList;

	private static MinionIdentity.NameList femaleNameList;

	private class NameList
	{
		public NameList(TextAsset file)
		{
			string[] array = file.text.Replace("  ", " ").Replace("\r\n", "\n").Split(new char[] { '\n' });
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[] { ' ' });
				if (array2[array2.Length - 1] != string.Empty && array2[array2.Length - 1] != null)
				{
					this.names.Add(array2[array2.Length - 1]);
				}
			}
			this.names.Shuffle<string>();
		}

		public string Next()
		{
			return this.names[this.idx++ % this.names.Count];
		}

		private List<string> names = new List<string>();

		private int idx;
	}
}
