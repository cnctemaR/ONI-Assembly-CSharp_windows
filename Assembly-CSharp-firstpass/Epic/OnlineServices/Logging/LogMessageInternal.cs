using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Logging
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LogMessageInternal : IDisposable
	{
		public string Category
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_Category, out @default);
				return @default;
			}
		}

		public string Message
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_Message, out @default);
				return @default;
			}
		}

		public LogLevel Level
		{
			get
			{
				LogLevel @default = Helper.GetDefault<LogLevel>();
				Helper.TryMarshalGet<LogLevel>(this.m_Level, out @default);
				return @default;
			}
		}

		public void Dispose()
		{
		}

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Category;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Message;

		private LogLevel m_Level;
	}
}
