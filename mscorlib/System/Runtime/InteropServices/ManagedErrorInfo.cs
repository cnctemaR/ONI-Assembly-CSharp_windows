using System;

namespace System.Runtime.InteropServices
{
	internal class ManagedErrorInfo : IErrorInfo
	{
		public ManagedErrorInfo(Exception e)
		{
			this.m_Exception = e;
		}

		public Exception Exception
		{
			get
			{
				return this.m_Exception;
			}
		}

		public int GetGUID(out Guid guid)
		{
			guid = Guid.Empty;
			return 0;
		}

		public int GetSource(out string source)
		{
			source = this.m_Exception.Source;
			return 0;
		}

		public int GetDescription(out string description)
		{
			description = this.m_Exception.Message;
			return 0;
		}

		public int GetHelpFile(out string helpFile)
		{
			helpFile = this.m_Exception.HelpLink;
			return 0;
		}

		public int GetHelpContext(out uint helpContext)
		{
			helpContext = 0U;
			return 0;
		}

		private Exception m_Exception;
	}
}
