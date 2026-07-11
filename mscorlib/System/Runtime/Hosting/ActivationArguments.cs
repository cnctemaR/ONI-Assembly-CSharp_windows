using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Hosting
{
	[ComVisible(true)]
	[Serializable]
	public sealed class ActivationArguments
	{
		public ActivationArguments(ActivationContext activationData)
		{
			if (activationData == null)
			{
				throw new ArgumentNullException("activationData");
			}
			this._context = activationData;
			this._identity = activationData.Identity;
		}

		public ActivationArguments(ApplicationIdentity applicationIdentity)
		{
			if (applicationIdentity == null)
			{
				throw new ArgumentNullException("applicationIdentity");
			}
			this._identity = applicationIdentity;
		}

		public ActivationArguments(ActivationContext activationContext, string[] activationData)
		{
			if (activationContext == null)
			{
				throw new ArgumentNullException("activationContext");
			}
			this._context = activationContext;
			this._identity = activationContext.Identity;
			this._data = activationData;
		}

		public ActivationArguments(ApplicationIdentity applicationIdentity, string[] activationData)
		{
			if (applicationIdentity == null)
			{
				throw new ArgumentNullException("applicationIdentity");
			}
			this._identity = applicationIdentity;
			this._data = activationData;
		}

		public ActivationContext ActivationContext
		{
			get
			{
				return this._context;
			}
		}

		public string[] ActivationData
		{
			get
			{
				return this._data;
			}
		}

		public ApplicationIdentity ApplicationIdentity
		{
			get
			{
				return this._identity;
			}
		}

		private ActivationContext _context;

		private ApplicationIdentity _identity;

		private string[] _data;
	}
}
