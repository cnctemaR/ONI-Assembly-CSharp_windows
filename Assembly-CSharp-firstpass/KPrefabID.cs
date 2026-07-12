using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/Plugins/KPrefabID")]
public class KPrefabID : KMonoBehaviour, ISaveLoadable
{
	public static int NextUniqueID
	{
		get
		{
			return KPrefabID.nextUniqueID;
		}
		set
		{
			KPrefabID.nextUniqueID = value;
		}
	}

	public event KPrefabID.PrefabFn instantiateFn;

	public event KPrefabID.PrefabFn prefabInitFn;

	public event KPrefabID.PrefabFn prefabSpawnFn;

	public bool pendingDestruction { get; private set; }

	public bool conflicted { get; private set; }

	public HashSet<Tag> Tags
	{
		get
		{
			DebugUtil.DevAssert(this.initialized, "This object is has not been initialized, Tags is not valid. Is it an inactive prefab?", null);
			return this.tags;
		}
	}

	public Tag PrefabID()
	{
		return this.PrefabTag;
	}

	public bool IsPrefabID(Tag prefab_id)
	{
		return this.PrefabTag == prefab_id;
	}

	public bool IsAnyPrefabID(Tag[] ids)
	{
		for (int i = 0; i < ids.Length; i++)
		{
			if (this.PrefabTag == ids[i])
			{
				return true;
			}
		}
		return false;
	}

	public void CopyTags(KPrefabID other)
	{
		foreach (Tag tag in other.tags)
		{
			this.tags.Add(tag);
		}
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
			this.instantiateFn = null;
		}
	}

	private void ValidateTags()
	{
		DebugUtil.Assert(this.PrefabTag.IsValid);
		foreach (Tag tag in this.serializedTags)
		{
			global::Debug.Assert(this.tags.Contains(tag), string.Format("serialized tag {0} is not contained in tags", tag));
		}
	}

	public void InitializeTags(bool force_initialize = false)
	{
		if (this.initialized && !force_initialize)
		{
			return;
		}
		foreach (Tag tag in this.serializedTags)
		{
			if (this.tags.Add(tag))
			{
				this.dirtyTagsHash = true;
			}
		}
		this.initialized = true;
	}

	public void UpdateSaveLoadTag()
	{
		this.SaveLoadTag = new Tag(this.PrefabTag.Name);
	}

	public Tag GetSaveLoadTag()
	{
		return this.SaveLoadTag;
	}

	public int GetTagsHash()
	{
		this.UpdateTagsHash();
		return this.TagsHash;
	}

	public void UpdateTagsHash()
	{
		this.InitializeTags(false);
		if (!this.dirtyTagsHash)
		{
			return;
		}
		int num = 0;
		foreach (Tag tag in this.tags)
		{
			if (tag != KPrefabID.IgnoredHashTag)
			{
				num ^= tag.GetHash();
			}
		}
		this.TagsHash = num;
		this.dirtyTagsHash = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<KPrefabID>(1969584890, KPrefabID.OnObjectDestroyedDelegate);
		this.InitializeTags(true);
		if (this.prefabInitFn != null)
		{
			this.prefabInitFn(base.gameObject);
			this.prefabInitFn = null;
		}
		IStateMachineControllerHack component = base.GetComponent<IStateMachineControllerHack>();
		if (component != null)
		{
			component.CreateSMIS();
		}
	}

	protected override void OnSpawn()
	{
		this.InitializeTags(true);
		IStateMachineControllerHack component = base.GetComponent<IStateMachineControllerHack>();
		if (component != null)
		{
			component.StartSMIS();
		}
		if (this.prefabSpawnFn != null)
		{
			this.prefabSpawnFn(base.gameObject);
			this.prefabSpawnFn = null;
		}
	}

	protected override void OnCmpEnable()
	{
		this.InitializeTags(true);
	}

	public void AddTag(Tag tag, bool serialize = false)
	{
		DebugUtil.Assert(tag.IsValid);
		if (this.tags.Add(tag))
		{
			this.dirtyTagsHash = true;
			base.Trigger(-1582839653, new TagChangedEventData(tag, true));
		}
		if (serialize)
		{
			this.serializedTags.Add(tag);
		}
	}

	public void RemoveTag(Tag tag)
	{
		if (this.tags.Remove(tag))
		{
			this.dirtyTagsHash = true;
			base.Trigger(-1582839653, new TagChangedEventData(tag, false));
		}
		this.serializedTags.Remove(tag);
	}

	public void SetTag(Tag tag, bool set)
	{
		if (set)
		{
			this.AddTag(tag, false);
			return;
		}
		this.RemoveTag(tag);
	}

	public bool HasTag(Tag tag)
	{
		return this.PrefabTag == tag || this.tags.Contains(tag);
	}

	public bool HasAnyTags(List<Tag> search_tags)
	{
		for (int i = 0; i < search_tags.Count; i++)
		{
			Tag tag = search_tags[i];
			if (this.PrefabTag == tag || this.tags.Contains(tag))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAnyTags(Tag[] search_tags)
	{
		foreach (Tag tag in search_tags)
		{
			if (this.PrefabTag == tag || this.tags.Contains(tag))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAllTags(Tag[] search_tags)
	{
		foreach (Tag tag in search_tags)
		{
			if (this.PrefabTag != tag && !this.tags.Contains(tag))
			{
				return false;
			}
		}
		return true;
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
		return base.name + "(" + this.InstanceID.ToString() + ")";
	}

	protected override void OnCleanUp()
	{
		this.pendingDestruction = true;
		if (this.InstanceID != -1)
		{
			KPrefabIDTracker.Get().Unregister(this);
		}
		base.Trigger(1969584890, null);
	}

	[OnDeserialized]
	internal void OnDeserializedMethod()
	{
		this.InitializeTags(true);
		KPrefabIDTracker kprefabIDTracker = KPrefabIDTracker.Get();
		if (kprefabIDTracker.GetInstance(this.InstanceID))
		{
			this.conflicted = true;
		}
		kprefabIDTracker.Register(this);
	}

	private void OnObjectDestroyed(object data)
	{
		this.pendingDestruction = true;
	}

	public void SetUnityEditorConfigOverride(string filename)
	{
	}

	public const int InvalidInstanceID = -1;

	private static int nextUniqueID = 0;

	[ReadOnly]
	public Tag SaveLoadTag;

	public Tag PrefabTag;

	private int TagsHash;

	private bool initialized;

	private bool dirtyTagsHash = true;

	[Serialize]
	public int InstanceID;

	public int defaultLayer;

	public List<Descriptor> AdditionalRequirements;

	public List<Descriptor> AdditionalEffects;

	[Serialize]
	private HashSet<Tag> serializedTags = new HashSet<Tag>();

	public string[] requiredDlcIds;

	private HashSet<Tag> tags = new HashSet<Tag>();

	private static Tag IgnoredHashTag = TagManager.Create("Preserved");

	private static readonly EventSystem.IntraObjectHandler<KPrefabID> OnObjectDestroyedDelegate = new EventSystem.IntraObjectHandler<KPrefabID>(delegate(KPrefabID component, object data)
	{
		component.OnObjectDestroyed(data);
	});

	public delegate void PrefabFn(GameObject go);
}
