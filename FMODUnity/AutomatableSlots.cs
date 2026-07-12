using System;
using UnityEngine.Serialization;

namespace FMODUnity
{
	[Serializable]
	public struct AutomatableSlots
	{
		public float GetValue(int index)
		{
			switch (index)
			{
			case 0:
				return this.Slot00;
			case 1:
				return this.Slot01;
			case 2:
				return this.Slot02;
			case 3:
				return this.Slot03;
			case 4:
				return this.Slot04;
			case 5:
				return this.Slot05;
			case 6:
				return this.Slot06;
			case 7:
				return this.Slot07;
			case 8:
				return this.Slot08;
			case 9:
				return this.Slot09;
			case 10:
				return this.Slot10;
			case 11:
				return this.Slot11;
			case 12:
				return this.Slot12;
			case 13:
				return this.Slot13;
			case 14:
				return this.Slot14;
			case 15:
				return this.Slot15;
			default:
				throw new ArgumentException(string.Format("Invalid slot index: {0}", index));
			}
		}

		public const int Count = 16;

		[FormerlySerializedAs("slot00")]
		public float Slot00;

		[FormerlySerializedAs("slot01")]
		public float Slot01;

		[FormerlySerializedAs("slot02")]
		public float Slot02;

		[FormerlySerializedAs("slot03")]
		public float Slot03;

		[FormerlySerializedAs("slot04")]
		public float Slot04;

		[FormerlySerializedAs("slot05")]
		public float Slot05;

		[FormerlySerializedAs("slot06")]
		public float Slot06;

		[FormerlySerializedAs("slot07")]
		public float Slot07;

		[FormerlySerializedAs("slot08")]
		public float Slot08;

		[FormerlySerializedAs("slot09")]
		public float Slot09;

		[FormerlySerializedAs("slot10")]
		public float Slot10;

		[FormerlySerializedAs("slot11")]
		public float Slot11;

		[FormerlySerializedAs("slot12")]
		public float Slot12;

		[FormerlySerializedAs("slot13")]
		public float Slot13;

		[FormerlySerializedAs("slot14")]
		public float Slot14;

		[FormerlySerializedAs("slot15")]
		public float Slot15;
	}
}
