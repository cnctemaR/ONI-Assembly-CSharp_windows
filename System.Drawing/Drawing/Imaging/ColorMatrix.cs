using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class ColorMatrix
	{
		public ColorMatrix()
		{
			this.color01 = (this.color02 = (this.color03 = (this.color04 = 0f)));
			this.color10 = (this.color12 = (this.color13 = (this.color14 = 0f)));
			this.color20 = (this.color21 = (this.color23 = (this.color24 = 0f)));
			this.color30 = (this.color31 = (this.color32 = (this.color34 = 0f)));
			this.color40 = (this.color41 = (this.color42 = (this.color43 = 0f)));
			this.color00 = (this.color11 = (this.color22 = (this.color33 = (this.color44 = 1f))));
		}

		[CLSCompliant(false)]
		public ColorMatrix(float[][] matrix)
		{
			this.color00 = matrix[0][0];
			this.color01 = matrix[0][1];
			this.color02 = matrix[0][2];
			this.color03 = matrix[0][3];
			this.color04 = matrix[0][4];
			this.color10 = matrix[1][0];
			this.color11 = matrix[1][1];
			this.color12 = matrix[1][2];
			this.color13 = matrix[1][3];
			this.color14 = matrix[1][4];
			this.color20 = matrix[2][0];
			this.color21 = matrix[2][1];
			this.color22 = matrix[2][2];
			this.color23 = matrix[2][3];
			this.color24 = matrix[2][4];
			this.color30 = matrix[3][0];
			this.color31 = matrix[3][1];
			this.color32 = matrix[3][2];
			this.color33 = matrix[3][3];
			this.color34 = matrix[3][4];
			this.color40 = matrix[4][0];
			this.color41 = matrix[4][1];
			this.color42 = matrix[4][2];
			this.color43 = matrix[4][3];
			this.color44 = matrix[4][4];
		}

		public float this[int row, int column]
		{
			get
			{
				switch (row)
				{
				case 0:
					switch (column)
					{
					case 0:
						return this.color00;
					case 1:
						return this.color01;
					case 2:
						return this.color02;
					case 3:
						return this.color03;
					case 4:
						return this.color04;
					}
					break;
				case 1:
					switch (column)
					{
					case 0:
						return this.color10;
					case 1:
						return this.color11;
					case 2:
						return this.color12;
					case 3:
						return this.color13;
					case 4:
						return this.color14;
					}
					break;
				case 2:
					switch (column)
					{
					case 0:
						return this.color20;
					case 1:
						return this.color21;
					case 2:
						return this.color22;
					case 3:
						return this.color23;
					case 4:
						return this.color24;
					}
					break;
				case 3:
					switch (column)
					{
					case 0:
						return this.color30;
					case 1:
						return this.color31;
					case 2:
						return this.color32;
					case 3:
						return this.color33;
					case 4:
						return this.color34;
					}
					break;
				case 4:
					switch (column)
					{
					case 0:
						return this.color40;
					case 1:
						return this.color41;
					case 2:
						return this.color42;
					case 3:
						return this.color43;
					case 4:
						return this.color44;
					}
					break;
				}
				throw new IndexOutOfRangeException("Index was outside the bounds of the array");
			}
			set
			{
				switch (row)
				{
				case 0:
					switch (column)
					{
					case 0:
						this.color00 = value;
						return;
					case 1:
						this.color01 = value;
						return;
					case 2:
						this.color02 = value;
						return;
					case 3:
						this.color03 = value;
						return;
					case 4:
						this.color04 = value;
						return;
					}
					break;
				case 1:
					switch (column)
					{
					case 0:
						this.color10 = value;
						return;
					case 1:
						this.color11 = value;
						return;
					case 2:
						this.color12 = value;
						return;
					case 3:
						this.color13 = value;
						return;
					case 4:
						this.color14 = value;
						return;
					}
					break;
				case 2:
					switch (column)
					{
					case 0:
						this.color20 = value;
						return;
					case 1:
						this.color21 = value;
						return;
					case 2:
						this.color22 = value;
						return;
					case 3:
						this.color23 = value;
						return;
					case 4:
						this.color24 = value;
						return;
					}
					break;
				case 3:
					switch (column)
					{
					case 0:
						this.color30 = value;
						return;
					case 1:
						this.color31 = value;
						return;
					case 2:
						this.color32 = value;
						return;
					case 3:
						this.color33 = value;
						return;
					case 4:
						this.color34 = value;
						return;
					}
					break;
				case 4:
					switch (column)
					{
					case 0:
						this.color40 = value;
						return;
					case 1:
						this.color41 = value;
						return;
					case 2:
						this.color42 = value;
						return;
					case 3:
						this.color43 = value;
						return;
					case 4:
						this.color44 = value;
						return;
					}
					break;
				}
				throw new IndexOutOfRangeException("Index was outside the bounds of the array");
			}
		}

		public float Matrix00
		{
			get
			{
				return this.color00;
			}
			set
			{
				this.color00 = value;
			}
		}

		public float Matrix01
		{
			get
			{
				return this.color01;
			}
			set
			{
				this.color01 = value;
			}
		}

		public float Matrix02
		{
			get
			{
				return this.color02;
			}
			set
			{
				this.color02 = value;
			}
		}

		public float Matrix03
		{
			get
			{
				return this.color03;
			}
			set
			{
				this.color03 = value;
			}
		}

		public float Matrix04
		{
			get
			{
				return this.color04;
			}
			set
			{
				this.color04 = value;
			}
		}

		public float Matrix10
		{
			get
			{
				return this.color10;
			}
			set
			{
				this.color10 = value;
			}
		}

		public float Matrix11
		{
			get
			{
				return this.color11;
			}
			set
			{
				this.color11 = value;
			}
		}

		public float Matrix12
		{
			get
			{
				return this.color12;
			}
			set
			{
				this.color12 = value;
			}
		}

		public float Matrix13
		{
			get
			{
				return this.color13;
			}
			set
			{
				this.color13 = value;
			}
		}

		public float Matrix14
		{
			get
			{
				return this.color14;
			}
			set
			{
				this.color14 = value;
			}
		}

		public float Matrix20
		{
			get
			{
				return this.color20;
			}
			set
			{
				this.color20 = value;
			}
		}

		public float Matrix21
		{
			get
			{
				return this.color21;
			}
			set
			{
				this.color21 = value;
			}
		}

		public float Matrix22
		{
			get
			{
				return this.color22;
			}
			set
			{
				this.color22 = value;
			}
		}

		public float Matrix23
		{
			get
			{
				return this.color23;
			}
			set
			{
				this.color23 = value;
			}
		}

		public float Matrix24
		{
			get
			{
				return this.color24;
			}
			set
			{
				this.color24 = value;
			}
		}

		public float Matrix30
		{
			get
			{
				return this.color30;
			}
			set
			{
				this.color30 = value;
			}
		}

		public float Matrix31
		{
			get
			{
				return this.color31;
			}
			set
			{
				this.color31 = value;
			}
		}

		public float Matrix32
		{
			get
			{
				return this.color32;
			}
			set
			{
				this.color32 = value;
			}
		}

		public float Matrix33
		{
			get
			{
				return this.color33;
			}
			set
			{
				this.color33 = value;
			}
		}

		public float Matrix34
		{
			get
			{
				return this.color34;
			}
			set
			{
				this.color34 = value;
			}
		}

		public float Matrix40
		{
			get
			{
				return this.color40;
			}
			set
			{
				this.color40 = value;
			}
		}

		public float Matrix41
		{
			get
			{
				return this.color41;
			}
			set
			{
				this.color41 = value;
			}
		}

		public float Matrix42
		{
			get
			{
				return this.color42;
			}
			set
			{
				this.color42 = value;
			}
		}

		public float Matrix43
		{
			get
			{
				return this.color43;
			}
			set
			{
				this.color43 = value;
			}
		}

		public float Matrix44
		{
			get
			{
				return this.color44;
			}
			set
			{
				this.color44 = value;
			}
		}

		internal static IntPtr Alloc(ColorMatrix cm)
		{
			if (cm == null)
			{
				return IntPtr.Zero;
			}
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ColorMatrix)));
			Marshal.StructureToPtr(cm, intPtr, false);
			return intPtr;
		}

		internal static void Free(IntPtr cm)
		{
			if (cm != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(cm);
			}
		}

		private float color00;

		private float color01;

		private float color02;

		private float color03;

		private float color04;

		private float color10;

		private float color11;

		private float color12;

		private float color13;

		private float color14;

		private float color20;

		private float color21;

		private float color22;

		private float color23;

		private float color24;

		private float color30;

		private float color31;

		private float color32;

		private float color33;

		private float color34;

		private float color40;

		private float color41;

		private float color42;

		private float color43;

		private float color44;
	}
}
