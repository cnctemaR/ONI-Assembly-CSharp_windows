using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Satsuma.IO.GraphML
{
	public abstract class DictionaryProperty<T> : GraphMLProperty, IClearable
	{
		public bool HasDefaultValue { get; set; }

		public T DefaultValue { get; set; }

		public Dictionary<object, T> Values { get; private set; }

		protected DictionaryProperty()
		{
			this.HasDefaultValue = false;
			this.Values = new Dictionary<object, T>();
		}

		public void Clear()
		{
			this.HasDefaultValue = false;
			this.Values.Clear();
		}

		public bool TryGetValue(object key, out T result)
		{
			if (this.Values.TryGetValue(key, out result))
			{
				return true;
			}
			if (this.HasDefaultValue)
			{
				result = this.DefaultValue;
				return true;
			}
			result = default(T);
			return false;
		}

		public override void ReadData(XElement x, object key)
		{
			if (x == null)
			{
				if (key == null)
				{
					this.HasDefaultValue = false;
					return;
				}
				this.Values.Remove(key);
				return;
			}
			else
			{
				T t = this.ReadValue(x);
				if (key == null)
				{
					this.HasDefaultValue = true;
					this.DefaultValue = t;
					return;
				}
				this.Values[key] = t;
				return;
			}
		}

		public override XElement WriteData(object key)
		{
			if (key == null)
			{
				if (!this.HasDefaultValue)
				{
					return null;
				}
				return this.WriteValue(this.DefaultValue);
			}
			else
			{
				T t;
				if (!this.Values.TryGetValue(key, out t))
				{
					return null;
				}
				return this.WriteValue(t);
			}
		}

		protected abstract T ReadValue(XElement x);

		protected abstract XElement WriteValue(T value);
	}
}
