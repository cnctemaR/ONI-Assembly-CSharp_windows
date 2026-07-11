using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/SolidConduitSerializer")]
public class SolidConduitSerializer : KMonoBehaviour, ISaveLoadableDetails
{
	protected override void OnPrefabInit()
	{
	}

	protected override void OnSpawn()
	{
	}

	public void Serialize(BinaryWriter writer)
	{
		SolidConduitFlow solidConduitFlow = Game.Instance.solidConduitFlow;
		List<int> cells = solidConduitFlow.GetSOAInfo().Cells;
		int num = 0;
		for (int i = 0; i < cells.Count; i++)
		{
			int num2 = cells[i];
			SolidConduitFlow.ConduitContents contents = solidConduitFlow.GetContents(num2);
			if (contents.pickupableHandle.IsValid() && solidConduitFlow.GetPickupable(contents.pickupableHandle))
			{
				num++;
			}
		}
		writer.Write(num);
		for (int j = 0; j < cells.Count; j++)
		{
			int num3 = cells[j];
			SolidConduitFlow.ConduitContents contents2 = solidConduitFlow.GetContents(num3);
			if (contents2.pickupableHandle.IsValid())
			{
				Pickupable pickupable = solidConduitFlow.GetPickupable(contents2.pickupableHandle);
				if (pickupable)
				{
					writer.Write(num3);
					SaveLoadRoot component = pickupable.GetComponent<SaveLoadRoot>();
					if (component != null)
					{
						string name = pickupable.GetComponent<KPrefabID>().GetSaveLoadTag().Name;
						writer.WriteKleiString(name);
						component.Save(writer);
					}
					else
					{
						global::Debug.Log("Tried to save obj in solid conduit but obj has no SaveLoadRoot", pickupable.gameObject);
					}
				}
			}
		}
	}

	public void Deserialize(IReader reader)
	{
		SolidConduitFlow solidConduitFlow = Game.Instance.solidConduitFlow;
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			int num2 = reader.ReadInt32();
			Tag tag = TagManager.Create(reader.ReadKleiString());
			SaveLoadRoot saveLoadRoot = SaveLoadRoot.Load(tag, reader);
			if (saveLoadRoot != null)
			{
				Pickupable component = saveLoadRoot.GetComponent<Pickupable>();
				if (component != null)
				{
					solidConduitFlow.SetContents(num2, component);
				}
			}
			else
			{
				global::Debug.Log("Tried to deserialize " + tag.ToString() + " into storage but failed", base.gameObject);
			}
		}
	}
}
