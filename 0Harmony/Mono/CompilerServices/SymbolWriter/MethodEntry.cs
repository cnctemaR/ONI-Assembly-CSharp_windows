using System;
using System.Collections.Generic;

namespace Mono.CompilerServices.SymbolWriter
{
	internal class MethodEntry : IComparable
	{
		public MethodEntry.Flags MethodFlags
		{
			get
			{
				return this.flags;
			}
		}

		public int Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		internal MethodEntry(MonoSymbolFile file, MyBinaryReader reader, int index)
		{
			this.SymbolFile = file;
			this.index = index;
			this.Token = reader.ReadInt32();
			this.DataOffset = reader.ReadInt32();
			this.LineNumberTableOffset = reader.ReadInt32();
			long position = reader.BaseStream.Position;
			reader.BaseStream.Position = (long)this.DataOffset;
			this.CompileUnitIndex = reader.ReadLeb128();
			this.LocalVariableTableOffset = reader.ReadLeb128();
			this.NamespaceID = reader.ReadLeb128();
			this.CodeBlockTableOffset = reader.ReadLeb128();
			this.ScopeVariableTableOffset = reader.ReadLeb128();
			this.RealNameOffset = reader.ReadLeb128();
			this.flags = (MethodEntry.Flags)reader.ReadLeb128();
			reader.BaseStream.Position = position;
			this.CompileUnit = file.GetCompileUnit(this.CompileUnitIndex);
		}

		internal MethodEntry(MonoSymbolFile file, CompileUnitEntry comp_unit, int token, ScopeVariable[] scope_vars, LocalVariableEntry[] locals, LineNumberEntry[] lines, CodeBlockEntry[] code_blocks, string real_name, MethodEntry.Flags flags, int namespace_id)
		{
			this.SymbolFile = file;
			this.real_name = real_name;
			this.locals = locals;
			this.code_blocks = code_blocks;
			this.scope_vars = scope_vars;
			this.flags = flags;
			this.index = -1;
			this.Token = token;
			this.CompileUnitIndex = comp_unit.Index;
			this.CompileUnit = comp_unit;
			this.NamespaceID = namespace_id;
			MethodEntry.CheckLineNumberTable(lines);
			this.lnt = new LineNumberTable(file, lines);
			file.NumLineNumbers += lines.Length;
			int num = ((locals != null) ? locals.Length : 0);
			if (num <= 32)
			{
				for (int i = 0; i < num; i++)
				{
					string name = locals[i].Name;
					for (int j = i + 1; j < num; j++)
					{
						if (locals[j].Name == name)
						{
							flags |= MethodEntry.Flags.LocalNamesAmbiguous;
							return;
						}
					}
				}
				return;
			}
			Dictionary<string, LocalVariableEntry> dictionary = new Dictionary<string, LocalVariableEntry>();
			foreach (LocalVariableEntry localVariableEntry in locals)
			{
				if (dictionary.ContainsKey(localVariableEntry.Name))
				{
					flags |= MethodEntry.Flags.LocalNamesAmbiguous;
					return;
				}
				dictionary.Add(localVariableEntry.Name, localVariableEntry);
			}
		}

		private static void CheckLineNumberTable(LineNumberEntry[] line_numbers)
		{
			int num = -1;
			int num2 = -1;
			if (line_numbers == null)
			{
				return;
			}
			foreach (LineNumberEntry lineNumberEntry in line_numbers)
			{
				if (lineNumberEntry.Equals(LineNumberEntry.Null))
				{
					throw new MonoSymbolFileException();
				}
				if (lineNumberEntry.Offset < num)
				{
					throw new MonoSymbolFileException();
				}
				if (lineNumberEntry.Offset > num)
				{
					num2 = lineNumberEntry.Row;
					num = lineNumberEntry.Offset;
				}
				else if (lineNumberEntry.Row > num2)
				{
					num2 = lineNumberEntry.Row;
				}
			}
		}

		internal void Write(MyBinaryWriter bw)
		{
			if (this.index <= 0 || this.DataOffset == 0)
			{
				throw new InvalidOperationException();
			}
			bw.Write(this.Token);
			bw.Write(this.DataOffset);
			bw.Write(this.LineNumberTableOffset);
		}

