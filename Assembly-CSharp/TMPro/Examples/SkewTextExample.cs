using System;
using System.Collections;
using UnityEngine;

namespace TMPro.Examples
{
	public class SkewTextExample : MonoBehaviour
	{
		private void Awake()
		{
			this.m_TextComponent = base.gameObject.GetComponent<TMP_Text>();
		}

		private void Start()
		{
			base.StartCoroutine(this.WarpText());
		}

		private AnimationCurve CopyAnimationCurve(AnimationCurve curve)
		{
			return new AnimationCurve
			{
				keys = curve.keys
			};
		}

		private IEnumerator WarpText()
		{
			this.VertexCurve.preWrapMode = WrapMode.Once;
			this.VertexCurve.postWrapMode = WrapMode.Once;
			this.m_TextComponent.havePropertiesChanged = true;
			this.CurveScale *= 10f;
			float old_CurveScale = this.CurveScale;
			float old_ShearValue = this.ShearAmount;
			AnimationCurve old_curve = this.CopyAnimationCurve(this.VertexCurve);
			for (;;)
			{
				if (!this.m_TextComponent.havePropertiesChanged && old_CurveScale == this.CurveScale && old_curve.keys[1].value == this.VertexCurve.keys[1].value && old_ShearValue == this.ShearAmount)
				{
					yield return null;
				}
				else
				{
					old_CurveScale = this.CurveScale;
					old_curve = this.CopyAnimationCurve(this.VertexCurve);
					old_ShearValue = this.ShearAmount;
					this.m_TextComponent.ForceMeshUpdate(false, false);
					TMP_TextInfo textInfo = this.m_TextComponent.textInfo;
					int characterCount = textInfo.characterCount;
					if (characterCount != 0)
					{
						float x = this.m_TextComponent.bounds.min.x;
						float x2 = this.m_TextComponent.bounds.max.x;
						for (int i = 0; i < characterCount; i++)
						{
							if (textInfo.characterInfo[i].isVisible)
							{
								int vertexIndex = textInfo.characterInfo[i].vertexIndex;
								int materialReferenceIndex = textInfo.characterInfo[i].materialReferenceIndex;
								Vector3[] vertices = textInfo.meshInfo[materialReferenceIndex].vertices;
								Vector3 vector = new Vector2((vertices[vertexIndex].x + vertices[vertexIndex + 2].x) / 2f, textInfo.characterInfo[i].baseLine);
								vertices[vertexIndex] += -vector;
								vertices[vertexIndex + 1] += -vector;
								vertices[vertexIndex + 2] += -vector;
								vertices[vertexIndex + 3] += -vector;
								float num = this.ShearAmount * 0.01f;
								Vector3 vector2 = new Vector3(num * (textInfo.characterInfo[i].topRight.y - textInfo.characterInfo[i].baseLine), 0f, 0f);
								Vector3 vector3 = new Vector3(num * (textInfo.characterInfo[i].baseLine - textInfo.characterInfo[i].bottomRight.y), 0f, 0f);
								vertices[vertexIndex] += -vector3;
								vertices[vertexIndex + 1] += vector2;
								vertices[vertexIndex + 2] += vector2;
								vertices[vertexIndex + 3] += -vector3;
								float num2 = (vector.x - x) / (x2 - x);
								float num3 = num2 + 0.0001f;
								float num4 = this.VertexCurve.Evaluate(num2) * this.CurveScale;
								float num5 = this.VertexCurve.Evaluate(num3) * this.CurveScale;
								Vector3 vector4 = new Vector3(1f, 0f, 0f);
								Vector3 vector5 = new Vector3(num3 * (x2 - x) + x, num5) - new Vector3(vector.x, num4);
								float num6 = Mathf.Acos(Vector3.Dot(vector4, vector5.normalized)) * 57.29578f;
								float num7 = ((Vector3.Cross(vector4, vector5).z > 0f) ? num6 : (360f - num6));
								Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3(0f, num4, 0f), Quaternion.Euler(0f, 0f, num7), Vector3.one);
								vertices[vertexIndex] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex]);
								vertices[vertexIndex + 1] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 1]);
								vertices[vertexIndex + 2] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 2]);
								vertices[vertexIndex + 3] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 3]);
								vertices[vertexIndex] += vector;
								vertices[vertexIndex + 1] += vector;
								vertices[vertexIndex + 2] += vector;
								vertices[vertexIndex + 3] += vector;
							}
						}
						this.m_TextComponent.UpdateVertexData();
						yield return null;
					}
				}
			}
			yield break;
		}

		private TMP_Text m_TextComponent;

		public AnimationCurve VertexCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(0.25f, 2f),
			new Keyframe(0.5f, 0f),
			new Keyframe(0.75f, 2f),
			new Keyframe(1f, 0f)
		});

		public float CurveScale = 1f;

		public float ShearAmount = 1f;
	}
}
