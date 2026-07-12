using System;
using UnityEngine;

namespace rendering
{
	public class BackWall : MonoBehaviour
	{
		private void Awake()
		{
			this.backwallMaterial.SetTexture("images", this.array);
		}

		[SerializeField]
		public Material backwallMaterial;

		[SerializeField]
		public Texture2DArray array;
	}
}
