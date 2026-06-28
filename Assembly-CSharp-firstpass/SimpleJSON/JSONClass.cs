using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleJSON
{
	public class JSONClass : JSONNode, IEnumerable
	{
		public override JSONNode this[string aKey]
		{
			get
			{
				JSONNode jsonnode;
				if (this.m_Dict.ContainsKey(aKey))
				{
					jsonnode = this.m_Dict[aKey];
				}
				else
				{
					jsonnode = new JSONLazyCreator(this, aKey);
				}
				return jsonnode;
			}
			set
			{
				if (this.m_Dict.ContainsKey(aKey))
				{
					this.m_Dict[aKey] = value;
				}
				else
				{
					this.m_Dict.Add(aKey, value);
				}
			}
		}

		public override JSONNode this[int aIndex]
		{
			get
			{
				JSONNode jsonnode;
				if (aIndex < 0 || aIndex >= this.m_Dict.Count)
				{
					jsonnode = null;
				}
				else
				{
					jsonnode = this.m_Dict.ElementAt<KeyValuePair<string, JSONNode>>(aIndex).Value;
				}
				return jsonnode;
			}
			set
			{
				if (aIndex >= 0 && aIndex < this.m_Dict.Count)
				{
					string key = this.m_Dict.ElementAt<KeyValuePair<string, JSONNode>>(aIndex).Key;
					this.m_Dict[key] = value;
				}
			}
		}

		public override int Count
		{
			get
			{
				return this.m_Dict.Count;
			}
		}

		public override void Add(string aKey, JSONNode aItem)
		{
			if (!string.IsNullOrEmpty(aKey))
			{
				if (this.m_Dict.ContainsKey(aKey))
				{
					this.m_Dict[aKey] = aItem;
				}
				else
				{
					this.m_Dict.Add(aKey, aItem);
				}
			}
			else
			{
				this.m_Dict.Add(Guid.NewGuid().ToString(), aItem);
			}
		}

		public override JSONNode Remove(string aKey)
		{
			JSONNode jsonnode;
			if (!this.m_Dict.ContainsKey(aKey))
			{
				jsonnode = null;
			}
			else
			{
				JSONNode jsonnode2 = this.m_Dict[aKey];
				this.m_Dict.Remove(aKey);
				jsonnode = jsonnode2;
			}
			return jsonnode;
		}

		public override JSONNode Remove(int aIndex)
		{
			JSONNode jsonnode;
			if (aIndex < 0 || aIndex >= this.m_Dict.Count)
			{
				jsonnode = null;
			}
			else
			{
				KeyValuePair<string, JSONNode> keyValuePair = this.m_Dict.ElementAt<KeyValuePair<string, JSONNode>>(aIndex);
				this.m_Dict.Remove(keyValuePair.Key);
				jsonnode = keyValuePair.Value;
			}
			return jsonnode;
		}

		public override JSONNode Remove(JSONNode aNode)
		{
			JSONNode jsonnode;
			try
			{
				KeyValuePair<string, JSONNode> keyValuePair = this.m_Dict.Where<KeyValuePair<string, JSONNode>>((KeyValuePair<string, JSONNode> k) => k.Value == aNode).First<KeyValuePair<string, JSONNode>>();
				this.m_Dict.Remove(keyValuePair.Key);
				jsonnode = aNode;
			}
			catch
			{
				jsonnode = null;
			}
			return jsonnode;
		}

		public override IEnumerable<JSONNode> Childs
		{
			get
			{
				foreach (KeyValuePair<string, JSONNode> N in this.m_Dict)
				{
					yield return N.Value;
				}
				yield break;
			}
		}

		public IEnumerator GetEnumerator()
		{
			foreach (KeyValuePair<string, JSONNode> N in this.m_Dict)
			{
				yield return N;
			}
			yield break;
		}

		public override string ToString()
		{
			string text = "{";
			foreach (KeyValuePair<string, JSONNode> keyValuePair in this.m_Dict)
			{
				if (text.Length > 2)
				{
					text += ", ";
				}
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					"\"",
					JSONNode.Escape(keyValuePair.Key),
					"\":",
					keyValuePair.Value.ToString()
				});
			}
			text += "}";
			return text;
		}

		public override string ToString(string aPrefix)
		{
			string text = "{ ";
			foreach (KeyValuePair<string, JSONNode> keyValuePair in this.m_Dict)
			{
				if (text.Length > 3)
				{
					text += ", ";
				}
				text = text + "\n" + aPrefix + "   ";
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					"\"",
					JSONNode.Escape(keyValuePair.Key),
					"\" : ",
					keyValuePair.Value.ToString(aPrefix + "   ")
				});
			}
			text = text + "\n" + aPrefix + "}";
			return text;
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(2);
			aWriter.Write(this.m_Dict.Count);
			foreach (string text in this.m_Dict.Keys)
			{
				aWriter.Write(text);
				this.m_Dict[text].Serialize(aWriter);
			}
		}

		private Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();
	}
}
