using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;

public class StateMachineSerializer
{
	public void Serialize(List<StateMachine.Instance> state_machines, BinaryWriter writer)
	{
		writer.Write(StateMachineSerializer.SERIALIZER_VERSION);
		long position = writer.BaseStream.Position;
		writer.Write(0);
		long position2 = writer.BaseStream.Position;
		try
		{
			int num = (int)writer.BaseStream.Position;
			int num2 = 0;
			writer.Write(num2);
			using (List<StateMachine.Instance>.Enumerator enumerator = state_machines.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (StateMachineSerializer.Entry.TrySerialize(enumerator.Current, writer))
					{
						num2++;
					}
				}
			}
			int num3 = (int)writer.BaseStream.Position;
			writer.BaseStream.Position = (long)num;
			writer.Write(num2);
			writer.BaseStream.Position = (long)num3;
		}
		catch (Exception ex)
		{
			Debug.Log("StateMachines: ");
			foreach (StateMachine.Instance instance in state_machines)
			{
				Debug.Log(instance.ToString());
			}
			Debug.LogError(ex);
		}
		long position3 = writer.BaseStream.Position;
		long num4 = position3 - position2;
		writer.BaseStream.Position = position;
		writer.Write((int)num4);
		writer.BaseStream.Position = position3;
	}

	public void Deserialize(IReader reader)
	{
		int num = reader.ReadInt32();
		int num2 = reader.ReadInt32();
		if (num < 10)
		{
			Debug.LogWarning(string.Concat(new string[]
			{
				"State machine serializer version mismatch: ",
				num.ToString(),
				"!=",
				StateMachineSerializer.SERIALIZER_VERSION.ToString(),
				"\nDiscarding data."
			}));
			reader.SkipBytes(num2);
			return;
		}
		if (num < 12)
		{
			this.entries = StateMachineSerializer.OldEntryV11.DeserializeOldEntries(reader, num);
			return;
		}
		int num3 = reader.ReadInt32();
		this.entries = new List<StateMachineSerializer.Entry>(num3);
		for (int i = 0; i < num3; i++)
		{
			StateMachineSerializer.Entry entry = StateMachineSerializer.Entry.Deserialize(reader, num);
			if (entry != null)
			{
				this.entries.Add(entry);
			}
		}
	}

	private static string TrimAssemblyInfo(string type_name)
	{
		int num = type_name.IndexOf("[[");
		if (num != -1)
		{
			return type_name.Substring(0, num);
		}
		return type_name;
	}

	public bool Restore(StateMachine.Instance instance)
	{
		Type type = instance.GetType();
		for (int i = 0; i < this.entries.Count; i++)
		{
			StateMachineSerializer.Entry entry = this.entries[i];
			if (entry.type == type && instance.serializationSuffix == entry.typeSuffix)
			{
				this.entries.RemoveAt(i);
				return entry.Restore(instance);
			}
		}
		return false;
	}

	private static bool DoesVersionHaveTypeSuffix(int version)
	{
		return version >= 20 || version == 11;
	}

	public const int SERIALIZER_PRE_DLC1 = 10;

	public const int SERIALIZER_TYPE_SUFFIX = 11;

	public const int SERIALIZER_OPTIMIZE_BUFFERS = 12;

	public const int SERIALIZER_EXPANSION1 = 20;

	private static int SERIALIZER_VERSION = 20;

	private const string TargetParameterName = "TargetParameter";

	private List<StateMachineSerializer.Entry> entries = new List<StateMachineSerializer.Entry>();

	private class Entry
	{
		public static bool TrySerialize(StateMachine.Instance smi, BinaryWriter writer)
		{
			if (!smi.IsRunning())
			{
				return false;
			}
			int num = (int)writer.BaseStream.Position;
			writer.Write(0);
			writer.WriteKleiString(smi.GetType().FullName);
			writer.WriteKleiString(smi.serializationSuffix);
			writer.WriteKleiString(smi.GetCurrentState().name);
			int num2 = (int)writer.BaseStream.Position;
			writer.Write(0);
			int num3 = (int)writer.BaseStream.Position;
			Serializer.SerializeTypeless(smi, writer);
			if (smi.GetStateMachine().serializable == StateMachine.SerializeType.ParamsOnly || smi.GetStateMachine().serializable == StateMachine.SerializeType.Both_DEPRECATED)
			{
				StateMachine.Parameter.Context[] parameterContexts = smi.GetParameterContexts();
				writer.Write(parameterContexts.Length);
				foreach (StateMachine.Parameter.Context context in parameterContexts)
				{
					long num4 = (long)((int)writer.BaseStream.Position);
					writer.Write(0);
					long num5 = (long)((int)writer.BaseStream.Position);
					writer.WriteKleiString(context.GetType().FullName);
					writer.WriteKleiString(context.parameter.name);
					context.Serialize(writer);
					long num6 = (long)((int)writer.BaseStream.Position);
					writer.BaseStream.Position = num4;
					long num7 = num6 - num5;
					writer.Write((int)num7);
					writer.BaseStream.Position = num6;
				}
			}
			int num8 = (int)writer.BaseStream.Position - num3;
			if (num8 > 0)
			{
				int num9 = (int)writer.BaseStream.Position;
				writer.BaseStream.Position = (long)num2;
				writer.Write(num8);
				writer.BaseStream.Position = (long)num9;
				return true;
			}
			writer.BaseStream.Position = (long)num;
			writer.BaseStream.SetLength((long)num);
			return false;
		}

		public static StateMachineSerializer.Entry Deserialize(IReader reader, int serializerVersion)
		{
			StateMachineSerializer.Entry entry = new StateMachineSerializer.Entry();
			reader.ReadInt32();
			entry.version = serializerVersion;
			string text = reader.ReadKleiString();
			entry.type = Type.GetType(text);
			entry.typeSuffix = (StateMachineSerializer.DoesVersionHaveTypeSuffix(serializerVersion) ? reader.ReadKleiString() : null);
			entry.currentState = reader.ReadKleiString();
			int num = reader.ReadInt32();
			entry.entryData = new FastReader(reader.ReadBytes(num));
			if (entry.type == null)
			{
				return null;
			}
			return entry;
		}

		public bool Restore(StateMachine.Instance smi)
		{
			if (Manager.HasDeserializationMapping(smi.GetType()))
			{
				Deserializer.DeserializeTypeless(smi, this.entryData);
			}
			StateMachine.SerializeType serializable = smi.GetStateMachine().serializable;
			if (serializable == StateMachine.SerializeType.Never)
			{
				return false;
			}
			if ((serializable == StateMachine.SerializeType.Both_DEPRECATED || serializable == StateMachine.SerializeType.ParamsOnly) && !this.entryData.IsFinished)
			{
				StateMachine.Parameter.Context[] parameterContexts = smi.GetParameterContexts();
				int num = this.entryData.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					int num2 = this.entryData.ReadInt32();
					int position = this.entryData.Position;
					string text = this.entryData.ReadKleiString();
					text = text.Replace("Version=2.0.0.0", "Version=4.0.0.0");
					string text2 = this.entryData.ReadKleiString();
					foreach (StateMachine.Parameter.Context context in parameterContexts)
					{
						if (context.parameter.name == text2 && (this.version > 10 || !(context.parameter.GetType().Name == "TargetParameter")) && context.GetType().FullName == text)
						{
							context.Deserialize(this.entryData, smi);
							break;
						}
					}
					this.entryData.SkipBytes(num2 - (this.entryData.Position - position));
				}
			}
			if (serializable == StateMachine.SerializeType.Both_DEPRECATED || serializable == StateMachine.SerializeType.CurrentStateOnly_DEPRECATED)
			{
				StateMachine.BaseState state = smi.GetStateMachine().GetState(this.currentState);
				if (state != null)
				{
					smi.OnParamsDeserialized();
					smi.GoTo(state);
					return true;
				}
			}
			return false;
		}

		public int version;

		public Type type;

		public string typeSuffix;

		public string currentState;

		public FastReader entryData;
	}

	private class OldEntryV11
	{
		public OldEntryV11(int version, int dataPos, Type type, string typeSuffix, string currentState)
		{
			this.version = version;
			this.dataPos = dataPos;
			this.type = type;
			this.typeSuffix = typeSuffix;
			this.currentState = currentState;
		}

		public static List<StateMachineSerializer.Entry> DeserializeOldEntries(IReader reader, int serializerVersion)
		{
			Debug.Assert(serializerVersion < 12);
			List<StateMachineSerializer.OldEntryV11> list = StateMachineSerializer.OldEntryV11.ReadEntries(reader, serializerVersion);
			byte[] array = StateMachineSerializer.OldEntryV11.ReadEntryData(reader);
			List<StateMachineSerializer.Entry> list2 = new List<StateMachineSerializer.Entry>(list.Count);
			foreach (StateMachineSerializer.OldEntryV11 oldEntryV in list)
			{
				StateMachineSerializer.Entry entry = new StateMachineSerializer.Entry();
				entry.version = serializerVersion;
				entry.type = oldEntryV.type;
				entry.typeSuffix = oldEntryV.typeSuffix;
				entry.currentState = oldEntryV.currentState;
				entry.entryData = new FastReader(array);
				entry.entryData.SkipBytes(oldEntryV.dataPos);
				list2.Add(entry);
			}
			return list2;
		}

		private static StateMachineSerializer.OldEntryV11 Deserialize(IReader reader, int serializerVersion)
		{
			int num = reader.ReadInt32();
			int num2 = reader.ReadInt32();
			string text = reader.ReadKleiString();
			string text2 = (StateMachineSerializer.DoesVersionHaveTypeSuffix(serializerVersion) ? reader.ReadKleiString() : null);
			string text3 = reader.ReadKleiString();
			Type type = Type.GetType(text);
			if (type == null)
			{
				return null;
			}
			return new StateMachineSerializer.OldEntryV11(num, num2, type, text2, text3);
		}

		private static List<StateMachineSerializer.OldEntryV11> ReadEntries(IReader reader, int serializerVersion)
		{
			List<StateMachineSerializer.OldEntryV11> list = new List<StateMachineSerializer.OldEntryV11>();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				StateMachineSerializer.OldEntryV11 oldEntryV = StateMachineSerializer.OldEntryV11.Deserialize(reader, serializerVersion);
				if (oldEntryV != null)
				{
					list.Add(oldEntryV);
				}
			}
			return list;
		}

		private static byte[] ReadEntryData(IReader reader)
		{
			int num = reader.ReadInt32();
			return reader.ReadBytes(num);
		}

		public int version;

		public int dataPos;

		public Type type;

		public string typeSuffix;

		public string currentState;
	}
}
