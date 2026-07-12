using System;
using UnityEngine;

public class MotdData_Box
{
	public bool ShouldDisplay()
	{
		long num = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		return num >= this.startTime && this.finishTime >= num;
	}

	public string category;

	public string guid;

	public long startTime;

	public long finishTime;

	public string title;

	public string text;

	public string image;

	public string href;

	public Texture2D resolvedImage;

	public bool resolvedImageIsFromDisk;
}
