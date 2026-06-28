using System;
using System.Collections.Generic;
using UnityEngine;

public class TexturePagePool
{
	public TexturePagePool()
	{
		this.activePages[0] = new List<TexturePage>();
		this.activePages[1] = new List<TexturePage>();
	}

	private int Clamp(int value)
	{
		int num;
		if (value == 0)
		{
			num = 32;
		}
		else if (value % 32 == 0)
		{
			num = value;
		}
		else
		{
			num = 32 + value / 32 * 32;
		}
		return num;
	}

	public TexturePage Alloc(string name, int width, int height, TextureFormat format)
	{
		int num = this.Clamp(width);
		int num2 = this.Clamp(height);
		int num3 = Time.frameCount % 2;
		foreach (TexturePage texturePage in this.activePages[num3])
		{
			this.freePages.Add(texturePage);
		}
		this.activePages[num3].Clear();
		for (int i = 0; i < this.freePages.Count; i++)
		{
			TexturePage texturePage2 = this.freePages[i];
			if (texturePage2.width == num && texturePage2.height == num2 && texturePage2.format == format)
			{
				this.freePages.RemoveAt(i);
				texturePage2.SetName(name);
				return texturePage2;
			}
		}
		return new TexturePage(name, num, num2, format);
	}

	public void Release(TexturePage page)
	{
		int num = (Time.frameCount + 1) % 2;
		this.activePages[num].Add(page);
	}

	private List<TexturePage>[] activePages = new List<TexturePage>[2];

	private List<TexturePage> freePages = new List<TexturePage>();
}
