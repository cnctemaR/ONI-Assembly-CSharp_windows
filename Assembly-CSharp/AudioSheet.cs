using System;
using FileHelpers;
using UnityEngine;

[Serializable]
public class AudioSheet
{
	public void Load()
	{
		FileHelperEngine fileHelperEngine = new FileHelperEngine(typeof(AudioSheet.SoundInfo));
		this.soundInfos = (AudioSheet.SoundInfo[])fileHelperEngine.ReadString(this.asset.text);
	}

	public TextAsset asset;

	public string defaultType;

	public AudioSheet.SoundInfo[] soundInfos;

	[IgnoreFirst(1)]
	[IgnoreEmptyLines]
	[DelimitedRecord(",")]
	[Serializable]
	public class SoundInfo
	{
		public string File;

		public string Anim;

		public string Type;

		[FieldNullValue(0f)]
		public float MinInterval;

		[FieldOptional]
		public string Name0;

		[FieldOptional]
		[FieldNullValue(0)]
		public int Frame0;

		[FieldOptional]
		public string Name1;

		[FieldOptional]
		[FieldNullValue(0)]
		public int Frame1;

		[FieldOptional]
		public string Name2;

		[FieldNullValue(0)]
		[FieldOptional]
		public int Frame2;

		[FieldOptional]
		public string Name3;

		[FieldOptional]
		[FieldNullValue(0)]
		public int Frame3;

		[FieldOptional]
		public string Name4;

		[FieldNullValue(0)]
		[FieldOptional]
		public int Frame4;

		[FieldOptional]
		public string Name5;

		[FieldOptional]
		[FieldNullValue(0)]
		public int Frame5;

		[FieldOptional]
		public string Name6;

		[FieldNullValue(0)]
		[FieldOptional]
		public int Frame6;

		[FieldOptional]
		public string Name7;

		[FieldNullValue(0)]
		[FieldOptional]
		public int Frame7;

		[FieldOptional]
		public string Name8;

		[FieldNullValue(0)]
		[FieldOptional]
		public int Frame8;

		[FieldOptional]
		public string Name9;

		[FieldNullValue(0)]
		[FieldOptional]
		public int Frame9;

		[FieldOptional]
		public string Name10;

		[FieldOptional]
		[FieldNullValue(0)]
		public int Frame10;

		[FieldOptional]
		public string Name11;

		[FieldNullValue(0)]
		[FieldOptional]
		public int Frame11;
	}
}
