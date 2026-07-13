using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro
{
	[DisallowMultipleComponent]
	public class TMP_SpriteAnimator : MonoBehaviour
	{
		private void Awake()
		{
			this.m_TextComponent = base.GetComponent<TMP_Text>();
		}

		private void OnEnable()
		{
		}

		private void OnDisable()
		{
		}

		public void StopAllAnimations()
		{
			base.StopAllCoroutines();
			this.m_animations.Clear();
		}

		public void DoSpriteAnimation(int currentCharacter, TMP_SpriteAsset spriteAsset, int start, int end, int framerate)
		{
			bool flag;
			if (!this.m_animations.TryGetValue(currentCharacter, out flag))
			{
				base.StartCoroutine(this.DoSpriteAnimationInternal(currentCharacter, spriteAsset, start, end, framerate));
				this.m_animations.Add(currentCharacter, true);
			}
		}

		private IEnumerator DoSpriteAnimationInternal(int currentCharacter, TMP_SpriteAsset spriteAsset, int start, int end, int framerate)
		{
			if (this.m_TextComponent == null)
			{
				yield break;
			}
			yield return null;
			int currentFrame = start;
			if (end > spriteAsset.spriteCharacterTable.Count)
			{
				end = spriteAsset.spriteCharacterTable.Count - 1;
			}
			TMP_CharacterInfo charInfo = this.m_TextComponent.textInfo.characterInfo[currentCharacter];
			int materialIndex = charInfo.materialReferenceIndex;
			int vertexIndex = charInfo.vertexIndex;
			TMP_MeshInfo meshInfo = this.m_TextComponent.textInfo.meshInfo[materialIndex];
			float baseSpriteScale = spriteAsset.spriteCharacterTable[start].scale * spriteAsset.spriteCharacterTable[start].glyph.scale;
			float elapsedTime = 0f;
			float targetTime = 1f / (float)Mathf.Abs(framerate);
			for (;;)
			{
				if (elapsedTime > targetTime)
				{
					elapsedTime = 0f;
					uint character = (uint)this.m_TextComponent.textInfo.characterInfo[currentCharacter].character;
					if (character == 3U || character == 8230U)
					{
						break;
					}
					TMP_SpriteCharacter tmp_SpriteCharacter = spriteAsset.spriteCharacterTable[currentFrame];
					Vector3[] vertices = meshInfo.vertices;
					Vector2 vector = new Vector2(charInfo.origin, charInfo.baseLine);
					float num = charInfo.scale / baseSpriteScale * tmp_SpriteCharacter.scale * tmp_SpriteCharacter.glyph.scale;
					Vector3 vector2 = new Vector3(vector.x + tmp_SpriteCharacter.glyph.metrics.horizontalBearingX * num, vector.y + (tmp_SpriteCharacter.glyph.metrics.horizontalBearingY - tmp_SpriteCharacter.glyph.metrics.height) * num);
					Vector3 vector3 = new Vector3(vector2.x, vector.y + tmp_SpriteCharacter.glyph.metrics.horizontalBearingY * num);
					Vector3 vector4 = new Vector3(vector.x + (tmp_SpriteCharacter.glyph.metrics.horizontalBearingX + tmp_SpriteCharacter.glyph.metrics.width) * num, vector3.y);
					Vector3 vector5 = new Vector3(vector4.x, vector2.y);
					vertices[vertexIndex] = vector2;
					vertices[vertexIndex + 1] = vector3;
					vertices[vertexIndex + 2] = vector4;
					vertices[vertexIndex + 3] = vector5;
					Vector4[] uvs = meshInfo.uvs0;
					Vector2 vector6 = new Vector2((float)tmp_SpriteCharacter.glyph.glyphRect.x / (float)spriteAsset.spriteSheet.width, (float)tmp_SpriteCharacter.glyph.glyphRect.y / (float)spriteAsset.spriteSheet.height);
					Vector2 vector7 = new Vector2(vector6.x, (float)(tmp_SpriteCharacter.glyph.glyphRect.y + tmp_SpriteCharacter.glyph.glyphRect.height) / (float)spriteAsset.spriteSheet.height);
					Vector2 vector8 = new Vector2((float)(tmp_SpriteCharacter.glyph.glyphRect.x + tmp_SpriteCharacter.glyph.glyphRect.width) / (float)spriteAsset.spriteSheet.width, vector7.y);
					Vector2 vector9 = new Vector2(vector8.x, vector6.y);
					uvs[vertexIndex] = vector6;
					uvs[vertexIndex + 1] = vector7;
					uvs[vertexIndex + 2] = vector8;
					uvs[vertexIndex + 3] = vector9;
					meshInfo.mesh.vertices = vertices;
					meshInfo.mesh.SetUVs(0, uvs);
					this.m_TextComponent.UpdateGeometry(meshInfo.mesh, materialIndex);
					if (framerate > 0)
					{
						if (currentFrame < end)
						{
							currentFrame++;
						}
						else
						{
							currentFrame = start;
						}
					}
					else if (currentFrame > start)
					{
						currentFrame--;
					}
					else
					{
						currentFrame = end;
					}
				}
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			this.m_animations.Remove(currentCharacter);
			yield break;
			yield break;
		}

		private Dictionary<int, bool> m_animations = new Dictionary<int, bool>(16);

		private TMP_Text m_TextComponent;
	}
}
