using System;
using System.Collections.Generic;
using Database;
using Klei.AI;
using UnityEngine;

public class DiseaseTrigger : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public void AddTrigger(GameHashes src_event, string[] disease_ids, DiseaseTrigger.SourceCallback source_callback)
	{
		this.triggers.Add(new DiseaseTrigger.TriggerInfo
		{
			srcEvent = src_event,
			diseaseIDs = disease_ids,
			sourceCallback = source_callback
		});
	}

	protected override void OnSpawn()
	{
		for (int i = 0; i < this.triggers.Count; i++)
		{
			DiseaseTrigger.TriggerInfo trigger = this.triggers[i];
			this.Subscribe((int)trigger.srcEvent, delegate(object data)
			{
				GameObject gameObject = (GameObject)data;
				global::Database.Diseases diseases = Db.Get().Diseases;
				int num = global::UnityEngine.Random.Range(0, trigger.diseaseIDs.Length);
				Disease disease = null;
				for (int j = 0; j < diseases.Count; j++)
				{
					if (diseases[j].Id == trigger.diseaseIDs[num])
					{
						disease = diseases[j];
						break;
					}
				}
				if (disease != null)
				{
					string text = trigger.sourceCallback(this.gameObject, gameObject.gameObject);
					DiseaseExposureInfo diseaseExposureInfo = new DiseaseExposureInfo(disease.Id, text);
					bool flag = true;
					Edible component = this.gameObject.GetComponent<Edible>();
					if (component != null)
					{
						Traits component2 = gameObject.GetComponent<Traits>();
						if (component2.HasTrait("IronGut"))
						{
							flag = false;
						}
					}
					if (flag)
					{
						Klei.AI.Diseases diseases2 = gameObject.GetComponent<MinionModifiers>().diseases;
						diseases2.Infect(diseaseExposureInfo);
					}
				}
				else
				{
					Output.LogErrorWithObj(this.gameObject, new object[] { "couldn't find disease with id [" + trigger.diseaseIDs[num] + "]" });
				}
			});
		}
	}

	public List<Descriptor> EffectDescriptors(GameObject go)
	{
		Dictionary<GameHashes, HashSet<string>> dictionary = new Dictionary<GameHashes, HashSet<string>>();
		foreach (DiseaseTrigger.TriggerInfo triggerInfo in this.triggers)
		{
			HashSet<string> hashSet = null;
			if (!dictionary.TryGetValue(triggerInfo.srcEvent, out hashSet))
			{
				hashSet = new HashSet<string>();
				dictionary[triggerInfo.srcEvent] = hashSet;
			}
			foreach (string text in triggerInfo.diseaseIDs)
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
				Disease disease = Db.Get().Diseases.Get(text2);
				list2.Add(disease.Name);
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

	public List<DiseaseTrigger.TriggerInfo> triggers = new List<DiseaseTrigger.TriggerInfo>();

	[Serializable]
	public struct TriggerInfo
	{
		[HashedEnum]
		public GameHashes srcEvent;

		public string[] diseaseIDs;

		public DiseaseTrigger.SourceCallback sourceCallback;
	}

	public delegate string SourceCallback(GameObject source, GameObject target);
}
