using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace System.Net
{
	internal sealed class TrackingValidationObjectDictionary : StringDictionary
	{
		internal TrackingValidationObjectDictionary(Dictionary<string, TrackingValidationObjectDictionary.ValidateAndParseValue> validators)
		{
			this.IsChanged = false;
			this._validators = validators;
		}

		private void PersistValue(string key, string value, bool addValue)
		{
			key = key.ToLowerInvariant();
			if (!string.IsNullOrEmpty(value))
			{
				TrackingValidationObjectDictionary.ValidateAndParseValue validateAndParseValue;
				if (this._validators != null && this._validators.TryGetValue(key, out validateAndParseValue))
				{
					object obj = validateAndParseValue(value);
					if (this._internalObjects == null)
					{
						this._internalObjects = new Dictionary<string, object>();
					}
					if (addValue)
					{
						this._internalObjects.Add(key, obj);
						base.Add(key, obj.ToString());
					}
					else
					{
						this._internalObjects[key] = obj;
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
			object obj;
			if (this._internalObjects != null && this._internalObjects.TryGetValue(key, out obj))
			{
				return obj;
			}
			return base[key];
		}

		internal void InternalSet(string key, object value)
		{
			if (this._internalObjects == null)
			{
				this._internalObjects = new Dictionary<string, object>();
			}
			this._internalObjects[key] = value;
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
			if (this._internalObjects != null)
			{
				this._internalObjects.Clear();
			}
			base.Clear();
			this.IsChanged = true;
		}

		public override void Remove(string key)
		{
			if (this._internalObjects != null)
			{
				this._internalObjects.Remove(key);
			}
			base.Remove(key);
			this.IsChanged = true;
		}

		private readonly Dictionary<string, TrackingValidationObjectDictionary.ValidateAndParseValue> _validators;

		private Dictionary<string, object> _internalObjects;

		internal delegate object ValidateAndParseValue(object valueToValidate);
	}
}
