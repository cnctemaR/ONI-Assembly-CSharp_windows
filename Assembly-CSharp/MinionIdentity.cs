using System;
using System.Collections.Generic;
using Klei.AI;
using Klei.CustomSettings;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MinionIdentity : KMonoBehaviour, ISaveLoadable, IAssignableIdentity, IListableOption
{
	[Serialize]
	public string genderStringKey { get; set; }

	[Serialize]
	public string nameStringKey { get; set; }

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
		this.headComp = MinionStartingStats.ApplyRace(base.gameObject, this.bodyData);
		FaceGraph component3 = base.GetComponent<FaceGraph>();
		component3.SetHeadComp(this.headComp);
		base.GetComponent<KBatchedAnimController>().AddBuildOverride(this.headComp.GetData(), true, true);
		base.GetComponent<KBatchedAnimController>().RemoveVisibleSymbol(KCompBuilder.snapTo_hat);
		base.GetComponent<KBatchedAnimController>().RemoveVisibleSymbol(KCompBuilder.snapTo_hat_hair);
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
		SettingLevel currentQualitySetting = Game.Instance.customSettings.GetCurrentQualitySetting("ImmuneSystem");
		if (currentQualitySetting.id == "Compromised")
		{
			Db.Get().Amounts.ImmuneLevel.deltaAttribute.Lookup(this).Add(UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.COMPROMISED.ATTRIBUTE_MODIFIER_NAME, new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, -0.025f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.COMPROMISED.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting.id == "Weak")
		{
			Db.Get().Amounts.ImmuneLevel.deltaAttribute.Lookup(this).Add(UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.WEAK.ATTRIBUTE_MODIFIER_NAME, new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, -0.008333334f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.WEAK.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting.id == "Strong")
		{
			Db.Get().Amounts.ImmuneLevel.deltaAttribute.Lookup(this).Add(UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.ATTRIBUTE_MODIFIER_NAME, new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, 0.008333334f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting.id == "Invincible")
		{
			Db.Get().Amounts.ImmuneLevel.deltaAttribute.Lookup(this).Add(UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.INVINCIBLE.ATTRIBUTE_MODIFIER_NAME, new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, float.PositiveInfinity, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.INVINCIBLE.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		SettingLevel currentQualitySetting2 = Game.Instance.customSettings.GetCurrentQualitySetting("Stress");
		if (currentQualitySetting2.id == "Doomed")
		{
			Db.Get().Amounts.Stress.deltaAttribute.Lookup(this).Add(UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.DOOMED.ATTRIBUTE_MODIFIER_NAME, new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.033333335f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.DOOMED.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting2.id == "Pessimistic")
		{
			Db.Get().Amounts.Stress.deltaAttribute.Lookup(this).Add(UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.PESSIMISTIC.ATTRIBUTE_MODIFIER_NAME, new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.016666668f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.PESSIMISTIC.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting2.id == "Optimistic")
		{
			Db.Get().Amounts.Stress.deltaAttribute.Lookup(this).Add(UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.OPTIMISTIC.ATTRIBUTE_MODIFIER_NAME, new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, -0.016666668f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.OPTIMISTIC.ATTRIBUTE_MODIFIER_NAME, false, false, true));
		}
		else if (currentQualitySetting2.id == "Indomitable")
		{
			Db.Get().Amounts.Stress.deltaAttribute.Lookup(this).Add(UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.INDOMITABLE.ATTRIBUTE_MODIFIER_NAME, new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, float.NegativeInfinity, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.INDOMITABLE.ATTRIBUTE_MODIFIER_NAME, false, false, true));
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

	[ContextMenu("TestHat")]
	public void TestHat()
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		AccessorySlot hat = Db.Get().AccessorySlots.Hat;
		MinionIdentity.testIdx = (MinionIdentity.testIdx + 1) % hat.accessories.Count;
		component.AddSymbolOverride(hat.targetSymbolId, hat.accessories[MinionIdentity.testIdx].symbol.build.batchTag, hat.accessories[MinionIdentity.testIdx].symbol, false);
		component.ShowSymbol(hat.targetSymbolId);
		AccessorySlot hair = Db.Get().AccessorySlots.Hair;
		AccessorySlot hairAlways = Db.Get().AccessorySlots.HairAlways;
		component.AddSymbolOverride(hair.targetSymbolId, hair.accessories[1].symbol.build.batchTag, hair.accessories[1].symbol, false);
		component.ShowSymbol(hair.targetSymbolId);
		component.AddSymbolOverride(hairAlways.targetSymbolId, hairAlways.accessories[1].symbol.build.batchTag, hairAlways.accessories[1].symbol, false);
		component.ShowSymbol(hairAlways.targetSymbolId);
		AccessorySlot hatHair = Db.Get().AccessorySlots.HatHair;
		component.AddSymbolOverride(hatHair.targetSymbolId, hatHair.accessories[1].symbol.build.batchTag, hatHair.accessories[1].symbol, false);
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

	private KCompBuildInstance headComp;

	private KAnimHashedString overrideExpression;

	private KAnimHashedString expression;

	public bool addToIdentityList = true;

	private static MinionIdentity.NameList maleNameList;

	private static MinionIdentity.NameList femaleNameList;

	private static int testIdx;

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
