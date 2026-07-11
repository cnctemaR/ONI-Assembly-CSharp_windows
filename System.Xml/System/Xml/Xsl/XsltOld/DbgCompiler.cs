using System;
using System.Collections;
using System.Xml.Xsl.XsltOld.Debugger;

namespace System.Xml.Xsl.XsltOld
{
	internal class DbgCompiler : Compiler
	{
		public DbgCompiler(IXsltDebugger debugger)
		{
			this.debugger = debugger;
		}

		public override IXsltDebugger Debugger
		{
			get
			{
				return this.debugger;
			}
		}

		public virtual VariableAction[] GlobalVariables
		{
			get
			{
				if (this.globalVarsCache == null)
				{
					this.globalVarsCache = (VariableAction[])this.globalVars.ToArray(typeof(VariableAction));
				}
				return this.globalVarsCache;
			}
		}

		public virtual VariableAction[] LocalVariables
		{
			get
			{
				if (this.localVarsCache == null)
				{
					this.localVarsCache = (VariableAction[])this.localVars.ToArray(typeof(VariableAction));
				}
				return this.localVarsCache;
			}
		}

		private void DefineVariable(VariableAction variable)
		{
			if (variable.IsGlobal)
			{
				for (int i = 0; i < this.globalVars.Count; i++)
				{
					VariableAction variableAction = (VariableAction)this.globalVars[i];
					if (variableAction.Name == variable.Name)
					{
						if (variable.Stylesheetid < variableAction.Stylesheetid)
						{
							this.globalVars[i] = variable;
							this.globalVarsCache = null;
						}
						return;
					}
				}
				this.globalVars.Add(variable);
				this.globalVarsCache = null;
				return;
			}
			this.localVars.Add(variable);
			this.localVarsCache = null;
		}

		private void UnDefineVariables(int count)
		{
			if (count != 0)
			{
				this.localVars.RemoveRange(this.localVars.Count - count, count);
				this.localVarsCache = null;
			}
		}

		internal override void PopScope()
		{
			this.UnDefineVariables(base.ScopeManager.CurrentScope.GetVeriablesCount());
			base.PopScope();
		}

		public override ApplyImportsAction CreateApplyImportsAction()
		{
			DbgCompiler.ApplyImportsActionDbg applyImportsActionDbg = new DbgCompiler.ApplyImportsActionDbg();
			applyImportsActionDbg.Compile(this);
			return applyImportsActionDbg;
		}

		public override ApplyTemplatesAction CreateApplyTemplatesAction()
		{
			DbgCompiler.ApplyTemplatesActionDbg applyTemplatesActionDbg = new DbgCompiler.ApplyTemplatesActionDbg();
			applyTemplatesActionDbg.Compile(this);
			return applyTemplatesActionDbg;
		}

		public override AttributeAction CreateAttributeAction()
		{
			DbgCompiler.AttributeActionDbg attributeActionDbg = new DbgCompiler.AttributeActionDbg();
			attributeActionDbg.Compile(this);
			return attributeActionDbg;
		}

		public override AttributeSetAction CreateAttributeSetAction()
		{
			DbgCompiler.AttributeSetActionDbg attributeSetActionDbg = new DbgCompiler.AttributeSetActionDbg();
			attributeSetActionDbg.Compile(this);
			return attributeSetActionDbg;
		}

		public override CallTemplateAction CreateCallTemplateAction()
		{
			DbgCompiler.CallTemplateActionDbg callTemplateActionDbg = new DbgCompiler.CallTemplateActionDbg();
			callTemplateActionDbg.Compile(this);
			return callTemplateActionDbg;
		}

		public override ChooseAction CreateChooseAction()
		{
			ChooseAction chooseAction = new ChooseAction();
			chooseAction.Compile(this);
			return chooseAction;
		}

		public override CommentAction CreateCommentAction()
		{
			DbgCompiler.CommentActionDbg commentActionDbg = new DbgCompiler.CommentActionDbg();
			commentActionDbg.Compile(this);
			return commentActionDbg;
		}

		public override CopyAction CreateCopyAction()
		{
			DbgCompiler.CopyActionDbg copyActionDbg = new DbgCompiler.CopyActionDbg();
			copyActionDbg.Compile(this);
			return copyActionDbg;
		}

