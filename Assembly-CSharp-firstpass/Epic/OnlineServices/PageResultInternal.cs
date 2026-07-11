using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct PageResultInternal : IDisposable
	{
		public int StartIndex
		{
			get
			{
				int @default = Helper.GetDefault<int>();
				Helper.TryMarshalGet<int>(this.m_StartIndex, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<int>(ref this.m_StartIndex, value);
			}
		}

		public int Count
		{
			get
			{
				int @default = Helper.GetDefault<int>();
				Helper.TryMarshalGet<int>(this.m_Count, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<int>(ref this.m_Count, value);
			}
		}

		public int TotalCount
		{
			get
			{
				int @default = Helper.GetDefault<int>();
				Helper.TryMarshalGet<int>(this.m_TotalCount, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<int>(ref this.m_TotalCount, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_StartIndex;

		private int m_Count;

		private int m_TotalCount;
	}
}
