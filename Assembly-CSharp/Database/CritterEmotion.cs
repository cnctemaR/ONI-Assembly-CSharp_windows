using System;
using UnityEngine;

namespace Database
{
	public class CritterEmotion
	{
		public CritterEmotion(string id, bool isPositiveEmotion, Sprite sprite)
		{
			this.id = id;
			this.isPositiveEmotion = isPositiveEmotion;
			this.sprite = sprite;
		}

		public string id;

		public bool isPositiveEmotion;

		public Sprite sprite;
	}
}
