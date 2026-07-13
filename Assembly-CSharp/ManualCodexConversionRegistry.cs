using System;
using System.Collections.Generic;

public class ManualCodexConversionRegistry
{
	public static List<ManualCodexConversionRegistry.ManualConversionEntry> GetConversionsForGivenConverter(Tag converter)
	{
		if (!ManualCodexConversionRegistry.conversionsByTag.ContainsKey(converter))
		{
			return null;
		}
		List<ManualCodexConversionRegistry.ManualConversionEntry> list = new List<ManualCodexConversionRegistry.ManualConversionEntry>();
		foreach (ManualCodexConversionRegistry.ManualConversionEntry manualConversionEntry in ManualCodexConversionRegistry.conversionsByTag[converter])
		{
			if (manualConversionEntry.converter != null && manualConversionEntry.converter.first == converter)
			{
				list.Add(manualConversionEntry);
			}
		}
		return list;
	}

	public static List<ManualCodexConversionRegistry.ManualConversionEntry> GetProducersForGivenOutput(Tag output)
	{
		if (!ManualCodexConversionRegistry.conversionsByTag.ContainsKey(output))
		{
			return null;
		}
		List<ManualCodexConversionRegistry.ManualConversionEntry> list = new List<ManualCodexConversionRegistry.ManualConversionEntry>();
		foreach (ManualCodexConversionRegistry.ManualConversionEntry manualConversionEntry in ManualCodexConversionRegistry.conversionsByTag[output])
		{
			if (manualConversionEntry.output != null && manualConversionEntry.output.first == output)
			{
				list.Add(manualConversionEntry);
			}
		}
		return list;
	}

	public static List<ManualCodexConversionRegistry.ManualConversionEntry> GetConsumersForGivenInput(Tag input)
	{
		if (!ManualCodexConversionRegistry.conversionsByTag.ContainsKey(input))
		{
			return null;
		}
		List<ManualCodexConversionRegistry.ManualConversionEntry> list = new List<ManualCodexConversionRegistry.ManualConversionEntry>();
		foreach (ManualCodexConversionRegistry.ManualConversionEntry manualConversionEntry in ManualCodexConversionRegistry.conversionsByTag[input])
		{
			if (manualConversionEntry.input != null && manualConversionEntry.input.first == input)
			{
				list.Add(manualConversionEntry);
			}
		}
		return list;
	}

	public static void AddConversion(Tag inputTag, float inputAmount, Tag converterTag, float converterAmount, Tag outputTag, float outputAmount, string headerDescription, Func<Tag, float, bool, string> inputCustomFormating = null, Func<Tag, float, bool, string> converterCustomFormating = null, Func<Tag, float, bool, string> outputCustomFormating = null)
	{
		ManualCodexConversionRegistry.ManualConversionEntry manualConversionEntry = new ManualCodexConversionRegistry.ManualConversionEntry(new global::Tuple<Tag, float>(converterTag, converterAmount), new global::Tuple<Tag, float>(inputTag, inputAmount), new global::Tuple<Tag, float>(outputTag, outputAmount), headerDescription, inputCustomFormating, converterCustomFormating, outputCustomFormating);
		if (converterTag != null)
		{
			List<ManualCodexConversionRegistry.ManualConversionEntry> list;
			if (!ManualCodexConversionRegistry.conversionsByTag.TryGetValue(converterTag, out list))
			{
				list = new List<ManualCodexConversionRegistry.ManualConversionEntry>();
				ManualCodexConversionRegistry.conversionsByTag[converterTag] = list;
			}
			ManualCodexConversionRegistry.conversionsByTag[converterTag].Add(manualConversionEntry);
		}
		if (inputTag != null)
		{
			List<ManualCodexConversionRegistry.ManualConversionEntry> list2;
			if (!ManualCodexConversionRegistry.conversionsByTag.TryGetValue(inputTag, out list2))
			{
				list2 = new List<ManualCodexConversionRegistry.ManualConversionEntry>();
				ManualCodexConversionRegistry.conversionsByTag[inputTag] = list2;
			}
			ManualCodexConversionRegistry.conversionsByTag[inputTag].Add(manualConversionEntry);
		}
		if (outputTag != null)
		{
			List<ManualCodexConversionRegistry.ManualConversionEntry> list3;
			if (!ManualCodexConversionRegistry.conversionsByTag.TryGetValue(outputTag, out list3))
			{
				list3 = new List<ManualCodexConversionRegistry.ManualConversionEntry>();
				ManualCodexConversionRegistry.conversionsByTag[outputTag] = list3;
			}
			ManualCodexConversionRegistry.conversionsByTag[outputTag].Add(manualConversionEntry);
		}
	}

	public static Dictionary<Tag, List<ManualCodexConversionRegistry.ManualConversionEntry>> conversionsByTag = new Dictionary<Tag, List<ManualCodexConversionRegistry.ManualConversionEntry>>();

	public class ManualConversionEntry
	{
		public ManualConversionEntry(global::Tuple<Tag, float> converter, global::Tuple<Tag, float> input, global::Tuple<Tag, float> output, string headerDescription, Func<Tag, float, bool, string> inputCustomFormating = null, Func<Tag, float, bool, string> converterCustomFormating = null, Func<Tag, float, bool, string> outputCustomFormating = null)
		{
			this.converter = converter;
			this.input = input;
			this.output = output;
			this.headerDescription = headerDescription;
			this.inputCustomFormating = inputCustomFormating;
			this.converterCustomFormating = converterCustomFormating;
			this.outputCustomFormating = outputCustomFormating;
		}

		public string headerDescription;

		public global::Tuple<Tag, float> converter;

		public global::Tuple<Tag, float> input;

		public global::Tuple<Tag, float> output;

		public Func<Tag, float, bool, string> inputCustomFormating;

		public Func<Tag, float, bool, string> converterCustomFormating;

		public Func<Tag, float, bool, string> outputCustomFormating;
	}
}