		internal void WriteData(MonoSymbolFile file, MyBinaryWriter bw)
		{
			if (this.index <= 0)
			{
				throw new InvalidOperationException();
			}
			this.LocalVariableTableOffset = (int)bw.BaseStream.Position;
			int num = ((this.locals != null) ? this.locals.Length : 0);
			bw.WriteLeb128(num);
			for (int i = 0; i < num; i++)
			{
				this.locals[i].Write(file, bw);
			}
			file.LocalCount += num;
			this.CodeBlockTableOffset = (int)bw.BaseStream.Position;
			int num2 = ((this.code_blocks != null) ? this.code_blocks.Length : 0);
			bw.WriteLeb128(num2);
			for (int j = 0; j < num2; j++)
			{
				this.code_blocks[j].Write(bw);
			}
			this.ScopeVariableTableOffset = (int)bw.BaseStream.Position;
			int num3 = ((this.scope_vars != null) ? this.scope_vars.Length : 0);
			bw.WriteLeb128(num3);
			for (int k = 0; k < num3; k++)
			{
				this.scope_vars[k].Write(bw);
			}
			if (this.real_name != null)
			{
				this.RealNameOffset = (int)bw.BaseStream.Position;
				bw.Write(this.real_name);
			}
			foreach (LineNumberEntry lineNumberEntry in this.lnt.LineNumbers)
			{
				if (lineNumberEntry.EndRow != -1 || lineNumberEntry.EndColumn != -1)
				{
					this.flags |= MethodEntry.Flags.EndInfoIncluded;
				}
			}
			this.LineNumberTableOffset = (int)bw.BaseStream.Position;
			this.lnt.Write(file, bw, (this.flags & MethodEntry.Flags.ColumnsInfoIncluded) > (MethodEntry.Flags)0, (this.flags & MethodEntry.Flags.EndInfoIncluded) > (MethodEntry.Flags)0);
			this.DataOffset = (int)bw.BaseStream.Position;
			bw.WriteLeb128(this.CompileUnitIndex);
			bw.WriteLeb128(this.LocalVariableTableOffset);
			bw.WriteLeb128(this.NamespaceID);
			bw.WriteLeb128(this.CodeBlockTableOffset);
			bw.WriteLeb128(this.ScopeVariableTableOffset);
			bw.WriteLeb128(this.RealNameOffset);
			bw.WriteLeb128((int)this.flags);
		}

		public void ReadAll()
		{
			this.GetLineNumberTable();
			this.GetLocals();
			this.GetCodeBlocks();
			this.GetScopeVariables();
			this.GetRealName();
		}

		public LineNumberTable GetLineNumberTable()
		{
			MonoSymbolFile symbolFile = this.SymbolFile;
			LineNumberTable lineNumberTable;
			lock (symbolFile)
			{
				if (this.lnt != null)
				{
					lineNumberTable = this.lnt;
				}
				else if (this.LineNumberTableOffset == 0)
				{
					lineNumberTable = null;
				}
				else
				{
					MyBinaryReader binaryReader = this.SymbolFile.BinaryReader;
					long position = binaryReader.BaseStream.Position;
					binaryReader.BaseStream.Position = (long)this.LineNumberTableOffset;
					this.lnt = LineNumberTable.Read(this.SymbolFile, binaryReader, (this.flags & MethodEntry.Flags.ColumnsInfoIncluded) > (MethodEntry.Flags)0, (this.flags & MethodEntry.Flags.EndInfoIncluded) > (MethodEntry.Flags)0);
					binaryReader.BaseStream.Position = position;
					lineNumberTable = this.lnt;
				}
			}
			return lineNumberTable;
		}

		public LocalVariableEntry[] GetLocals()
		{
			MonoSymbolFile symbolFile = this.SymbolFile;
			LocalVariableEntry[] array;
			lock (symbolFile)
			{
				if (this.locals != null)
				{
					array = this.locals;
				}
				else if (this.LocalVariableTableOffset == 0)
				{
					array = null;
				}
				else
				{
					MyBinaryReader binaryReader = this.SymbolFile.BinaryReader;
					long position = binaryReader.BaseStream.Position;
					binaryReader.BaseStream.Position = (long)this.LocalVariableTableOffset;
					int num = binaryReader.ReadLeb128();
					this.locals = new LocalVariableEntry[num];
					for (int i = 0; i < num; i++)
					{
						this.locals[i] = new LocalVariableEntry(this.SymbolFile, binaryReader);
					}
					binaryReader.BaseStream.Position = position;
					array = this.locals;
				}
			}
			return array;
		}

