using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
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
		SolidConduitFlow.SOAInfo soainfo = solidConduitFlow.GetSOAInfo();
		List<int> cells = soainfo.Cells;
		int num = 0;
		for (int i = 0; i < cells.Count; i++)
		{
			int num2 = cells[i];
			SolidConduitFlow.ConduitContents contents = solidConduitFlow.GetContents(num2);
			if (contents.pickupableHandle.IsValid())
			{
				Pickupable pickupable = solidConduitFlow.GetPickupable(contents.pickupableHandle);
				if (pickupable)
				{
					num++;
				}
			}
		}
		writer.Write(num);
		for (int j = 0; j < cells.Count; j++)
		{
			int num3 = cells[j];
			SolidConduitFlow.ConduitContents contents2 = solidConduitFlow.GetContents(num3);
			bool flag = contents2.pickupableHandle.IsValid();
			if (flag)
			{
				Pickupable pickupable2 = solidConduitFlow.GetPickupable(contents2.pickupableHandle);
				if (pickupable2)
				{
					writer.Write(num3);
					SaveLoadRoot component = pickupable2.GetComponent<SaveLoadRoot>();
					if (component != null)
					{
						string name = pickupable2.GetComponent<KPrefabID>().GetSaveLoadTag().Name;
						writer.WriteKleiString(name);
						component.Save(writer);
					}
					else
					{
						Debug.Log("Tried to save obj in solid conduit but obj has no SaveLoadRoot", pickupable2.gameObject);
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
			string text = reader.ReadKleiString();
			Tag tag = TagManager.Create(text);
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
				Debug.Log("Tried to deserialize " + tag.ToString() + " into storage but failed", base.gameObject);
			}
		}
	}
}
