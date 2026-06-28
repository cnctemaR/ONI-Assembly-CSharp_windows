using System;
using KSerialization;
using Satsuma;
using UnityEngine;

namespace Klei
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Node
	{
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
		}

		public Node(Node node, string type)
		{
			this.node = node;
			this.type = type;
		}

		public Node node { get; private set; }

		[Serialize]
		public string type { get; private set; }

		public void SetType(string newtype)
		{
			this.type = newtype;
		}

		[Serialize]
		public Vector2 position { get; private set; }

		public void SetPosition(Vector2 newPos)
		{
			this.position = newPos;
		}

		[Serialize]
		public TagSet tags = new TagSet();

		public TagSet biomeSpecificTags = new TagSet();
	}
}