		public override CopyOfAction CreateCopyOfAction()
		{
			DbgCompiler.CopyOfActionDbg copyOfActionDbg = new DbgCompiler.CopyOfActionDbg();
			copyOfActionDbg.Compile(this);
			return copyOfActionDbg;
		}

		public override ElementAction CreateElementAction()
		{
			DbgCompiler.ElementActionDbg elementActionDbg = new DbgCompiler.ElementActionDbg();
			elementActionDbg.Compile(this);
			return elementActionDbg;
		}

		public override ForEachAction CreateForEachAction()
		{
			DbgCompiler.ForEachActionDbg forEachActionDbg = new DbgCompiler.ForEachActionDbg();
			forEachActionDbg.Compile(this);
			return forEachActionDbg;
		}

		public override IfAction CreateIfAction(IfAction.ConditionType type)
		{
			DbgCompiler.IfActionDbg ifActionDbg = new DbgCompiler.IfActionDbg(type);
			ifActionDbg.Compile(this);
			return ifActionDbg;
		}

		public override MessageAction CreateMessageAction()
		{
			DbgCompiler.MessageActionDbg messageActionDbg = new DbgCompiler.MessageActionDbg();
			messageActionDbg.Compile(this);
			return messageActionDbg;
		}

		public override NewInstructionAction CreateNewInstructionAction()
		{
			DbgCompiler.NewInstructionActionDbg newInstructionActionDbg = new DbgCompiler.NewInstructionActionDbg();
			newInstructionActionDbg.Compile(this);
			return newInstructionActionDbg;
		}

		public override NumberAction CreateNumberAction()
		{
			DbgCompiler.NumberActionDbg numberActionDbg = new DbgCompiler.NumberActionDbg();
			numberActionDbg.Compile(this);
			return numberActionDbg;
		}

		public override ProcessingInstructionAction CreateProcessingInstructionAction()
		{
			DbgCompiler.ProcessingInstructionActionDbg processingInstructionActionDbg = new DbgCompiler.ProcessingInstructionActionDbg();
			processingInstructionActionDbg.Compile(this);
			return processingInstructionActionDbg;
		}

		public override void CreateRootAction()
		{
			base.RootAction = new DbgCompiler.RootActionDbg();
			base.RootAction.Compile(this);
		}

		public override SortAction CreateSortAction()
		{
			DbgCompiler.SortActionDbg sortActionDbg = new DbgCompiler.SortActionDbg();
			sortActionDbg.Compile(this);
			return sortActionDbg;
		}

		public override TemplateAction CreateTemplateAction()
		{
			DbgCompiler.TemplateActionDbg templateActionDbg = new DbgCompiler.TemplateActionDbg();
			templateActionDbg.Compile(this);
			return templateActionDbg;
		}

		public override TemplateAction CreateSingleTemplateAction()
		{
			DbgCompiler.TemplateActionDbg templateActionDbg = new DbgCompiler.TemplateActionDbg();
			templateActionDbg.CompileSingle(this);
			return templateActionDbg;
		}

		public override TextAction CreateTextAction()
		{
			DbgCompiler.TextActionDbg textActionDbg = new DbgCompiler.TextActionDbg();
			textActionDbg.Compile(this);
			return textActionDbg;
		}

		public override UseAttributeSetsAction CreateUseAttributeSetsAction()
		{
			DbgCompiler.UseAttributeSetsActionDbg useAttributeSetsActionDbg = new DbgCompiler.UseAttributeSetsActionDbg();
			useAttributeSetsActionDbg.Compile(this);
			return useAttributeSetsActionDbg;
		}

		public override ValueOfAction CreateValueOfAction()
		{
			DbgCompiler.ValueOfActionDbg valueOfActionDbg = new DbgCompiler.ValueOfActionDbg();
			valueOfActionDbg.Compile(this);
			return valueOfActionDbg;
		}

		public override VariableAction CreateVariableAction(VariableType type)
		{
			DbgCompiler.VariableActionDbg variableActionDbg = new DbgCompiler.VariableActionDbg(type);
			variableActionDbg.Compile(this);
			return variableActionDbg;
		}

		public override WithParamAction CreateWithParamAction()
		{
			DbgCompiler.WithParamActionDbg withParamActionDbg = new DbgCompiler.WithParamActionDbg();
			withParamActionDbg.Compile(this);
			return withParamActionDbg;
		}

		public override BeginEvent CreateBeginEvent()
		{
			return new DbgCompiler.BeginEventDbg(this);
		}

