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

		[FieldNullValue(AmbienceType.None)]
		[FieldOptional]
		[FieldOrder(2)]
		public AmbienceType ambienceType;

		[FieldOrder(3)]
		[FieldOptional]
		[FieldNullValue(SolidAmbienceType.None)]
		public SolidAmbienceType solidAmbienceType;

		[FieldNullValue("")]
		[FieldOrder(4)]
		[FieldOptional]
		public string miningSound;

		[FieldOrder(5)]
		[FieldNullValue("")]
		[FieldOptional]
		public string miningBreakSound;

		[FieldOptional]
		[FieldNullValue("")]
		[FieldOrder(6)]
		public string oreBumpSound;

		[FieldNullValue("")]
		[FieldOrder(7)]
		[FieldOptional]
		public string floorEventAudioCategory;
	}
}
