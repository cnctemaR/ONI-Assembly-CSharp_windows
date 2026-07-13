using System;

namespace FMOD
{
	public struct DSP_METERING_INFO
	{
		public int numsamples;

		public DSP_METERING_INFO.LEVEL_ARRAY peaklevel;

		public DSP_METERING_INFO.LEVEL_ARRAY rmslevel;

		public short numchannels;

		public struct LEVEL_ARRAY
		{
			public float this[int index]
			{
				get
				{
					switch (index)
					{
					case 0:
						return this.ch0;
					case 1:
						return this.ch1;
					case 2:
						return this.ch2;
					case 3:
						return this.ch3;
					case 4:
						return this.ch4;
					case 5:
						return this.ch5;
					case 6:
						return this.ch6;
					case 7:
						return this.ch7;
					case 8:
						return this.ch8;
					case 9:
						return this.ch9;
					case 10:
						return this.ch10;
					case 11:
						return this.ch11;
					case 12:
						return this.ch12;
					case 13:
						return this.ch13;
					case 14:
						return this.ch14;
					case 15:
						return this.ch15;
					case 16:
						return this.ch16;
					case 17:
						return this.ch17;
					case 18:
						return this.ch18;
					case 19:
						return this.ch19;
					case 20:
						return this.ch20;
					case 21:
						return this.ch21;
					case 22:
						return this.ch22;
					case 23:
						return this.ch23;
					case 24:
						return this.ch24;
					case 25:
						return this.ch25;
					case 26:
						return this.ch26;
					case 27:
						return this.ch27;
					case 28:
						return this.ch28;
					case 29:
						return this.ch29;
					case 30:
						return this.ch30;
					case 31:
						return this.ch31;
					default:
						throw new IndexOutOfRangeException();
					}
				}
			}

			public readonly int Length
			{
				get
				{
					return 32;
				}
			}

			public static implicit operator float[](DSP_METERING_INFO.LEVEL_ARRAY levels)
			{
				float[] array = new float[levels.Length];
				for (int i = 0; i < levels.Length; i++)
				{
					array[i] = levels[i];
				}
				return array;
			}

			public void CopyTo(float[] buffer)
			{
				int num = ((buffer.Length >= this.Length) ? this.Length : buffer.Length);
				for (int i = 0; i < num; i++)
				{
					buffer[i] = this[i];
				}
			}

			private float ch0;

			private float ch1;

			private float ch2;

			private float ch3;

			private float ch4;

			private float ch5;

			private float ch6;

			private float ch7;

			private float ch8;

			private float ch9;

			private float ch10;

			private float ch11;

			private float ch12;

			private float ch13;

			private float ch14;

			private float ch15;

			private float ch16;

			private float ch17;

			private float ch18;

			private float ch19;

			private float ch20;

			private float ch21;

			private float ch22;

			private float ch23;

			private float ch24;

			private float ch25;

			private float ch26;

			private float ch27;

			private float ch28;

			private float ch29;

			private float ch30;

			private float ch31;
		}
	}
}
