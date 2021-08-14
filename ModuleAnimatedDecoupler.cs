using System;
using System.Linq;
using System.Collections;
using KSP;
using UnityEngine;

namespace AnimatedDecoupler
{
	public class ModuleAnimatedDecoupler : ModuleDecouple, IScalarModule
	{
        [KSPField]
		public string animationName = "";

		[KSPField()]
		public bool waitForAnimation = false;

		[KSPField(isPersistant = true)]
		public bool animationComplete = false;
		
		[KSPField]
		public int layer = 0;

        [KSPField]
        public string moduleID = "animatedDecoupler";

		protected Animation anim;

		protected bool isDecoupling;

		protected bool isResetting;

		protected bool decoupleAfterAnimation = false;

		protected ModuleCargoBay cargoBay;

		public ModuleAnimatedDecoupler ():
		base()
		{
		}

		//[KSPAction("Decouple")]
		public new void DecoupleAction(KSPActionParam param)
		{
            if (anim != null)
            {
                anim.Play (animationName);
                isDecoupling = true;
                OnMoving.Fire (0f, 1f);
                if (waitForAnimation)
                    StartCoroutine (DelayedDecouple ());
                else
                    OnDecouple ();
            }
            else
                OnDecouple ();
		}

		public new void Decouple()
		{
            if (anim != null)
            {
                anim.Play (animationName);
                isDecoupling = true;
                OnMoving.Fire (0f, 1f);
                if (waitForAnimation)
                    StartCoroutine (DelayedDecouple ());
                else
                    OnDecouple ();
            }
            else
                OnDecouple ();
		}
              

		public override void OnAwake()
		{
			this.OnMovingEvent = new EventData<float, float>("ModuleAnimatedDecoupler.OnMovingEvent");
			this.OnStoppedEvent = new EventData<float>("ModuleAnimateDecoupler.OnStoppedEvent");
			base.OnAwake ();
		}

		public override void OnStart (StartState state)
		{
			// TODO Consider deprecating checkForDecoupling; it should no longer be necessary
			//GameEvents.onStageSeparation.Add (checkForDecoupling);
			GameEvents.onVesselWasModified.Add (OnVesselWasModified);

			cargoBay = part.FindModuleImplementing<ModuleCargoBay> ();
			base.OnStart (state);
			Debug.Log ("ModuleAnimatedDecoupler.OnStart(), isDecoupled = " + this.isDecoupled.ToString ());
			if (animationName != "")
			{
				anim = part.FindModelAnimators(animationName).FirstOrDefault();
				if (this.anim == null)
				{
					Debug.Log ("ModuleAnimatedDecoupler: Animations not found");
				}
				else
				{
					Debug.Log ("ModuleAnimatedDecoupler.OnStart() - Animation found named " + animationName);
					// If Decoupled or animation already played then set animation to end.
					this.anim[animationName].layer = layer;
					if (this.animationComplete || this.isDecoupled)
					{
						this.anim[animationName].normalizedTime = 1f;
					}
				}
			}
		}

		public override void OnActive()
		{
			if (staged)
			{
                if (anim != null)
                {
                    anim.Play (animationName);
                    isDecoupling = true;
                    OnMoving.Fire (0f, 1f);
                    if (waitForAnimation)
                        StartCoroutine (DelayedDecouple ());
                    else
                        OnDecouple ();
                }
				else
					OnDecouple ();
			}
		}

		private void OnVesselWasModified (Vessel v)
		{
			if (v != null && v == vessel)
			{
				if (!(isDecoupling || isDecoupled))
				{
					if(part.FindAttachNode (this.explosiveNodeID).attachedPart == null)
					{
						isDecoupling = true;
						OnMoving.Fire (0f, 1f);
						OnStop.Fire (1f);
					}
				}
			}
		}

		// TODO Consider deprecating checkForDecoupling; it should no longer be necessary
		/*
        private void checkForDecoupling(EventReport separationData)
		{
			return;
			if (separationData.eventType == FlightEvents.STAGESEPARATION && separationData.origin == this.part)
			{
				// PROBABLY got called because we decoupled, but no way to know because ModuleDecouple doesn't SET isDecoupled until after the event fires. 
				OnMoving.Fire (0f, 1f);
				if (animationName != "" && anim != null && (!this.animationComplete || !this.anim.IsPlaying (animationName)))
				{
					if (waitForAnimation)
					{
						//PlayAnimation();
					}
					else
					{
						anim.Play(animationName);
						this.OnMoving.Fire (0f, 1f);
						this.animationComplete = true;
					}
					Debug.Log ("ModuleAnimatedDecoupler.onStageSeparation() triggered animation " + this.animationName);
				}
				this.isDecoupling = true;
				this.OnStop.Fire (1f);
			}
		}
        */

		//public void PlayAnimation()
		//{
		//	animation.Play(animationName);
		//	this.isDecoupling = true;
		//	this.OnMoving.Fire (0f, 1f);
		//	StartCoroutine( CheckEventTime() );
		//}

		IEnumerator DelayedDecouple()
		{
			yield return new WaitForSeconds(EventTime);
			this.animationComplete = true;
			this.OnStop.Fire (1f);
			OnDecouple ();
		}

		public void OnDestroy()
		{
			//GameEvents.onStageSeparation.Remove (checkForDecoupling);
			GameEvents.onVesselWasModified.Remove (OnVesselWasModified);
		}

		//
		// Properties
		//
		private EventData <float, float> OnMovingEvent;
		private EventData <float> OnStoppedEvent;

		float EventTime
		{
			get
			{
				return anim[animationName].length / anim[animationName].speed;
			}
		}

		//AnimationState AnimState
		//{
		//	get
		//	{
		//		return anim[animationName];
		//	}
		//}

		public bool CanMove
		{
			get
			{
				//return part.ShieldedFromAirstream;
				return true;
			}
		}
		
		public float GetScalar
		{
			get
			{
				return part.FindAttachNode(this.explosiveNodeID).attachedPart == null || isResetting || isDecoupling ? 1f : 0f;
			}
		}

        public string ScalarModuleID
        {
            get
            {
                return this.moduleID;
            }
        }

        public EventData<float, float> OnMoving
		{
			get
			{
				return OnMovingEvent;
			}
		}
		
		public EventData<float> OnStop
		{
			get
			{
				return OnStoppedEvent;
			}
		}
		
		//
		// Methods
		//
		public bool IsMoving ()
		{
			if (animationName != "")
            {
                if (anim != null)
                {
					return anim.IsPlaying (animationName);
				}
                else
                {
					return false;
				}
			}
            else
            {
				return false;
			}
		}

		public void SetScalar (float t)
		{
		}
		
		public void SetUIRead (bool state)
		{
		}

		public void SetUIWrite (bool state)
		{
		}
	}
}
