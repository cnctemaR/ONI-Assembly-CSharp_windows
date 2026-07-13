using System;
using System.IO;
using UnityEngine.Bindings;

namespace Unity.Hierarchy
{
	[Serializable]
	internal sealed class HierarchyViewState
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
		internal static void BinarySerialization(BinaryWriter writer, HierarchyViewState value)
		{
			writer.Write(1751737714U);
			writer.Write(0);
			writer.Write(1);
			writer.Write(0);
			int num = (int)writer.BaseStream.Position;
			writer.Write((int)value.ValidContent);
			writer.Write(value.ViewModelState.Length);
			writer.Write(value.ViewModelState);
			writer.Write(value.SearchText ?? string.Empty);
			HierarchyViewColumnState[] columns = value.Columns;
			writer.Write(columns.Length);
			foreach (HierarchyViewColumnState hierarchyViewColumnState in columns)
			{
				writer.Write(hierarchyViewColumnState.ColumnId ?? string.Empty);
				writer.Write(hierarchyViewColumnState.Visible);
				writer.Write(hierarchyViewColumnState.Width);
				writer.Write(hierarchyViewColumnState.Index);
			}
			writer.Write(value.ScrollPositionX);
			writer.Write(value.ScrollPositionY);
			int num2 = (int)writer.BaseStream.Position;
			int num3 = num2 - num;
			writer.Write(1919117433U);
			writer.Seek(num - 4, SeekOrigin.Begin);
			writer.Write(num3);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
		internal static HierarchyViewState BinaryDeserialization(BinaryReader reader)
		{
			HierarchyViewState hierarchyViewState = new HierarchyViewState();
			bool flag = reader.ReadUInt32() != 1751737714U || reader.ReadInt32() != 0;
			HierarchyViewState hierarchyViewState2;
			if (flag)
			{
				hierarchyViewState2 = null;
			}
			else
			{
				int num = reader.ReadInt32();
				bool flag2 = num != 1;
				if (flag2)
				{
					hierarchyViewState2 = null;
				}
				else
				{
					int num2 = reader.ReadInt32();
					int num3 = (int)reader.BaseStream.Position;
					hierarchyViewState.ValidContent = (HierarchyViewState.Content)reader.ReadInt32();
					hierarchyViewState.ViewModelState = reader.ReadBytes(reader.ReadInt32());
					hierarchyViewState.SearchText = reader.ReadString();
					int num4 = reader.ReadInt32();
					for (int i = 0; i < num4; i++)
					{
						HierarchyViewColumnState hierarchyViewColumnState = new HierarchyViewColumnState();
						hierarchyViewColumnState.ColumnId = reader.ReadString();
						hierarchyViewColumnState.Visible = reader.ReadBoolean();
						hierarchyViewColumnState.Width = reader.ReadSingle();
						hierarchyViewColumnState.Index = reader.ReadInt32();
					}
					hierarchyViewState.ScrollPositionX = reader.ReadSingle();
					hierarchyViewState.ScrollPositionY = reader.ReadSingle();
					int num5 = (int)reader.BaseStream.Position;
					int num6 = num5 - num3;
					bool flag3 = num6 != num2;
					if (flag3)
					{
						hierarchyViewState2 = null;
					}
					else
					{
						bool flag4 = reader.ReadUInt32() != 1919117433U;
						if (flag4)
						{
							hierarchyViewState2 = null;
						}
						else
						{
							hierarchyViewState2 = hierarchyViewState;
						}
					}
				}
			}
			return hierarchyViewState2;
		}

		public HierarchyViewState()
		{
			this.ValidContent = HierarchyViewState.Content.Invalid;
		}

		public HierarchyViewState(HierarchyViewState.Content content)
		{
			this.ValidContent = content;
		}

		public override string ToString()
		{
			string text = string.Format("Content: {0}", this.ValidContent);
			bool flag = (this.ValidContent & HierarchyViewState.Content.SearchText) > HierarchyViewState.Content.Invalid;
			if (flag)
			{
				text = text + "Text:" + this.SearchText + " ";
			}
			bool flag2 = (this.ValidContent & HierarchyViewState.Content.ViewModelState) > HierarchyViewState.Content.Invalid;
			if (flag2)
			{
				text += string.Format("{0}", this.ViewModelState.Length);
			}
			bool flag3 = (this.ValidContent & HierarchyViewState.Content.Columns) > HierarchyViewState.Content.Invalid;
			if (flag3)
			{
				text += string.Format("ColsCount:{0} ", this.Columns.Length);
			}
			bool flag4 = (this.ValidContent & HierarchyViewState.Content.ScrollPosition) > HierarchyViewState.Content.Invalid;
			if (flag4)
			{
				text += string.Format("Scroll:({0},{1})", this.ScrollPositionX, this.ScrollPositionY);
			}
			return text;
		}

		private const int SerialVersion = 1;

		private const uint FileIdentifierToken = 1751737714U;

		private const uint EndOfFileToken = 1919117433U;

		public HierarchyViewState.Content ValidContent;

		public byte[] ViewModelState;

		public string SearchText;

		public HierarchyViewColumnState[] Columns = Array.Empty<HierarchyViewColumnState>();

		public float ScrollPositionX = -1f;

		public float ScrollPositionY = -1f;

		[Flags]
		public enum Content
		{
			Invalid = 0,
			ViewModelState = 2,
			SearchText = 4,
			Columns = 8,
			ScrollPosition = 16,
			All = 30,
			Layout = 8,
			Settings = 8,
			DomainReload = 30,
			EnterPlayMode = 22,
			ExitPlayMode = 22,
			Stage = 18
		}
	}
}
