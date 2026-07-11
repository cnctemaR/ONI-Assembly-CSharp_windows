using System;
using System.Configuration;

namespace System.Security.Authentication.ExtendedProtection.Configuration
{
	[MonoTODO]
	public sealed class ExtendedProtectionPolicyElement : ConfigurationElement
	{
		static ExtendedProtectionPolicyElement()
		{
			Type typeFromHandle = typeof(ExtendedProtectionPolicyElement);
			ExtendedProtectionPolicyElement.custom_service_names = ConfigUtil.BuildProperty(typeFromHandle, "CustomServiceNames");
			ExtendedProtectionPolicyElement.policy_enforcement = ConfigUtil.BuildProperty(typeFromHandle, "PolicyEnforcement");
			ExtendedProtectionPolicyElement.protection_scenario = ConfigUtil.BuildProperty(typeFromHandle, "ProtectionScenario");
			foreach (ConfigurationProperty configurationProperty in new ConfigurationProperty[]
			{
				ExtendedProtectionPolicyElement.custom_service_names,
				ExtendedProtectionPolicyElement.policy_enforcement,
				ExtendedProtectionPolicyElement.protection_scenario
			})
			{
				ExtendedProtectionPolicyElement.properties.Add(configurationProperty);
			}
		}

		[ConfigurationProperty("customServiceNames")]
		public ServiceNameElementCollection CustomServiceNames
		{
			get
			{
				return (ServiceNameElementCollection)base[ExtendedProtectionPolicyElement.custom_service_names];
			}
		}

		[ConfigurationProperty("policyEnforcement")]
		public PolicyEnforcement PolicyEnforcement
		{
			get
			{
				return (PolicyEnforcement)base[ExtendedProtectionPolicyElement.policy_enforcement];
			}
			set
			{
				base[ExtendedProtectionPolicyElement.policy_enforcement] = value;
			}
		}

		[ConfigurationProperty("protectionScenario", DefaultValue = ProtectionScenario.TransportSelected)]
		public ProtectionScenario ProtectionScenario
		{
			get
			{
				return (ProtectionScenario)base[ExtendedProtectionPolicyElement.protection_scenario];
			}
			set
			{
				base[ExtendedProtectionPolicyElement.protection_scenario] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ExtendedProtectionPolicyElement.properties;
			}
		}

		public ExtendedProtectionPolicy BuildPolicy()
		{
			throw new NotImplementedException();
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		private static ConfigurationProperty custom_service_names;

		private static ConfigurationProperty policy_enforcement;

		private static ConfigurationProperty protection_scenario;
	}
}
