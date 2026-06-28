using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class KPrefabID : KMonoBehaviour, ISaveLoadable
{
	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event KPrefabID.PrefabFn instantiateFn;

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event KPrefabID.PrefabFn prefabInitFn;

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event KPrefabID.PrefabFn prefabSpawnFn;

	public bool pendingDestruction { get; private set; }

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
		base.OnPrefabInit();
		base.Subscribe(1969584890, new Action<object>(this.OnObjectDestroyed));
		this.InitializeTags();
		if (this.prefabInitFn != null)
		{
			this.prefabInitFn(base.gameObject);
		}
		IStateMachineControllerHack component = base.GetComponent<IStateMachineControllerHack>();
		if (component != null)
		{
			component.CreateSMIS();
		}
	}

	protected override void OnSpawn()
	{
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
		if (tag.IsValid && !this.HasPrefabTag(tag))
		{
			this.PrefabTags = new List<Tag>(this.PrefabTags) { tag }.ToArray();
		}
	}

	public void AddPrefabTags(List<Tag> tags)
	{
		List<Tag> list = tags.FindAll((Tag t) => t.IsValid && !this.HasPrefabTag(t));
		if (list.Count > 0)
		{
			List<Tag> list2 = new List<Tag>(this.PrefabTags);
			list2.AddRange(list);
			this.PrefabTags = list2.ToArray();
		}
	}

	public bool HasPrefabTag(Tag tag)
	{
		if (tag == this.PrefabTag)
		{
			return true;
		}
		for (int i = 0; i < this.PrefabTags.Length; i++)
		{
			if (tag == this.PrefabTags[i])
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAnyPrefabTags(IList<Tag> searchTags)
	{
		for (int i = 0; i < searchTags.Count; i++)
		{
			Tag tag = searchTags[i];
			if (this.PrefabTag == tag)
			{
				return true;
			}
			for (int j = 0; j < this.PrefabTags.Length; j++)
			{
				if (tag == this.PrefabTags[j])
				{
					return true;
				}
			}
		}
		return false;
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
			base.Trigger(-1582839653, null);
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
		base.Trigger(-1582839653, null);
	}

	public void RemoveTag(Tag tag)
	{
		if (this.HasTag(tag))
		{
			List<Tag> list = new List<Tag>(this.Tags);
			list.Remove(tag);
			this.tags = list.ToArray();
			base.Trigger(-1582839653, null);
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
		this.pendingDestruction = true;
		KPrefabIDTracker.Get().Unregister(this);
		base.Trigger(1969584890, null);
	}

	[Conditional("UNITY_EDITOR")]
	public void AddLog(global::Logger logger)
	{
		if (this.logs == null)
		{
			this.logs = new List<global::Logger>();
		}
		this.logs.Add(logger);
	}

	[Conditional("UNITY_EDITOR")]
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

	public void AddAdditionalRequirement(List<Descriptor> additional)
	{
		if (this.AdditionalRequirements == null)
		{
			this.AdditionalRequirements = new List<Descriptor>();
		}
		this.AdditionalRequirements.AddRange(additional);
	}

	public void AddAdditionalEffect(List<Descriptor> additional)
	{
		if (this.AdditionalEffects == null)
		{
			this.AdditionalEffects = new List<Descriptor>();
		}
		this.AdditionalRequirements.AddRange(additional);
	}

	private void OnObjectDestroyed(object data)
	{
		this.pendingDestruction = true;
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

	private List<global::Logger> logs;

	private LoggerFSS tagLog = new LoggerFSS("Tags");

	public List<Descriptor> AdditionalRequirements;

	public List<Descriptor> AdditionalEffects;

	private Tag[] tags;

	public CellAlignment defaultSpawnOffset = CellAlignment.Bottom;

	public delegate void PrefabFn(GameObject go);
}
