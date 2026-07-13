using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TMPro.Examples
{
	public class TMP_TextSelector_B : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler, IPointerUpHandler
	{
		private void Awake()
		{
			this.m_TextMeshPro = base.gameObject.GetComponent<TextMeshProUGUI>();
			this.m_Canvas = base.gameObject.GetComponentInParent<Canvas>();
			if (this.m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				this.m_Camera = null;
			}
			else
			{
				this.m_Camera = this.m_Canvas.worldCamera;
			}
			this.m_TextPopup_RectTransform = global::UnityEngine.Object.Instantiate<RectTransform>(this.TextPopup_Prefab_01);
			this.m_TextPopup_RectTransform.SetParent(this.m_Canvas.transform, false);
			this.m_TextPopup_TMPComponent = this.m_TextPopup_RectTransform.GetComponentInChildren<TextMeshProUGUI>();
			this.m_TextPopup_RectTransform.gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Add(new Action<global::UnityEngine.Object>(this.ON_TEXT_CHANGED));
		}

		private void OnDisable()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(new Action<global::UnityEngine.Object>(this.ON_TEXT_CHANGED));
		}

		private void ON_TEXT_CHANGED(global::UnityEngine.Object obj)
		{
			if (obj == this.m_TextMeshPro)
			{
				this.m_cachedMeshInfoVertexData = this.m_TextMeshPro.textInfo.CopyMeshInfoVertexData();
			}
		}

		private void LateUpdate()
		{
			if (this.isHoveringObject)
			{
				int num = TMP_TextUtilities.FindIntersectingCharacter(this.m_TextMeshPro, Input.mousePosition, this.m_Camera, true);
				if (num == -1 || num != this.m_lastIndex)
				{
					this.RestoreCachedVertexAttributes(this.m_lastIndex);
					this.m_lastIndex = -1;
				}
				if (num != -1 && num != this.m_lastIndex && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
				{
					this.m_lastIndex = num;
					int materialReferenceIndex = this.m_TextMeshPro.textInfo.characterInfo[num].materialReferenceIndex;
					int vertexIndex = this.m_TextMeshPro.textInfo.characterInfo[num].vertexIndex;
					Vector3[] vertices = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].vertices;
					Vector3 vector = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2f;
					vertices[vertexIndex] -= vector;
					vertices[vertexIndex + 1] = vertices[vertexIndex + 1] - vector;
					vertices[vertexIndex + 2] = vertices[vertexIndex + 2] - vector;
					vertices[vertexIndex + 3] = vertices[vertexIndex + 3] - vector;
					float num2 = 1.5f;
					this.m_matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * num2);
					vertices[vertexIndex] = this.m_matrix.MultiplyPoint3x4(vertices[vertexIndex]);
					vertices[vertexIndex + 1] = this.m_matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
					vertices[vertexIndex + 2] = this.m_matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
					vertices[vertexIndex + 3] = this.m_matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);
					vertices[vertexIndex] += vector;
					vertices[vertexIndex + 1] = vertices[vertexIndex + 1] + vector;
					vertices[vertexIndex + 2] = vertices[vertexIndex + 2] + vector;
					vertices[vertexIndex + 3] = vertices[vertexIndex + 3] + vector;
					Color32 color = new Color32(byte.MaxValue, byte.MaxValue, 192, byte.MaxValue);
					Color32[] colors = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].colors32;
					colors[vertexIndex] = color;
					colors[vertexIndex + 1] = color;
					colors[vertexIndex + 2] = color;
					colors[vertexIndex + 3] = color;
					TMP_MeshInfo tmp_MeshInfo = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex];
					int num3 = vertices.Length - 4;
					tmp_MeshInfo.SwapVertexData(vertexIndex, num3);
					this.m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
				}
				int num4 = TMP_TextUtilities.FindIntersectingWord(this.m_TextMeshPro, Input.mousePosition, this.m_Camera);
				if (this.m_TextPopup_RectTransform != null && this.m_selectedWord != -1 && (num4 == -1 || num4 != this.m_selectedWord))
				{
					TMP_WordInfo tmp_WordInfo = this.m_TextMeshPro.textInfo.wordInfo[this.m_selectedWord];
					for (int i = 0; i < tmp_WordInfo.characterCount; i++)
					{
						int num5 = tmp_WordInfo.firstCharacterIndex + i;
						int materialReferenceIndex2 = this.m_TextMeshPro.textInfo.characterInfo[num5].materialReferenceIndex;
						int vertexIndex2 = this.m_TextMeshPro.textInfo.characterInfo[num5].vertexIndex;
						Color32[] colors2 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex2].colors32;
						Color32 color2 = colors2[vertexIndex2].Tint(1.33333f);
						colors2[vertexIndex2] = color2;
						colors2[vertexIndex2 + 1] = color2;
						colors2[vertexIndex2 + 2] = color2;
						colors2[vertexIndex2 + 3] = color2;
					}
					this.m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
					this.m_selectedWord = -1;
				}
				if (num4 != -1 && num4 != this.m_selectedWord && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
				{
					this.m_selectedWord = num4;
					TMP_WordInfo tmp_WordInfo2 = this.m_TextMeshPro.textInfo.wordInfo[num4];
					for (int j = 0; j < tmp_WordInfo2.characterCount; j++)
					{
						int num6 = tmp_WordInfo2.firstCharacterIndex + j;
						int materialReferenceIndex3 = this.m_TextMeshPro.textInfo.characterInfo[num6].materialReferenceIndex;
						int vertexIndex3 = this.m_TextMeshPro.textInfo.characterInfo[num6].vertexIndex;
						Color32[] colors3 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex3].colors32;
						Color32 color3 = colors3[vertexIndex3].Tint(0.75f);
						colors3[vertexIndex3] = color3;
						colors3[vertexIndex3 + 1] = color3;
						colors3[vertexIndex3 + 2] = color3;
						colors3[vertexIndex3 + 3] = color3;
					}
					this.m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
				}
				int num7 = TMP_TextUtilities.FindIntersectingLink(this.m_TextMeshPro, Input.mousePosition, this.m_Camera);
				if ((num7 == -1 && this.m_selectedLink != -1) || num7 != this.m_selectedLink)
				{
					this.m_TextPopup_RectTransform.gameObject.SetActive(false);
					this.m_selectedLink = -1;
				}
				if (num7 != -1 && num7 != this.m_selectedLink)
				{
					this.m_selectedLink = num7;
					TMP_LinkInfo tmp_LinkInfo = this.m_TextMeshPro.textInfo.linkInfo[num7];
					Vector3 vector2;
					RectTransformUtility.ScreenPointToWorldPointInRectangle(this.m_TextMeshPro.rectTransform, Input.mousePosition, this.m_Camera, out vector2);
					string linkID = tmp_LinkInfo.GetLinkID();
					if (linkID == "id_01")
					{
						this.m_TextPopup_RectTransform.position = vector2;
						this.m_TextPopup_RectTransform.gameObject.SetActive(true);
						this.m_TextPopup_TMPComponent.text = "You have selected link <#ffff00> ID 01";
						return;
					}
					if (!(linkID == "id_02"))
					{
						return;
					}
					this.m_TextPopup_RectTransform.position = vector2;
					this.m_TextPopup_RectTransform.gameObject.SetActive(true);
					this.m_TextPopup_TMPComponent.text = "You have selected link <#ffff00> ID 02";
					return;
				}
			}
			else if (this.m_lastIndex != -1)
			{
				this.RestoreCachedVertexAttributes(this.m_lastIndex);
				this.m_lastIndex = -1;
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			this.isHoveringObject = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			this.isHoveringObject = false;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
		}

		public void OnPointerUp(PointerEventData eventData)
		{
		}

		private void RestoreCachedVertexAttributes(int index)
		{
			if (index == -1 || index > this.m_TextMeshPro.textInfo.characterCount - 1)
			{
				return;
			}
			int materialReferenceIndex = this.m_TextMeshPro.textInfo.characterInfo[index].materialReferenceIndex;
			int vertexIndex = this.m_TextMeshPro.textInfo.characterInfo[index].vertexIndex;
			Vector3[] vertices = this.m_cachedMeshInfoVertexData[materialReferenceIndex].vertices;
			Vector3[] vertices2 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].vertices;
			vertices2[vertexIndex] = vertices[vertexIndex];
			vertices2[vertexIndex + 1] = vertices[vertexIndex + 1];
			vertices2[vertexIndex + 2] = vertices[vertexIndex + 2];
			vertices2[vertexIndex + 3] = vertices[vertexIndex + 3];
			Color32[] colors = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].colors32;
			Color32[] array = this.m_cachedMeshInfoVertexData[materialReferenceIndex].colors32;
			colors[vertexIndex] = array[vertexIndex];
			colors[vertexIndex + 1] = array[vertexIndex + 1];
			colors[vertexIndex + 2] = array[vertexIndex + 2];
			colors[vertexIndex + 3] = array[vertexIndex + 3];
			Vector4[] array2 = this.m_cachedMeshInfoVertexData[materialReferenceIndex].uvs0;
			Vector4[] uvs = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].uvs0;
			uvs[vertexIndex] = array2[vertexIndex];
			uvs[vertexIndex + 1] = array2[vertexIndex + 1];
			uvs[vertexIndex + 2] = array2[vertexIndex + 2];
			uvs[vertexIndex + 3] = array2[vertexIndex + 3];
			Vector2[] array3 = this.m_cachedMeshInfoVertexData[materialReferenceIndex].uvs2;
			Vector2[] uvs2 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].uvs2;
			uvs2[vertexIndex] = array3[vertexIndex];
			uvs2[vertexIndex + 1] = array3[vertexIndex + 1];
			uvs2[vertexIndex + 2] = array3[vertexIndex + 2];
			uvs2[vertexIndex + 3] = array3[vertexIndex + 3];
			int num = (vertices.Length / 4 - 1) * 4;
			vertices2[num] = vertices[num];
			vertices2[num + 1] = vertices[num + 1];
			vertices2[num + 2] = vertices[num + 2];
			vertices2[num + 3] = vertices[num + 3];
			array = this.m_cachedMeshInfoVertexData[materialReferenceIndex].colors32;
			Color32[] colors2 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].colors32;
			colors2[num] = array[num];
			colors2[num + 1] = array[num + 1];
			colors2[num + 2] = array[num + 2];
			colors2[num + 3] = array[num + 3];
			array2 = this.m_cachedMeshInfoVertexData[materialReferenceIndex].uvs0;
			Vector4[] uvs3 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].uvs0;
			uvs3[num] = array2[num];
			uvs3[num + 1] = array2[num + 1];
			uvs3[num + 2] = array2[num + 2];
			uvs3[num + 3] = array2[num + 3];
			array3 = this.m_cachedMeshInfoVertexData[materialReferenceIndex].uvs2;
			Vector2[] uvs4 = this.m_TextMeshPro.textInfo.meshInfo[materialReferenceIndex].uvs2;
			uvs4[num] = array3[num];
			uvs4[num + 1] = array3[num + 1];
			uvs4[num + 2] = array3[num + 2];
			uvs4[num + 3] = array3[num + 3];
			this.m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
		}

		public RectTransform TextPopup_Prefab_01;

		private RectTransform m_TextPopup_RectTransform;

		private TextMeshProUGUI m_TextPopup_TMPComponent;

		private const string k_LinkText = "You have selected link <#ffff00>";

		private const string k_WordText = "Word Index: <#ffff00>";

		private TextMeshProUGUI m_TextMeshPro;

		private Canvas m_Canvas;

		private Camera m_Camera;

		private bool isHoveringObject;

		private int m_selectedWord = -1;

		private int m_selectedLink = -1;

		private int m_lastIndex = -1;

		private Matrix4x4 m_matrix;

		private TMP_MeshInfo[] m_cachedMeshInfoVertexData;
	}
}
