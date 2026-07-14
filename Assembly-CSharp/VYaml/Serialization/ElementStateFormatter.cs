using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class ElementStateFormatter : IYamlFormatter<Element.State>, IYamlFormatter
	{
		public void Serialize(ref Utf8YamlEmitter emitter, Element.State value, YamlSerializationContext context)
		{
			Element.State state = value & Element.State.Solid;
			List<string> list = new List<string> { state.ToString() };
			foreach (ValueTuple<Element.State, string> valueTuple in ElementStateFormatter.Flags)
			{
				Element.State item = valueTuple.Item1;
				string item2 = valueTuple.Item2;
				if ((value & item) != Element.State.Vacuum)
				{
					list.Add(item2);
				}
			}
			emitter.WriteString(string.Join(", ", list), ScalarStyle.Any);
		}

		public Element.State Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return Element.State.Vacuum;
			}
			string text = parser.ReadScalarAsString();
			if (text == null)
			{
				return Element.State.Vacuum;
			}
			Element.State state = Element.State.Vacuum;
			string[] array = text.Split(',', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i].Trim();
				Element.State state2;
				if (Enum.TryParse<Element.State>(text2, true, out state2))
				{
					state |= state2;
				}
				else
				{
					Debug.LogWarning("ElementStateFormatter: Unknown state flag '" + text2 + "'");
				}
			}
			return state;
		}

		public static readonly ElementStateFormatter Instance = new ElementStateFormatter();

		[TupleElementNames(new string[] { "flag", "name" })]
		private static readonly ValueTuple<Element.State, string>[] Flags = new ValueTuple<Element.State, string>[]
		{
			new ValueTuple<Element.State, string>(Element.State.TemperatureInsulated, "TemperatureInsulated"),
			new ValueTuple<Element.State, string>(Element.State.Unstable, "Unstable"),
			new ValueTuple<Element.State, string>(Element.State.Unbreakable, "Unbreakable")
		};
	}
}
