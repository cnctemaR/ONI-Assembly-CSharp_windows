using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace System.Net
{
	internal class TrackingValidationObjectDictionary : StringDictionary
	{
		internal TrackingValidationObjectDictionary(IDictionary<string, TrackingValidationObjectDictionary.ValidateAndParseValue> validators)
		{
			this.IsChanged = false;
			this.validators = validators;
		}

		private void PersistValue(string key, string value, bool addValue)
		{
			key = key.ToLowerInvariant();
			if (!string.IsNullOrEmpty(value))
			{
				if (this.validators != null && this.validators.ContainsKey(key))
				{
					object obj = this.validators[key](value);
					if (this.internalObjects == null)
					{
						this.internalObjects = new Dictionary<string, object>();
					}
					if (addValue)
					{
						this.internalObjects.Add(key, obj);
						base.Add(key, obj.ToString());
					}
					else
					{
						this.internalObjects[key] = obj;
						base[key] = obj.ToString();
					}
				}
				else if (addValue)
				{
					base.Add(key, value);
				}
				else
				{
					base[key] = value;
				}
				this.IsChanged = true;
			}
		}

		internal bool IsChanged { get; set; }

		internal object InternalGet(string key)
		{
			if (this.internalObjects != null && this.internalObjects.ContainsKey(key))
			{
				return this.internalObjects[key];
			}
			return base[key];
		}

		internal void InternalSet(string key, object value)
		{
			if (this.internalObjects == null)
			{
				this.internalObjects = new Dictionary<string, object>();
			}
			this.internalObjects[key] = value;
			base[key] = value.ToString();
			this.IsChanged = true;
		}

		public override string this[string key]
		{
			get
			{
				return base[key];
			}
			set
			{
				this.PersistValue(key, value, false);
			}
		}

		public override void Add(string key, string value)
		{
			this.PersistValue(key, value, true);
		}

		public override void Clear()
		{
			if (this.internalObjects != null)
			{
				this.internalObjects.Clear();
			}
			base.Clear();
			this.IsChanged = true;
		}

		public override void Remove(string key)
		{
			if (this.internalObjects != null && this.internalObjects.ContainsKey(key))
			{
				this.internalObjects.Remove(key);
			}
			base.Remove(key);
			this.IsChanged = true;
		}

		private IDictionary<string, object> internalObjects;

		private readonly IDictionary<string, TrackingValidationObjectDictionary.ValidateAndParseValue> validators;

		internal delegate object ValidateAndParseValue(object valueToValidate);
	}
}
