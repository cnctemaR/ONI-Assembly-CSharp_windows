using System;
using System.Collections;
using UnityEngine;

namespace TMPro.Examples
{
	public class VertexJitter : MonoBehaviour
	{
		private void Awake()
		{
			this.m_TextComponent = base.GetComponent<TMP_Text>();
		}

		private void OnEnable()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Add(new Action<global::UnityEngine.Object>(this.ON_TEXT_CHANGED));
		}

		private void OnDisable()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(new Action<global::UnityEngine.Object>(this.ON_TEXT_CHANGED));
		}

		private void Start()
		{
			base.StartCoroutine(this.AnimateVertexColors());
		}

		private void ON_TEXT_CHANGED(global::UnityEngine.Object obj)
		{
			this.hasTextChanged = true;
		}

		private IEnumerator AnimateVertexColors()
		{
			this.m_TextComponent.ForceMeshUpdate();
			TMP_TextInfo textInfo = this.m_TextComponent.textInfo;
			Vector3[][] copyOfVertices = new Vector3[0][];
			int loopCount = 0;
			this.hasTextChanged = true;
			VertexJitter.VertexAnim[] vertexAnim = new VertexJitter.VertexAnim[1024];
			for (int i = 0; i < 1024; i++)
			{
				vertexAnim[i].angleRange = global::UnityEngine.Random.Range(10f, 25f);
				vertexAnim[i].speed = global::UnityEngine.Random.Range(1f, 3f);
			}
			for (;;)
			{
				if (this.hasTextChanged)
				{
					if (copyOfVertices.Length < textInfo.meshInfo.Length)
					{
						copyOfVertices = new Vector3[textInfo.meshInfo.Length][];
					}
					for (int j = 0; j < textInfo.meshInfo.Length; j++)
					{
						int length = textInfo.meshInfo[j].vertices.Length;
						copyOfVertices[j] = new Vector3[length];
					}
					this.hasTextChanged = false;
				}
				int characterCount = textInfo.characterCount;
				if (characterCount == 0)
				{
					yield return new WaitForSeconds(0.25f);
				}
				else
				{
					for (int k = 0; k < characterCount; k++)
					{
						TMP_CharacterInfo charInfo = textInfo.characterInfo[k];
						if (charInfo.isVisible)
						{
							VertexJitter.VertexAnim vertAnim = vertexAnim[k];
							int materialIndex = textInfo.characterInfo[k].materialReferenceIndex;
							int vertexIndex = textInfo.characterInfo[k].vertexIndex;
							Vector3[] sourceVertices = textInfo.meshInfo[materialIndex].vertices;
							Vector2 charMidBasline = (sourceVertices[vertexIndex] + sourceVertices[vertexIndex + 2]) / 2f;
							Vector3 offset = charMidBasline;
							copyOfVertices[materialIndex][vertexIndex] = sourceVertices[vertexIndex] - offset;
							copyOfVertices[materialIndex][vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
							copyOfVertices[materialIndex][vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
							copyOfVertices[materialIndex][vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;
							vertAnim.angle = Mathf.SmoothStep(-vertAnim.angleRange, vertAnim.angleRange, Mathf.PingPong((float)loopCount / 25f * vertAnim.speed, 1f));
							Vector3 jitterOffset = new Vector3(global::UnityEngine.Random.Range(-0.25f, 0.25f), global::UnityEngine.Random.Range(-0.25f, 0.25f), 0f);
							Matrix4x4 matrix = Matrix4x4.TRS(jitterOffset * this.CurveScale, Quaternion.Euler(0f, 0f, global::UnityEngine.Random.Range(-5f, 5f) * this.AngleMultiplier), Vector3.one);
							copyOfVertices[materialIndex][vertexIndex] = matrix.MultiplyPoint3x4(copyOfVertices[materialIndex][vertexIndex]);
							copyOfVertices[materialIndex][vertexIndex + 1] = matrix.MultiplyPoint3x4(copyOfVertices[materialIndex][vertexIndex + 1]);
							copyOfVertices[materialIndex][vertexIndex + 2] = matrix.MultiplyPoint3x4(copyOfVertices[materialIndex][vertexIndex + 2]);
							copyOfVertices[materialIndex][vertexIndex + 3] = matrix.MultiplyPoint3x4(copyOfVertices[materialIndex][vertexIndex + 3]);
							copyOfVertices[materialIndex][vertexIndex] += offset;
							copyOfVertices[materialIndex][vertexIndex + 1] += offset;
							copyOfVertices[materialIndex][vertexIndex + 2] += offset;
							copyOfVertices[materialIndex][vertexIndex + 3] += offset;
							vertexAnim[k] = vertAnim;
						}
					}
					for (int l = 0; l < textInfo.meshInfo.Length; l++)
					{
						textInfo.meshInfo[l].mesh.vertices = copyOfVertices[l];
						this.m_TextComponent.UpdateGeometry(textInfo.meshInfo[l].mesh, l);
					}
					loopCount++;
					yield return new WaitForSeconds(0.1f);
				}
			}
			yield break;
		}

		public float AngleMultiplier = 1f;

		public float SpeedMultiplier = 1f;

		public float CurveScale = 1f;

		private TMP_Text m_TextComponent;

		private bool hasTextChanged;

		private struct VertexAnim
		{
			public float angleRange;

			public float angle;

			public float speed;
		}
	}
}
