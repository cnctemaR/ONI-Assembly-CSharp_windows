using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;
using UnityEngine;

public class StateMachineSerializer
{
	public void Serialize(List<StateMachine.Instance> state_machines, BinaryWriter writer)
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		List<StateMachineSerializer.Entry> list = this.CreateEntries(state_machines, binaryWriter);
		long num = this.WriteHeader(writer);
		long position = writer.BaseStream.Position;
		this.WriteEntries(list, writer);
		this.WriteEntryData(memoryStream, writer);
		this.WriteDataSize(position, num, writer);
	}

	public void Deserialize(IReader reader)
	{
		if (!this.ReadHeader(reader))
		{
			return;
		}
		this.entries = this.ReadEntries(reader);
		this.entryData = this.ReadEntryData(reader);
	}

	private List<StateMachineSerializer.Entry> CreateEntries(List<StateMachine.Instance> state_machines, BinaryWriter entry_writer)
	{
		List<StateMachineSerializer.Entry> list = new List<StateMachineSerializer.Entry>();
		foreach (StateMachine.Instance instance in state_machines)
		{
			if (instance.GetStateMachine().serializable)
			{
				if (instance.IsRunning())
				{
					StateMachineSerializer.Entry entry = new StateMachineSerializer.Entry(instance, entry_writer);
					list.Add(entry);
				}
			}
		}
		return list;
	}

	private void WriteEntryData(MemoryStream stream, BinaryWriter writer)
	{
		writer.Write((int)stream.Length);
		writer.Write(stream.ToArray());
	}

	private FastReader ReadEntryData(IReader reader)
	{
		int num = reader.ReadInt32();
		byte[] array = reader.ReadBytes(num);
		return new FastReader(array);
	}

	private void WriteDataSize(long data_start_pos, long data_size_pos, BinaryWriter writer)
	{
		long position = writer.BaseStream.Position;
		long num = position - data_start_pos;
		writer.BaseStream.Position = data_size_pos;
		writer.Write((int)num);
		writer.BaseStream.Position = position;
	}

	private long WriteHeader(BinaryWriter writer)
	{
		int num = 0;
		writer.Write(StateMachineSerializer.serializerVersion);
		long position = writer.BaseStream.Position;
		writer.Write(num);
		return position;
	}

	private bool ReadHeader(IReader reader)
	{
		int num = reader.ReadInt32();
		int num2 = reader.ReadInt32();
		if (num != StateMachineSerializer.serializerVersion)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"State machine serializer version mismatch: ",
				num,
				"!=",
				StateMachineSerializer.serializerVersion,
				"\nDiscarding data."
			}));
			reader.SkipBytes(num2);
			return false;
		}
		return true;
	}

	private void WriteEntries(List<StateMachineSerializer.Entry> serialized_entries, BinaryWriter writer)
	{
		writer.Write(serialized_entries.Count);
		for (int i = 0; i < serialized_entries.Count; i++)
		{
			serialized_entries[i].Serialize(writer);
		}
	}

	private List<StateMachineSerializer.Entry> ReadEntries(IReader reader)
	{
		List<StateMachineSerializer.Entry> list = new List<StateMachineSerializer.Entry>();
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			StateMachineSerializer.Entry entry = StateMachineSerializer.Entry.Deserialize(reader);
			if (entry != null)
			{
				list.Add(entry);
			}
		}
		return list;
	}

	private bool Restore(StateMachineSerializer.Entry entry, StateMachine.Instance smi)
	{
		if (entry.version != smi.GetStateMachine().version)
		{
			return false;
		}
		StateMachine.BaseState state = smi.GetStateMachine().GetState(entry.currentState);
		if (state == null)
		{
			return false;
		}
		this.entryData.Position = entry.dataPos;
		Deserializer.DeserializeTypeless(smi, this.entryData);
		StateMachine.Parameter.Context[] parameterContexts = smi.GetParameterContexts();
		int num = this.entryData.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			int num2 = this.entryData.ReadInt32();
			int position = this.entryData.Position;
			string text = this.entryData.ReadKleiString();
			string text2 = this.entryData.ReadKleiString();
			foreach (StateMachine.Parameter.Context context in parameterContexts)
			{
				if (context.parameter.name == text2 && context.GetType().FullName == text)
				{
					context.Deserialize(this.entryData);
					break;
				}
			}
			this.entryData.SkipBytes(num2 - (this.entryData.Position - position));
		}
		smi.GoTo(state);
		return true;
	}

	public bool Restore(StateMachine.Instance instance)
	{
		if (this.entryData == null)
		{
			return false;
		}
		Type type = instance.GetType();
		for (int i = 0; i < this.entries.Count; i++)
		{
			StateMachineSerializer.Entry entry = this.entries[i];
			if (entry.type == type)
			{
				this.entries.RemoveAt(i);
				return this.Restore(entry, instance);
			}
		}
		return false;
	}

	private static int serializerVersion = 10;

	private List<StateMachineSerializer.Entry> entries = new List<StateMachineSerializer.Entry>();

	private FastReader entryData;

	private class Entry
	{
		public Entry(int version, int data_pos, Type type, string current_state)
		{
			this.version = version;
			this.dataPos = data_pos;
			this.type = type;
			this.currentState = current_state;
		}

		public Entry(StateMachine.Instance smi, BinaryWriter entry_writer)
		{
			this.version = smi.GetStateMachine().version;
			this.dataPos = (int)entry_writer.BaseStream.Position;
			this.type = smi.GetType();
			this.currentState = smi.GetCurrentState().name;
			Serializer.SerializeTypeless(smi, entry_writer);
			StateMachine.Parameter.Context[] parameterContexts = smi.GetParameterContexts();
			entry_writer.Write(parameterContexts.Length);
			foreach (StateMachine.Parameter.Context context in parameterContexts)
			{
				long num = (long)((int)entry_writer.BaseStream.Position);
				entry_writer.Write(0);
				long num2 = (long)((int)entry_writer.BaseStream.Position);
				entry_writer.WriteKleiString(context.GetType().FullName);
				entry_writer.WriteKleiString(context.parameter.name);
				context.Serialize(entry_writer);
				long num3 = (long)((int)entry_writer.BaseStream.Position);
				entry_writer.BaseStream.Position = num;
				long num4 = num3 - num2;
				entry_writer.Write((int)num4);
				entry_writer.BaseStream.Position = num3;
			}
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(this.version);
			writer.Write(this.dataPos);
			writer.WriteKleiString(this.type.FullName);
			writer.WriteKleiString(this.currentState);
		}

		public static StateMachineSerializer.Entry Deserialize(IReader reader)
		{
			int num = reader.ReadInt32();
			int num2 = reader.ReadInt32();
			string text = reader.ReadKleiString();
			string text2 = reader.ReadKleiString();
			Type type = Type.GetType(text);
			if (type == null)
			{
				Debug.LogWarning("Missing state machine of type: " + text);
				return null;
			}
			return new StateMachineSerializer.Entry(num, num2, type, text2);
		}

		public int version;

		public int dataPos;

		public Type type;

		public string currentState;
	}
}
