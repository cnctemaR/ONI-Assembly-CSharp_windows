using System;
using System.Collections.Generic;
using System.Xml.Xsl.Runtime;

namespace System.Xml.Xsl.Qil
{
	internal class QilExpression : QilNode
	{
		public QilExpression(QilNodeType nodeType, QilNode root, QilFactory factory)
			: base(nodeType)
		{
			this._factory = factory;
			this._isDebug = factory.False();
			this._defWSet = factory.LiteralObject(new XmlWriterSettings
			{
				ConformanceLevel = ConformanceLevel.Auto
			});
			this._wsRules = factory.LiteralObject(new List<WhitespaceRule>());
			this._gloVars = factory.GlobalVariableList();
			this._gloParams = factory.GlobalParameterList();
			this._earlBnd = factory.LiteralObject(new List<EarlyBoundInfo>());
			this._funList = factory.FunctionList();
			this._rootNod = root;
		}

		public override int Count
		{
			get
			{
				return 8;
			}
		}

		public override QilNode this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this._isDebug;
				case 1:
					return this._defWSet;
				case 2:
					return this._wsRules;
				case 3:
					return this._gloParams;
				case 4:
					return this._gloVars;
				case 5:
					return this._earlBnd;
				case 6:
					return this._funList;
				case 7:
					return this._rootNod;
				default:
					throw new IndexOutOfRangeException();
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					this._isDebug = value;
					return;
				case 1:
					this._defWSet = value;
					return;
				case 2:
					this._wsRules = value;
					return;
				case 3:
					this._gloParams = value;
					return;
				case 4:
					this._gloVars = value;
					return;
				case 5:
					this._earlBnd = value;
					return;
				case 6:
					this._funList = value;
					return;
				case 7:
					this._rootNod = value;
					return;
				default:
					throw new IndexOutOfRangeException();
				}
			}
		}

		public QilFactory Factory
		{
			get
			{
				return this._factory;
			}
			set
			{
				this._factory = value;
			}
		}

		public bool IsDebug
		{
			get
			{
				return this._isDebug.NodeType == QilNodeType.True;
			}
			set
			{
				this._isDebug = (value ? this._factory.True() : this._factory.False());
			}
		}

		public XmlWriterSettings DefaultWriterSettings
		{
			get
			{
				return (XmlWriterSettings)((QilLiteral)this._defWSet).Value;
			}
			set
			{
				value.ReadOnly = true;
				((QilLiteral)this._defWSet).Value = value;
			}
		}

		public IList<WhitespaceRule> WhitespaceRules
		{
			get
			{
				return (IList<WhitespaceRule>)((QilLiteral)this._wsRules).Value;
			}
			set
			{
				((QilLiteral)this._wsRules).Value = value;
			}
		}

		public QilList GlobalParameterList
		{
			get
			{
				return (QilList)this._gloParams;
			}
			set
			{
				this._gloParams = value;
			}
		}

		public QilList GlobalVariableList
		{
			get
			{
				return (QilList)this._gloVars;
			}
			set
			{
				this._gloVars = value;
			}
		}

		public IList<EarlyBoundInfo> EarlyBoundTypes
		{
			get
			{
				return (IList<EarlyBoundInfo>)((QilLiteral)this._earlBnd).Value;
			}
			set
			{
				((QilLiteral)this._earlBnd).Value = value;
			}
		}

		public QilList FunctionList
		{
			get
			{
				return (QilList)this._funList;
			}
			set
			{
				this._funList = value;
			}
		}

		public QilNode Root
		{
			get
			{
				return this._rootNod;
			}
			set
			{
				this._rootNod = value;
			}
		}

		private QilFactory _factory;

		private QilNode _isDebug;

		private QilNode _defWSet;

		private QilNode _wsRules;

		private QilNode _gloVars;

		private QilNode _gloParams;

		private QilNode _earlBnd;

		private QilNode _funList;

		private QilNode _rootNod;
	}
}
