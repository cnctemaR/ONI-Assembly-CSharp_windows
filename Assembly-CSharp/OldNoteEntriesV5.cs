using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

public class OldNoteEntriesV5
{
	public void Deserialize(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			OldNoteEntriesV5.NoteStorageBlock noteStorageBlock = default(OldNoteEntriesV5.NoteStorageBlock);
			noteStorageBlock.Deserialize(reader);
			this.storageBlocks.Add(noteStorageBlock);
		}
	}

	public List<OldNoteEntriesV5.NoteStorageBlock> storageBlocks = new List<OldNoteEntriesV5.NoteStorageBlock>();

	[StructLayout(LayoutKind.Explicit)]
	public struct NoteEntry
	{
		[FieldOffset(0)]
		public int reportEntryId;

		[FieldOffset(4)]
		public int noteHash;

		[FieldOffset(8)]
		public float value;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct NoteEntryArray
	{
		public int StructSizeInBytes
		{
			get
			{
				return Marshal.SizeOf(typeof(OldNoteEntriesV5.NoteEntry));
			}
		}

		[FieldOffset(0)]
		public byte[] bytes;

		[FieldOffset(0)]
		public OldNoteEntriesV5.NoteEntry[] structs;
	}

	public struct NoteStorageBlock
	{
		public void Deserialize(BinaryReader reader)
		{
			this.entryCount = reader.ReadInt32();
			this.entries.bytes = reader.ReadBytes(this.entries.StructSizeInBytes * this.entryCount);
		}

		public int entryCount;

		public OldNoteEntriesV5.NoteEntryArray entries;
	}
}
