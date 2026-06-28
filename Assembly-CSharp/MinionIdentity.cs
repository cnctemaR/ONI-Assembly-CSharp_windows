using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MinionIdentity : KMonoBehaviour, ISaveLoadableJson
{
	protected override void OnPrefabInit()
	{
		if (this.name == null)
		{
			this.name = MinionIdentity.ChooseRandomName(false);
		}
		if (GameClock.Instance != null)
		{
			this.arrivalTime = GameClock.Instance.GetTime();
		}
		KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
		component.OnUpdateBounds = (Action<Bounds>)Delegate.Combine(component.OnUpdateBounds, new Action<Bounds>(this.OnUpdateBounds));
		this.Subscribe(1623392196, new EventSystem.EventHandler(this.OnDied));
	}

	protected override void OnSpawn()
	{
		this.SetName(this.name);
		if (this.addToIdentityList)
		{
			Components.MinionIdentities.Add(this);
		}
		if (base.GetComponent<Health>() != null && !base.GetComponent<Health>().IsDead())
		{
			Components.LiveMinionIdentities.Add(this);
		}
		this.raceId = "Human";
		this.bodyType = BodyType.Human;
		if (this.raceId != null && this.raceId != string.Empty)
		{
			Accessorizer component = base.gameObject.GetComponent<Accessorizer>();
			this.faceData = default(Accessorizer.FaceData);
			component.GetFaceSlots(ref this.faceData);
			this.headComp = MinionStartingStats.ApplyRace(base.gameObject, MinionResources.Get().races.Get(this.raceId), this.bodyType, this.faceData, this.isMale);
			base.GetComponent<FaceGraph>().SetHeadComp(this.headComp);
		}
		this.voiceId = "0";
		this.voiceId += (this.voiceIdx + 1).ToString();
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

	public static string ChooseRandomName(bool is_male)
	{
		if (MinionIdentity.femaleNameList == null)
		{
			MinionIdentity.maleNameList = new MinionIdentity.NameList(Game.Instance.maleNamesFile);
			MinionIdentity.femaleNameList = new MinionIdentity.NameList(Game.Instance.femaleNamesFile);
		}
		if (is_male)
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
	public bool isMale;

	[Serialize]
	public int voiceIdx;

	[Serialize]
	public Accessorizer.FaceData faceData;

	private string voiceId;

	private KCompBuildInstance headComp;

	private KAnimHashedString overrideExpression;

	private KAnimHashedString expression;

	[HideInInspector]
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
