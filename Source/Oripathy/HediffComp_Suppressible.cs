﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace Originium
{
    public class HediffComp_Suppressible : HediffComp
    {
        private HediffCompProperties_Suppressible Props
        {
            get
            {
                return (HediffCompProperties_Suppressible)this.props;
            }
        }
        public override void CompPostMake()
        {
            base.CompPostMake();
            this.doSuppression();
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (base.Pawn.IsHashIntervalTick(this.Props.checkInterval))
            {
                doSuppression();
            }
            
        }

        private void doSuppression()
        {
            HediffDef hediffDef = ((this.isSuppressed()) ? this.Props.suppressedHediff : this.Props.unsuppressedHediff);

            if (hediffDef == null) 
            {
                Log.Message("HediffComp_Suppressible Error: HediffDef not defined");
                return;
            }

            Hediff hediff = base.Pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef, false); 

            if (hediff != null)
            {
                if (this.parent == hediff) { return; }
                hediff.Severity += this.parent.Severity;
            }
            else
            {
                hediff = base.Pawn.health.GetOrAddHediff(this.Props.suppressedHediff);
                hediff.Severity = this.parent.Severity;
            }
            base.Pawn.health.RemoveHediff(this.parent);

        }

        private bool isSuppressed()
        {
            Hediff suppressor = base.Pawn.health.hediffSet.GetFirstHediffOfDef(this.Props.suppressor);
            if (suppressor != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
