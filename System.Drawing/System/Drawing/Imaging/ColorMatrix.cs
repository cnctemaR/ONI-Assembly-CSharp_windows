using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class ColorMatrix
	{
		public ColorMatrix()
		{
			this._matrix00 = 1f;
			this._matrix11 = 1f;
			this._matrix22 = 1f;
			this._matrix33 = 1f;
			this._matrix44 = 1f;
		}

		public float Matrix00
		{
			get
			{
				return this._matrix00;
			}
			set
			{
				this._matrix00 = value;
			}
		}

		public float Matrix01
		{
			get
			{
				return this._matrix01;
			}
			set
			{
				this._matrix01 = value;
			}
		}

		public float Matrix02
		{
			get
			{
				return this._matrix02;
			}
			set
			{
				this._matrix02 = value;
			}
		}

		public float Matrix03
		{
			get
			{
				return this._matrix03;
			}
			set
			{
				this._matrix03 = value;
			}
		}

		public float Matrix04
		{
			get
			{
				return this._matrix04;
			}
			set
			{
				this._matrix04 = value;
			}
		}

		public float Matrix10
		{
			get
			{
				return this._matrix10;
			}
			set
			{
				this._matrix10 = value;
			}
		}

		public float Matrix11
		{
			get
			{
				return this._matrix11;
			}
			set
			{
				this._matrix11 = value;
			}
		}

		public float Matrix12
		{
			get
			{
				return this._matrix12;
			}
			set
			{
				this._matrix12 = value;
			}
		}

		public float Matrix13
		{
			get
			{
				return this._matrix13;
			}
			set
			{
				this._matrix13 = value;
			}
		}

		public float Matrix14
		{
			get
			{
				return this._matrix14;
			}
			set
			{
				this._matrix14 = value;
			}
		}

		public float Matrix20
		{
			get
			{
				return this._matrix20;
			}
			set
			{
				this._matrix20 = value;
			}
		}

		public float Matrix21
		{
			get
			{
				return this._matrix21;
			}
			set
			{
				this._matrix21 = value;
			}
		}

		public float Matrix22
		{
			get
			{
				return this._matrix22;
			}
			set
			{
				this._matrix22 = value;
			}
		}

		public float Matrix23
		{
			get
			{
				return this._matrix23;
			}
			set
			{
				this._matrix23 = value;
			}
		}

		public float Matrix24
		{
			get
			{
				return this._matrix24;
			}
			set
			{
				this._matrix24 = value;
			}
		}

		public float Matrix30
		{
			get
			{
				return this._matrix30;
			}
			set
			{
				this._matrix30 = value;
			}
		}

		public float Matrix31
		{
			get
			{
				return this._matrix31;
			}
			set
			{
				this._matrix31 = value;
			}
		}

		public float Matrix32
		{
			get
			{
				return this._matrix32;
			}
			set
			{
				this._matrix32 = value;
			}
		}

		public float Matrix33
		{
			get
			{
				return this._matrix33;
			}
			set
			{
				this._matrix33 = value;
			}
		}

		public float Matrix34
		{
			get
			{
				return this._matrix34;
			}
			set
			{
				this._matrix34 = value;
			}
		}

		public float Matrix40
		{
			get
			{
				return this._matrix40;
			}
			set
			{
				this._matrix40 = value;
			}
		}

		public float Matrix41
		{
			get
			{
				return this._matrix41;
			}
			set
			{
				this._matrix41 = value;
			}
		}

		public float Matrix42
		{
			get
			{
				return this._matrix42;
			}
			set
			{
				this._matrix42 = value;
			}
		}

		public float Matrix43
		{
			get
			{
				return this._matrix43;
			}
			set
			{
				this._matrix43 = value;
			}
		}

		public float Matrix44
		{
			get
			{
				return this._matrix44;
			}
			set
			{
				this._matrix44 = value;
			}
		}

		[CLSCompliant(false)]
		public ColorMatrix(float[][] newColorMatrix)
		{
			this.SetMatrix(newColorMatrix);
		}

		internal void SetMatrix(float[][] newColorMatrix)
		{
			this._matrix00 = newColorMatrix[0][0];
			this._matrix01 = newColorMatrix[0][1];
			this._matrix02 = newColorMatrix[0][2];
			this._matrix03 = newColorMatrix[0][3];
			this._matrix04 = newColorMatrix[0][4];
			this._matrix10 = newColorMatrix[1][0];
			this._matrix11 = newColorMatrix[1][1];
			this._matrix12 = newColorMatrix[1][2];
			this._matrix13 = newColorMatrix[1][3];
			this._matrix14 = newColorMatrix[1][4];
			this._matrix20 = newColorMatrix[2][0];
			this._matrix21 = newColorMatrix[2][1];
			this._matrix22 = newColorMatrix[2][2];
			this._matrix23 = newColorMatrix[2][3];
			this._matrix24 = newColorMatrix[2][4];
			this._matrix30 = newColorMatrix[3][0];
			this._matrix31 = newColorMatrix[3][1];
			this._matrix32 = newColorMatrix[3][2];
			this._matrix33 = newColorMatrix[3][3];
			this._matrix34 = newColorMatrix[3][4];
			this._matrix40 = newColorMatrix[4][0];
			this._matrix41 = newColorMatrix[4][1];
			this._matrix42 = newColorMatrix[4][2];
			this._matrix43 = newColorMatrix[4][3];
			this._matrix44 = newColorMatrix[4][4];
		}

		internal float[][] GetMatrix()
		{
			float[][] array = new float[5][];
			for (int i = 0; i < 5; i++)
			{
				array[i] = new float[5];
			}
			array[0][0] = this._matrix00;
			array[0][1] = this._matrix01;
			array[0][2] = this._matrix02;
			array[0][3] = this._matrix03;
			array[0][4] = this._matrix04;
			array[1][0] = this._matrix10;
			array[1][1] = this._matrix11;
			array[1][2] = this._matrix12;
			array[1][3] = this._matrix13;
			array[1][4] = this._matrix14;
			array[2][0] = this._matrix20;
			array[2][1] = this._matrix21;
			array[2][2] = this._matrix22;
			array[2][3] = this._matrix23;
			array[2][4] = this._matrix24;
			array[3][0] = this._matrix30;
			array[3][1] = this._matrix31;
			array[3][2] = this._matrix32;
			array[3][3] = this._matrix33;
			array[3][4] = this._matrix34;
			array[4][0] = this._matrix40;
			array[4][1] = this._matrix41;
			array[4][2] = this._matrix42;
			array[4][3] = this._matrix43;
			array[4][4] = this._matrix44;
			return array;
		}

		public float this[int row, int column]
		{
			get
			{
				return this.GetMatrix()[row][column];
			}
			set
			{
				float[][] matrix = this.GetMatrix();
				matrix[row][column] = value;
				this.SetMatrix(matrix);
			}
		}

		private float _matrix00;

		private float _matrix01;

		private float _matrix02;

		private float _matrix03;

		private float _matrix04;

		private float _matrix10;

		private float _matrix11;

		private float _matrix12;

		private float _matrix13;

		private float _matrix14;

		private float _matrix20;

		private float _matrix21;

		private float _matrix22;

		private float _matrix23;

		private float _matrix24;

		private float _matrix30;

		private float _matrix31;

		private float _matrix32;

		private float _matrix33;

		private float _matrix34;

		private float _matrix40;

		private float _matrix41;

		private float _matrix42;

		private float _matrix43;

		private float _matrix44;
	}
}
