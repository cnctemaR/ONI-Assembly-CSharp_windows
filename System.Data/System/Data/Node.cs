using System;

namespace System.Data
{
	internal class Node
	{
		public Node(DataRow row)
		{
			this._row = row;
		}

		internal int GetBalance()
		{
			if (this._iBalance == -2)
			{
				throw new SystemException("Node is deleted.");
			}
			return this._iBalance;
		}

		internal void Delete()
		{
			this._iBalance = -2;
			this._nLeft = null;
			this._nRight = null;
			this._nParent = null;
		}

		internal DataRow Row
		{
			get
			{
				return this._row;
			}
		}

		internal Node Left
		{
			get
			{
				if (this._iBalance == -2)
				{
					throw new SystemException("Node is deleted.");
				}
				return this._nLeft;
			}
			set
			{
				if (this._iBalance == -2)
				{
					throw new SystemException("Node is deleted.");
				}
				this._nLeft = value;
			}
		}

		internal Node Right
		{
			get
			{
				if (this._iBalance == -2)
				{
					throw new SystemException("Node is deleted.");
				}
				return this._nRight;
			}
			set
			{
				if (this._iBalance == -2)
				{
					throw new SystemException("Node is deleted.");
				}
				this._nRight = value;
			}
		}

		internal Node Parent
		{
			get
			{
				if (this._iBalance == -2)
				{
					throw new SystemException("Node is deleted.");
				}
				return this._nParent;
			}
			set
			{
				if (this._iBalance == -2)
				{
					throw new SystemException("Node is deleted.");
				}
				this._nParent = value;
			}
		}

		internal bool IsRoot()
		{
			return this._nParent == null;
		}

		internal void SetBalance(int b)
		{
			if (this._iBalance == -2)
			{
				throw new SystemException("Node is deleted.");
			}
			this._iBalance = b;
		}

		internal bool From()
		{
			if (this.IsRoot())
			{
				return true;
			}
			if (this._iBalance == -2)
			{
				throw new SystemException("Node is deleted.");
			}
			Node parent = this.Parent;
			return this.Equals(parent.Left);
		}

		internal object[] GetData()
		{
			if (this._iBalance == -2)
			{
				throw new SystemException("Node is deleted.");
			}
			return this._row.ItemArray;
		}

		internal bool Equals(Node n)
		{
			if (this._iBalance == -2)
			{
				throw new SystemException("Node is deleted.");
			}
			return n == this;
		}

		protected int _iBalance;

		internal Node _nNext;

		protected Node _nLeft;

		protected Node _nRight;

		protected Node _nParent;

		protected DataRow _row;
	}
}
