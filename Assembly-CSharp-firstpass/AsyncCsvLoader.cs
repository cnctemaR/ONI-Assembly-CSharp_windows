using System;
using UnityEngine;

public abstract class AsyncCsvLoader<LoaderType, CsvEntryType> : GlobalAsyncLoader<LoaderType> where LoaderType : class where CsvEntryType : Resource, new()
{
	public AsyncCsvLoader(TextAsset asset)
	{
		this.text = asset.text;
		this.name = asset.name;
	}

	public override void Run()
	{
		this.entries = new ResourceLoader<CsvEntryType>(this.text, this.name).resources.ToArray();
		this.text = null;
		this.name = null;
	}

	private string text;

	private string name;

	public CsvEntryType[] entries;
}
