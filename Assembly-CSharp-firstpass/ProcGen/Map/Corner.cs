using System;
using KSerialization;
using UnityEngine;

namespace ProcGen.Map
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Corner
	{
		public Corner(Vector2 position)
		{
			this.position = position;
		}

		public Vector2 position;
	}
}