		public override TextEvent CreateTextEvent()
		{
			return new DbgCompiler.TextEventDbg(this);
		}

		private IXsltDebugger debugger;

		private ArrayList globalVars = new ArrayList();

		private ArrayList localVars = new ArrayList();

		private VariableAction[] globalVarsCache;

		private VariableAction[] localVarsCache;

		private class ApplyImportsActionDbg : ApplyImportsAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class ApplyTemplatesActionDbg : ApplyTemplatesAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class AttributeActionDbg : AttributeAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class AttributeSetActionDbg : AttributeSetAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class CallTemplateActionDbg : CallTemplateAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class CommentActionDbg : CommentAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class CopyActionDbg : CopyAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class CopyOfActionDbg : CopyOfAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class ElementActionDbg : ElementAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class ForEachActionDbg : ForEachAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.PushDebuggerStack();
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
				if (frame.State == -1)
				{
					processor.PopDebuggerStack();
				}
			}

			private DbgData dbgData;
		}

		private class IfActionDbg : IfAction
		{
			internal IfActionDbg(IfAction.ConditionType type)
				: base(type)
			{
			}

			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class MessageActionDbg : MessageAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class NewInstructionActionDbg : NewInstructionAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class NumberActionDbg : NumberAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class ProcessingInstructionActionDbg : ProcessingInstructionAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class RootActionDbg : RootAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
				string builtInTemplatesUri = compiler.Debugger.GetBuiltInTemplatesUri();
				if (builtInTemplatesUri != null && builtInTemplatesUri.Length != 0)
				{
					compiler.AllowBuiltInMode = true;
					this.builtInSheet = compiler.RootAction.CompileImport(compiler, compiler.ResolveUri(builtInTemplatesUri), int.MaxValue);
					compiler.AllowBuiltInMode = false;
				}
				this.dbgData.ReplaceVariables(((DbgCompiler)compiler).GlobalVariables);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.PushDebuggerStack();
					processor.OnInstructionExecute();
					processor.PushDebuggerStack();
				}
				base.Execute(processor, frame);
				if (frame.State == -1)
				{
					processor.PopDebuggerStack();
					processor.PopDebuggerStack();
				}
			}

			private DbgData dbgData;
		}

		private class SortActionDbg : SortAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class TemplateActionDbg : TemplateAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.PushDebuggerStack();
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
				if (frame.State == -1)
				{
					processor.PopDebuggerStack();
				}
			}

			private DbgData dbgData;
		}

		private class TextActionDbg : TextAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class UseAttributeSetsActionDbg : UseAttributeSetsAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class ValueOfActionDbg : ValueOfAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class VariableActionDbg : VariableAction
		{
			internal VariableActionDbg(VariableType type)
				: base(type)
			{
			}

			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
				((DbgCompiler)compiler).DefineVariable(this);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class WithParamActionDbg : WithParamAction
		{
			internal override DbgData GetDbgData(ActionFrame frame)
			{
				return this.dbgData;
			}

			internal override void Compile(Compiler compiler)
			{
				this.dbgData = new DbgData(compiler);
				base.Compile(compiler);
			}

			internal override void Execute(Processor processor, ActionFrame frame)
			{
				if (frame.State == 0)
				{
					processor.OnInstructionExecute();
				}
				base.Execute(processor, frame);
			}

			private DbgData dbgData;
		}

		private class BeginEventDbg : BeginEvent
		{
			internal override DbgData DbgData
			{
				get
				{
					return this.dbgData;
				}
			}

			public BeginEventDbg(Compiler compiler)
				: base(compiler)
			{
				this.dbgData = new DbgData(compiler);
			}

			public override bool Output(Processor processor, ActionFrame frame)
			{
				base.OnInstructionExecute(processor);
				return base.Output(processor, frame);
			}

			private DbgData dbgData;
		}

		private class TextEventDbg : TextEvent
		{
			internal override DbgData DbgData
			{
				get
				{
					return this.dbgData;
				}
			}

			public TextEventDbg(Compiler compiler)
				: base(compiler)
			{
				this.dbgData = new DbgData(compiler);
			}

			public override bool Output(Processor processor, ActionFrame frame)
			{
				base.OnInstructionExecute(processor);
				return base.Output(processor, frame);
			}

			private DbgData dbgData;
		}
	}
}
