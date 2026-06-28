using System;

namespace System.ComponentModel.Design
{
	public class StandardCommands
	{
		static StandardCommands()
		{
			Guid guid = new Guid("5efc7975-14bc-11cf-9b2b-00aa00573819");
			Guid guid2 = new Guid("74d21313-2aee-11d1-8bfb-00a0c90f26f7");
			StandardCommands.AlignBottom = new CommandID(guid, 1);
			StandardCommands.AlignHorizontalCenters = new CommandID(guid, 2);
			StandardCommands.AlignLeft = new CommandID(guid, 3);
			StandardCommands.AlignRight = new CommandID(guid, 4);
			StandardCommands.AlignToGrid = new CommandID(guid, 5);
			StandardCommands.AlignTop = new CommandID(guid, 6);
			StandardCommands.AlignVerticalCenters = new CommandID(guid, 7);
			StandardCommands.ArrangeBottom = new CommandID(guid, 8);
			StandardCommands.ArrangeIcons = new CommandID(guid2, 12298);
			StandardCommands.ArrangeRight = new CommandID(guid, 9);
			StandardCommands.BringForward = new CommandID(guid, 10);
			StandardCommands.BringToFront = new CommandID(guid, 11);
			StandardCommands.CenterHorizontally = new CommandID(guid, 12);
			StandardCommands.CenterVertically = new CommandID(guid, 13);
			StandardCommands.Copy = new CommandID(guid, 15);
			StandardCommands.Cut = new CommandID(guid, 16);
			StandardCommands.Delete = new CommandID(guid, 17);
			StandardCommands.F1Help = new CommandID(guid, 377);
			StandardCommands.Group = new CommandID(guid, 20);
			StandardCommands.HorizSpaceConcatenate = new CommandID(guid, 21);
			StandardCommands.HorizSpaceDecrease = new CommandID(guid, 22);
			StandardCommands.HorizSpaceIncrease = new CommandID(guid, 23);
			StandardCommands.HorizSpaceMakeEqual = new CommandID(guid, 24);
			StandardCommands.LineupIcons = new CommandID(guid2, 12299);
			StandardCommands.LockControls = new CommandID(guid, 369);
			StandardCommands.MultiLevelRedo = new CommandID(guid, 30);
			StandardCommands.MultiLevelUndo = new CommandID(guid, 44);
			StandardCommands.Paste = new CommandID(guid, 26);
			StandardCommands.Properties = new CommandID(guid, 28);
			StandardCommands.PropertiesWindow = new CommandID(guid, 235);
			StandardCommands.Redo = new CommandID(guid, 29);
			StandardCommands.Replace = new CommandID(guid, 230);
			StandardCommands.SelectAll = new CommandID(guid, 31);
			StandardCommands.SendBackward = new CommandID(guid, 32);
			StandardCommands.SendToBack = new CommandID(guid, 33);
			StandardCommands.ShowGrid = new CommandID(guid, 103);
			StandardCommands.ShowLargeIcons = new CommandID(guid2, 12300);
			StandardCommands.SizeToControl = new CommandID(guid, 35);
			StandardCommands.SizeToControlHeight = new CommandID(guid, 36);
			StandardCommands.SizeToControlWidth = new CommandID(guid, 37);
			StandardCommands.SizeToFit = new CommandID(guid, 38);
			StandardCommands.SizeToGrid = new CommandID(guid, 39);
			StandardCommands.SnapToGrid = new CommandID(guid, 40);
			StandardCommands.TabOrder = new CommandID(guid, 41);
			StandardCommands.Undo = new CommandID(guid, 43);
			StandardCommands.Ungroup = new CommandID(guid, 45);
			StandardCommands.VerbFirst = new CommandID(guid2, 8192);
			StandardCommands.VerbLast = new CommandID(guid2, 8448);
			StandardCommands.VertSpaceConcatenate = new CommandID(guid, 46);
			StandardCommands.VertSpaceDecrease = new CommandID(guid, 47);
			StandardCommands.VertSpaceIncrease = new CommandID(guid, 48);
			StandardCommands.VertSpaceMakeEqual = new CommandID(guid, 49);
			StandardCommands.ViewGrid = new CommandID(guid, 125);
			StandardCommands.DocumentOutline = new CommandID(guid, 239);
			StandardCommands.ViewCode = new CommandID(guid, 333);
		}

		public static readonly CommandID AlignBottom;

		public static readonly CommandID AlignHorizontalCenters;

		public static readonly CommandID AlignLeft;

		public static readonly CommandID AlignRight;

		public static readonly CommandID AlignToGrid;

		public static readonly CommandID AlignTop;

		public static readonly CommandID AlignVerticalCenters;

		public static readonly CommandID ArrangeBottom;

		public static readonly CommandID ArrangeIcons;

		public static readonly CommandID ArrangeRight;

		public static readonly CommandID BringForward;

		public static readonly CommandID BringToFront;

		public static readonly CommandID CenterHorizontally;

		public static readonly CommandID CenterVertically;

		public static readonly CommandID Copy;

		public static readonly CommandID Cut;

		public static readonly CommandID Delete;

		public static readonly CommandID F1Help;

		public static readonly CommandID Group;

		public static readonly CommandID HorizSpaceConcatenate;

		public static readonly CommandID HorizSpaceDecrease;

		public static readonly CommandID HorizSpaceIncrease;

		public static readonly CommandID HorizSpaceMakeEqual;

		public static readonly CommandID LineupIcons;

		public static readonly CommandID LockControls;

		public static readonly CommandID MultiLevelRedo;

		public static readonly CommandID MultiLevelUndo;

		public static readonly CommandID Paste;

		public static readonly CommandID Properties;

		public static readonly CommandID PropertiesWindow;

		public static readonly CommandID Redo;

		public static readonly CommandID Replace;

		public static readonly CommandID SelectAll;

		public static readonly CommandID SendBackward;

		public static readonly CommandID SendToBack;

		public static readonly CommandID ShowGrid;

		public static readonly CommandID ShowLargeIcons;

		public static readonly CommandID SizeToControl;

		public static readonly CommandID SizeToControlHeight;

		public static readonly CommandID SizeToControlWidth;

		public static readonly CommandID SizeToFit;

		public static readonly CommandID SizeToGrid;

		public static readonly CommandID SnapToGrid;

		public static readonly CommandID TabOrder;

		public static readonly CommandID Undo;

		public static readonly CommandID Ungroup;

		public static readonly CommandID VerbFirst;

		public static readonly CommandID VerbLast;

		public static readonly CommandID VertSpaceConcatenate;

		public static readonly CommandID VertSpaceDecrease;

		public static readonly CommandID VertSpaceIncrease;

		public static readonly CommandID VertSpaceMakeEqual;

		public static readonly CommandID ViewGrid;

		public static readonly CommandID DocumentOutline;

		public static readonly CommandID ViewCode;
	}
}
