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
using KSP;
using UnityEngine;

namespace AnimatedDecoupler
{
	public class ModuleAnimatedDecoupler : ModuleDecouple
	{
		[KSPField()]
		public string animationName;

		protected Animation[] anims;
		protected bool animationComplete;

		public ModuleAnimatedDecoupler ():
		base()
		{
		}

		public override void OnStart (StartState state)
		{
			print ("ModuleAnimatedDecoupler.OnStart(), isDecoupled = " + isDecoupled.ToString ());
			anims = part.FindModelAnimators(animationName);
			if (anims == null || anims.Length == 0)
			{
				print ("ModuleAnimatedDecoupler: Animations not found");
			}
			base.OnStart (state);
		}

		public override void OnActive()
		{
			print ("ModuleAnimatedDecoupler.OnActive() start; isDecoupled = " + this.isDecoupled.ToString ());
			base.OnActive ();
			print ("ModuleAnimatedDecoupler.OnActive() finished; isDecoupled = " + this.isDecoupled.ToString ());
			if (this.isDecoupled && (object)anims != null && !animationComplete) 
			{
				try
				{
					anims[0].Play (animationName);
					animationComplete = true;
				}
				catch (Exception e)
				{
					Debug.Log ("ModuleAnimatedDecoupler error! " + e.Message);
				}
			}
		}
	}
}

