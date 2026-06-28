using System;

namespace SimpleJSON
{
	internal class JSONLazyCreator : JSONNode
	{
		public JSONLazyCreator(JSONNode aNode)
		{
			this.m_Node = aNode;
			this.m_Key = null;
		}

		public JSONLazyCreator(JSONNode aNode, string aKey)
		{
			this.m_Node = aNode;
			this.m_Key = aKey;
		}

		private void Set(JSONNode aVal)
		{
			if (this.m_Key == null)
			{
				this.m_Node.Add(aVal);
			}
			else
			{
				this.m_Node.Add(this.m_Key, aVal);
			}
			this.m_Node = null;
		}

		public override JSONNode this[int aIndex]
		{
			get
			{
				return new JSONLazyCreator(this);
			}
			set
			{
				this.Set(new JSONArray { value });
			}
		}

		public override JSONNode this[string aKey]
		{
			get
			{
				return new JSONLazyCreator(this, aKey);
			}
			set
			{
				this.Set(new JSONClass { { aKey, value } });
			}
		}

		public override void Add(JSONNode aItem)
		{
			this.Set(new JSONArray { aItem });
		}

		public override void Add(string aKey, JSONNode aItem)
		{
			this.Set(new JSONClass { { aKey, aItem } });
		}

		public static bool operator ==(JSONLazyCreator a, object b)
		{
			return b == null || object.ReferenceEquals(a, b);
		}

		public static bool operator !=(JSONLazyCreator a, object b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return obj == null || object.ReferenceEquals(this, obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return "";
		}

		public override string ToString(string aPrefix)
		{
			return "";
		}

		public override int AsInt
		{
			get
			{
				JSONData jsondata = new JSONData(0);
				this.Set(jsondata);
				return 0;
			}
			set
			{
				JSONData jsondata = new JSONData(value);
				this.Set(jsondata);
			}
		}

		public override float AsFloat
		{
			get
			{
				JSONData jsondata = new JSONData(0f);
				this.Set(jsondata);
				return 0f;
			}
			set
			{
				JSONData jsondata = new JSONData(value);
				this.Set(jsondata);
			}
		}

		public override double AsDouble
		{
			get
			{
				JSONData jsondata = new JSONData(0.0);
				this.Set(jsondata);
				return 0.0;
			}
			set
			{
				JSONData jsondata = new JSONData(value);
				this.Set(jsondata);
			}
		}

		public override bool AsBool
		{
			get
			{
				JSONData jsondata = new JSONData(false);
				this.Set(jsondata);
				return false;
			}
			set
			{
				JSONData jsondata = new JSONData(value);
				this.Set(jsondata);
			}
		}

		public override JSONArray AsArray
		{
			get
			{
				JSONArray jsonarray = new JSONArray();
				this.Set(jsonarray);
				return jsonarray;
			}
		}

		public override JSONClass AsObject
		{
			get
			{
				JSONClass jsonclass = new JSONClass();
				this.Set(jsonclass);
				return jsonclass;
			}
		}

		private JSONNode m_Node = null;

		private string m_Key = null;
	}
}
