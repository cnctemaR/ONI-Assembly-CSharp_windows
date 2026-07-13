using System;
using UnityEngine;

namespace TMPro.Examples
{
	public class ObjectSpin : MonoBehaviour
	{
		private void Awake()
		{
			this.m_transform = base.transform;
			this.m_initial_Rotation = this.m_transform.rotation.eulerAngles;
			this.m_initial_Position = this.m_transform.position;
			Light component = base.GetComponent<Light>();
			this.m_lightColor = ((component != null) ? component.color : Color.black);
		}

		private void Update()
		{
			switch (this.Motion)
			{
			case ObjectSpin.MotionType.Rotation:
				this.m_transform.Rotate(0f, this.SpinSpeed * Time.deltaTime, 0f);
				return;
			case ObjectSpin.MotionType.SearchLight:
				this.m_time += this.SpinSpeed * Time.deltaTime;
				this.m_transform.rotation = Quaternion.Euler(this.m_initial_Rotation.x, Mathf.Sin(this.m_time) * (float)this.RotationRange + this.m_initial_Rotation.y, this.m_initial_Rotation.z);
				return;
			case ObjectSpin.MotionType.Translation:
			{
				this.m_time += this.TranslationSpeed * Time.deltaTime;
				float num = this.TranslationDistance.x * Mathf.Cos(this.m_time);
				float num2 = this.TranslationDistance.y * Mathf.Sin(this.m_time) * Mathf.Cos(this.m_time * 1f);
				float num3 = this.TranslationDistance.z * Mathf.Sin(this.m_time);
				this.m_transform.position = this.m_initial_Position + new Vector3(num, num3, num2);
				this.m_prevPOS = this.m_transform.position;
				return;
			}
			default:
				return;
			}
		}

		public ObjectSpin.MotionType Motion;

		public Vector3 TranslationDistance = new Vector3(5f, 0f, 0f);

		public float TranslationSpeed = 1f;

		public float SpinSpeed = 5f;

		public int RotationRange = 15;

		private Transform m_transform;

		private float m_time;

		private Vector3 m_prevPOS;

		private Vector3 m_initial_Rotation;

		private Vector3 m_initial_Position;

		private Color32 m_lightColor;

		public enum MotionType
		{
			Rotation,
			SearchLight,
			Translation
		}
	}
}
