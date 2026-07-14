using System;
using UnityEngine;

public class PoopData
{
	public PoopData(bool skipSpawningPoop, Storage storage, string popupMessage = null, Sprite popupIcon = null)
	{
		this.skipSpawningPoop = skipSpawningPoop;
		this.storage = storage;
		this.popupMessage = popupMessage;
		this.popupIcon = popupIcon;
	}

	public bool skipSpawningPoop;

	public Storage storage;

	public string popupMessage;

	public Sprite popupIcon;
}
