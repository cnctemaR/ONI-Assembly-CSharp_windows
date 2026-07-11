using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Utils
{
	public abstract class DataMap<T>
	{
		public DataMap()
		{
			this.AllocateBuffer();
		}

		public DataMap(int width, int height)
		{
			this.AllocateBuffer(width, height);
		}

		public DataMap(DataMap<T> copy)
		{
			this.CopyFrom(copy);
		}

		public int Width
		{
			get
			{
				return this._width;
			}
		}

		public int Height
		{
			get
			{
				return this._height;
			}
		}

		public int Stride
		{
			get
			{
				return this._stride;
			}
			set
			{
				this._stride = value;
			}
		}

		public T BorderValue
		{
			get
			{
				return this._borderValue;
			}
			set
			{
				this._borderValue = value;
			}
		}

		public int MemoryUsage
		{
			get
			{
				return this._memoryUsage;
			}
		}

		public float MemoryUsageKb
		{
			get
			{
				return (float)this.MemoryUsage / 8192f;
			}
		}

		public float MemoryUsageMo
		{
			get
			{
				return (float)this.MemoryUsage / 8388608f;
			}
		}

		public T[] GetSlab(int y)
		{
			T[] array = new T[this._stride];
			if (this._data != null && y >= 0 && y < this._height)
			{
				Array.Copy(this._data, y * this._stride, array, 0, this._stride);
			}
			else
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this._borderValue;
				}
			}
			return array;
		}

		public T GetValue(int x, int y)
		{
			if (this._data != null && x >= 0 && x < this._width && y >= 0 && y < this._height)
			{
				return this._data[y * this._stride + x];
			}
			return this._borderValue;
		}

		public void SetValue(int x, int y, T value)
		{
			if (this._data != null && x >= 0 && x < this._width && y >= 0 && y < this._height)
			{
				this._data[y * this._stride + x] = value;
			}
		}

		public void SetSize(int width, int height)
		{
			if (width < 0 || height < 0)
			{
				throw new ArgumentException("Map dimension must be greater or equal 0");
			}
			if (this._hasMaxDimension && (width > this._maxWidth || height > this._maxHeight))
			{
				throw new ArgumentException(string.Format("Map dimension must be lower than {0} * {1}", this._maxWidth, this._maxHeight));
			}
			this.AllocateBuffer(width, height);
		}

		public void CopyFrom(DataMap<T> source)
		{
			this.AllocateBuffer(source._width, source._height);
			if (this._cellsCount > 0)
			{
				Array.Copy(source._data, 0, this._data, 0, this._cellsCount);
			}
			this._borderValue = source._borderValue;
		}

		public void CopyTo(DataMap<T> dest)
		{
			if (dest == null)
			{
				throw new ArgumentNullException("Dest is null");
			}
			dest.CopyFrom(this);
		}

		public void CopyTo(ref T[] buffer)
		{
			if (this._data == null)
			{
				return;
			}
			if (buffer == null)
			{
				buffer = new T[this._cellsCount];
			}
			int num = ((this._data.Length <= buffer.Length) ? this._data.Length : buffer.Length);
			Array.Copy(this._data, 0, buffer, 0, num);
		}

		public T[] Share()
		{
			if (this._data == null)
			{
				throw new NullReferenceException("The internal buffer is null");
			}
			return this._data;
		}

		public void Reset()
		{
			this.AllocateBuffer(0, 0);
		}

		public void DeleteAndReset()
		{
			this._data = null;
			this.AllocateBuffer(0, 0);
		}

		public void ReclaimMemory()
		{
			if (this._data != null && this._data.Length > this._cellsCount)
			{
				Array.Resize<T>(ref this._data, this._cellsCount);
			}
		}

		public void Clear(T value)
		{
			if (this._data != null)
			{
				for (int i = 0; i <= this._cellsCount; i++)
				{
					this._data[i] = value;
				}
			}
		}

		public void Clear()
		{
			if (this._data != null)
			{
				Array.Clear(this._data, 0, this._cellsCount);
			}
		}

		protected abstract int SizeofT();

		protected abstract T MinvalofT();

		protected abstract T MaxvalofT();

		protected void AllocateBuffer()
		{
			this._cellsCount = this._width * this._height;
			this._stride = this._width;
			this._memoryUsage = this._cellsCount * this.SizeofT();
			if (this._cellsCount == 0)
			{
				return;
			}
			if (this._data == null)
			{
				this._data = new T[this._cellsCount];
			}
			else if (this._data.Length < this._cellsCount)
			{
				Array.Resize<T>(ref this._data, this._cellsCount);
			}
		}

		protected void AllocateBuffer(int width, int height)
		{
			this._width = width;
			this._height = height;
			this.AllocateBuffer();
		}

		protected T _borderValue;

		protected int _width;

		protected int _height;

		private int _stride;

		protected int _memoryUsage;

		protected int _cellsCount;

		protected T[] _data;

		protected bool _hasMaxDimension;

		protected int _maxWidth;

		protected int _maxHeight;
	}
}
