using System;
using FileHelpers;

public class ElementsAudio
{
	public static ElementsAudio Instance
	{
		get
		{
			if (ElementsAudio._instance == null)
			{
				ElementsAudio._instance = new ElementsAudio();
			}
			return ElementsAudio._instance;
		}
	}

	public void LoadData(string csvData)
	{
		FileHelperEngine fileHelperEngine = new FileHelperEngine(typeof(ElementsAudio.ElementAudioConfig));
		this.elementAudioConfigs = fileHelperEngine.ReadString(csvData) as ElementsAudio.ElementAudioConfig[];
	}

	public ElementsAudio.ElementAudioConfig GetConfigForElement(SimHashes id)
	{
		if (this.elementAudioConfigs != null)
		{
			for (int i = 0; i < this.elementAudioConfigs.Length; i++)
			{
				if (this.elementAudioConfigs[i].elementID == id)
				{
					return this.elementAudioConfigs[i];
				}
			}
		}
		return null;
	}

	private static ElementsAudio _instance;

	private ElementsAudio.ElementAudioConfig[] elementAudioConfigs;

	[IgnoreEmptyLines]
	[IgnoreFirst(1)]
	[DelimitedRecord(",")]
	public class ElementAudioConfig
	{
		[FieldOrder(1)]
		public SimHashes elementID;

		[FieldOrder(2)]
		[FieldNullValue(AmbienceType.None)]
		[FieldOptional]
		public AmbienceType ambienceType;

		[FieldNullValue(SolidAmbienceType.None)]
		[FieldOptional]
		[FieldOrder(3)]
		public SolidAmbienceType solidAmbienceType;

		[FieldOptional]
		[FieldNullValue("")]
		[FieldOrder(4)]
		public string miningSound;

		[FieldOptional]
		[FieldNullValue("")]
		[FieldOrder(5)]
		public string miningBreakSound;

		[FieldNullValue("")]
		[FieldOptional]
		[FieldOrder(6)]
		public string oreBumpSound;

		[FieldNullValue("")]
		[FieldOrder(7)]
		[FieldOptional]
		public string floorEventAudioCategory;
	}
}
