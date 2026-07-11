using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.TitleStorage
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct QueryFileListOptionsInternal : IDisposable
	{
		public int ApiVersion
		{
			get
			{
				int @default = Helper.GetDefault<int>();
				Helper.TryMarshalGet<int>(this.m_ApiVersion, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<int>(ref this.m_ApiVersion, value);
			}
		}

		public ProductUserId LocalUserId
		{
			get
			{
				ProductUserId @default = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet<ProductUserId>(this.m_LocalUserId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_LocalUserId, value);
			}
		}

		public string[] ListOfTags
		{
			get
			{
				string[] @default = Helper.GetDefault<string[]>();
				Helper.TryMarshalGet<string>(this.m_ListOfTags, out @default, this.m_ListOfTagsCount);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_ListOfTags, value, out this.m_ListOfTagsCount);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_ListOfTags);
		}

		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private IntPtr m_ListOfTags;

		private uint m_ListOfTagsCount;
	}
}
