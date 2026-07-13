using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using MonoMod.Utils;

namespace MonoMod.Cil
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class ILContext : IDisposable
	{
		public MethodDefinition Method { get; private set; }

		public ILProcessor IL { get; private set; }

		public Mono.Cecil.Cil.MethodBody Body
		{
			get
			{
				return this.Method.Body;
			}
		}

		public ModuleDefinition Module
		{
			get
			{
				return this.Method.Module;
			}
		}

		public Mono.Collections.Generic.Collection<Instruction> Instrs
		{
			get
			{
				return this.Body.Instructions;
			}
		}

		public global::System.Collections.ObjectModel.ReadOnlyCollection<ILLabel> Labels
		{
			get
			{
				return this._Labels.AsReadOnly();
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.IL == null;
			}
		}

		[Nullable(2)]
		[method: NullableContext(2)]
		[field: Nullable(2)]
		public event Action OnDispose;

		public ILContext(MethodDefinition method)
		{
			Helpers.ThrowIfArgumentNull<MethodDefinition>(method, "method");
			this.Method = method;
			this.IL = method.Body.GetILProcessor();
		}

		public void Invoke(ILContext.Manipulator manip)
		{
			Helpers.ThrowIfArgumentNull<ILContext.Manipulator>(manip, "manip");
			if (this.IsReadOnly)
			{
				throw new InvalidOperationException();
			}
			foreach (Instruction instruction in this.Instrs)
			{
				Instruction instruction2 = instruction.Operand as Instruction;
				if (instruction2 != null)
				{
					instruction.Operand = new ILLabel(this, instruction2);
				}
				else
				{
					Instruction[] array = instruction.Operand as Instruction[];
					if (array != null)
					{
						instruction.Operand = array.Select<Instruction, ILLabel>((Instruction t) => new ILLabel(this, t)).ToArray<ILLabel>();
					}
				}
			}
			manip(this);
			if (this.IsReadOnly)
			{
				return;
			}
			foreach (Instruction instruction3 in this.Instrs)
			{
				ILLabel illabel = instruction3.Operand as ILLabel;
				if (illabel != null)
				{
					instruction3.Operand = illabel.Target;
				}
				else
				{
					ILLabel[] array2 = instruction3.Operand as ILLabel[];
					if (array2 != null)
					{
						instruction3.Operand = array2.Select<ILLabel, Instruction>((ILLabel l) => l.Target).ToArray<Instruction>();
					}
				}
			}
			this.Method.FixShortLongOps();
		}

		public void MakeReadOnly()
		{
			this.Method = null;
			this.IL = null;
			this._Labels.Clear();
			this._Labels.Capacity = 0;
		}

		[Obsolete("Use new ILCursor(il).Goto(index)")]
		public ILCursor At(int index)
		{
			return new ILCursor(this).Goto(index, MoveType.Before, false);
		}

		[Obsolete("Use new ILCursor(il).Goto(index)")]
		public ILCursor At(ILLabel label)
		{
			return new ILCursor(this).GotoLabel(label, MoveType.AfterLabel, false);
		}

		[Obsolete("Use new ILCursor(il).Goto(index)")]
		public ILCursor At(Instruction instr)
		{
			return new ILCursor(this).Goto(instr, MoveType.Before, false);
		}

		public FieldReference Import(FieldInfo field)
		{
			return this.Module.ImportReference(field);
		}

		public MethodReference Import(MethodBase method)
		{
			return this.Module.ImportReference(method);
		}

		public TypeReference Import(Type type)
		{
			return this.Module.ImportReference(type);
		}

		public ILLabel DefineLabel()
		{
			return new ILLabel(this);
		}

		public ILLabel DefineLabel(Instruction target)
		{
			return new ILLabel(this, target);
		}

		[NullableContext(2)]
		public int IndexOf(Instruction instr)
		{
			if (instr == null)
			{
				return this.Instrs.Count;
			}
			int num = this.Instrs.IndexOf(instr);
			if (num != -1)
			{
				return num;
			}
			return this.Instrs.Count;
		}

		public IEnumerable<ILLabel> GetIncomingLabels([Nullable(2)] Instruction instr)
		{
			return this._Labels.Where<ILLabel>((ILLabel l) => l.Target == instr);
		}

		[NullableContext(2)]
		public int AddReference<T>(in T value)
		{
			int count = this.managedObjectRefs.Count;
			DynamicReferenceCell dynamicReferenceCell;
			DataScope<DynamicReferenceCell> dataScope = DynamicReferenceManager.AllocReference<T>(in value, out dynamicReferenceCell);
			this.managedObjectRefs.Add(dataScope);
			return count;
		}

		[NullableContext(2)]
		public T GetReference<T>(int id)
		{
			if (id < 0 || id >= this.managedObjectRefs.Count)
			{
				throw new ArgumentOutOfRangeException("id");
			}
			return DynamicReferenceManager.GetValue<T>(this.managedObjectRefs[id].Data);
		}

		[NullableContext(2)]
		public void SetReference<T>(int id, in T value)
		{
			if (id < 0 || id >= this.managedObjectRefs.Count)
			{
				throw new ArgumentOutOfRangeException("id");
			}
			DynamicReferenceManager.SetValue<T>(this.managedObjectRefs[id].Data, in value);
		}

		public DynamicReferenceCell GetReferenceCell(int id)
		{
			if (id < 0 || id >= this.managedObjectRefs.Count)
			{
				throw new ArgumentOutOfRangeException("id");
			}
			return this.managedObjectRefs[id].Data;
		}

		public VariableDefinition CreateLocal<[Nullable(2)] T>()
		{
			return this.CreateLocal(typeof(T));
		}

		public VariableDefinition CreateLocal(Type type)
		{
			return this.CreateLocal(this.Import(type));
		}

		public VariableDefinition CreateLocal(TypeReference typeRef)
		{
			VariableDefinition variableDefinition = new VariableDefinition(typeRef);
			this.Method.Body.Variables.Add(variableDefinition);
			return variableDefinition;
		}

		public override string ToString()
		{
			if (this.Method == null)
			{
				return "// ILContext: READONLY";
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = stringBuilder;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
			defaultInterpolatedStringHandler.AppendLiteral("// ILContext: ");
			defaultInterpolatedStringHandler.AppendFormatted<MethodDefinition>(this.Method);
			stringBuilder2.AppendLine(defaultInterpolatedStringHandler.ToStringAndClear());
			foreach (Instruction instruction in this.Instrs)
			{
				ILContext.ToString(stringBuilder, instruction);
			}
			return stringBuilder.ToString();
		}

		internal static StringBuilder ToString(StringBuilder builder, [Nullable(2)] Instruction instr)
		{
			if (instr == null)
			{
				return builder;
			}
			object operand = instr.Operand;
			ILLabel illabel = operand as ILLabel;
			if (illabel != null)
			{
				instr.Operand = illabel.Target;
			}
			else
			{
				ILLabel[] array = operand as ILLabel[];
				if (array != null)
				{
					instr.Operand = array.Select<ILLabel, Instruction>((ILLabel l) => l.Target).ToArray<Instruction>();
				}
			}
			builder.AppendLine(instr.ToString());
			instr.Operand = operand;
			return builder;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				Action onDispose = this.OnDispose;
				if (onDispose != null)
				{
					onDispose();
				}
				this.OnDispose = null;
				foreach (DataScope<DynamicReferenceCell> dataScope in this.managedObjectRefs)
				{
					dataScope.Dispose();
				}
				this.managedObjectRefs.Clear();
				this.managedObjectRefs.Capacity = 0;
				this.MakeReadOnly();
				this.disposedValue = true;
			}
		}

		~ILContext()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal List<ILLabel> _Labels = new List<ILLabel>();

		private bool disposedValue;

		[Nullable(new byte[] { 1, 0 })]
		private readonly List<DataScope<DynamicReferenceCell>> managedObjectRefs = new List<DataScope<DynamicReferenceCell>>();

		[NullableContext(0)]
		public delegate void Manipulator(ILContext il);
	}
}
