using System;
using System.Collections.Specialized;

namespace System.Configuration.Provider
{
	public abstract class ProviderBase
	{
		public virtual void Initialize(string name, NameValueCollection config)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("Provider name cannot be null or empty.", "name");
			}
			if (this.alreadyInitialized)
			{
				throw new InvalidOperationException("This provider instance has already been initialized.");
			}
			this.alreadyInitialized = true;
			this._name = name;
			if (config != null)
			{
				this._description = config["description"];
				config.Remove("description");
			}
			if (string.IsNullOrEmpty(this._description))
			{
				this._description = this._name;
			}
		}

		public virtual string Name
		{
			get
			{
				return this._name;
			}
		}

		public virtual string Description
		{
			get
			{
				return this._description;
			}
		}

		private bool alreadyInitialized;

		private string _description;

		private string _name;
	}
}
