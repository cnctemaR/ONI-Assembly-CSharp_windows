using System;
using System.Runtime.Remoting.Activation;

namespace System.Runtime.Remoting.Messaging
{
	internal class ConstructionCallDictionary : MessageDictionary
	{
		public ConstructionCallDictionary(IConstructionCallMessage message)
			: base(message)
		{
			base.MethodKeys = ConstructionCallDictionary.InternalKeys;
		}

		protected override object GetMethodProperty(string key)
		{
			if (key == "__Activator")
			{
				return ((IConstructionCallMessage)this._message).Activator;
			}
			if (key == "__CallSiteActivationAttributes")
			{
				return ((IConstructionCallMessage)this._message).CallSiteActivationAttributes;
			}
			if (key == "__ActivationType")
			{
				return ((IConstructionCallMessage)this._message).ActivationType;
			}
			if (key == "__ContextProperties")
			{
				return ((IConstructionCallMessage)this._message).ContextProperties;
			}
			if (!(key == "__ActivationTypeName"))
			{
				return base.GetMethodProperty(key);
			}
			return ((IConstructionCallMessage)this._message).ActivationTypeName;
		}

		protected override void SetMethodProperty(string key, object value)
		{
			if (key == "__Activator")
			{
				((IConstructionCallMessage)this._message).Activator = (IActivator)value;
				return;
			}
			if (!(key == "__CallSiteActivationAttributes") && !(key == "__ActivationType") && !(key == "__ContextProperties") && !(key == "__ActivationTypeName"))
			{
				base.SetMethodProperty(key, value);
				return;
			}
			throw new ArgumentException("key was invalid");
		}

		public static string[] InternalKeys = new string[]
		{
			"__Uri", "__MethodName", "__TypeName", "__MethodSignature", "__Args", "__CallContext", "__CallSiteActivationAttributes", "__ActivationType", "__ContextProperties", "__Activator",
			"__ActivationTypeName"
		};
	}
}
