using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class MinionStorageDataHolder : KMonoBehaviour, StoredMinionIdentity.IStoredMinionExtension
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public MinionStorageDataHolder.DataPack Internal_GetDataPack(string ID)
	{
		if (this.storedDataPacks != null)
		{
			MinionStorageDataHolder.DataPack dataPack = this.storedDataPacks.Find((MinionStorageDataHolder.DataPack d) => d.ID == ID);
			if (dataPack != null)
			{
				return dataPack;
			}
		}
		return null;
	}

	public void Internal_UpdateData(string ID, MinionStorageDataHolder.DataPackData data)
	{
		this.SetData(ID, data, false);
	}

	private void SetData(string ID, MinionStorageDataHolder.DataPackData data, bool markAsNewDataToRead)
	{
		if (this.storedDataPacks == null)
		{
			this.storedDataPacks = new List<MinionStorageDataHolder.DataPack>();
		}
		MinionStorageDataHolder.DataPack dataPack = this.storedDataPacks.Find((MinionStorageDataHolder.DataPack d) => d.ID == ID);
		if (dataPack == null)
		{
			dataPack = new MinionStorageDataHolder.DataPack(ID);
			this.storedDataPacks.Add(dataPack);
		}
		dataPack.SetData(data, markAsNewDataToRead);
	}

	public void PullFrom(StoredMinionIdentity source)
	{
		MinionStorageDataHolder component = source.GetComponent<MinionStorageDataHolder>();
		if (component != null && component.storedDataPacks != null)
		{
			for (int i = 0; i < component.storedDataPacks.Count; i++)
			{
				MinionStorageDataHolder.DataPack dataPack = component.storedDataPacks[i];
				if (dataPack != null)
				{
					this.SetData(dataPack.ID, dataPack.ReadData(), true);
				}
			}
		}
	}

	public void PushTo(StoredMinionIdentity destination)
	{
		Action<StoredMinionIdentity> onCopyBegins = this.OnCopyBegins;
		if (onCopyBegins != null)
		{
			onCopyBegins(destination);
		}
		this.AddStoredMinionGameObjectRequirements(destination.gameObject);
		MinionStorageDataHolder component = destination.gameObject.GetComponent<MinionStorageDataHolder>();
		if (this.storedDataPacks != null)
		{
			for (int i = 0; i < this.storedDataPacks.Count; i++)
			{
				MinionStorageDataHolder.DataPack dataPack = this.storedDataPacks[i];
				if (dataPack != null)
				{
					component.SetData(dataPack.ID, dataPack.ReadData(), true);
				}
			}
		}
	}

	public void AddStoredMinionGameObjectRequirements(GameObject storedMinionGameObject)
	{
		storedMinionGameObject.AddOrGet<MinionStorageDataHolder>();
	}

	public Action<StoredMinionIdentity> OnCopyBegins;

	[Serialize]
	private List<MinionStorageDataHolder.DataPack> storedDataPacks;

	[SerializationConfig(MemberSerialization.OptIn)]
	public class DataPackData
	{
		[Serialize]
		public bool[] Bools;

		[Serialize]
		public Tag[] Tags;
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	public class DataPack
	{
		public bool IsStoringNewData
		{
			get
			{
				return this.isStoringNewData;
			}
		}

		public string ID
		{
			get
			{
				return this.id;
			}
		}

		public DataPack(string id)
		{
			this.id = id;
		}

		public void SetData(MinionStorageDataHolder.DataPackData data, bool markAsNewDataToRead)
		{
			this.data = data;
			if (markAsNewDataToRead)
			{
				this.isStoringNewData = markAsNewDataToRead;
			}
		}

		public MinionStorageDataHolder.DataPackData ReadData()
		{
			this.isStoringNewData = false;
			return this.data;
		}

		public MinionStorageDataHolder.DataPackData PeekData()
		{
			return this.data;
		}

		[Serialize]
		private string id;

		[Serialize]
		private bool isStoringNewData;

		[Serialize]
		private MinionStorageDataHolder.DataPackData data;
	}
}
