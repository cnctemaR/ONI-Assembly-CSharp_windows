using System;
using UnityEngine;

public class TexturePage
{
	public TexturePage(string name, int width, int height, TextureFormat format)
	{
		this.width = width;
		this.height = height;
		this.format = format;
		this.texture = new Texture2D(width, height, format, false);
		this.texture.name = name;
		this.texture.filterMode = FilterMode.Point;
		this.texture.wrapMode = TextureWrapMode.Clamp;
		this.SetName(name);
	}

	public void SetName(string name)
	{
		this.texture.name = name;
	}

	public int width;

	public int height;

	public TextureFormat format;

	public TexturePagePool pool;

	public Texture2D texture;
}
