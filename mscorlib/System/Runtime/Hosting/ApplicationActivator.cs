using System;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Policy;

namespace System.Runtime.Hosting
{
	[ComVisible(true)]
	[MonoTODO("missing manifest support")]
	public class ApplicationActivator
	{
		public virtual ObjectHandle CreateInstance(ActivationContext activationContext)
		{
			return this.CreateInstance(activationContext, null);
		}

		public virtual ObjectHandle CreateInstance(ActivationContext activationContext, string[] activationCustomData)
		{
			if (activationContext == null)
			{
				throw new ArgumentNullException("activationContext");
			}
			AppDomainSetup appDomainSetup = new AppDomainSetup(activationContext);
			return ApplicationActivator.CreateInstanceHelper(appDomainSetup);
		}

		protected static ObjectHandle CreateInstanceHelper(AppDomainSetup adSetup)
		{
			if (adSetup == null)
			{
				throw new ArgumentNullException("adSetup");
			}
			if (adSetup.ActivationArguments == null)
			{
				string text = Locale.GetText("{0} is missing it's {1} property");
				throw new ArgumentException(string.Format(text, "AppDomainSetup", "ActivationArguments"), "adSetup");
			}
			HostSecurityManager hostSecurityManager;
			if (AppDomain.CurrentDomain.DomainManager != null)
			{
				hostSecurityManager = AppDomain.CurrentDomain.DomainManager.HostSecurityManager;
			}
			else
			{
				hostSecurityManager = new HostSecurityManager();
			}
			Evidence evidence = new Evidence();
			evidence.AddHost(adSetup.ActivationArguments);
			TrustManagerContext trustManagerContext = new TrustManagerContext();
			ApplicationTrust applicationTrust = hostSecurityManager.DetermineApplicationTrust(evidence, null, trustManagerContext);
			if (!applicationTrust.IsApplicationTrustedToRun)
			{
				string text2 = Locale.GetText("Current policy doesn't allow execution of addin.");
				throw new PolicyException(text2);
			}
			AppDomain appDomain = AppDomain.CreateDomain("friendlyName", null, adSetup);
			return appDomain.CreateInstance("assemblyName", "typeName", null);
		}
	}
}
