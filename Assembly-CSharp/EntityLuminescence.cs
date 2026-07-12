using System;
using Klei.AI;
using UnityEngine;

public class EntityLuminescence : GameStateMachine<EntityLuminescence, EntityLuminescence.Instance, IStateMachineTarget, EntityLuminescence.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.root;
	}

	public class Def : StateMachine.BaseDef
	{
		public Color lightColor;

		public float lightRange;

		public float lightAngle;

		public Vector2 lightOffset;

		public Vector2 lightDirection;

		public global::LightShape lightShape;
	}

	public new class Instance : GameStateMachine<EntityLuminescence, EntityLuminescence.Instance, IStateMachineTarget, EntityLuminescence.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, EntityLuminescence.Def def)
			: base(master, def)
		{
			this.light.Color = def.lightColor;
			this.light.Range = def.lightRange;
			this.light.Angle = def.lightAngle;
			this.light.Direction = def.lightDirection;
			this.light.Offset = def.lightOffset;
			this.light.shape = def.lightShape;
		}

		public override void StartSM()
		{
			base.StartSM();
			this.luminescence = Db.Get().Attributes.Luminescence.Lookup(base.gameObject);
			AttributeInstance attributeInstance = this.luminescence;
			attributeInstance.OnDirty = (global::System.Action)Delegate.Combine(attributeInstance.OnDirty, new global::System.Action(this.OnLuminescenceChanged));
			this.RefreshLight();
		}

		private void OnLuminescenceChanged()
		{
			this.RefreshLight();
		}

		public void RefreshLight()
		{
			if (this.luminescence != null)
			{
				int num = (int)this.luminescence.GetTotalValue();
				this.light.Lux = num;
				bool flag = num > 0;
				if (this.light.enabled != flag)
				{
					this.light.enabled = flag;
				}
			}
		}

		protected override void OnCleanUp()
		{
			if (this.luminescence != null)
			{
				AttributeInstance attributeInstance = this.luminescence;
				attributeInstance.OnDirty = (global::System.Action)Delegate.Remove(attributeInstance.OnDirty, new global::System.Action(this.OnLuminescenceChanged));
			}
			base.OnCleanUp();
		}

		[MyCmpAdd]
		private Light2D light;

		private AttributeInstance luminescence;
	}
}
