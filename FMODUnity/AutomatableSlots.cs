using System;

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
				return this.slot00;
			case 1:
				return this.slot01;
			case 2:
				return this.slot02;
			case 3:
				return this.slot03;
			case 4:
				return this.slot04;
			case 5:
				return this.slot05;
			case 6:
				return this.slot06;
			case 7:
				return this.slot07;
			case 8:
				return this.slot08;
			case 9:
				return this.slot09;
			case 10:
				return this.slot10;
			case 11:
				return this.slot11;
			case 12:
				return this.slot12;
			case 13:
				return this.slot13;
			case 14:
				return this.slot14;
			case 15:
				return this.slot15;
			default:
				throw new ArgumentException(string.Format("Invalid slot index: {0}", index));
			}
		}

		public float slot00;

		public float slot01;

		public float slot02;

		public float slot03;

		public float slot04;

		public float slot05;

		public float slot06;

		public float slot07;

		public float slot08;

		public float slot09;

		public float slot10;

		public float slot11;

		public float slot12;

		public float slot13;

		public float slot14;

		public float slot15;

		public const int Count = 16;
	}
}
