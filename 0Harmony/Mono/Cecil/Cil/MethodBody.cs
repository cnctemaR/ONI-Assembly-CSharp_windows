using System;
using System.Threading;
using Mono.Collections.Generic;

namespace Mono.Cecil.Cil
{
	internal sealed class MethodBody
	{
		public MethodDefinition Method
		{
			get
			{
				return this.method;
			}
		}

		public int MaxStackSize
		{
			get
			{
				return this.max_stack_size;
			}
			set
			{
				this.max_stack_size = value;
			}
		}

		public int CodeSize
		{
			get
			{
				return this.code_size;
			}
		}

		public bool InitLocals
		{
			get
			{
				return this.init_locals;
			}
			set
			{
				this.init_locals = value;
			}
		}

		public MetadataToken LocalVarToken
		{
			get
			{
				return this.local_var_token;
			}
			set
			{
				this.local_var_token = value;
			}
		}

		public Collection<Instruction> Instructions
		{
			get
			{
				if (this.instructions == null)
				{
					Interlocked.CompareExchange<Collection<Instruction>>(ref this.instructions, new InstructionCollection(this.method), null);
				}
				return this.instructions;
			}
		}

		public bool HasExceptionHandlers
		{
			get
			{
				return !this.exceptions.IsNullOrEmpty<ExceptionHandler>();
			}
		}

		public Collection<ExceptionHandler> ExceptionHandlers
		{
			get
			{
				if (this.exceptions == null)
				{
					Interlocked.CompareExchange<Collection<ExceptionHandler>>(ref this.exceptions, new Collection<ExceptionHandler>(), null);
				}
				return this.exceptions;
			}
		}

		public bool HasVariables
		{
			get
			{
				return !this.variables.IsNullOrEmpty<VariableDefinition>();
			}
		}

		public Collection<VariableDefinition> Variables
		{
			get
			{
				if (this.variables == null)
				{
					Interlocked.CompareExchange<Collection<VariableDefinition>>(ref this.variables, new VariableDefinitionCollection(this.method), null);
				}
				return this.variables;
			}
		}

		public ParameterDefinition ThisParameter
		{
			get
			{
				if (this.method == null || this.method.DeclaringType == null)
				{
					throw new NotSupportedException();
				}
				if (!this.method.HasThis)
				{
					return null;
				}
				if (this.this_parameter == null)
				{
					Interlocked.CompareExchange<ParameterDefinition>(ref this.this_parameter, MethodBody.CreateThisParameter(this.method), null);
				}
				return this.this_parameter;
			}
		}

		private static ParameterDefinition CreateThisParameter(MethodDefinition method)
		{
			TypeReference typeReference = method.DeclaringType;
			if (typeReference.HasGenericParameters)
			{
				GenericInstanceType genericInstanceType = new GenericInstanceType(typeReference, typeReference.GenericParameters.Count);
				for (int i = 0; i < typeReference.GenericParameters.Count; i++)
				{
					genericInstanceType.GenericArguments.Add(typeReference.GenericParameters[i]);
				}
				typeReference = genericInstanceType;
			}
			if (typeReference.IsValueType || typeReference.IsPrimitive)
			{
				typeReference = new ByReferenceType(typeReference);
			}
			return new ParameterDefinition(typeReference, method);
		}

		public MethodBody(MethodDefinition method)
		{
			this.method = method;
		}

		public ILProcessor GetILProcessor()
		{
			return new ILProcessor(this);
		}

		internal readonly MethodDefinition method;

		internal ParameterDefinition this_parameter;

		internal int max_stack_size;

		internal int code_size;

		internal bool init_locals;

		internal MetadataToken local_var_token;

		internal Collection<Instruction> instructions;

		internal Collection<ExceptionHandler> exceptions;

		internal Collection<VariableDefinition> variables;
	}
}
