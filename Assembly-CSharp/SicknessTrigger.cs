using System;
using System.Collections.Generic;
using Database;
using Klei.AI;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SicknessTrigger")]
public class SicknessTrigger : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public void AddTrigger(GameHashes src_event, string[] sickness_ids, SicknessTrigger.SourceCallback source_callback)
	{
		this.triggers.Add(new SicknessTrigger.TriggerInfo
		{
			srcEvent = src_event,
			sickness_ids = sickness_ids,
			sourceCallback = source_callback
		});
	}

	protected override void OnSpawn()
	{
		for (int i = 0; i < this.triggers.Count; i++)
		{
			SicknessTrigger.TriggerInfo trigger = this.triggers[i];
			base.Subscribe((int)trigger.srcEvent, delegate(object data)
			{
				this.OnSicknessTrigger((GameObject)data, trigger);
			});
		}
	}

	private void OnSicknessTrigger(GameObject target, SicknessTrigger.TriggerInfo trigger)
	{
		int num = global::UnityEngine.Random.Range(0, trigger.sickness_ids.Length);
		string text = trigger.sickness_ids[num];
		Sickness sickness = null;
		global::Database.Sicknesses sicknesses = Db.Get().Sicknesses;
		for (int i = 0; i < sicknesses.Count; i++)
		{
			if (sicknesses[i].Id == text)
			{
				sickness = sicknesses[i];
				break;
			}
		}
		if (sickness != null)
		{
			string text2 = trigger.sourceCallback(base.gameObject, target);
			SicknessExposureInfo sicknessExposureInfo = new SicknessExposureInfo(sickness.Id, text2);
			target.GetComponent<MinionModifiers>().sicknesses.Infect(sicknessExposureInfo);
			return;
		}
		DebugUtil.DevLogErrorFormat(base.gameObject, "Couldn't find sickness with id [{0}]", new object[] { text });
	}

	public List<Descriptor> EffectDescriptors(GameObject go)
	{
		Dictionary<GameHashes, HashSet<string>> dictionary = new Dictionary<GameHashes, HashSet<string>>();
		foreach (SicknessTrigger.TriggerInfo triggerInfo in this.triggers)
		{
			HashSet<string> hashSet = null;
			if (!dictionary.TryGetValue(triggerInfo.srcEvent, out hashSet))
			{
				hashSet = new HashSet<string>();
				dictionary[triggerInfo.srcEvent] = hashSet;
			}
			foreach (string text in triggerInfo.sickness_ids)
			{
				hashSet.Add(text);
			}
		}
		List<Descriptor> list = new List<Descriptor>();
		List<string> list2 = new List<string>();
		string properName = base.GetComponent<KSelectable>().GetProperName();
		foreach (KeyValuePair<GameHashes, HashSet<string>> keyValuePair in dictionary)
		{
			HashSet<string> value = keyValuePair.Value;
			list2.Clear();
			foreach (string text2 in value)
			{
				Sickness sickness = Db.Get().Sicknesses.TryGet(text2);
				list2.Add(sickness.Name);
			}
			string text3 = string.Join(", ", list2.ToArray());
			string text4 = Strings.Get("STRINGS.DUPLICANTS.DISEASES.TRIGGERS." + Enum.GetName(typeof(GameHashes), keyValuePair.Key).ToUpper()).String;
			string text5 = Strings.Get("STRINGS.DUPLICANTS.DISEASES.TRIGGERS.TOOLTIPS." + Enum.GetName(typeof(GameHashes), keyValuePair.Key).ToUpper()).String;
			text4 = text4.Replace("{ItemName}", properName).Replace("{Diseases}", text3);
			text5 = text5.Replace("{ItemName}", properName).Replace("{Diseases}", text3);
			list.Add(new Descriptor(text4, text5, Descriptor.DescriptorType.Effect, false));
		}
		return list;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return this.EffectDescriptors(go);
	}

	public List<SicknessTrigger.TriggerInfo> triggers = new List<SicknessTrigger.TriggerInfo>();

	public delegate string SourceCallback(GameObject source, GameObject target);

	[Serializable]
	public struct TriggerInfo
	{
		[HashedEnum]
		public GameHashes srcEvent;

		public string[] sickness_ids;

		public SicknessTrigger.SourceCallback sourceCallback;
	}
}
