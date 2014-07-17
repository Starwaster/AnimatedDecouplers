// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using System.Linq; 
using KSP;
using UnityEngine;

namespace AnimatedDecoupler
{
	public class ModuleAnimatedDecoupler : ModuleDecouple
	{
		[KSPField]
		public string animationName;

		protected Animation anim;

		[KSPField(isPersistant = true)]
		public bool animationComplete = false;

		public ModuleAnimatedDecoupler ():
		base()
		{
		}

		public override void OnStart (StartState state)
		{
			GameEvents.onStageSeparation.Add (checkForDecoupling);
			base.OnStart (state);
			Debug.Log ("ModuleAnimatedDecoupler.OnStart(), isDecoupled = " + this.isDecoupled.ToString ());
			anim = part.FindModelAnimators(animationName).FirstOrDefault();
			if ((object)this.anim == null)
			{
				Debug.Log ("ModuleAnimatedDecoupler: Animations not found");
			}
			else
			{
				Debug.Log ("ModuleAnimatedDecoupler.OnStart() - Animation found named " + animationName);
				// If Decoupled or animation already played then set animation to end.
				if (this.animationComplete || this.isDecoupled)
				{
					this.anim[animationName].normalizedTime = 1f;
				}
			}
		}

		public void checkForDecoupling(EventReport separationData)
		{
			if (separationData.eventType == FlightEvents.STAGESEPARATION && separationData.origin == this.part)
			{
				// PROBABLY got called because we decoupled, but no way to know because ModuleDecouple doesn't SET isDecoupled until after the event fires. 
				if (!this.animationComplete || !this.anim.IsPlaying (animationName))
				{
					this.anim.Play (animationName);
					this.animationComplete = true;
					this.isDecoupled = true;
					Debug.Log ("ModuleAnimatedDecoupler.onStageSeparation() triggered animation " + this.animationName);
				}
			}
		}
		// Disabling; OnActive() not reliable for determining decoupled state and can be triggered by other mods.
		// Using GameEvents.onStageSeparation instead.
		/*
		public override void OnActive()
		{
			Debug.Log ("ModuleAnimatedDecoupler.OnActive() start; isDecoupled = " + this.isDecoupled.ToString () + ", animationComplete = " + this.animationComplete.ToString ());
			base.OnActive ();
			if (this.isDecoupled && (object)anims != null && !animationComplete) 
			{
				try
				{
					this.anim.Play (animationName);
					this.animationComplete = true;
					Debug.Log ("ModuleAnimatedDecoupler played animation " + this.animationName + "!");
				} 
				catch (Exception e)
				{
					Debug.Log ("ModuleAnimatedDecoupler error! " + e.Message);
				}
				Debug.Log ("ModuleAnimatedDecoupler.OnActive() finished; isDecoupled = " + this.isDecoupled.ToString () + ", animationComplete = " + this.animationComplete.ToString ());
			}
			else
			{
				Debug.Log ("ModuleAnimatedDecoupler unable to play animation (OnActive)");
			}
		}
		*/
	}
}