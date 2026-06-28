using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class NTTaggedData : ITaggedData
	{
		public short TagID
		{
			get
			{
				return 10;
			}
		}

		public void SetData(byte[] data, int index, int count)
		{
			using (MemoryStream memoryStream = new MemoryStream(data, index, count, false))
			{
				using (ZipHelperStream zipHelperStream = new ZipHelperStream(memoryStream))
				{
					zipHelperStream.ReadLEInt();
					while (zipHelperStream.Position < zipHelperStream.Length)
					{
						int num = zipHelperStream.ReadLEShort();
						int num2 = zipHelperStream.ReadLEShort();
						if (num == 1)
						{
							if (num2 >= 24)
							{
								long num3 = zipHelperStream.ReadLELong();
								this._lastModificationTime = DateTime.FromFileTime(num3);
								long num4 = zipHelperStream.ReadLELong();
								this._lastAccessTime = DateTime.FromFileTime(num4);
								long num5 = zipHelperStream.ReadLELong();
								this._createTime = DateTime.FromFileTime(num5);
							}
							break;
						}
						zipHelperStream.Seek((long)num2, SeekOrigin.Current);
					}
				}
			}
		}

		public byte[] GetData()
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (ZipHelperStream zipHelperStream = new ZipHelperStream(memoryStream))
				{
					zipHelperStream.IsStreamOwner = false;
					zipHelperStream.WriteLEInt(0);
					zipHelperStream.WriteLEShort(1);
					zipHelperStream.WriteLEShort(24);
					zipHelperStream.WriteLELong(this._lastModificationTime.ToFileTime());
					zipHelperStream.WriteLELong(this._lastAccessTime.ToFileTime());
					zipHelperStream.WriteLELong(this._createTime.ToFileTime());
					array = memoryStream.ToArray();
				}
			}
			return array;
		}

		public static bool IsValidValue(DateTime value)
		{
			bool flag = true;
			try
			{
				value.ToFileTimeUtc();
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public DateTime LastModificationTime
		{
			get
			{
				return this._lastModificationTime;
			}
			set
			{
				if (!NTTaggedData.IsValidValue(value))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._lastModificationTime = value;
			}
		}

		public DateTime CreateTime
		{
			get
			{
				return this._createTime;
			}
			set
			{
				if (!NTTaggedData.IsValidValue(value))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._createTime = value;
			}
		}

		public DateTime LastAccessTime
		{
			get
			{
				return this._lastAccessTime;
			}
			set
			{
				if (!NTTaggedData.IsValidValue(value))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._lastAccessTime = value;
			}
		}

		private DateTime _lastAccessTime = DateTime.FromFileTime(0L);

		private DateTime _lastModificationTime = DateTime.FromFileTime(0L);

		private DateTime _createTime = DateTime.FromFileTime(0L);
	}
}
