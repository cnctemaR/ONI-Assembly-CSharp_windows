using System;
using System.Collections;
using UnityEngine;

namespace TMPro.Examples
{
	public class FloatingText : MonoBehaviour
	{
		private void Awake()
		{
			this.m_transform = base.transform;
			this.m_navAgent = base.GetComponent<NavMeshAgent>();
			this.m_floatingText = new GameObject(this.m_transform.name + " floating text");
			this.m_floatingText_Transform = this.m_floatingText.transform;
			this.m_floatingText_Transform.parent = this.m_transform;
			this.m_floatingText_Transform.localPosition = new Vector3(0f, 1f, 0f);
			this.m_cameraTransform = Camera.main.transform;
		}

		private void Start()
		{
			if (this.SpawnType == 0)
			{
				this.m_textMeshPro = this.m_floatingText.AddComponent<TextMeshPro>();
				this.m_textMeshPro.color = new Color32((byte)global::UnityEngine.Random.Range(0, 255), (byte)global::UnityEngine.Random.Range(0, 255), (byte)global::UnityEngine.Random.Range(0, 255), byte.MaxValue);
				this.m_textMeshPro.fontSize = 16f;
				this.m_textMeshPro.text = string.Empty;
				base.StartCoroutine(this.DisplayTextMeshProFloatingText());
			}
			else
			{
				this.m_textMesh = this.m_floatingText.AddComponent<TextMesh>();
				this.m_textMesh.font = Resources.Load("Fonts/ARIAL", typeof(Font)) as Font;
				this.m_textMesh.GetComponent<Renderer>().sharedMaterial = this.m_textMesh.font.material;
				this.m_textMesh.color = new Color32((byte)global::UnityEngine.Random.Range(0, 255), (byte)global::UnityEngine.Random.Range(0, 255), (byte)global::UnityEngine.Random.Range(0, 255), byte.MaxValue);
				this.m_textMesh.anchor = TextAnchor.LowerCenter;
				this.m_textMesh.fontSize = 16;
				base.StartCoroutine(this.DisplayTextMeshFloatingText());
			}
		}

		public IEnumerator DisplayTextMeshProFloatingText()
		{
			for (;;)
			{
				this.m_textMeshPro.text = this.m_navAgent.remainingDistance.ToString("f2");
				if (!this.lastPOS.Compare(this.m_cameraTransform.position, 1000) || !this.lastRotation.Compare(this.m_cameraTransform.rotation, 1000))
				{
					this.lastPOS = this.m_cameraTransform.position;
					this.lastRotation = this.m_cameraTransform.rotation;
					this.m_floatingText_Transform.rotation = this.lastRotation;
				}
				yield return new WaitForEndOfFrame();
			}
			yield break;
		}

		public IEnumerator DisplayTextMeshFloatingText()
		{
			for (;;)
			{
				this.m_textMesh.text = this.m_navAgent.remainingDistance.ToString("f2");
				if (!this.lastPOS.Compare(this.m_cameraTransform.position, 1000) || !this.lastRotation.Compare(this.m_cameraTransform.rotation, 1000))
				{
					this.lastPOS = this.m_cameraTransform.position;
					this.lastRotation = this.m_cameraTransform.rotation;
					this.m_floatingText_Transform.rotation = this.lastRotation;
				}
				yield return new WaitForEndOfFrame();
			}
			yield break;
		}

		private GameObject m_floatingText;

		private TextMeshPro m_textMeshPro;

		private TextMesh m_textMesh;

		private NavMeshAgent m_navAgent;

		private Transform m_transform;

		private Transform m_floatingText_Transform;

		private Transform m_cameraTransform;

		private Vector3 lastPOS = Vector3.zero;

		private Quaternion lastRotation = Quaternion.identity;

		public int SpawnType;
	}
}
