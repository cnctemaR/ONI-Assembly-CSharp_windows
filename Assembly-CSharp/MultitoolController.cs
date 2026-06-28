using System;
using UnityEngine;

public class MultitoolController : GameStateMachine<MultitoolController, MultitoolController.Instance, Worker>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.pre;
		base.Target(this.worker);
		this.root.ToggleSnapOn("dig");
		this.pre.Enter(delegate(MultitoolController.Instance smi)
		{
			smi.PlayPre();
			this.worker.Get<Facing>(smi).Face(this.workable.Get(smi).transform.position);
		}).OnAnimQueueComplete(this.loop);
		this.loop.Enter("PlayLoop", delegate(MultitoolController.Instance smi)
		{
			smi.PlayLoop();
		}).Enter("CreateHitEffect", delegate(MultitoolController.Instance smi)
		{
			smi.CreateHitEffect();
		}).Exit("DestroyHitEffect", delegate(MultitoolController.Instance smi)
		{
			smi.DestroyHitEffect();
		});
	}

	public static string[] GetAnimationStrings(Workable workable, Worker worker, string toolString = "dig")
	{
		bool flag = Grid.PosToCell(workable) == Grid.PosToCell(worker);
		if (flag)
		{
			string[] array = new string[] { "build_in_place_pre", "build_in_place_loop" };
			if (toolString != string.Empty)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = array[i].Replace("build", toolString);
				}
			}
		}
		string[][][] array2 = new string[][][]
		{
			new string[][]
			{
				new string[] { "{verb}_dn_pre", "{verb}_dn_loop" },
				new string[] { "ladder_{verb}_dn_pre", "ladder_{verb}_dn_loop" }
			},
			new string[][]
			{
				new string[] { "{verb}_diag_dn_pre", "{verb}_diag_dn_loop" },
				new string[] { "ladder_{verb}_diag_dn_pre", "ladder_{verb}_loop_diag_dn" }
			},
			new string[][]
			{
				new string[] { "{verb}_fwd_pre", "{verb}_fwd_loop" },
				new string[] { "ladder_{verb}_pre", "ladder_{verb}_loop" }
			},
			new string[][]
			{
				new string[] { "{verb}_diag_up_pre", "{verb}_diag_up_loop" },
				new string[] { "ladder_{verb}_diag_up_pre", "ladder_{verb}_loop_diag_up" }
			},
			new string[][]
			{
				new string[] { "{verb}_up_pre", "{verb}_up_loop" },
				new string[] { "ladder_{verb}_up_pre", "ladder_{verb}_up_loop" }
			}
		};
		if (toolString == "build")
		{
			toolString = "dig";
		}
		foreach (string[][] array4 in array2)
		{
			foreach (string[] array6 in array4)
			{
				for (int l = 0; l < array6.Length; l++)
				{
					array6[l] = array6[l].Replace("{verb}", toolString);
				}
			}
		}
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		MultitoolController.GetTargetPoints(workable, worker, out zero2, out zero);
		Vector2 vector = new Vector2(zero.x - zero2.x, zero.y - zero2.y);
		Vector2 normalized = vector.normalized;
		float num = Vector2.Angle(new Vector2(0f, -1f), normalized);
		float num2 = Mathf.Lerp(0f, 1f, num / 180f);
		int num3 = array2.Length;
		int num4 = (int)(num2 * (float)num3);
		num4 = Math.Min(num4, num3 - 1);
		int num5 = ((worker.GetComponent<Navigator>().CurrentNavType != NavType.Ladder) ? 0 : 1);
		return array2[num4][num5];
	}

	private static void GetTargetPoints(Workable workable, Worker worker, out Vector3 source, out Vector3 target)
	{
		target = workable.GetTargetPoint();
		source = worker.transform.position;
		source.y += 0.7f;
	}

	public GameStateMachine<MultitoolController, MultitoolController.Instance, Worker>.State pre;

	public GameStateMachine<MultitoolController, MultitoolController.Instance, Worker>.State loop;

	public StateMachine<MultitoolController, MultitoolController.Instance, Worker>.TargetParameter worker;

	public StateMachine<MultitoolController, MultitoolController.Instance, Worker>.TargetParameter workable;

	public new class Instance : GameStateMachine<MultitoolController, MultitoolController.Instance, Worker>.GameInstance
	{
		public Instance(Workable workable, Worker worker, string context, GameObject hit_effect)
			: base(worker)
		{
			this.hitEffectPrefab = hit_effect;
			worker.GetComponent<AnimEventHandler>().SetContext(new HashedString(context));
			base.sm.worker.Set(worker, base.smi);
			base.sm.workable.Set(workable, base.smi);
			this.anims = MultitoolController.GetAnimationStrings(workable, worker, "dig");
		}

		public void PlayPre()
		{
			base.sm.worker.Get<KAnimControllerBase>(base.smi).Play(this.anims[0], KAnim.PlayMode.Once, 1f, 0f);
		}

		public void PlayLoop()
		{
			KAnimControllerBase kanimControllerBase = base.sm.worker.Get<KAnimControllerBase>(base.smi);
			if (kanimControllerBase.currentAnim != this.anims[1])
			{
				base.sm.worker.Get<KAnimControllerBase>(base.smi).Play(this.anims[1], KAnim.PlayMode.Loop, 1f, 0f);
			}
		}

		public void UpdateHitEffectTarget()
		{
			if (this.hitEffect == null)
			{
				return;
			}
			Workable workable = base.sm.workable.Get<Workable>(base.smi);
			Worker worker = base.sm.worker.Get<Worker>(base.smi);
			AnimEventHandler component = worker.GetComponent<AnimEventHandler>();
			Vector3 targetPoint = workable.GetTargetPoint();
			worker.GetComponent<Facing>().Face(workable.transform.position);
			this.anims = MultitoolController.GetAnimationStrings(workable, worker, "dig");
			this.PlayLoop();
			component.SetTargetPos(targetPoint);
			component.UpdateWorkTarget(workable.GetTargetPoint());
			this.hitEffect.transform.position = targetPoint;
		}

		public void CreateHitEffect()
		{
			Worker worker = base.sm.worker.Get<Worker>(base.smi);
			Workable workable = base.sm.workable.Get<Workable>(base.smi);
			if (worker == null || workable == null)
			{
				return;
			}
			if (Grid.PosToCell(workable) != Grid.PosToCell(worker))
			{
				worker.Trigger(-673283254, null);
			}
			worker.Trigger(-1762453998, null);
			if (this.hitEffectPrefab == null)
			{
				return;
			}
			if (this.hitEffect != null)
			{
				this.DestroyHitEffect();
			}
			AnimEventHandler component = worker.GetComponent<AnimEventHandler>();
			Vector3 targetPoint = workable.GetTargetPoint();
			component.SetTargetPos(targetPoint);
			this.hitEffect = GameUtil.KInstantiate(this.hitEffectPrefab, targetPoint, Grid.SceneLayer.FXFront2, Folder.FX, null, 0);
			component.UpdateWorkTarget(workable.GetTargetPoint());
		}

		public void DestroyHitEffect()
		{
			Worker worker = base.sm.worker.Get<Worker>(base.smi);
			if (worker != null)
			{
				worker.Trigger(-1559999068, null);
				worker.Trigger(939543986, null);
			}
			if (this.hitEffectPrefab == null)
			{
				return;
			}
			if (this.hitEffect == null)
			{
				return;
			}
			this.hitEffect.DeleteObject();
		}

		private GameObject hitEffectPrefab;

		private GameObject hitEffect;

		private string[] anims;

		private bool inPlace;
	}

	private enum DigDirection
	{
		dig_down,
		dig_up
	}
}
