using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetAnimator
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

	public int GetFrameFromElapsedTime(float elapsed_time)
	{
		return Mathf.Min(this.sheet.numFrames, (int)(elapsed_time / 0.033333335f));
	}

	public int GetFrameFromElapsedTimeLooping(float elapsed_time)
	{
		int num = (int)(elapsed_time / 0.033333335f);
		if (num > this.sheet.numFrames)
		{
			num %= this.sheet.numFrames;
		}
		return num;
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
			animInfo.frame = Mathf.Min(this.sheet.numFrames, (int)(animInfo.elapsedTime / 0.033333335f));
			if (animInfo.frame >= this.sheet.numFrames)
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

	public void Render(List<SpriteSheetAnimator.AnimInfo> anim_infos, bool apply_rotation)
	{
		List<Vector3> list = ListPool<Vector3, SpriteSheetAnimManager>.Allocate();
		List<Vector2> list2 = ListPool<Vector2, SpriteSheetAnimManager>.Allocate();
		List<Color32> list3 = ListPool<Color32, SpriteSheetAnimManager>.Allocate();
		List<int> list4 = ListPool<int, SpriteSheetAnimManager>.Allocate();
		this.mesh.Clear();
		if (apply_rotation)
		{
			int count = anim_infos.Count;
			for (int i = 0; i < count; i++)
			{
				SpriteSheetAnimator.AnimInfo animInfo = anim_infos[i];
				Vector2 vector = animInfo.size * 0.5f;
				Vector3 vector2 = animInfo.rotation * -vector;
				Vector3 vector3 = animInfo.rotation * new Vector2(vector.x, -vector.y);
				Vector3 vector4 = animInfo.rotation * new Vector2(-vector.x, vector.y);
				Vector3 vector5 = animInfo.rotation * vector;
				list.Add(animInfo.pos + vector2);
				list.Add(animInfo.pos + vector3);
				list.Add(animInfo.pos + vector5);
				list.Add(animInfo.pos + vector4);
				Vector2 vector6;
				Vector2 vector7;
				Vector2 vector8;
				Vector2 vector9;
				this.GetUVs(animInfo.frame, out vector6, out vector7, out vector8, out vector9);
				list2.Add(vector6);
				list2.Add(vector7);
				list2.Add(vector9);
				list2.Add(vector8);
				list3.Add(animInfo.colour);
				list3.Add(animInfo.colour);
				list3.Add(animInfo.colour);
				list3.Add(animInfo.colour);
				int num = i * 4;
				list4.Add(num);
				list4.Add(num + 1);
				list4.Add(num + 2);
				list4.Add(num);
				list4.Add(num + 2);
				list4.Add(num + 3);
			}
		}
		else
		{
			int count2 = anim_infos.Count;
			for (int j = 0; j < count2; j++)
			{
				SpriteSheetAnimator.AnimInfo animInfo2 = anim_infos[j];
				Vector2 vector10 = animInfo2.size * 0.5f;
				Vector3 vector11 = -vector10;
				Vector3 vector12 = new Vector2(vector10.x, -vector10.y);
				Vector3 vector13 = new Vector2(-vector10.x, vector10.y);
				Vector3 vector14 = vector10;
				list.Add(animInfo2.pos + vector11);
				list.Add(animInfo2.pos + vector12);
				list.Add(animInfo2.pos + vector14);
				list.Add(animInfo2.pos + vector13);
				Vector2 vector15;
				Vector2 vector16;
				Vector2 vector17;
				Vector2 vector18;
				this.GetUVs(animInfo2.frame, out vector15, out vector16, out vector17, out vector18);
				list2.Add(vector15);
				list2.Add(vector16);
				list2.Add(vector18);
				list2.Add(vector17);
				list3.Add(animInfo2.colour);
				list3.Add(animInfo2.colour);
				list3.Add(animInfo2.colour);
				list3.Add(animInfo2.colour);
				int num2 = j * 4;
				list4.Add(num2);
				list4.Add(num2 + 1);
				list4.Add(num2 + 2);
				list4.Add(num2);
				list4.Add(num2 + 2);
				list4.Add(num2 + 3);
			}
		}
		this.mesh.SetVertices(list);
		this.mesh.SetUVs(0, list2);
		this.mesh.SetColors(list3);
		this.mesh.SetTriangles(list4, 0);
		Graphics.DrawMesh(this.mesh, Vector3.zero, Quaternion.identity, this.sheet.material, this.sheet.renderLayer, null, 0, this.materialProperties);
		ListPool<int, SpriteSheetAnimManager>.Free(list4);
		ListPool<Color32, SpriteSheetAnimManager>.Free(list3);
		ListPool<Vector2, SpriteSheetAnimManager>.Free(list2);
		ListPool<Vector3, SpriteSheetAnimManager>.Free(list);
	}

	public void Render()
	{
		this.Render(this.anims, false);
		this.Render(this.rotatedAnims, true);
	}

	private SpriteSheet sheet;

	private Mesh mesh;

	private MaterialPropertyBlock materialProperties;

	private List<SpriteSheetAnimator.AnimInfo> anims = new List<SpriteSheetAnimator.AnimInfo>();

	private List<SpriteSheetAnimator.AnimInfo> rotatedAnims = new List<SpriteSheetAnimator.AnimInfo>();

	public struct AnimInfo
	{
		public int frame;

		public float elapsedTime;

		public Vector3 pos;

		public Quaternion rotation;

		public Vector2 size;

		public Color32 colour;
	}
}
