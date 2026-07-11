using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Activation;

namespace System.Runtime.Remoting.Contexts
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Class)]
	[Serializable]
	public class ContextAttribute : Attribute, IContextAttribute, IContextProperty
	{
		public ContextAttribute(string name)
		{
			this.AttributeName = name;
		}

		public virtual string Name
		{
			get
			{
				return this.AttributeName;
			}
		}

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			if (!(o is ContextAttribute))
			{
				return false;
			}
			ContextAttribute contextAttribute = (ContextAttribute)o;
			return !(contextAttribute.AttributeName != this.AttributeName);
		}

		public virtual void Freeze(Context newContext)
		{
		}

		public override int GetHashCode()
		{
			if (this.AttributeName == null)
			{
				return 0;
			}
			return this.AttributeName.GetHashCode();
		}

		public virtual void GetPropertiesForNewContext(IConstructionCallMessage ctorMsg)
		{
			if (ctorMsg == null)
			{
				throw new ArgumentNullException("ctorMsg");
			}
			IList contextProperties = ctorMsg.ContextProperties;
			contextProperties.Add(this);
		}

		public virtual bool IsContextOK(Context ctx, IConstructionCallMessage ctorMsg)
		{
			if (ctorMsg == null)
			{
				throw new ArgumentNullException("ctorMsg");
			}
			if (ctx == null)
			{
				throw new ArgumentNullException("ctx");
			}
			if (!ctorMsg.ActivationType.IsContextful)
			{
				return true;
			}
			IContextProperty property = ctx.GetProperty(this.AttributeName);
			return property != null && this == property;
		}

		public virtual bool IsNewContextOK(Context newCtx)
		{
			return true;
		}

		protected string AttributeName;
	}
}
