using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MinionIdentity : KMonoBehaviour, ISaveLoadable
{
	[Serialize]
	public string originalName { get; set; }

	protected override void OnPrefabInit()
	{
		if (this.name == null)
		{
			this.name = MinionIdentity.ChooseRandomName();
		}
		if (GameClock.Instance != null)
		{
			this.arrivalTime = GameClock.Instance.GetTime();
		}
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.OnUpdateBounds = (Action<Bounds>)Delegate.Combine(component.OnUpdateBounds, new Action<Bounds>(this.OnUpdateBounds));
		this.Subscribe(1623392196, new Action<object>(this.OnDied));
	}

	protected override void OnSpawn()
	{
		this.SetName(this.name);
		if (this.originalName == null)
		{
			this.originalName = this.name;
		}
		if (this.addToIdentityList)
		{
			Components.MinionIdentities.Add(this);
			if (!base.gameObject.HasTag(GameTags.Dead))
			{
				Components.LiveMinionIdentities.Add(this);
			}
		}
		this.raceId = "Human";
		this.bodyType = BodyType.Human;
		Accessorizer component = base.gameObject.GetComponent<Accessorizer>();
		this.bodyData = default(KCompBuilder.BodyData);
		component.GetBodySlots(ref this.bodyData);
		this.headComp = MinionStartingStats.ApplyRace(base.gameObject, MinionResources.Get().races.Get(this.raceId), this.bodyType, this.bodyData);
		FaceGraph component2 = base.GetComponent<FaceGraph>();
		component2.SetHeadComp(this.headComp);
		base.GetComponent<KBatchedAnimController>().AddBuildOverride(this.headComp.GetData(), true, true);
		this.voiceId = "0";
		this.voiceId += (this.voiceIdx + 1).ToString();
		Prioritizable component3 = base.GetComponent<Prioritizable>();
		if (component3 != null)
		{
			component3.showIcon = false;
		}
		CustomGameSettings.SettingLevel currentQualitySetting = Game.Instance.customSettings.GetCurrentQualitySetting("ImmuneSystem");
		if (currentQualitySetting.id == "Weak")
		{
			Db.Get().Amounts.ImmuneLevel.deltaAttribute.Lookup(this).Add(UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.WEAK.ATTRIBUTE_MODIFIER_NAME, new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, -0.008333334f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.WEAK.ATTRIBUTE_MODIFIER_NAME, false, false));
		}
		else if (currentQualitySetting.id == "Strong")
		{
			Db.Get().Amounts.ImmuneLevel.deltaAttribute.Lookup(this).Add(UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.ATTRIBUTE_MODIFIER_NAME, new AttributeModifier(Db.Get().Amounts.ImmuneLevel.deltaAttribute.Id, 0.008333334f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.ATTRIBUTE_MODIFIER_NAME, false, false));
		}
		CustomGameSettings.SettingLevel currentQualitySetting2 = Game.Instance.customSettings.GetCurrentQualitySetting("Stress");
		if (currentQualitySetting2.id == "Pessimistic")
		{
			Db.Get().Amounts.Stress.deltaAttribute.Lookup(this).Add(UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.PESSIMISTIC.ATTRIBUTE_MODIFIER_NAME, new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, 0.016666668f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.PESSIMISTIC.ATTRIBUTE_MODIFIER_NAME, false, false));
		}
		else if (currentQualitySetting2.id == "Optimistic")
		{
			Db.Get().Amounts.Stress.deltaAttribute.Lookup(this).Add(UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.OPTIMISTIC.ATTRIBUTE_MODIFIER_NAME, new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, -0.016666668f, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.OPTIMISTIC.ATTRIBUTE_MODIFIER_NAME, false, false));
		}
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
		Components.MinionIdentities.Remove(this);
		Components.LiveMinionIdentities.Remove(this);
	}

	private void OnUpdateBounds(Bounds bounds)
	{
		BoxCollider2D component = base.GetComponent<BoxCollider2D>();
		component.offset = bounds.center;
		component.size = bounds.extents;
	}

	private void OnDied(object data)
	{
		Components.LiveMinionIdentities.Remove(this);
	}

	[MyCmpReq]
	private KSelectable selectable;

	public int femaleVoiceCount;

	public int maleVoiceCount;

	[Serialize]
	private new string name;

	[Serialize]
	[ReadOnly]
	public float arrivalTime;

	[Serialize]
	public string raceId;

	[Serialize]
	public BodyType bodyType;

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
