﻿using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Originium
{
    public class GameCondition_OriginiumRain : GameCondition_ForceWeather
    {
        public override int TransitionTicks
        {
            get
            {
                return 5000;
            }
        }

        public override void GameConditionTick()
        {
            base.GameConditionTick();
            List<Map> affectedMaps = base.AffectedMaps;
            if (Find.TickManager.TicksGame % damageInterval == 0)
            {
                //Log.Message(damageInterval);
                for (int i = 0; i < affectedMaps.Count; i++)
                {
                    this.DoPawnsToxicDamage(affectedMaps[i]);
                }
            }
            for (int i = 0; i < this.overlays.Count; i++)
            {
                for (int j = 0; j < affectedMaps.Count; j++)
                {
                    this.overlays[i].TickOverlay(affectedMaps[j]);
                }
            }
        }
        private void DoPawnsToxicDamage(Map map)
        {
            IReadOnlyList<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
            for (int i = 0; i < allPawnsSpawned.Count; i++)
            {
                if (!allPawnsSpawned[i].kindDef.immuneToGameConditionEffects)
                {
                    GameCondition_OriginiumRain.DoPawnToxicDamage(allPawnsSpawned[i], true);
                }
            }
        }
        public static void DoPawnToxicDamage(Pawn p, bool protectedByRoof = true, bool protectedIndoors = false, float damageMultiplier = 1f)
        {
            if (p.Spawned && protectedByRoof && p.Position.Roofed(p.Map))
            {
                return;
            }
            if (protectedIndoors && !p.Position.GetRoom(p.Map).PsychologicallyOutdoors)
            {
                return;
            }
            float num = 0.023006668f;
            if (ModsConfig.BiotechActive)
            {
                num *= Mathf.Max(1f - p.GetStatValue(RimWorld.StatDefOf.ToxicEnvironmentResistance, true, -1), 0f);
            }
            else
            {
                num *= Mathf.Max(1f - p.GetStatValue(StatDefOf.RK_OriginiumResistance, true, -1), 0f);
            }
            //Log.Message(damageMultiplier);
            num *= damageMultiplier;
            if (num != 0f)
            {
                float num2 = Mathf.Lerp(0.85f, 1.15f, Rand.ValueSeeded(p.thingIDNumber ^ 74374237));
                num *= num2;
                HealthUtility.AdjustSeverity(p, HediffDefOf.RK_OriginiumBuildup, num);
            }
        }
        public override void GameConditionDraw(Map map)
        {
            for (int i = 0; i < this.overlays.Count; i++)
            {
                this.overlays[i].DrawOverlay(map);
            }
        }
        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue(this, (float)this.TransitionTicks, 1f);
        }
        public override SkyTarget? SkyTarget(Map map)
        {
            return new SkyTarget?(new SkyTarget(0.85f, this.OriginiumRainColors, 1f, 1f));
        }
        public override void End()
        {
            base.End();
            base.SingleMap.weatherDecider.StartNextWeather();
        }
        public override float AnimalDensityFactor(Map map)
        {
            return 0f;
        }
        public override bool AllowEnjoyableOutsideNow(Map map)
        {
            return false;
        }

        private static int damageInterval = 3451;

        private SkyColorSet OriginiumRainColors = new SkyColorSet(new ColorInt(255, 191, 48).ToColor, new ColorInt(212, 184, 42).ToColor, new Color(0.6f, 0.6f, 0.6f), 0.85f);

        private List<SkyOverlay> overlays = new List<SkyOverlay>();
    }
}
