using System;
using UnityEngine;

namespace Wasabimole.InspectorNavigator
{
	[Serializable]
	public class ObjectReference
	{
		[SerializeField]
		public global::UnityEngine.Object OReference;

		[SerializeField]
		public ObjectType OType;

		[SerializeField]
		public Vector3 CPosition;

		[SerializeField]
		public Quaternion CRotation;

		[SerializeField]
		public float CSize;

		[SerializeField]
		public bool COrtho;
	}
}
