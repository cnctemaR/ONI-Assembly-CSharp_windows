using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SimpleJSON
{
	public class JSONNode
	{
		public virtual void Add(string aKey, JSONNode aItem)
		{
		}

		public virtual JSONNode this[int aIndex]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual JSONNode this[string aKey]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual string Value
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public virtual int Count
		{
			get
			{
				return 0;
			}
		}

		public virtual void Add(JSONNode aItem)
		{
			this.Add("", aItem);
		}

		public virtual JSONNode Remove(string aKey)
		{
			return null;
		}

		public virtual JSONNode Remove(int aIndex)
		{
			return null;
		}

		public virtual JSONNode Remove(JSONNode aNode)
		{
			return aNode;
		}

		public virtual IEnumerable<JSONNode> Childs
		{
			get
			{
				yield break;
			}
		}

		public IEnumerable<JSONNode> DeepChilds
		{
			get
			{
				foreach (JSONNode C in this.Childs)
				{
					foreach (JSONNode D in C.DeepChilds)
					{
						yield return D;
					}
				}
				yield break;
			}
		}

		public override string ToString()
		{
			return "JSONNode";
		}

		public virtual string ToString(string aPrefix)
		{
			return "JSONNode";
		}

		public virtual int AsInt
		{
			get
			{
				int num = 0;
				int num2;
				if (int.TryParse(this.Value, out num))
				{
					num2 = num;
				}
				else
				{
					num2 = 0;
				}
				return num2;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		public virtual float AsFloat
		{
			get
			{
				float num = 0f;
				float num2;
				if (float.TryParse(this.Value, out num))
				{
					num2 = num;
				}
				else
				{
					num2 = 0f;
				}
				return num2;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		public virtual double AsDouble
		{
			get
			{
				double num = 0.0;
				double num2;
				if (double.TryParse(this.Value, out num))
				{
					num2 = num;
				}
				else
				{
					num2 = 0.0;
				}
				return num2;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		public virtual bool AsBool
		{
			get
			{
				bool flag = false;
				bool flag2;
				if (bool.TryParse(this.Value, out flag))
				{
					flag2 = flag;
				}
				else
				{
					flag2 = !string.IsNullOrEmpty(this.Value);
				}
				return flag2;
			}
			set
			{
				this.Value = ((!value) ? "false" : "true");
			}
		}

		public virtual JSONArray AsArray
		{
			get
			{
				return this as JSONArray;
			}
		}

		public virtual JSONClass AsObject
		{
			get
			{
				return this as JSONClass;
			}
		}

		public static implicit operator JSONNode(string s)
		{
			return new JSONData(s);
		}

		public static implicit operator string(JSONNode d)
		{
			return (!(d == null)) ? d.Value : null;
		}

		public static bool operator ==(JSONNode a, object b)
		{
			return (b == null && a is JSONLazyCreator) || object.ReferenceEquals(a, b);
		}

		public static bool operator !=(JSONNode a, object b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return object.ReferenceEquals(this, obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal static string Escape(string aText)
		{
			string text = "";
			foreach (char c in aText)
			{
				switch (c)
				{
				case '\b':
					text += "\\b";
					break;
				case '\t':
					text += "\\t";
					break;
				case '\n':
					text += "\\n";
					break;
				default:
					if (c != '"')
					{
						if (c != '\\')
						{
							text += c;
						}
						else
						{
							text += "\\\\";
						}
					}
					else
					{
						text += "\\\"";
					}
					break;
				case '\f':
					text += "\\f";
					break;
				case '\r':
					text += "\\r";
					break;
				}
			}
			return text;
		}

		public static JSONNode Parse(string aJSON)
		{
			Stack<JSONNode> stack = new Stack<JSONNode>();
			JSONNode jsonnode = null;
			int i = 0;
			string text = "";
			string text2 = "";
			bool flag = false;
			while (i < aJSON.Length)
			{
				char c = aJSON[i];
				switch (c)
				{
				case '\t':
					goto IL_0342;
				case '\n':
				case '\r':
					break;
				default:
					switch (c)
					{
					case '[':
						if (flag)
						{
							text += aJSON[i];
							goto IL_046E;
						}
						stack.Push(new JSONArray());
						if (jsonnode != null)
						{
							text2 = text2.Trim();
							if (jsonnode is JSONArray)
							{
								jsonnode.Add(stack.Peek());
							}
							else if (text2 != "")
							{
								jsonnode.Add(text2, stack.Peek());
							}
						}
						text2 = "";
						text = "";
						jsonnode = stack.Peek();
						goto IL_046E;
					case '\\':
						i++;
						if (flag)
						{
							char c2 = aJSON[i];
							switch (c2)
							{
							case 'r':
								text += '\r';
								break;
							default:
								if (c2 != 'b')
								{
									if (c2 != 'f')
									{
										if (c2 != 'n')
										{
											text += c2;
										}
										else
										{
											text += '\n';
										}
									}
									else
									{
										text += '\f';
									}
								}
								else
								{
									text += '\b';
								}
								break;
							case 't':
								text += '\t';
								break;
							case 'u':
							{
								string text3 = aJSON.Substring(i + 1, 4);
								text += (char)int.Parse(text3, NumberStyles.AllowHexSpecifier);
								i += 4;
								break;
							}
							}
						}
						goto IL_046E;
					case ']':
						break;
					default:
						switch (c)
						{
						case ' ':
							goto IL_0342;
						default:
							switch (c)
							{
							case '{':
								if (flag)
								{
									text += aJSON[i];
									goto IL_046E;
								}
								stack.Push(new JSONClass());
								if (jsonnode != null)
								{
									text2 = text2.Trim();
									if (jsonnode is JSONArray)
									{
										jsonnode.Add(stack.Peek());
									}
									else if (text2 != "")
									{
										jsonnode.Add(text2, stack.Peek());
									}
								}
								text2 = "";
								text = "";
								jsonnode = stack.Peek();
								goto IL_046E;
							default:
								if (c != ',')
								{
									if (c != ':')
									{
										text += aJSON[i];
										goto IL_046E;
									}
									if (flag)
									{
										text += aJSON[i];
										goto IL_046E;
									}
									text2 = text;
									text = "";
									goto IL_046E;
								}
								else
								{
									if (flag)
									{
										text += aJSON[i];
										goto IL_046E;
									}
									if (text != "")
									{
										if (jsonnode is JSONArray)
										{
											jsonnode.Add(text);
										}
										else if (text2 != "")
										{
											jsonnode.Add(text2, text);
										}
									}
									text2 = "";
									text = "";
									goto IL_046E;
								}
								break;
							case '}':
								break;
							}
							break;
						case '"':
							flag ^= true;
							goto IL_046E;
						}
						break;
					}
					if (flag)
					{
						text += aJSON[i];
					}
					else
					{
						if (stack.Count == 0)
						{
							throw new Exception("JSON Parse: Too many closing brackets");
						}
						stack.Pop();
						if (text != "")
						{
							text2 = text2.Trim();
							if (jsonnode is JSONArray)
							{
								jsonnode.Add(text);
							}
							else if (text2 != "")
							{
								jsonnode.Add(text2, text);
							}
						}
						text2 = "";
						text = "";
						if (stack.Count > 0)
						{
							jsonnode = stack.Peek();
						}
					}
					break;
				}
				IL_046E:
				i++;
				continue;
				IL_0342:
				if (flag)
				{
					text += aJSON[i];
				}
				goto IL_046E;
			}
			if (flag)
			{
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}
			return jsonnode;
		}

		public virtual void Serialize(BinaryWriter aWriter)
		{
		}

		public void SaveToStream(Stream aData)
		{
			BinaryWriter binaryWriter = new BinaryWriter(aData);
			this.Serialize(binaryWriter);
		}

		public void SaveToCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public void SaveToCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public string SaveToCompressedBase64()
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public void SaveToFile(string aFileName)
		{
			Directory.CreateDirectory(new FileInfo(aFileName).Directory.FullName);
			using (FileStream fileStream = File.OpenWrite(aFileName))
			{
				this.SaveToStream(fileStream);
			}
		}

		public string SaveToBase64()
		{
			string text;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.SaveToStream(memoryStream);
				memoryStream.Position = 0L;
				text = Convert.ToBase64String(memoryStream.ToArray());
			}
			return text;
		}

		public static JSONNode Deserialize(BinaryReader aReader)
		{
			JSONBinaryTag jsonbinaryTag = (JSONBinaryTag)aReader.ReadByte();
			JSONNode jsonnode;
			switch (jsonbinaryTag)
			{
			case JSONBinaryTag.Array:
			{
				int num = aReader.ReadInt32();
				JSONArray jsonarray = new JSONArray();
				for (int i = 0; i < num; i++)
				{
					jsonarray.Add(JSONNode.Deserialize(aReader));
				}
				jsonnode = jsonarray;
				break;
			}
			case JSONBinaryTag.Class:
			{
				int num2 = aReader.ReadInt32();
				JSONClass jsonclass = new JSONClass();
				for (int j = 0; j < num2; j++)
				{
					string text = aReader.ReadString();
					JSONNode jsonnode2 = JSONNode.Deserialize(aReader);
					jsonclass.Add(text, jsonnode2);
				}
				jsonnode = jsonclass;
				break;
			}
			case JSONBinaryTag.Value:
				jsonnode = new JSONData(aReader.ReadString());
				break;
			case JSONBinaryTag.IntValue:
				jsonnode = new JSONData(aReader.ReadInt32());
				break;
			case JSONBinaryTag.DoubleValue:
				jsonnode = new JSONData(aReader.ReadDouble());
				break;
			case JSONBinaryTag.BoolValue:
				jsonnode = new JSONData(aReader.ReadBoolean());
				break;
			case JSONBinaryTag.FloatValue:
				jsonnode = new JSONData(aReader.ReadSingle());
				break;
			default:
				throw new Exception("Error deserializing JSON. Unknown tag: " + jsonbinaryTag);
			}
			return jsonnode;
		}

		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static JSONNode LoadFromCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static JSONNode LoadFromStream(Stream aData)
		{
			JSONNode jsonnode;
			using (BinaryReader binaryReader = new BinaryReader(aData))
			{
				jsonnode = JSONNode.Deserialize(binaryReader);
			}
			return jsonnode;
		}

		public static JSONNode LoadFromFile(string aFileName)
		{
			JSONNode jsonnode;
			using (FileStream fileStream = File.OpenRead(aFileName))
			{
				jsonnode = JSONNode.LoadFromStream(fileStream);
			}
			return jsonnode;
		}

		public static JSONNode LoadFromBase64(string aBase64)
		{
			byte[] array = Convert.FromBase64String(aBase64);
			return JSONNode.LoadFromStream(new MemoryStream(array)
			{
				Position = 0L
			});
		}
	}
}
