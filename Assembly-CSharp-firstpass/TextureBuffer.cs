using System;
using UnityEngine;

[Serializable]
public class TextureBuffer
{
	public TextureBuffer(string name, int width, int height, TextureFormat format, FilterMode filter_mode, TexturePagePool pool)
	{
		this.name = name;
		this.format = format;
		this.pool = pool;
		this.texture = new RenderTexture(width, height, 0, TextureUtil.GetRenderTextureFormat(format));
		this.texture.name = name;
		this.texture.filterMode = filter_mode;
		this.texture.wrapMode = TextureWrapMode.Clamp;
		this.material = new Material(Shader.Find("Klei/TexturePage"));
	}

	public TextureRegion Lock(int x, int y, int width, int height)
	{
		TexturePage texturePage = this.pool.Alloc(this.name, width, height, this.format);
		return new TextureRegion(x, y, texturePage, this);
	}

	public void Unlock(TextureRegion region)
	{
		region.page.texture.LoadRawTextureData(region.page.bytes);
		region.page.texture.Apply();
		this.material.SetVector("_Region", new Vector4((float)region.x / (float)this.texture.width, (float)region.y / (float)this.texture.height, (float)(region.x + region.page.width) / (float)this.texture.width, (float)(region.y + region.page.height) / (float)this.texture.height));
		this.material.SetTexture("_MainTex", region.page.texture);
		Graphics.Blit(region.page.texture, this.texture, this.material);
		this.pool.Release(region.page);
	}

	public string name;

	public int bytesPerPixel;

	public TexturePagePool pool;

	public TextureFormat format;

	public RenderTexture texture;

	public Material material;
}
