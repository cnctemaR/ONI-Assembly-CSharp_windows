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

	[IgnoreFirst(1)]
	[DelimitedRecord(",")]
	[IgnoreEmptyLines]
	public class ElementAudioConfig
	{
		[FieldOrder(1)]
		public SimHashes elementID;

		[FieldOrder(2)]
		[FieldNullValue(AmbienceType.None)]
		[FieldOptional]
		public AmbienceType ambienceType;

		[FieldOptional]
		[FieldNullValue(SolidAmbienceType.None)]
		[FieldOrder(3)]
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

		[FieldOptional]
		[FieldOrder(7)]
		[FieldNullValue("")]
		public string floorEventAudioCategory;
	}
}
