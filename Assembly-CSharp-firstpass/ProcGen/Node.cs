using System;
using KSerialization;
using Satsuma;
using UnityEngine;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Node
	{
		internal Node node { get; private set; }

		public void SetNode(Node node)
		{
			global::Debug.Assert(!this.nodeSet, "Tried initializing a Node twice, that ain't gonna work.");
			this.node = node;
			this.nodeSet = true;
		}

		[Serialize]
		public string type { get; private set; }

		public void SetType(string newtype)
		{
			this.type = newtype;
		}

		public string GetSubworld()
		{
			foreach (Tag tag in this.tags)
			{
				if (tag.Name.Contains("subworlds/"))
				{
					return tag.Name;
				}
			}
			return "MISSING";
		}

		public string GetBiome()
		{
			foreach (Tag tag in this.tags)
			{
				if (tag.Name.Contains("biomes/"))
				{
					return tag.Name;
				}
			}
			return "MISSING";
		}

		public string GetFeature()
		{
			foreach (Tag tag in this.tags)
			{
				if (tag.Name.Contains("features/"))
				{
					return tag.Name;
				}
			}
			return null;
		}

		[Serialize]
		public Vector2 position { get; private set; }

		public void SetPosition(Vector2 newPos)
		{
			this.position = newPos;
		}

		public Node()
		{
		}

		public Node(string type)
		{
			this.type = type;
		}

		public Node(Node other)
		{
			this.position = other.position;
			this.node = other.node;
			this.type = other.type;
			this.tags = new TagSet(other.tags);
			this.featureSpecificTags = new TagSet(other.featureSpecificTags);
			this.biomeSpecificTags = new TagSet(other.biomeSpecificTags);
		}

		public Node(Node node, string type, Vector2 position = default(Vector2))
		{
			this.node = node;
			this.type = type;
			this.position = position;
		}

		private bool nodeSet;

		[Serialize]
		public TagSet tags = new TagSet();

		[Serialize]
		public Tag templateTag = Tag.Invalid;

		[Serialize]
		public TagSet featureSpecificTags = new TagSet();

		[Serialize]
		public TagSet biomeSpecificTags = new TagSet();
	}
}
