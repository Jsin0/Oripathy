﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Oripathy
{
    public class Hediff_Oripathy : HediffWithComps
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            if (pawn.IsColonist)
            {
                Messages.Message(this.pawn.Name + " has contracted Oripathy!", MessageTypeDefOf.NegativeHealthEvent);
            }
        }
        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            base.Notify_PawnDied(dinfo, culprit);
            this.TryTriggerWarmupTimer();
        }
        private int GetFinalDelay()
        {
            int delay;
            delay = (int)((0.3 / this.Severity) * this.ticksDelay);
            if (delay > 180000) //3 days max
            {
                return 180000;
            }
            else
            {
                return delay;
            }
        }
        private void TryTriggerWarmupTimer()
        {
            this.shattering = true;
            this.warmupTimer.Start(GenTicks.TicksGame, this.GetFinalDelay(), new Action(this.TryShatter));
            Log.Message("Oripathy: " + this.pawn.Name + " will soon shatter soon");
        }
        private void TryShatter()
        {
            if (this.pawn.Corpse == null)
            {
                Log.Error("Oripathy: cannot shatter, corpse is null");
                return;
            }

            Messages.Message(this.pawn.Name + "'s cropse will soon shatter.", MessageTypeDefOf.NegativeEvent);
            Log.Message(this.pawn.Name);
            this.shatterTimer.Start(GenTicks.TicksGame, Hediff_Oripathy.shatterDurationSeconds.SecondsToTicks(), new Action(this.DoShatterCorpse));
        }

        private void TryTriggerShatterEffect()
        {
            Corpse corpse;
            if ((corpse = this.pawn.ParentHolder as Corpse) != null)
            {
                if (this.effecter == null)
                {
                    this.effecter = EffecterDefOf.ExtinguisherExplosion.Spawn(corpse, this.pawn.MapHeld, Vector3.zero);
                    this.pawn.MapHeld.effecterMaintainer.AddEffecterToMaintain(this.effecter, corpse, 250);
                }
            }
        }
        public override void Tick()
        {
            Corpse corpse;
            if ((corpse = this.pawn.ParentHolder as Corpse) != null && this.shatterSustainer == null)
            {
                SoundInfo soundInfo = SoundInfo.InMap(corpse, MaintenanceType.PerTickRare);
                this.shatterSustainer = SoundDefOf.Tunnel.TrySpawnSustainer(soundInfo);
            }
        }
        public void TickRare()
        {
            if (!this.shattering)
            {
                return;
            }
            if (!this.warmupTimer.Finished)
            {
                //Log.Message("Warmup timer not done.");
                this.warmupTimer.TickInterval();
            }
            else if (!this.shatterTimer.Finished)
            {
                //Log.Message("Shatter timer not done.");
                this.shatterTimer.TickInterval();
                if(this.shatterSustainer != null && !this.shatterSustainer.Ended)
                {
                    Sustainer sustainer = this.shatterSustainer;
                    if (sustainer != null)
                    {
                        sustainer.Maintain();
                    }
                }
                if(this.effecter != null)
                {
                    this.effecter.ticksLeft = (this.shatterTimer.Finished ? 0 : (this.effecter.ticksLeft + 250));
                }
            }
        }
        private void DoShatterCorpse()
        {
            //Log.Message("countdown timer done.");
            if (base.pawn.MapHeld != null)
            {
                //Log.Message(base.pawn.Name + " is shattering.");
                Sustainer sustainer = this.shatterSustainer;
                if (sustainer != null)
                {
                    sustainer.End();
                }
                GenExplosion.DoExplosion(base.pawn.Position, base.pawn.MapHeld, 3f, DamageDefOf.OriginiumDust, base.pawn, 0, -1f, null, null, null, null, null, 0f, 1, null, false, null, 0f, 1, 0f, false, null, null, null, true, 1f, 0f, true, null, 1f, null, null);
            }
            else
            { 
                Log.Error("corpse in null map, no explosion");
            }

            this.shattering = false;
            base.pawn.Corpse.Destroy(DestroyMode.Vanish);
        }

        public override void Notify_PawnCorpseDestroyed()
        {
            Sustainer sustainer = this.shatterSustainer;
            if (sustainer == null)
            {
                return;
            }
            sustainer.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look<TickTimer>(ref this.warmupTimer, "warmupTimer", Array.Empty<object>());
            Scribe_Deep.Look<TickTimer>(ref this.shatterTimer, "shatterTimer", Array.Empty<object>());
            Scribe_Values.Look<bool>(ref this.shattering, "shattering", false);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                this.shatterTimer.OnFinish = new Action(this.DoShatterCorpse);
                this.warmupTimer.OnFinish = new Action(this.TryShatter);
                if (!this.shattering && this.pawn.Dead)
                {
                    TryTriggerWarmupTimer();
                }
            }
        }


        private TickTimer warmupTimer = new TickTimer();

        private TickTimer shatterTimer = new TickTimer();

        private static readonly float
            shatterDurationSeconds = 30f;

        private bool shattering;

        private int ticksDelay = 60000;

        private Effecter effecter;

        private Sustainer shatterSustainer;



    }
}
