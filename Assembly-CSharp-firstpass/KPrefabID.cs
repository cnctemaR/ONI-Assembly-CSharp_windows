using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class KPrefabID : KMonoBehaviour, ISaveLoadableJson
{
	public event KPrefabID.PrefabFn instantiateFn;

	public event KPrefabID.PrefabFn prefabInitFn;

	public event KPrefabID.PrefabFn prefabSpawnFn;

	public Tag[] Tags
	{
		get
		{
			this.InitializeTags();
			return this.tags;
		}
	}

	public void ResetLayer()
	{
		base.gameObject.layer = this.defaultLayer;
	}

	public void CopyInitFunctions(KPrefabID other)
	{
		this.instantiateFn = other.instantiateFn;
		this.prefabInitFn = other.prefabInitFn;
		this.prefabSpawnFn = other.prefabSpawnFn;
	}

	public void RunInstantiateFn()
	{
		if (this.instantiateFn != null)
		{
			this.instantiateFn(base.gameObject);
		}
	}

	public void InitializeTags()
	{
		if (this.tags == null || this.tags.Length == 0)
		{
			List<Tag> list = new List<Tag>();
			if (!this.PrefabTag.IsValid)
			{
				this.PrefabTag.Name = base.gameObject.name;
			}
			list.Add(this.PrefabTag);
			if (this.PrefabTags != null)
			{
				foreach (Tag tag in this.PrefabTags)
				{
					if (tag.IsValid)
					{
						list.Add(tag);
					}
				}
			}
			this.tags = list.ToArray();
		}
	}

	public void UpdateSaveLoadTag()
	{
		this.SaveLoadTag = new Tag(this.PrefabTag.Name);
	}

	public Tag GetSaveLoadTag()
	{
		return this.SaveLoadTag;
	}

	protected override void OnPrefabInit()
	{
		this.InitializeTags();
		if (this.prefabInitFn != null)
		{
			this.prefabInitFn(base.gameObject);
		}
	}

	protected override void OnSpawn()
	{
		this.AddLog(base.GetEventLog());
		if (this.prefabSpawnFn != null)
		{
			this.prefabSpawnFn(base.gameObject);
		}
	}

	public static int GetID(string str)
	{
		int num = 0;
		if (str != null)
		{
			num = Hash.SDBMLower(str);
		}
		return num;
	}

	public void AddPrefabTag(Tag tag)
	{
		if (tag.IsValid && Array.IndexOf<Tag>(this.PrefabTags, tag) == -1)
		{
			this.PrefabTags = new List<Tag>(this.PrefabTags) { tag }.ToArray();
		}
	}

	public void AddPrefabTags(List<Tag> tags)
	{
		List<Tag> list = tags.FindAll((Tag t) => t.IsValid && Array.IndexOf<Tag>(this.PrefabTags, t) == -1);
		if (list.Count > 0)
		{
			List<Tag> list2 = new List<Tag>(this.PrefabTags);
			foreach (Tag tag in list)
			{
				list2.Add(tag);
			}
			this.PrefabTags = list2.ToArray();
		}
	}

	public void AddTag(Tag tag)
	{
		if (this.HasTag(tag))
		{
			return;
		}
		if (tag.IsValid)
		{
			this.tags = new List<Tag>(this.Tags) { tag }.ToArray();
			this.onTagsChanged.Signal();
		}
		else
		{
			DebugUtil.Assert(tag.IsValid, "Assert!");
		}
	}

	public void AddTags(IList<Tag> additional_tags)
	{
		if (additional_tags == null || additional_tags.Count == 0)
		{
			return;
		}
		List<Tag> list = new List<Tag>(this.Tags);
		foreach (Tag tag in additional_tags)
		{
			if (!list.Contains(tag))
			{
				if (tag.IsValid)
				{
					list.Add(tag);
				}
				else
				{
					DebugUtil.Assert(tag.IsValid, "Assert!");
				}
			}
		}
		this.tags = list.ToArray();
		this.onTagsChanged.Signal();
	}

	public void RemoveTag(Tag tag)
	{
		if (this.HasTag(tag))
		{
			List<Tag> list = new List<Tag>(this.Tags);
			list.Remove(tag);
			this.tags = list.ToArray();
			this.onTagsChanged.Signal();
		}
	}

	public bool HasTag(Tag tag)
	{
		bool flag = false;
		Tag[] array = this.Tags;
		for (int i = 0; i < array.Length; i++)
		{
			if (tag == array[i])
			{
				flag = true;
				break;
			}
		}
		return flag;
	}

	public bool HasTags(IList<Tag> searchTags)
	{
		bool flag = true;
		Tag[] array = this.Tags;
		foreach (Tag tag in searchTags)
		{
			bool flag2 = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (tag == array[i])
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				flag = false;
				break;
			}
		}
		return flag;
	}

	public bool HasAnyTags(IList<Tag> searchTags)
	{
		Tag[] array = this.Tags;
		for (int i = 0; i < searchTags.Count; i++)
		{
			Tag tag = searchTags[i];
			for (int j = 0; j < array.Length; j++)
			{
				if (tag == array[j])
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool HasSameTags(KPrefabID prefab_id)
	{
		return this.PrefabTag == prefab_id.PrefabTag && this.Tags.Length == prefab_id.Tags.Length;
	}

	public override bool Equals(object o)
	{
		KPrefabID kprefabID = o as KPrefabID;
		return kprefabID != null && this.PrefabTag == kprefabID.PrefabTag;
	}

	public override int GetHashCode()
	{
		return this.PrefabTag.GetHashCode();
	}

	public static int GetUniqueID()
	{
		return KPrefabID.NextUniqueID++;
	}

	public string GetDebugName()
	{
		return string.Concat(new object[] { base.name, "(", this.InstanceID, ")" });
	}

	public KPrefabID GetOriginalPrefab()
	{
		return KPrefabIDTracker.Get().GetOriginalPrefab(this);
	}

	protected override void OnCleanUp()
	{
		KPrefabIDTracker.Get().Unregister(this);
		this.Trigger(1969584890, null);
	}

	public void AddLog(global::Logger logger)
	{
		if (this.logs == null)
		{
			this.logs = new List<global::Logger>();
		}
		this.logs.Add(logger);
	}

	public void RemoveLog(global::Logger logger)
	{
		this.logs.Remove(logger);
		if (this.logs.Count == 0)
		{
			this.logs = null;
		}
	}

	public List<global::Logger> GetLogs()
	{
		return this.logs;
	}

	[OnDeserialized]
	internal void OnDeserializedMethod()
	{
		KPrefabIDTracker.Get().Update(this);
	}

	public const int InvalidInstanceID = -1;

	public static int NextUniqueID;

	[ReadOnly]
	public Tag SaveLoadTag;

	public Tag PrefabTag;

	public Tag[] PrefabTags = new Tag[0];

	[Serialize]
	public int InstanceID;

	public int defaultLayer;

	public global::System.Action onTagsChanged;

	private List<global::Logger> logs;

	private Tag[] tags;

	public CellAlignment defaultSpawnOffset = CellAlignment.Bottom;

	public delegate void PrefabFn(GameObject go);
}
