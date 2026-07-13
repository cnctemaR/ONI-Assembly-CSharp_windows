using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Iced.Intel.BlockEncoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class Block
	{
		public Instr[] Instructions
		{
			get
			{
				return this.instructions;
			}
		}

		public Block(BlockEncoder blockEncoder, CodeWriter codeWriter, ulong rip, [Nullable(2)] List<RelocInfo> relocInfos)
		{
			this.CodeWriter = new CodeWriterImpl(codeWriter);
			this.RIP = rip;
			this.relocInfos = relocInfos;
			this.instructions = Array2.Empty<Instr>();
			this.dataList = new List<BlockData>();
			this.alignment = (ulong)(blockEncoder.Bitness / 8);
			this.validData = new List<BlockData>();
		}

		internal void SetInstructions(Instr[] instructions)
		{
			this.instructions = instructions;
		}

		public BlockData AllocPointerLocation()
		{
			BlockData blockData = new BlockData
			{
				IsValid = true
			};
			this.dataList.Add(blockData);
			return blockData;
		}

		public void InitializeData()
		{
			ulong num;
			if (this.Instructions.Length != 0)
			{
				Instr instr = this.Instructions[this.Instructions.Length - 1];
				num = instr.IP + (ulong)instr.Size;
			}
			else
			{
				num = this.RIP;
			}
			this.validDataAddress = num;
			ulong num2 = (num + this.alignment - 1UL) & ~(this.alignment - 1UL);
			this.validDataAddressAligned = num2;
			foreach (BlockData blockData in this.dataList)
			{
				if (blockData.IsValid)
				{
					blockData.__dont_use_address = num2;
					blockData.__dont_use_address_initd = true;
					this.validData.Add(blockData);
					num2 += this.alignment;
				}
			}
		}

		public void WriteData()
		{
			if (this.validData.Count == 0)
			{
				return;
			}
			CodeWriterImpl codeWriter = this.CodeWriter;
			int num = (int)(this.validDataAddressAligned - this.validDataAddress);
			for (int i = 0; i < num; i++)
			{
				codeWriter.WriteByte(204);
			}
			List<RelocInfo> list = this.relocInfos;
			if ((int)this.alignment == 8)
			{
				using (List<BlockData>.Enumerator enumerator = this.validData.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BlockData blockData = enumerator.Current;
						if (list != null)
						{
							list.Add(new RelocInfo(RelocKind.Offset64, blockData.Address));
						}
						uint num2 = (uint)blockData.Data;
						codeWriter.WriteByte((byte)num2);
						codeWriter.WriteByte((byte)(num2 >> 8));
						codeWriter.WriteByte((byte)(num2 >> 16));
						codeWriter.WriteByte((byte)(num2 >> 24));
						num2 = (uint)(blockData.Data >> 32);
						codeWriter.WriteByte((byte)num2);
						codeWriter.WriteByte((byte)(num2 >> 8));
						codeWriter.WriteByte((byte)(num2 >> 16));
						codeWriter.WriteByte((byte)(num2 >> 24));
					}
					return;
				}
			}
			throw new InvalidOperationException();
		}

		public bool CanAddRelocInfos
		{
			get
			{
				return this.relocInfos != null;
			}
		}

		public void AddRelocInfo(RelocInfo relocInfo)
		{
			List<RelocInfo> list = this.relocInfos;
			if (list == null)
			{
				return;
			}
			list.Add(relocInfo);
		}

		public readonly CodeWriterImpl CodeWriter;

		public readonly ulong RIP;

		[Nullable(2)]
		public readonly List<RelocInfo> relocInfos;

		private Instr[] instructions;

		private readonly List<BlockData> dataList;

		private readonly ulong alignment;

		private readonly List<BlockData> validData;

		private ulong validDataAddress;

		private ulong validDataAddressAligned;
	}
}
