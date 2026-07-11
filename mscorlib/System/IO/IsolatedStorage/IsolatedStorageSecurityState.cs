using System;
using System.Security;

namespace System.IO.IsolatedStorage
{
	public class IsolatedStorageSecurityState : SecurityState
	{
		internal IsolatedStorageSecurityState()
		{
		}

		public IsolatedStorageSecurityOptions Options
		{
			get
			{
				return IsolatedStorageSecurityOptions.IncreaseQuotaForApplication;
			}
		}

		public long Quota
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
			}
		}

		public long UsedSize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override void EnsureState()
		{
			throw new NotImplementedException();
		}
	}
}
