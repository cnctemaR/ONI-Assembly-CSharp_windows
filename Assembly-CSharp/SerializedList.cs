using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class SerializedList<ItemType>
{
	public int Count
	{
		get
		{
			return this.items.Count;
		}
	}

	public IEnumerator<ItemType> GetEnumerator()
	{
		return this.items.GetEnumerator();
	}

	public ItemType this[int idx]
	{
		get
		{
			return this.items[idx];
		}
	}

	public void Add(ItemType item)
	{
		this.items.Add(item);
	}

	public void Remove(ItemType item)
	{
		this.items.Remove(item);
	}

	public void RemoveAt(int idx)
	{
		this.items.RemoveAt(idx);
	}

	public bool Contains(ItemType item)
	{
		return this.items.Contains(item);
	}

	public void Clear()
	{
		this.items.Clear();
	}

	[OnSerializing]
	private void OnSerializing()
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(this.items.Count);
		foreach (ItemType itemType in this.items)
		{
			binaryWriter.WriteKleiString(itemType.GetType().FullName);
			long position = binaryWriter.BaseStream.Position;
			binaryWriter.Write(0);
			long position2 = binaryWriter.BaseStream.Position;
			Serializer.SerializeTypeless(itemType, binaryWriter);
			long position3 = binaryWriter.BaseStream.Position;
			long num = position3 - position2;
			binaryWriter.BaseStream.Position = position;
			binaryWriter.Write((int)num);
			binaryWriter.BaseStream.Position = position3;
		}
		memoryStream.Flush();
		this.serializationBuffer = memoryStream.ToArray();
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.serializationBuffer == null)
		{
			return;
		}
		FastReader fastReader = new FastReader(this.serializationBuffer);
		int num = fastReader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			string text = fastReader.ReadKleiString();
			int num2 = fastReader.ReadInt32();
			int position = fastReader.Position;
			Type type = Type.GetType(text);
			if (type == null)
			{
				DebugUtil.LogWarningArgs(new object[] { "Type no longer exists: " + text });
				fastReader.SkipBytes(num2);
			}
			else
			{
				ItemType itemType;
				if (typeof(ItemType) != type)
				{
					itemType = (ItemType)((object)Activator.CreateInstance(type));
				}
				else
				{
					itemType = default(ItemType);
				}
				Deserializer.DeserializeTypeless(itemType, fastReader);
				if (fastReader.Position != position + num2)
				{
					DebugUtil.LogWarningArgs(new object[]
					{
						"Expected to be at offset",
						position + num2,
						"but was only at offset",
						fastReader.Position,
						". Skipping to catch up."
					});
					fastReader.SkipBytes(position + num2 - fastReader.Position);
				}
				this.items.Add(itemType);
			}
		}
	}

	[Serialize]
	private byte[] serializationBuffer;

	private List<ItemType> items = new List<ItemType>();
}
