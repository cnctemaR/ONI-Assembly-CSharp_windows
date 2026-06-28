using System;
using System.Collections.Generic;
using UnityEngine;

internal class SpriteSheetAnimator
{
	public SpriteSheetAnimator(SpriteSheet sheet)
	{
		this.sheet = sheet;
		this.mesh = new Mesh();
		this.mesh.name = "SpriteSheetAnimator";
		this.mesh.MarkDynamic();
		this.materialProperties = new MaterialPropertyBlock();
		this.materialProperties.SetTexture("_MainTex", sheet.texture);
	}

	public void Play(Vector3 pos, Quaternion rotation, Vector2 size, Color colour)
	{
		if (rotation == Quaternion.identity)
		{
			this.anims.Add(new SpriteSheetAnimator.AnimInfo
			{
				elapsedTime = 0f,
				pos = pos,
				rotation = rotation,
				size = size,
				colour = colour
			});
		}
		else
		{
			this.rotatedAnims.Add(new SpriteSheetAnimator.AnimInfo
			{
				elapsedTime = 0f,
				pos = pos,
				rotation = rotation,
				size = size,
				colour = colour
			});
		}
	}

	private void GetUVs(int frame, out Vector2 uv_bl, out Vector2 uv_br, out Vector2 uv_tl, out Vector2 uv_tr)
	{
		int num = frame / this.sheet.numXFrames;
		int num2 = frame % this.sheet.numXFrames;
		float num3 = (float)num2 * this.sheet.uvFrameSize.x;
		float num4 = (float)(num2 + 1) * this.sheet.uvFrameSize.x;
		float num5 = 1f - (float)(num + 1) * this.sheet.uvFrameSize.y;
		float num6 = 1f - (float)num * this.sheet.uvFrameSize.y;
		uv_bl = new Vector2(num3, num5);
		uv_br = new Vector2(num4, num5);
		uv_tl = new Vector2(num3, num6);
		uv_tr = new Vector2(num4, num6);
	}

	public void UpdateAnims(float dt)
	{
		this.UpdateAnims(dt, this.anims);
		this.UpdateAnims(dt, this.rotatedAnims);
	}

	private void UpdateAnims(float dt, IList<SpriteSheetAnimator.AnimInfo> anims)
	{
		int num = anims.Count;
		int i = 0;
		while (i < num)
		{
			SpriteSheetAnimator.AnimInfo animInfo = anims[i];
			animInfo.elapsedTime += dt;
			int num2 = Mathf.Min(this.sheet.numFrames, (int)(animInfo.elapsedTime / 0.033333335f));
			if (num2 >= this.sheet.numFrames)
			{
				num--;
				anims[i] = anims[num];
				anims.RemoveAt(num);
			}
			else
			{
				anims[i] = animInfo;
				i++;
			}
		}
	}

	public void Render(Vector3 root_pos, List<Vector3> vertices, List<Vector2> uvs, List<Color32> colours, List<int> indices)
	{
		this.mesh.Clear();
		vertices.Clear();
		uvs.Clear();
		indices.Clear();
		colours.Clear();
		int num = this.anims.Count;
		for (int i = 0; i < num; i++)
		{
			SpriteSheetAnimator.AnimInfo animInfo = this.anims[i];
			Vector2 vector = animInfo.size * 0.5f;
			Vector3 vector2 = -vector;
			Vector3 vector3 = new Vector2(vector.x, -vector.y);
			Vector3 vector4 = new Vector2(-vector.x, vector.y);
			Vector3 vector5 = vector;
			vertices.Add(animInfo.pos + vector2);
			vertices.Add(animInfo.pos + vector3);
			vertices.Add(animInfo.pos + vector5);
			vertices.Add(animInfo.pos + vector4);
			int num2 = Mathf.Min(this.sheet.numFrames - 1, (int)(animInfo.elapsedTime / 0.033333335f));
			Vector2 vector6;
			Vector2 vector7;
			Vector2 vector8;
			Vector2 vector9;
			this.GetUVs(num2, out vector6, out vector7, out vector8, out vector9);
			uvs.Add(vector6);
			uvs.Add(vector7);
			uvs.Add(vector9);
			uvs.Add(vector8);
			colours.Add(animInfo.colour);
			colours.Add(animInfo.colour);
			colours.Add(animInfo.colour);
			colours.Add(animInfo.colour);
			int num3 = i * 4;
			indices.Add(num3);
			indices.Add(num3 + 1);
			indices.Add(num3 + 2);
			indices.Add(num3);
			indices.Add(num3 + 2);
			indices.Add(num3 + 3);
		}
		num = this.rotatedAnims.Count;
		for (int j = 0; j < num; j++)
		{
			SpriteSheetAnimator.AnimInfo animInfo2 = this.rotatedAnims[j];
			Vector2 vector10 = animInfo2.size * 0.5f;
			Vector3 vector11 = animInfo2.rotation * -vector10;
			Vector3 vector12 = animInfo2.rotation * new Vector2(vector10.x, -vector10.y);
			Vector3 vector13 = animInfo2.rotation * new Vector2(-vector10.x, vector10.y);
			Vector3 vector14 = animInfo2.rotation * vector10;
			vertices.Add(animInfo2.pos + vector11);
			vertices.Add(animInfo2.pos + vector12);
			vertices.Add(animInfo2.pos + vector14);
			vertices.Add(animInfo2.pos + vector13);
			int num4 = Mathf.Min(this.sheet.numFrames - 1, (int)(animInfo2.elapsedTime / 0.033333335f));
			Vector2 vector15;
			Vector2 vector16;
			Vector2 vector17;
			Vector2 vector18;
			this.GetUVs(num4, out vector15, out vector16, out vector17, out vector18);
			uvs.Add(vector15);
			uvs.Add(vector16);
			uvs.Add(vector18);
			uvs.Add(vector17);
			colours.Add(animInfo2.colour);
			colours.Add(animInfo2.colour);
			colours.Add(animInfo2.colour);
			colours.Add(animInfo2.colour);
			int num5 = j * 4;
			indices.Add(num5);
			indices.Add(num5 + 1);
			indices.Add(num5 + 2);
			indices.Add(num5);
			indices.Add(num5 + 2);
			indices.Add(num5 + 3);
		}
		this.mesh.SetVertices(vertices);
		this.mesh.SetUVs(0, uvs);
		this.mesh.SetColors(colours);
		this.mesh.SetTriangles(indices, 0);
		Graphics.DrawMesh(this.mesh, root_pos, Quaternion.identity, this.sheet.material, this.sheet.renderLayer, null, 0, this.materialProperties);
	}

	private SpriteSheet sheet;

	private Mesh mesh;

	private MaterialPropertyBlock materialProperties;

	private List<SpriteSheetAnimator.AnimInfo> anims = new List<SpriteSheetAnimator.AnimInfo>();

	private List<SpriteSheetAnimator.AnimInfo> rotatedAnims = new List<SpriteSheetAnimator.AnimInfo>();

	private struct AnimInfo
	{
		public int frame;

		public float elapsedTime;

		public Vector3 pos;

		public Quaternion rotation;

		public Vector2 size;

		public Color32 colour;
	}
}
