using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Text.RegularExpressions
{
	internal abstract class RegexCompiler
	{
		private static FieldInfo RegexRunnerField(string fieldname)
		{
			return typeof(RegexRunner).GetField(fieldname, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		private static MethodInfo RegexRunnerMethod(string methname)
		{
			return typeof(RegexRunner).GetMethod(methname, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		internal static RegexRunnerFactory Compile(RegexCode code, RegexOptions options)
		{
			return new RegexLWCGCompiler().FactoryInstanceFromCode(code, options);
		}

		internal static void CompileToAssembly(RegexCompilationInfo[] regexes, AssemblyName an, CustomAttributeBuilder[] attribs, string resourceFile)
		{
			RegexTypeCompiler regexTypeCompiler = new RegexTypeCompiler(an, attribs, resourceFile);
			for (int i = 0; i < regexes.Length; i++)
			{
				if (regexes[i] == null)
				{
					throw new ArgumentNullException("regexes", global::SR.GetString("The array cannot contain null elements."));
				}
				string pattern = regexes[i].Pattern;
				RegexOptions options = regexes[i].Options;
				string text;
				if (regexes[i].Namespace.Length == 0)
				{
					text = regexes[i].Name;
				}
				else
				{
					text = regexes[i].Namespace + "." + regexes[i].Name;
				}
				TimeSpan matchTimeout = regexes[i].MatchTimeout;
				RegexTree regexTree = RegexParser.Parse(pattern, options);
				RegexCode regexCode = RegexWriter.Write(regexTree);
				Type type = regexTypeCompiler.FactoryTypeFromCode(regexCode, options, text);
				regexTypeCompiler.GenerateRegexType(pattern, options, text, regexes[i].IsPublic, regexCode, regexTree, type, matchTimeout);
			}
			regexTypeCompiler.Save();
		}

		internal int AddBacktrackNote(int flags, Label l, int codepos)
		{
			if (this._notes == null || this._notecount >= this._notes.Length)
			{
				RegexCompiler.BacktrackNote[] array = new RegexCompiler.BacktrackNote[(this._notes == null) ? 16 : (this._notes.Length * 2)];
				if (this._notes != null)
				{
					Array.Copy(this._notes, 0, array, 0, this._notecount);
				}
				this._notes = array;
			}
			this._notes[this._notecount] = new RegexCompiler.BacktrackNote(flags, l, codepos);
			int notecount = this._notecount;
			this._notecount = notecount + 1;
			return notecount;
		}

		internal int AddTrack()
		{
			return this.AddTrack(128);
		}

		internal int AddTrack(int flags)
		{
			return this.AddBacktrackNote(flags, this.DefineLabel(), this._codepos);
		}

		internal int AddGoto(int destpos)
		{
			if (this._goto[destpos] == -1)
			{
				this._goto[destpos] = this.AddBacktrackNote(0, this._labels[destpos], destpos);
			}
			return this._goto[destpos];
		}

		internal int AddUniqueTrack(int i)
		{
			return this.AddUniqueTrack(i, 128);
		}

		internal int AddUniqueTrack(int i, int flags)
		{
			if (this._uniquenote[i] == -1)
			{
				this._uniquenote[i] = this.AddTrack(flags);
			}
			return this._uniquenote[i];
		}

		internal Label DefineLabel()
		{
			return this._ilg.DefineLabel();
		}

		internal void MarkLabel(Label l)
		{
			this._ilg.MarkLabel(l);
		}

		internal int Operand(int i)
		{
			return this._codes[this._codepos + i + 1];
		}

		internal bool IsRtl()
		{
			return (this._regexopcode & 64) != 0;
		}

		internal bool IsCi()
		{
			return (this._regexopcode & 512) != 0;
		}

		internal int Code()
		{
			return this._regexopcode & 63;
		}

		internal void Ldstr(string str)
		{
			this._ilg.Emit(OpCodes.Ldstr, str);
		}

		internal void Ldc(int i)
		{
			if (i <= 127 && i >= -128)
			{
				this._ilg.Emit(OpCodes.Ldc_I4_S, (byte)i);
				return;
			}
			this._ilg.Emit(OpCodes.Ldc_I4, i);
		}

		internal void LdcI8(long i)
		{
			if (i <= 2147483647L && i >= -2147483648L)
			{
				this.Ldc((int)i);
				this._ilg.Emit(OpCodes.Conv_I8);
				return;
			}
			this._ilg.Emit(OpCodes.Ldc_I8, i);
		}

		internal void Dup()
		{
			this._ilg.Emit(OpCodes.Dup);
		}

		internal void Ret()
		{
			this._ilg.Emit(OpCodes.Ret);
		}

		internal void Pop()
		{
			this._ilg.Emit(OpCodes.Pop);
		}

		internal void Add()
		{
			this._ilg.Emit(OpCodes.Add);
		}

		internal void Add(bool negate)
		{
			if (negate)
			{
				this._ilg.Emit(OpCodes.Sub);
				return;
			}
			this._ilg.Emit(OpCodes.Add);
		}

		internal void Sub()
		{
			this._ilg.Emit(OpCodes.Sub);
		}

		internal void Sub(bool negate)
		{
			if (negate)
			{
				this._ilg.Emit(OpCodes.Add);
				return;
			}
			this._ilg.Emit(OpCodes.Sub);
		}

		internal void Ldloc(LocalBuilder lt)
		{
			this._ilg.Emit(OpCodes.Ldloc_S, lt);
		}

		internal void Stloc(LocalBuilder lt)
		{
			this._ilg.Emit(OpCodes.Stloc_S, lt);
		}

		internal void Ldthis()
		{
			this._ilg.Emit(OpCodes.Ldarg_0);
		}

		internal void Ldthisfld(FieldInfo ft)
		{
			this.Ldthis();
			this._ilg.Emit(OpCodes.Ldfld, ft);
		}

		internal void Mvfldloc(FieldInfo ft, LocalBuilder lt)
		{
			this.Ldthisfld(ft);
			this.Stloc(lt);
		}

		internal void Mvlocfld(LocalBuilder lt, FieldInfo ft)
		{
			this.Ldthis();
			this.Ldloc(lt);
			this.Stfld(ft);
		}

		internal void Stfld(FieldInfo ft)
		{
			this._ilg.Emit(OpCodes.Stfld, ft);
		}

		internal void Callvirt(MethodInfo mt)
		{
			this._ilg.Emit(OpCodes.Callvirt, mt);
		}

		internal void Call(MethodInfo mt)
		{
			this._ilg.Emit(OpCodes.Call, mt);
		}

		internal void Newobj(ConstructorInfo ct)
		{
			this._ilg.Emit(OpCodes.Newobj, ct);
		}

		internal void BrfalseFar(Label l)
		{
			this._ilg.Emit(OpCodes.Brfalse, l);
		}

		internal void BrtrueFar(Label l)
		{
			this._ilg.Emit(OpCodes.Brtrue, l);
		}

		internal void BrFar(Label l)
		{
			this._ilg.Emit(OpCodes.Br, l);
		}

		internal void BleFar(Label l)
		{
			this._ilg.Emit(OpCodes.Ble, l);
		}

		internal void BltFar(Label l)
		{
			this._ilg.Emit(OpCodes.Blt, l);
		}

		internal void BgeFar(Label l)
		{
			this._ilg.Emit(OpCodes.Bge, l);
		}

		internal void BgtFar(Label l)
		{
			this._ilg.Emit(OpCodes.Bgt, l);
		}

		internal void BneFar(Label l)
		{
			this._ilg.Emit(OpCodes.Bne_Un, l);
		}

		internal void BeqFar(Label l)
		{
			this._ilg.Emit(OpCodes.Beq, l);
		}

		internal void Brfalse(Label l)
		{
			this._ilg.Emit(OpCodes.Brfalse_S, l);
		}

		internal void Br(Label l)
		{
			this._ilg.Emit(OpCodes.Br_S, l);
		}

		internal void Ble(Label l)
		{
			this._ilg.Emit(OpCodes.Ble_S, l);
		}

		internal void Blt(Label l)
		{
			this._ilg.Emit(OpCodes.Blt_S, l);
		}

		internal void Bge(Label l)
		{
			this._ilg.Emit(OpCodes.Bge_S, l);
		}

		internal void Bgt(Label l)
		{
			this._ilg.Emit(OpCodes.Bgt_S, l);
		}

		internal void Bgtun(Label l)
		{
			this._ilg.Emit(OpCodes.Bgt_Un_S, l);
		}

		internal void Bne(Label l)
		{
			this._ilg.Emit(OpCodes.Bne_Un_S, l);
		}

		internal void Beq(Label l)
		{
			this._ilg.Emit(OpCodes.Beq_S, l);
		}

		internal void Ldlen()
		{
			this._ilg.Emit(OpCodes.Ldlen);
		}

		internal void Rightchar()
		{
			this.Ldloc(this._textV);
			this.Ldloc(this._textposV);
			this.Callvirt(RegexCompiler._getcharM);
		}

		internal void Rightcharnext()
		{
			this.Ldloc(this._textV);
			this.Ldloc(this._textposV);
			this.Dup();
			this.Ldc(1);
			this.Add();
			this.Stloc(this._textposV);
			this.Callvirt(RegexCompiler._getcharM);
		}

		internal void Leftchar()
		{
			this.Ldloc(this._textV);
			this.Ldloc(this._textposV);
			this.Ldc(1);
			this.Sub();
			this.Callvirt(RegexCompiler._getcharM);
		}

		internal void Leftcharnext()
		{
			this.Ldloc(this._textV);
			this.Ldloc(this._textposV);
			this.Ldc(1);
			this.Sub();
			this.Dup();
			this.Stloc(this._textposV);
			this.Callvirt(RegexCompiler._getcharM);
		}

		internal void Track()
		{
			this.ReadyPushTrack();
			this.Ldc(this.AddTrack());
			this.DoPush();
		}

		internal void Trackagain()
		{
			this.ReadyPushTrack();
			this.Ldc(this._backpos);
			this.DoPush();
		}

		internal void PushTrack(LocalBuilder lt)
		{
			this.ReadyPushTrack();
			this.Ldloc(lt);
			this.DoPush();
		}

		internal void TrackUnique(int i)
		{
			this.ReadyPushTrack();
			this.Ldc(this.AddUniqueTrack(i));
			this.DoPush();
		}

		internal void TrackUnique2(int i)
		{
			this.ReadyPushTrack();
			this.Ldc(this.AddUniqueTrack(i, 256));
			this.DoPush();
		}

		internal void ReadyPushTrack()
		{
			this._ilg.Emit(OpCodes.Ldloc_S, this._trackV);
			this._ilg.Emit(OpCodes.Ldloc_S, this._trackposV);
			this._ilg.Emit(OpCodes.Ldc_I4_1);
			this._ilg.Emit(OpCodes.Sub);
			this._ilg.Emit(OpCodes.Dup);
			this._ilg.Emit(OpCodes.Stloc_S, this._trackposV);
		}

		internal void PopTrack()
		{
			this._ilg.Emit(OpCodes.Ldloc_S, this._trackV);
			this._ilg.Emit(OpCodes.Ldloc_S, this._trackposV);
			this._ilg.Emit(OpCodes.Dup);
			this._ilg.Emit(OpCodes.Ldc_I4_1);
			this._ilg.Emit(OpCodes.Add);
			this._ilg.Emit(OpCodes.Stloc_S, this._trackposV);
			this._ilg.Emit(OpCodes.Ldelem_I4);
		}

		internal void TopTrack()
		{
			this._ilg.Emit(OpCodes.Ldloc_S, this._trackV);
			this._ilg.Emit(OpCodes.Ldloc_S, this._trackposV);
			this._ilg.Emit(OpCodes.Ldelem_I4);
		}

		internal void PushStack(LocalBuilder lt)
		{
			this.ReadyPushStack();
			this._ilg.Emit(OpCodes.Ldloc_S, lt);
			this.DoPush();
		}

		internal void ReadyReplaceStack(int i)
		{
			this._ilg.Emit(OpCodes.Ldloc_S, this._stackV);
			this._ilg.Emit(OpCodes.Ldloc_S, this._stackposV);
			if (i != 0)
			{
				this.Ldc(i);
				this._ilg.Emit(OpCodes.Add);
			}
		}

		internal void ReadyPushStack()
		{
			this._ilg.Emit(OpCodes.Ldloc_S, this._stackV);
			this._ilg.Emit(OpCodes.Ldloc_S, this._stackposV);
			this._ilg.Emit(OpCodes.Ldc_I4_1);
			this._ilg.Emit(OpCodes.Sub);
			this._ilg.Emit(OpCodes.Dup);
			this._ilg.Emit(OpCodes.Stloc_S, this._stackposV);
		}

		internal void TopStack()
		{
			this._ilg.Emit(OpCodes.Ldloc_S, this._stackV);
			this._ilg.Emit(OpCodes.Ldloc_S, this._stackposV);
			this._ilg.Emit(OpCodes.Ldelem_I4);
		}

		internal void PopStack()
		{
			this._ilg.Emit(OpCodes.Ldloc_S, this._stackV);
			this._ilg.Emit(OpCodes.Ldloc_S, this._stackposV);
			this._ilg.Emit(OpCodes.Dup);
			this._ilg.Emit(OpCodes.Ldc_I4_1);
			this._ilg.Emit(OpCodes.Add);
			this._ilg.Emit(OpCodes.Stloc_S, this._stackposV);
			this._ilg.Emit(OpCodes.Ldelem_I4);
		}

		internal void PopDiscardStack()
		{
			this.PopDiscardStack(1);
		}

		internal void PopDiscardStack(int i)
		{
			this._ilg.Emit(OpCodes.Ldloc_S, this._stackposV);
			this.Ldc(i);
			this._ilg.Emit(OpCodes.Add);
			this._ilg.Emit(OpCodes.Stloc_S, this._stackposV);
		}

		internal void DoReplace()
		{
			this._ilg.Emit(OpCodes.Stelem_I4);
		}

		internal void DoPush()
		{
			this._ilg.Emit(OpCodes.Stelem_I4);
		}

		internal void Back()
		{
			this._ilg.Emit(OpCodes.Br, this._backtrack);
		}

		internal void Goto(int i)
		{
			if (i < this._codepos)
			{
				Label label = this.DefineLabel();
				this.Ldloc(this._trackposV);
				this.Ldc(this._trackcount * 4);
				this.Ble(label);
				this.Ldloc(this._stackposV);
				this.Ldc(this._trackcount * 3);
				this.BgtFar(this._labels[i]);
				this.MarkLabel(label);
				this.ReadyPushTrack();
				this.Ldc(this.AddGoto(i));
				this.DoPush();
				this.BrFar(this._backtrack);
				return;
			}
			this.BrFar(this._labels[i]);
		}

		internal int NextCodepos()
		{
			return this._codepos + RegexCode.OpcodeSize(this._codes[this._codepos]);
		}

		internal Label AdvanceLabel()
		{
			return this._labels[this.NextCodepos()];
		}

		internal void Advance()
		{
			this._ilg.Emit(OpCodes.Br, this.AdvanceLabel());
		}

		internal void CallToLower()
		{
			if ((this._options & RegexOptions.CultureInvariant) != RegexOptions.None)
			{
				this.Call(RegexCompiler._getInvariantCulture);
			}
			else
			{
				this.Call(RegexCompiler._getCurrentCulture);
			}
			this.Call(RegexCompiler._chartolowerM);
		}

		internal void GenerateForwardSection()
		{
			this._labels = new Label[this._codes.Length];
			this._goto = new int[this._codes.Length];
			for (int i = 0; i < this._codes.Length; i += RegexCode.OpcodeSize(this._codes[i]))
			{
				this._goto[i] = -1;
				this._labels[i] = this._ilg.DefineLabel();
			}
			this._uniquenote = new int[10];
			for (int j = 0; j < 10; j++)
			{
				this._uniquenote[j] = -1;
			}
			this.Mvfldloc(RegexCompiler._textF, this._textV);
			this.Mvfldloc(RegexCompiler._textstartF, this._textstartV);
			this.Mvfldloc(RegexCompiler._textbegF, this._textbegV);
			this.Mvfldloc(RegexCompiler._textendF, this._textendV);
			this.Mvfldloc(RegexCompiler._textposF, this._textposV);
			this.Mvfldloc(RegexCompiler._trackF, this._trackV);
			this.Mvfldloc(RegexCompiler._trackposF, this._trackposV);
			this.Mvfldloc(RegexCompiler._stackF, this._stackV);
			this.Mvfldloc(RegexCompiler._stackposF, this._stackposV);
			this._backpos = -1;
			for (int i = 0; i < this._codes.Length; i += RegexCode.OpcodeSize(this._codes[i]))
			{
				this.MarkLabel(this._labels[i]);
				this._codepos = i;
				this._regexopcode = this._codes[i];
				this.GenerateOneCode();
			}
		}

		internal void GenerateMiddleSection()
		{
			this.DefineLabel();
			this.MarkLabel(this._backtrack);
			this.Mvlocfld(this._trackposV, RegexCompiler._trackposF);
			this.Mvlocfld(this._stackposV, RegexCompiler._stackposF);
			this.Ldthis();
			this.Callvirt(RegexCompiler._ensurestorageM);
			this.Mvfldloc(RegexCompiler._trackposF, this._trackposV);
			this.Mvfldloc(RegexCompiler._stackposF, this._stackposV);
			this.Mvfldloc(RegexCompiler._trackF, this._trackV);
			this.Mvfldloc(RegexCompiler._stackF, this._stackV);
			this.PopTrack();
			Label[] array = new Label[this._notecount];
			for (int i = 0; i < this._notecount; i++)
			{
				array[i] = this._notes[i]._label;
			}
			this._ilg.Emit(OpCodes.Switch, array);
		}

		internal void GenerateBacktrackSection()
		{
			for (int i = 0; i < this._notecount; i++)
			{
				RegexCompiler.BacktrackNote backtrackNote = this._notes[i];
				if (backtrackNote._flags != 0)
				{
					this._ilg.MarkLabel(backtrackNote._label);
					this._codepos = backtrackNote._codepos;
					this._backpos = i;
					this._regexopcode = this._codes[backtrackNote._codepos] | backtrackNote._flags;
					this.GenerateOneCode();
				}
			}
		}

		internal void GenerateFindFirstChar()
		{
			this._textposV = this.DeclareInt();
			this._textV = this.DeclareString();
			this._tempV = this.DeclareInt();
			this._temp2V = this.DeclareInt();
			if ((this._anchors & 53) != 0)
			{
				if (!this._code._rightToLeft)
				{
					if ((this._anchors & 1) != 0)
					{
						Label label = this.DefineLabel();
						this.Ldthisfld(RegexCompiler._textposF);
						this.Ldthisfld(RegexCompiler._textbegF);
						this.Ble(label);
						this.Ldthis();
						this.Ldthisfld(RegexCompiler._textendF);
						this.Stfld(RegexCompiler._textposF);
						this.Ldc(0);
						this.Ret();
						this.MarkLabel(label);
					}
					if ((this._anchors & 4) != 0)
					{
						Label label2 = this.DefineLabel();
						this.Ldthisfld(RegexCompiler._textposF);
						this.Ldthisfld(RegexCompiler._textstartF);
						this.Ble(label2);
						this.Ldthis();
						this.Ldthisfld(RegexCompiler._textendF);
						this.Stfld(RegexCompiler._textposF);
						this.Ldc(0);
						this.Ret();
						this.MarkLabel(label2);
					}
					if ((this._anchors & 16) != 0)
					{
						Label label3 = this.DefineLabel();
						this.Ldthisfld(RegexCompiler._textposF);
						this.Ldthisfld(RegexCompiler._textendF);
						this.Ldc(1);
						this.Sub();
						this.Bge(label3);
						this.Ldthis();
						this.Ldthisfld(RegexCompiler._textendF);
						this.Ldc(1);
						this.Sub();
						this.Stfld(RegexCompiler._textposF);
						this.MarkLabel(label3);
					}
					if ((this._anchors & 32) != 0)
					{
						Label label4 = this.DefineLabel();
						this.Ldthisfld(RegexCompiler._textposF);
						this.Ldthisfld(RegexCompiler._textendF);
						this.Bge(label4);
						this.Ldthis();
						this.Ldthisfld(RegexCompiler._textendF);
						this.Stfld(RegexCompiler._textposF);
						this.MarkLabel(label4);
					}
				}
				else
				{
					if ((this._anchors & 32) != 0)
					{
						Label label5 = this.DefineLabel();
						this.Ldthisfld(RegexCompiler._textposF);
						this.Ldthisfld(RegexCompiler._textendF);
						this.Bge(label5);
						this.Ldthis();
						this.Ldthisfld(RegexCompiler._textbegF);
						this.Stfld(RegexCompiler._textposF);
						this.Ldc(0);
						this.Ret();
						this.MarkLabel(label5);
					}
					if ((this._anchors & 16) != 0)
					{
						Label label6 = this.DefineLabel();
						Label label7 = this.DefineLabel();
						this.Ldthisfld(RegexCompiler._textposF);
						this.Ldthisfld(RegexCompiler._textendF);
						this.Ldc(1);
						this.Sub();
						this.Blt(label6);
						this.Ldthisfld(RegexCompiler._textposF);
						this.Ldthisfld(RegexCompiler._textendF);
						this.Beq(label7);
						this.Ldthisfld(RegexCompiler._textF);
						this.Ldthisfld(RegexCompiler._textposF);
						this.Callvirt(RegexCompiler._getcharM);
						this.Ldc(10);
						this.Beq(label7);
						this.MarkLabel(label6);
						this.Ldthis();
						this.Ldthisfld(RegexCompiler._textbegF);
						this.Stfld(RegexCompiler._textposF);
						this.Ldc(0);
						this.Ret();
						this.MarkLabel(label7);
					}
					if ((this._anchors & 4) != 0)
					{
						Label label8 = this.DefineLabel();
						this.Ldthisfld(RegexCompiler._textposF);
						this.Ldthisfld(RegexCompiler._textstartF);
						this.Bge(label8);
						this.Ldthis();
						this.Ldthisfld(RegexCompiler._textbegF);
						this.Stfld(RegexCompiler._textposF);
						this.Ldc(0);
						this.Ret();
						this.MarkLabel(label8);
					}
					if ((this._anchors & 1) != 0)
					{
						Label label9 = this.DefineLabel();
						this.Ldthisfld(RegexCompiler._textposF);
						this.Ldthisfld(RegexCompiler._textbegF);
						this.Ble(label9);
						this.Ldthis();
						this.Ldthisfld(RegexCompiler._textbegF);
						this.Stfld(RegexCompiler._textposF);
						this.MarkLabel(label9);
					}
				}
				this.Ldc(1);
				this.Ret();
				return;
			}
			if (this._bmPrefix != null && this._bmPrefix._negativeUnicode == null)
			{
				LocalBuilder tempV = this._tempV;
				LocalBuilder tempV2 = this._tempV;
				LocalBuilder temp2V = this._temp2V;
				Label label10 = this.DefineLabel();
				Label label11 = this.DefineLabel();
				Label label12 = this.DefineLabel();
				Label label13 = this.DefineLabel();
				this.DefineLabel();
				Label label14 = this.DefineLabel();
				int num;
				int num2;
				if (!this._code._rightToLeft)
				{
					num = -1;
					num2 = this._bmPrefix._pattern.Length - 1;
				}
				else
				{
					num = this._bmPrefix._pattern.Length;
					num2 = 0;
				}
				int num3 = (int)this._bmPrefix._pattern[num2];
				this.Mvfldloc(RegexCompiler._textF, this._textV);
				if (!this._code._rightToLeft)
				{
					this.Ldthisfld(RegexCompiler._textendF);
				}
				else
				{
					this.Ldthisfld(RegexCompiler._textbegF);
				}
				this.Stloc(temp2V);
				this.Ldthisfld(RegexCompiler._textposF);
				if (!this._code._rightToLeft)
				{
					this.Ldc(this._bmPrefix._pattern.Length - 1);
					this.Add();
				}
				else
				{
					this.Ldc(this._bmPrefix._pattern.Length);
					this.Sub();
				}
				this.Stloc(this._textposV);
				this.Br(label13);
				this.MarkLabel(label10);
				if (!this._code._rightToLeft)
				{
					this.Ldc(this._bmPrefix._pattern.Length);
				}
				else
				{
					this.Ldc(-this._bmPrefix._pattern.Length);
				}
				this.MarkLabel(label11);
				this.Ldloc(this._textposV);
				this.Add();
				this.Stloc(this._textposV);
				this.MarkLabel(label13);
				this.Ldloc(this._textposV);
				this.Ldloc(temp2V);
				if (!this._code._rightToLeft)
				{
					this.BgeFar(label12);
				}
				else
				{
					this.BltFar(label12);
				}
				this.Rightchar();
				if (this._bmPrefix._caseInsensitive)
				{
					this.CallToLower();
				}
				this.Dup();
				this.Stloc(tempV);
				this.Ldc(num3);
				this.BeqFar(label14);
				this.Ldloc(tempV);
				this.Ldc(this._bmPrefix._lowASCII);
				this.Sub();
				this.Dup();
				this.Stloc(tempV);
				this.Ldc(this._bmPrefix._highASCII - this._bmPrefix._lowASCII);
				this.Bgtun(label10);
				Label[] array = new Label[this._bmPrefix._highASCII - this._bmPrefix._lowASCII + 1];
				for (int i = this._bmPrefix._lowASCII; i <= this._bmPrefix._highASCII; i++)
				{
					if (this._bmPrefix._negativeASCII[i] == num)
					{
						array[i - this._bmPrefix._lowASCII] = label10;
					}
					else
					{
						array[i - this._bmPrefix._lowASCII] = this.DefineLabel();
					}
				}
				this.Ldloc(tempV);
				this._ilg.Emit(OpCodes.Switch, array);
				for (int i = this._bmPrefix._lowASCII; i <= this._bmPrefix._highASCII; i++)
				{
					if (this._bmPrefix._negativeASCII[i] != num)
					{
						this.MarkLabel(array[i - this._bmPrefix._lowASCII]);
						this.Ldc(this._bmPrefix._negativeASCII[i]);
						this.BrFar(label11);
					}
				}
				this.MarkLabel(label14);
				this.Ldloc(this._textposV);
				this.Stloc(tempV2);
				for (int i = this._bmPrefix._pattern.Length - 2; i >= 0; i--)
				{
					Label label15 = this.DefineLabel();
					int num4;
					if (!this._code._rightToLeft)
					{
						num4 = i;
					}
					else
					{
						num4 = this._bmPrefix._pattern.Length - 1 - i;
					}
					this.Ldloc(this._textV);
					this.Ldloc(tempV2);
					this.Ldc(1);
					this.Sub(this._code._rightToLeft);
					this.Dup();
					this.Stloc(tempV2);
					this.Callvirt(RegexCompiler._getcharM);
					if (this._bmPrefix._caseInsensitive)
					{
						this.CallToLower();
					}
					this.Ldc((int)this._bmPrefix._pattern[num4]);
					this.Beq(label15);
					this.Ldc(this._bmPrefix._positive[num4]);
					this.BrFar(label11);
					this.MarkLabel(label15);
				}
				this.Ldthis();
				this.Ldloc(tempV2);
				if (this._code._rightToLeft)
				{
					this.Ldc(1);
					this.Add();
				}
				this.Stfld(RegexCompiler._textposF);
				this.Ldc(1);
				this.Ret();
				this.MarkLabel(label12);
				this.Ldthis();
				if (!this._code._rightToLeft)
				{
					this.Ldthisfld(RegexCompiler._textendF);
				}
				else
				{
					this.Ldthisfld(RegexCompiler._textbegF);
				}
				this.Stfld(RegexCompiler._textposF);
				this.Ldc(0);
				this.Ret();
				return;
			}
			if (this._fcPrefix == null)
			{
				this.Ldc(1);
				this.Ret();
				return;
			}
			LocalBuilder temp2V2 = this._temp2V;
			LocalBuilder tempV3 = this._tempV;
			Label label16 = this.DefineLabel();
			Label label17 = this.DefineLabel();
			Label label18 = this.DefineLabel();
			Label label19 = this.DefineLabel();
			Label label20 = this.DefineLabel();
			this.Mvfldloc(RegexCompiler._textposF, this._textposV);
			this.Mvfldloc(RegexCompiler._textF, this._textV);
			if (!this._code._rightToLeft)
			{
				this.Ldthisfld(RegexCompiler._textendF);
				this.Ldloc(this._textposV);
			}
			else
			{
				this.Ldloc(this._textposV);
				this.Ldthisfld(RegexCompiler._textbegF);
			}
			this.Sub();
			this.Stloc(temp2V2);
			this.Ldloc(temp2V2);
			this.Ldc(0);
			this.BleFar(label19);
			this.MarkLabel(label16);
			this.Ldloc(temp2V2);
			this.Ldc(1);
			this.Sub();
			this.Stloc(temp2V2);
			if (this._code._rightToLeft)
			{
				this.Leftcharnext();
			}
			else
			{
				this.Rightcharnext();
			}
			if (this._fcPrefix.CaseInsensitive)
			{
				this.CallToLower();
			}
			if (!RegexCharClass.IsSingleton(this._fcPrefix.Prefix))
			{
				this.Ldstr(this._fcPrefix.Prefix);
				this.Call(RegexCompiler._charInSetM);
				this.BrtrueFar(label17);
			}
			else
			{
				this.Ldc((int)RegexCharClass.SingletonChar(this._fcPrefix.Prefix));
				this.Beq(label17);
			}
			this.MarkLabel(label20);
			this.Ldloc(temp2V2);
			this.Ldc(0);
			if (!RegexCharClass.IsSingleton(this._fcPrefix.Prefix))
			{
				this.BgtFar(label16);
			}
			else
			{
				this.Bgt(label16);
			}
			this.Ldc(0);
			this.BrFar(label18);
			this.MarkLabel(label17);
			this.Ldloc(this._textposV);
			this.Ldc(1);
			this.Sub(this._code._rightToLeft);
			this.Stloc(this._textposV);
			this.Ldc(1);
			this.MarkLabel(label18);
			this.Mvlocfld(this._textposV, RegexCompiler._textposF);
			this.Ret();
			this.MarkLabel(label19);
			this.Ldc(0);
			this.Ret();
		}

		internal void GenerateInitTrackCount()
		{
			this.Ldthis();
			this.Ldc(this._trackcount);
			this.Stfld(RegexCompiler._trackcountF);
			this.Ret();
		}

		internal LocalBuilder DeclareInt()
		{
			return this._ilg.DeclareLocal(typeof(int));
		}

		internal LocalBuilder DeclareIntArray()
		{
			return this._ilg.DeclareLocal(typeof(int[]));
		}

		internal LocalBuilder DeclareString()
		{
			return this._ilg.DeclareLocal(typeof(string));
		}

		internal void GenerateGo()
		{
			this._textposV = this.DeclareInt();
			this._textV = this.DeclareString();
			this._trackposV = this.DeclareInt();
			this._trackV = this.DeclareIntArray();
			this._stackposV = this.DeclareInt();
			this._stackV = this.DeclareIntArray();
			this._tempV = this.DeclareInt();
			this._temp2V = this.DeclareInt();
			this._temp3V = this.DeclareInt();
			this._textbegV = this.DeclareInt();
			this._textendV = this.DeclareInt();
			this._textstartV = this.DeclareInt();
			this._labels = null;
			this._notes = null;
			this._notecount = 0;
			this._backtrack = this.DefineLabel();
			this.GenerateForwardSection();
			this.GenerateMiddleSection();
			this.GenerateBacktrackSection();
		}

		internal void GenerateOneCode()
		{
			this.Ldthis();
			this.Callvirt(RegexCompiler._checkTimeoutM);
			int regexopcode = this._regexopcode;
			if (regexopcode <= 285)
			{
				if (regexopcode <= 164)
				{
					switch (regexopcode)
					{
					case 0:
					case 1:
					case 2:
					case 64:
					case 65:
					case 66:
						goto IL_1438;
					case 3:
					case 4:
					case 5:
					case 67:
					case 68:
					case 69:
						goto IL_1604;
					case 6:
					case 7:
					case 8:
					case 70:
					case 71:
					case 72:
						goto IL_18EF;
					case 9:
					case 10:
					case 11:
					case 73:
					case 74:
					case 75:
						break;
					case 12:
						goto IL_1024;
					case 13:
					case 77:
						goto IL_11F6;
					case 14:
					{
						Label label = this._labels[this.NextCodepos()];
						this.Ldloc(this._textposV);
						this.Ldloc(this._textbegV);
						this.Ble(label);
						this.Leftchar();
						this.Ldc(10);
						this.BneFar(this._backtrack);
						return;
					}
					case 15:
					{
						Label label2 = this._labels[this.NextCodepos()];
						this.Ldloc(this._textposV);
						this.Ldloc(this._textendV);
						this.Bge(label2);
						this.Rightchar();
						this.Ldc(10);
						this.BneFar(this._backtrack);
						return;
					}
					case 16:
					case 17:
						this.Ldthis();
						this.Ldloc(this._textposV);
						this.Ldloc(this._textbegV);
						this.Ldloc(this._textendV);
						this.Callvirt(RegexCompiler._isboundaryM);
						if (this.Code() == 16)
						{
							this.BrfalseFar(this._backtrack);
							return;
						}
						this.BrtrueFar(this._backtrack);
						return;
					case 18:
						this.Ldloc(this._textposV);
						this.Ldloc(this._textbegV);
						this.BgtFar(this._backtrack);
						return;
					case 19:
						this.Ldloc(this._textposV);
						this.Ldthisfld(RegexCompiler._textstartF);
						this.BneFar(this._backtrack);
						return;
					case 20:
						this.Ldloc(this._textposV);
						this.Ldloc(this._textendV);
						this.Ldc(1);
						this.Sub();
						this.BltFar(this._backtrack);
						this.Ldloc(this._textposV);
						this.Ldloc(this._textendV);
						this.Bge(this._labels[this.NextCodepos()]);
						this.Rightchar();
						this.Ldc(10);
						this.BneFar(this._backtrack);
						return;
					case 21:
						this.Ldloc(this._textposV);
						this.Ldloc(this._textendV);
						this.BltFar(this._backtrack);
						return;
					case 22:
						this.Back();
						return;
					case 23:
						this.PushTrack(this._textposV);
						this.Track();
						return;
					case 24:
					{
						LocalBuilder tempV = this._tempV;
						Label label3 = this.DefineLabel();
						this.PopStack();
						this.Dup();
						this.Stloc(tempV);
						this.PushTrack(tempV);
						this.Ldloc(this._textposV);
						this.Beq(label3);
						this.PushTrack(this._textposV);
						this.PushStack(this._textposV);
						this.Track();
						this.Goto(this.Operand(0));
						this.MarkLabel(label3);
						this.TrackUnique2(5);
						return;
					}
					case 25:
					{
						LocalBuilder tempV2 = this._tempV;
						Label label4 = this.DefineLabel();
						Label label5 = this.DefineLabel();
						Label label6 = this.DefineLabel();
						this.PopStack();
						this.Dup();
						this.Stloc(tempV2);
						this.Ldloc(tempV2);
						this.Ldc(-1);
						this.Beq(label5);
						this.PushTrack(tempV2);
						this.Br(label6);
						this.MarkLabel(label5);
						this.PushTrack(this._textposV);
						this.MarkLabel(label6);
						this.Ldloc(this._textposV);
						this.Beq(label4);
						this.PushTrack(this._textposV);
						this.Track();
						this.Br(this.AdvanceLabel());
						this.MarkLabel(label4);
						this.ReadyPushStack();
						this.Ldloc(tempV2);
						this.DoPush();
						this.TrackUnique2(6);
						return;
					}
					case 26:
						this.ReadyPushStack();
						this.Ldc(-1);
						this.DoPush();
						this.ReadyPushStack();
						this.Ldc(this.Operand(0));
						this.DoPush();
						this.TrackUnique(1);
						return;
					case 27:
						this.PushStack(this._textposV);
						this.ReadyPushStack();
						this.Ldc(this.Operand(0));
						this.DoPush();
						this.TrackUnique(1);
						return;
					case 28:
					{
						LocalBuilder tempV3 = this._tempV;
						LocalBuilder temp2V = this._temp2V;
						Label label7 = this.DefineLabel();
						Label label8 = this.DefineLabel();
						this.PopStack();
						this.Stloc(tempV3);
						this.PopStack();
						this.Dup();
						this.Stloc(temp2V);
						this.PushTrack(temp2V);
						this.Ldloc(this._textposV);
						this.Bne(label7);
						this.Ldloc(tempV3);
						this.Ldc(0);
						this.Bge(label8);
						this.MarkLabel(label7);
						this.Ldloc(tempV3);
						this.Ldc(this.Operand(1));
						this.Bge(label8);
						this.PushStack(this._textposV);
						this.ReadyPushStack();
						this.Ldloc(tempV3);
						this.Ldc(1);
						this.Add();
						this.DoPush();
						this.Track();
						this.Goto(this.Operand(0));
						this.MarkLabel(label8);
						this.PushTrack(tempV3);
						this.TrackUnique2(7);
						return;
					}
					case 29:
					{
						LocalBuilder tempV4 = this._tempV;
						LocalBuilder temp2V2 = this._temp2V;
						Label label9 = this.DefineLabel();
						this.DefineLabel();
						Label[] labels = this._labels;
						this.NextCodepos();
						this.PopStack();
						this.Stloc(tempV4);
						this.PopStack();
						this.Stloc(temp2V2);
						this.Ldloc(tempV4);
						this.Ldc(0);
						this.Bge(label9);
						this.PushTrack(temp2V2);
						this.PushStack(this._textposV);
						this.ReadyPushStack();
						this.Ldloc(tempV4);
						this.Ldc(1);
						this.Add();
						this.DoPush();
						this.TrackUnique2(8);
						this.Goto(this.Operand(0));
						this.MarkLabel(label9);
						this.PushTrack(temp2V2);
						this.PushTrack(tempV4);
						this.PushTrack(this._textposV);
						this.Track();
						return;
					}
					case 30:
						this.ReadyPushStack();
						this.Ldc(-1);
						this.DoPush();
						this.TrackUnique(0);
						return;
					case 31:
						this.PushStack(this._textposV);
						this.TrackUnique(0);
						return;
					case 32:
						if (this.Operand(1) != -1)
						{
							this.Ldthis();
							this.Ldc(this.Operand(1));
							this.Callvirt(RegexCompiler._ismatchedM);
							this.BrfalseFar(this._backtrack);
						}
						this.PopStack();
						this.Stloc(this._tempV);
						if (this.Operand(1) != -1)
						{
							this.Ldthis();
							this.Ldc(this.Operand(0));
							this.Ldc(this.Operand(1));
							this.Ldloc(this._tempV);
							this.Ldloc(this._textposV);
							this.Callvirt(RegexCompiler._transferM);
						}
						else
						{
							this.Ldthis();
							this.Ldc(this.Operand(0));
							this.Ldloc(this._tempV);
							this.Ldloc(this._textposV);
							this.Callvirt(RegexCompiler._captureM);
						}
						this.PushTrack(this._tempV);
						if (this.Operand(0) != -1 && this.Operand(1) != -1)
						{
							this.TrackUnique(4);
							return;
						}
						this.TrackUnique(3);
						return;
					case 33:
						this.ReadyPushTrack();
						this.PopStack();
						this.Dup();
						this.Stloc(this._textposV);
						this.DoPush();
						this.Track();
						return;
					case 34:
						this.ReadyPushStack();
						this.Ldthisfld(RegexCompiler._trackF);
						this.Ldlen();
						this.Ldloc(this._trackposV);
						this.Sub();
						this.DoPush();
						this.ReadyPushStack();
						this.Ldthis();
						this.Callvirt(RegexCompiler._crawlposM);
						this.DoPush();
						this.TrackUnique(1);
						return;
					case 35:
					{
						Label label10 = this.DefineLabel();
						Label label11 = this.DefineLabel();
						this.PopStack();
						this.Ldthisfld(RegexCompiler._trackF);
						this.Ldlen();
						this.PopStack();
						this.Sub();
						this.Stloc(this._trackposV);
						this.Dup();
						this.Ldthis();
						this.Callvirt(RegexCompiler._crawlposM);
						this.Beq(label11);
						this.MarkLabel(label10);
						this.Ldthis();
						this.Callvirt(RegexCompiler._uncaptureM);
						this.Dup();
						this.Ldthis();
						this.Callvirt(RegexCompiler._crawlposM);
						this.Bne(label10);
						this.MarkLabel(label11);
						this.Pop();
						this.Back();
						return;
					}
					case 36:
						this.PopStack();
						this.Stloc(this._tempV);
						this.Ldthisfld(RegexCompiler._trackF);
						this.Ldlen();
						this.PopStack();
						this.Sub();
						this.Stloc(this._trackposV);
						this.PushTrack(this._tempV);
						this.TrackUnique(9);
						return;
					case 37:
						this.Ldthis();
						this.Ldc(this.Operand(0));
						this.Callvirt(RegexCompiler._ismatchedM);
						this.BrfalseFar(this._backtrack);
						return;
					case 38:
						this.Goto(this.Operand(0));
						return;
					case 39:
					case 43:
					case 44:
					case 45:
					case 46:
					case 47:
					case 48:
					case 49:
					case 50:
					case 51:
					case 52:
					case 53:
					case 54:
					case 55:
					case 56:
					case 57:
					case 58:
					case 59:
					case 60:
					case 61:
					case 62:
					case 63:
						goto IL_1AE4;
					case 40:
						this.Mvlocfld(this._textposV, RegexCompiler._textposF);
						this.Ret();
						return;
					case 41:
					case 42:
						this.Ldthis();
						this.Ldloc(this._textposV);
						this.Ldloc(this._textbegV);
						this.Ldloc(this._textendV);
						this.Callvirt(RegexCompiler._isECMABoundaryM);
						if (this.Code() == 41)
						{
							this.BrfalseFar(this._backtrack);
							return;
						}
						this.BrtrueFar(this._backtrack);
						return;
					case 76:
						goto IL_110B;
					default:
						switch (regexopcode)
						{
						case 131:
						case 132:
						case 133:
							goto IL_184F;
						case 134:
						case 135:
						case 136:
							goto IL_19D9;
						case 137:
						case 138:
						case 139:
						case 140:
						case 141:
						case 142:
						case 143:
						case 144:
						case 145:
						case 146:
						case 147:
						case 148:
						case 149:
						case 150:
						case 163:
							goto IL_1AE4;
						case 151:
							this.PopTrack();
							this.Stloc(this._textposV);
							this.Goto(this.Operand(0));
							return;
						case 152:
							this.PopTrack();
							this.Stloc(this._textposV);
							this.PopStack();
							this.Pop();
							this.TrackUnique2(5);
							this.Advance();
							return;
						case 153:
							this.PopTrack();
							this.Stloc(this._textposV);
							this.PushStack(this._textposV);
							this.TrackUnique2(6);
							this.Goto(this.Operand(0));
							return;
						case 154:
						case 155:
							this.PopDiscardStack(2);
							this.Back();
							return;
						case 156:
						{
							LocalBuilder tempV5 = this._tempV;
							Label label12 = this.DefineLabel();
							this.PopStack();
							this.Ldc(1);
							this.Sub();
							this.Dup();
							this.Stloc(tempV5);
							this.Ldc(0);
							this.Blt(label12);
							this.PopStack();
							this.Stloc(this._textposV);
							this.PushTrack(tempV5);
							this.TrackUnique2(7);
							this.Advance();
							this.MarkLabel(label12);
							this.ReadyReplaceStack(0);
							this.PopTrack();
							this.DoReplace();
							this.PushStack(tempV5);
							this.Back();
							return;
						}
						case 157:
						{
							Label label13 = this.DefineLabel();
							LocalBuilder tempV6 = this._tempV;
							this.PopTrack();
							this.Stloc(this._textposV);
							this.PopTrack();
							this.Dup();
							this.Stloc(tempV6);
							this.Ldc(this.Operand(1));
							this.Bge(label13);
							this.Ldloc(this._textposV);
							this.TopTrack();
							this.Beq(label13);
							this.PushStack(this._textposV);
							this.ReadyPushStack();
							this.Ldloc(tempV6);
							this.Ldc(1);
							this.Add();
							this.DoPush();
							this.TrackUnique2(8);
							this.Goto(this.Operand(0));
							this.MarkLabel(label13);
							this.ReadyPushStack();
							this.PopTrack();
							this.DoPush();
							this.PushStack(tempV6);
							this.Back();
							return;
						}
						case 158:
						case 159:
							this.PopDiscardStack();
							this.Back();
							return;
						case 160:
							this.ReadyPushStack();
							this.PopTrack();
							this.DoPush();
							this.Ldthis();
							this.Callvirt(RegexCompiler._uncaptureM);
							if (this.Operand(0) != -1 && this.Operand(1) != -1)
							{
								this.Ldthis();
								this.Callvirt(RegexCompiler._uncaptureM);
							}
							this.Back();
							return;
						case 161:
							this.ReadyPushStack();
							this.PopTrack();
							this.DoPush();
							this.Back();
							return;
						case 162:
							this.PopDiscardStack(2);
							this.Back();
							return;
						case 164:
						{
							Label label14 = this.DefineLabel();
							Label label15 = this.DefineLabel();
							this.PopTrack();
							this.Dup();
							this.Ldthis();
							this.Callvirt(RegexCompiler._crawlposM);
							this.Beq(label15);
							this.MarkLabel(label14);
							this.Ldthis();
							this.Callvirt(RegexCompiler._uncaptureM);
							this.Dup();
							this.Ldthis();
							this.Callvirt(RegexCompiler._crawlposM);
							this.Bne(label14);
							this.MarkLabel(label15);
							this.Pop();
							this.Back();
							return;
						}
						default:
							goto IL_1AE4;
						}
						break;
					}
				}
				else
				{
					if (regexopcode - 195 <= 2)
					{
						goto IL_184F;
					}
					if (regexopcode - 198 <= 2)
					{
						goto IL_19D9;
					}
					switch (regexopcode)
					{
					case 280:
						this.ReadyPushStack();
						this.PopTrack();
						this.DoPush();
						this.Back();
						return;
					case 281:
						this.ReadyReplaceStack(0);
						this.PopTrack();
						this.DoReplace();
						this.Back();
						return;
					case 282:
					case 283:
						goto IL_1AE4;
					case 284:
						this.PopTrack();
						this.Stloc(this._tempV);
						this.ReadyPushStack();
						this.PopTrack();
						this.DoPush();
						this.PushStack(this._tempV);
						this.Back();
						return;
					case 285:
						this.ReadyReplaceStack(1);
						this.PopTrack();
						this.DoReplace();
						this.ReadyReplaceStack(0);
						this.TopStack();
						this.Ldc(1);
						this.Sub();
						this.DoReplace();
						this.Back();
						return;
					default:
						goto IL_1AE4;
					}
				}
			}
			else if (regexopcode <= 645)
			{
				switch (regexopcode)
				{
				case 512:
				case 513:
				case 514:
					goto IL_1438;
				case 515:
				case 516:
				case 517:
					goto IL_1604;
				case 518:
				case 519:
				case 520:
					goto IL_18EF;
				case 521:
				case 522:
				case 523:
					break;
				case 524:
					goto IL_1024;
				case 525:
					goto IL_11F6;
				default:
					switch (regexopcode)
					{
					case 576:
					case 577:
					case 578:
						goto IL_1438;
					case 579:
					case 580:
					case 581:
						goto IL_1604;
					case 582:
					case 583:
					case 584:
						goto IL_18EF;
					case 585:
					case 586:
					case 587:
						break;
					case 588:
						goto IL_110B;
					case 589:
						goto IL_11F6;
					default:
						if (regexopcode - 643 > 2)
						{
							goto IL_1AE4;
						}
						goto IL_184F;
					}
					break;
				}
			}
			else
			{
				if (regexopcode - 646 <= 2)
				{
					goto IL_19D9;
				}
				if (regexopcode - 707 <= 2)
				{
					goto IL_184F;
				}
				if (regexopcode - 710 > 2)
				{
					goto IL_1AE4;
				}
				goto IL_19D9;
			}
			this.Ldloc(this._textposV);
			if (!this.IsRtl())
			{
				this.Ldloc(this._textendV);
				this.BgeFar(this._backtrack);
				this.Rightcharnext();
			}
			else
			{
				this.Ldloc(this._textbegV);
				this.BleFar(this._backtrack);
				this.Leftcharnext();
			}
			if (this.IsCi())
			{
				this.CallToLower();
			}
			if (this.Code() == 11)
			{
				this.Ldstr(this._strings[this.Operand(0)]);
				this.Call(RegexCompiler._charInSetM);
				this.BrfalseFar(this._backtrack);
				return;
			}
			this.Ldc(this.Operand(0));
			if (this.Code() == 9)
			{
				this.BneFar(this._backtrack);
				return;
			}
			this.BeqFar(this._backtrack);
			return;
			IL_1024:
			string text = this._strings[this.Operand(0)];
			this.Ldc(text.Length);
			this.Ldloc(this._textendV);
			this.Ldloc(this._textposV);
			this.Sub();
			this.BgtFar(this._backtrack);
			for (int i = 0; i < text.Length; i++)
			{
				this.Ldloc(this._textV);
				this.Ldloc(this._textposV);
				if (i != 0)
				{
					this.Ldc(i);
					this.Add();
				}
				this.Callvirt(RegexCompiler._getcharM);
				if (this.IsCi())
				{
					this.CallToLower();
				}
				this.Ldc((int)text[i]);
				this.BneFar(this._backtrack);
			}
			this.Ldloc(this._textposV);
			this.Ldc(text.Length);
			this.Add();
			this.Stloc(this._textposV);
			return;
			IL_110B:
			string text2 = this._strings[this.Operand(0)];
			this.Ldc(text2.Length);
			this.Ldloc(this._textposV);
			this.Ldloc(this._textbegV);
			this.Sub();
			this.BgtFar(this._backtrack);
			int j = text2.Length;
			while (j > 0)
			{
				j--;
				this.Ldloc(this._textV);
				this.Ldloc(this._textposV);
				this.Ldc(text2.Length - j);
				this.Sub();
				this.Callvirt(RegexCompiler._getcharM);
				if (this.IsCi())
				{
					this.CallToLower();
				}
				this.Ldc((int)text2[j]);
				this.BneFar(this._backtrack);
			}
			this.Ldloc(this._textposV);
			this.Ldc(text2.Length);
			this.Sub();
			this.Stloc(this._textposV);
			return;
			IL_11F6:
			LocalBuilder tempV7 = this._tempV;
			LocalBuilder temp2V3 = this._temp2V;
			Label label16 = this.DefineLabel();
			this.Ldthis();
			this.Ldc(this.Operand(0));
			this.Callvirt(RegexCompiler._ismatchedM);
			if ((this._options & RegexOptions.ECMAScript) != RegexOptions.None)
			{
				this.Brfalse(this.AdvanceLabel());
			}
			else
			{
				this.BrfalseFar(this._backtrack);
			}
			this.Ldthis();
			this.Ldc(this.Operand(0));
			this.Callvirt(RegexCompiler._matchlengthM);
			this.Dup();
			this.Stloc(tempV7);
			if (!this.IsRtl())
			{
				this.Ldloc(this._textendV);
				this.Ldloc(this._textposV);
			}
			else
			{
				this.Ldloc(this._textposV);
				this.Ldloc(this._textbegV);
			}
			this.Sub();
			this.BgtFar(this._backtrack);
			this.Ldthis();
			this.Ldc(this.Operand(0));
			this.Callvirt(RegexCompiler._matchindexM);
			if (!this.IsRtl())
			{
				this.Ldloc(tempV7);
				this.Add(this.IsRtl());
			}
			this.Stloc(temp2V3);
			this.Ldloc(this._textposV);
			this.Ldloc(tempV7);
			this.Add(this.IsRtl());
			this.Stloc(this._textposV);
			this.MarkLabel(label16);
			this.Ldloc(tempV7);
			this.Ldc(0);
			this.Ble(this.AdvanceLabel());
			this.Ldloc(this._textV);
			this.Ldloc(temp2V3);
			this.Ldloc(tempV7);
			if (this.IsRtl())
			{
				this.Ldc(1);
				this.Sub();
				this.Dup();
				this.Stloc(tempV7);
			}
			this.Sub(this.IsRtl());
			this.Callvirt(RegexCompiler._getcharM);
			if (this.IsCi())
			{
				this.CallToLower();
			}
			this.Ldloc(this._textV);
			this.Ldloc(this._textposV);
			this.Ldloc(tempV7);
			if (!this.IsRtl())
			{
				this.Dup();
				this.Ldc(1);
				this.Sub();
				this.Stloc(tempV7);
			}
			this.Sub(this.IsRtl());
			this.Callvirt(RegexCompiler._getcharM);
			if (this.IsCi())
			{
				this.CallToLower();
			}
			this.Beq(label16);
			this.Back();
			return;
			IL_1438:
			LocalBuilder tempV8 = this._tempV;
			Label label17 = this.DefineLabel();
			int num = this.Operand(1);
			if (num == 0)
			{
				return;
			}
			this.Ldc(num);
			if (!this.IsRtl())
			{
				this.Ldloc(this._textendV);
				this.Ldloc(this._textposV);
			}
			else
			{
				this.Ldloc(this._textposV);
				this.Ldloc(this._textbegV);
			}
			this.Sub();
			this.BgtFar(this._backtrack);
			this.Ldloc(this._textposV);
			this.Ldc(num);
			this.Add(this.IsRtl());
			this.Stloc(this._textposV);
			this.Ldc(num);
			this.Stloc(tempV8);
			this.MarkLabel(label17);
			this.Ldloc(this._textV);
			this.Ldloc(this._textposV);
			this.Ldloc(tempV8);
			if (this.IsRtl())
			{
				this.Ldc(1);
				this.Sub();
				this.Dup();
				this.Stloc(tempV8);
				this.Add();
			}
			else
			{
				this.Dup();
				this.Ldc(1);
				this.Sub();
				this.Stloc(tempV8);
				this.Sub();
			}
			this.Callvirt(RegexCompiler._getcharM);
			if (this.IsCi())
			{
				this.CallToLower();
			}
			if (this.Code() == 2)
			{
				this.Ldstr(this._strings[this.Operand(0)]);
				this.Call(RegexCompiler._charInSetM);
				this.BrfalseFar(this._backtrack);
			}
			else
			{
				this.Ldc(this.Operand(0));
				if (this.Code() == 0)
				{
					this.BneFar(this._backtrack);
				}
				else
				{
					this.BeqFar(this._backtrack);
				}
			}
			this.Ldloc(tempV8);
			this.Ldc(0);
			if (this.Code() == 2)
			{
				this.BgtFar(label17);
				return;
			}
			this.Bgt(label17);
			return;
			IL_1604:
			LocalBuilder tempV9 = this._tempV;
			LocalBuilder temp2V4 = this._temp2V;
			Label label18 = this.DefineLabel();
			Label label19 = this.DefineLabel();
			int num2 = this.Operand(1);
			if (num2 != 0)
			{
				if (!this.IsRtl())
				{
					this.Ldloc(this._textendV);
					this.Ldloc(this._textposV);
				}
				else
				{
					this.Ldloc(this._textposV);
					this.Ldloc(this._textbegV);
				}
				this.Sub();
				if (num2 != 2147483647)
				{
					Label label20 = this.DefineLabel();
					this.Dup();
					this.Ldc(num2);
					this.Blt(label20);
					this.Pop();
					this.Ldc(num2);
					this.MarkLabel(label20);
				}
				this.Dup();
				this.Stloc(temp2V4);
				this.Ldc(1);
				this.Add();
				this.Stloc(tempV9);
				this.MarkLabel(label18);
				this.Ldloc(tempV9);
				this.Ldc(1);
				this.Sub();
				this.Dup();
				this.Stloc(tempV9);
				this.Ldc(0);
				if (this.Code() == 5)
				{
					this.BleFar(label19);
				}
				else
				{
					this.Ble(label19);
				}
				if (this.IsRtl())
				{
					this.Leftcharnext();
				}
				else
				{
					this.Rightcharnext();
				}
				if (this.IsCi())
				{
					this.CallToLower();
				}
				if (this.Code() == 5)
				{
					this.Ldstr(this._strings[this.Operand(0)]);
					this.Call(RegexCompiler._charInSetM);
					this.BrtrueFar(label18);
				}
				else
				{
					this.Ldc(this.Operand(0));
					if (this.Code() == 3)
					{
						this.Beq(label18);
					}
					else
					{
						this.Bne(label18);
					}
				}
				this.Ldloc(this._textposV);
				this.Ldc(1);
				this.Sub(this.IsRtl());
				this.Stloc(this._textposV);
				this.MarkLabel(label19);
				this.Ldloc(temp2V4);
				this.Ldloc(tempV9);
				this.Ble(this.AdvanceLabel());
				this.ReadyPushTrack();
				this.Ldloc(temp2V4);
				this.Ldloc(tempV9);
				this.Sub();
				this.Ldc(1);
				this.Sub();
				this.DoPush();
				this.ReadyPushTrack();
				this.Ldloc(this._textposV);
				this.Ldc(1);
				this.Sub(this.IsRtl());
				this.DoPush();
				this.Track();
				return;
			}
			return;
			IL_184F:
			this.PopTrack();
			this.Stloc(this._textposV);
			this.PopTrack();
			this.Stloc(this._tempV);
			this.Ldloc(this._tempV);
			this.Ldc(0);
			this.BleFar(this.AdvanceLabel());
			this.ReadyPushTrack();
			this.Ldloc(this._tempV);
			this.Ldc(1);
			this.Sub();
			this.DoPush();
			this.ReadyPushTrack();
			this.Ldloc(this._textposV);
			this.Ldc(1);
			this.Sub(this.IsRtl());
			this.DoPush();
			this.Trackagain();
			this.Advance();
			return;
			IL_18EF:
			LocalBuilder tempV10 = this._tempV;
			int num3 = this.Operand(1);
			if (num3 != 0)
			{
				if (!this.IsRtl())
				{
					this.Ldloc(this._textendV);
					this.Ldloc(this._textposV);
				}
				else
				{
					this.Ldloc(this._textposV);
					this.Ldloc(this._textbegV);
				}
				this.Sub();
				if (num3 != 2147483647)
				{
					Label label21 = this.DefineLabel();
					this.Dup();
					this.Ldc(num3);
					this.Blt(label21);
					this.Pop();
					this.Ldc(num3);
					this.MarkLabel(label21);
				}
				this.Dup();
				this.Stloc(tempV10);
				this.Ldc(0);
				this.Ble(this.AdvanceLabel());
				this.ReadyPushTrack();
				this.Ldloc(tempV10);
				this.Ldc(1);
				this.Sub();
				this.DoPush();
				this.PushTrack(this._textposV);
				this.Track();
				return;
			}
			return;
			IL_19D9:
			this.PopTrack();
			this.Stloc(this._textposV);
			this.PopTrack();
			this.Stloc(this._temp2V);
			if (!this.IsRtl())
			{
				this.Rightcharnext();
			}
			else
			{
				this.Leftcharnext();
			}
			if (this.IsCi())
			{
				this.CallToLower();
			}
			if (this.Code() == 8)
			{
				this.Ldstr(this._strings[this.Operand(0)]);
				this.Call(RegexCompiler._charInSetM);
				this.BrfalseFar(this._backtrack);
			}
			else
			{
				this.Ldc(this.Operand(0));
				if (this.Code() == 6)
				{
					this.BneFar(this._backtrack);
				}
				else
				{
					this.BeqFar(this._backtrack);
				}
			}
			this.Ldloc(this._temp2V);
			this.Ldc(0);
			this.BleFar(this.AdvanceLabel());
			this.ReadyPushTrack();
			this.Ldloc(this._temp2V);
			this.Ldc(1);
			this.Sub();
			this.DoPush();
			this.PushTrack(this._textposV);
			this.Trackagain();
			this.Advance();
			return;
			IL_1AE4:
			throw new NotImplementedException(global::SR.GetString("Unimplemented state."));
		}

		internal static FieldInfo _textbegF = RegexCompiler.RegexRunnerField("runtextbeg");

		internal static FieldInfo _textendF = RegexCompiler.RegexRunnerField("runtextend");

		internal static FieldInfo _textstartF = RegexCompiler.RegexRunnerField("runtextstart");

		internal static FieldInfo _textposF = RegexCompiler.RegexRunnerField("runtextpos");

		internal static FieldInfo _textF = RegexCompiler.RegexRunnerField("runtext");

		internal static FieldInfo _trackposF = RegexCompiler.RegexRunnerField("runtrackpos");

		internal static FieldInfo _trackF = RegexCompiler.RegexRunnerField("runtrack");

		internal static FieldInfo _stackposF = RegexCompiler.RegexRunnerField("runstackpos");

		internal static FieldInfo _stackF = RegexCompiler.RegexRunnerField("runstack");

		internal static FieldInfo _trackcountF = RegexCompiler.RegexRunnerField("runtrackcount");

		internal static MethodInfo _ensurestorageM = RegexCompiler.RegexRunnerMethod("EnsureStorage");

		internal static MethodInfo _captureM = RegexCompiler.RegexRunnerMethod("Capture");

		internal static MethodInfo _transferM = RegexCompiler.RegexRunnerMethod("TransferCapture");

		internal static MethodInfo _uncaptureM = RegexCompiler.RegexRunnerMethod("Uncapture");

		internal static MethodInfo _ismatchedM = RegexCompiler.RegexRunnerMethod("IsMatched");

		internal static MethodInfo _matchlengthM = RegexCompiler.RegexRunnerMethod("MatchLength");

		internal static MethodInfo _matchindexM = RegexCompiler.RegexRunnerMethod("MatchIndex");

		internal static MethodInfo _isboundaryM = RegexCompiler.RegexRunnerMethod("IsBoundary");

		internal static MethodInfo _isECMABoundaryM = RegexCompiler.RegexRunnerMethod("IsECMABoundary");

		internal static MethodInfo _chartolowerM = typeof(char).GetMethod("ToLower", new Type[]
		{
			typeof(char),
			typeof(CultureInfo)
		});

		internal static MethodInfo _getcharM = typeof(string).GetMethod("get_Chars", new Type[] { typeof(int) });

		internal static MethodInfo _crawlposM = RegexCompiler.RegexRunnerMethod("Crawlpos");

		internal static MethodInfo _charInSetM = RegexCompiler.RegexRunnerMethod("CharInClass");

		internal static MethodInfo _getCurrentCulture = typeof(CultureInfo).GetMethod("get_CurrentCulture");

		internal static MethodInfo _getInvariantCulture = typeof(CultureInfo).GetMethod("get_InvariantCulture");

		internal static MethodInfo _checkTimeoutM = RegexCompiler.RegexRunnerMethod("CheckTimeout");

		internal ILGenerator _ilg;

		internal LocalBuilder _textstartV;

		internal LocalBuilder _textbegV;

		internal LocalBuilder _textendV;

		internal LocalBuilder _textposV;

		internal LocalBuilder _textV;

		internal LocalBuilder _trackposV;

		internal LocalBuilder _trackV;

		internal LocalBuilder _stackposV;

		internal LocalBuilder _stackV;

		internal LocalBuilder _tempV;

		internal LocalBuilder _temp2V;

		internal LocalBuilder _temp3V;

		internal RegexCode _code;

		internal int[] _codes;

		internal string[] _strings;

		internal RegexPrefix _fcPrefix;

		internal RegexBoyerMoore _bmPrefix;

		internal int _anchors;

		internal Label[] _labels;

		internal RegexCompiler.BacktrackNote[] _notes;

		internal int _notecount;

		internal int _trackcount;

		internal Label _backtrack;

		internal int _regexopcode;

		internal int _codepos;

		internal int _backpos;

		internal RegexOptions _options;

		internal int[] _uniquenote;

		internal int[] _goto;

		internal const int stackpop = 0;

		internal const int stackpop2 = 1;

		internal const int stackpop3 = 2;

		internal const int capback = 3;

		internal const int capback2 = 4;

		internal const int branchmarkback2 = 5;

		internal const int lazybranchmarkback2 = 6;

		internal const int branchcountback2 = 7;

		internal const int lazybranchcountback2 = 8;

		internal const int forejumpback = 9;

		internal const int uniquecount = 10;

		internal sealed class BacktrackNote
		{
			internal BacktrackNote(int flags, Label label, int codepos)
			{
				this._codepos = codepos;
				this._flags = flags;
				this._label = label;
			}

			internal int _codepos;

			internal int _flags;

			internal Label _label;
		}
	}
}