		public CodeBlockEntry[] GetCodeBlocks()
		{
			MonoSymbolFile symbolFile = this.SymbolFile;
			CodeBlockEntry[] array;
			lock (symbolFile)
			{
				if (this.code_blocks != null)
				{
					array = this.code_blocks;
				}
				else if (this.CodeBlockTableOffset == 0)
				{
					array = null;
				}
				else
				{
					MyBinaryReader binaryReader = this.SymbolFile.BinaryReader;
					long position = binaryReader.BaseStream.Position;
					binaryReader.BaseStream.Position = (long)this.CodeBlockTableOffset;
					int num = binaryReader.ReadLeb128();
					this.code_blocks = new CodeBlockEntry[num];
					for (int i = 0; i < num; i++)
					{
						this.code_blocks[i] = new CodeBlockEntry(i, binaryReader);
					}
					binaryReader.BaseStream.Position = position;
					array = this.code_blocks;
				}
			}
			return array;
		}

		public ScopeVariable[] GetScopeVariables()
		{
			MonoSymbolFile symbolFile = this.SymbolFile;
			ScopeVariable[] array;
			lock (symbolFile)
			{
				if (this.scope_vars != null)
				{
					array = this.scope_vars;
				}
				else if (this.ScopeVariableTableOffset == 0)
				{
					array = null;
				}
				else
				{
					MyBinaryReader binaryReader = this.SymbolFile.BinaryReader;
					long position = binaryReader.BaseStream.Position;
					binaryReader.BaseStream.Position = (long)this.ScopeVariableTableOffset;
					int num = binaryReader.ReadLeb128();
					this.scope_vars = new ScopeVariable[num];
					for (int i = 0; i < num; i++)
					{
						this.scope_vars[i] = new ScopeVariable(binaryReader);
					}
					binaryReader.BaseStream.Position = position;
					array = this.scope_vars;
				}
			}
			return array;
		}

		public string GetRealName()
		{
			MonoSymbolFile symbolFile = this.SymbolFile;
			string text;
			lock (symbolFile)
			{
				if (this.real_name != null)
				{
					text = this.real_name;
				}
				else if (this.RealNameOffset == 0)
				{
					text = null;
				}
				else
				{
					this.real_name = this.SymbolFile.BinaryReader.ReadString(this.RealNameOffset);
					text = this.real_name;
				}
			}
			return text;
		}

		public int CompareTo(object obj)
		{
			MethodEntry methodEntry = (MethodEntry)obj;
			if (methodEntry.Token < this.Token)
			{
				return 1;
			}
			if (methodEntry.Token > this.Token)
			{
				return -1;
			}
			return 0;
		}

		public override string ToString()
		{
			return string.Format("[Method {0}:{1:x}:{2}:{3}]", new object[] { this.index, this.Token, this.CompileUnitIndex, this.CompileUnit });
		}

		public readonly int CompileUnitIndex;

		public readonly int Token;

		public readonly int NamespaceID;

		private int DataOffset;

		private int LocalVariableTableOffset;

		private int LineNumberTableOffset;

		private int CodeBlockTableOffset;

		private int ScopeVariableTableOffset;

		private int RealNameOffset;

		private MethodEntry.Flags flags;

		private int index;

		public readonly CompileUnitEntry CompileUnit;

		private LocalVariableEntry[] locals;

		private CodeBlockEntry[] code_blocks;

		private ScopeVariable[] scope_vars;

		private LineNumberTable lnt;

		private string real_name;

		public readonly MonoSymbolFile SymbolFile;

		public const int Size = 12;

		[Flags]
		public enum Flags
		{
			LocalNamesAmbiguous = 1,
			ColumnsInfoIncluded = 2,
			EndInfoIncluded = 4
		}
	}
}
