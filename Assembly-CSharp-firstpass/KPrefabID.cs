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

	public HashSet<Tag> Tags
	{
		get
		{
			this.InitializeTags();
			return this.tags;
		}
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

	public void InitializeTags()
	{
		DebugUtil.Assert(this.PrefabTag.IsValid, "Assert!", string.Empty, string.Empty);
		this.tags.Add(this.PrefabTag);
		this.dirtyTagBits = true;
	}

	public void UpdateSaveLoadTag()
	{
		this.SaveLoadTag = new Tag(this.PrefabTag.Name);
	}

	public Tag GetSaveLoadTag()
	{
		return this.SaveLoadTag;
	}

	public TagBits GetTagBits()
	{
		this.InitializeTags();
		if (this.dirtyTagBits)
		{
			this.tagBits = default(TagBits);
			foreach (Tag tag in this.tags)
			{
				this.tagBits.SetTag(tag);
			}
			this.dirtyTagBits = false;
		}
		return this.tagBits;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<KPrefabID>(1969584890, KPrefabID.OnObjectDestroyedDelegate);
		this.InitializeTags();
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

	public void AddTag(Tag tag)
	{
		DebugUtil.Assert(tag.IsValid, "Assert!", string.Empty, string.Empty);
		if (this.Tags.Add(tag))
		{
			this.dirtyTagBits = true;
			base.Trigger(-1582839653, null);
		}
	}

	public void RemoveTag(Tag tag)
	{
		if (this.Tags.Remove(tag))
		{
			this.dirtyTagBits = true;
			base.Trigger(-1582839653, null);
		}
	}

	public void SetTag(Tag tag, bool set)
	{
		if (set)
		{
			this.AddTag(tag);
		}
		else
		{
			this.RemoveTag(tag);
		}
	}

	public bool HasTag(Tag tag)
	{
		return this.Tags.Contains(tag);
	}

	public bool HasAnyTags(List<Tag> search_tags)
	{
		this.InitializeTags();
		foreach (Tag tag in search_tags)
		{
			if (this.tags.Contains(tag))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAnyTags(Tag[] search_tags)
	{
		this.InitializeTags();
		foreach (Tag tag in search_tags)
		{
			if (this.tags.Contains(tag))
			{
				return true;
			}
		}
		return false;
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
		KPrefabIDTracker.Get().Update(this);
	}

	private void OnObjectDestroyed(object data)
	{
		this.pendingDestruction = true;
	}

	public const int InvalidInstanceID = -1;

	public static int NextUniqueID = 0;

	[ReadOnly]
	public Tag SaveLoadTag;

	public Tag PrefabTag;

	private TagBits tagBits;

	private bool dirtyTagBits = true;

	[Serialize]
	public int InstanceID;

	public int defaultLayer;

	public List<Descriptor> AdditionalRequirements;

	public List<Descriptor> AdditionalEffects;

	private HashSet<Tag> tags = new HashSet<Tag>();

	private static readonly EventSystem.IntraObjectHandler<KPrefabID> OnObjectDestroyedDelegate = new EventSystem.IntraObjectHandler<KPrefabID>(delegate(KPrefabID component, object data)
	{
		component.OnObjectDestroyed(data);
	});

	public delegate void PrefabFn(GameObject go);
}
