﻿using RimWorld;
using System;
using Verse;

namespace RimKnights
{
    [DefOf]
    public static class DamageDefOf
    {
        static DamageDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DamageDefOf));
        }

        public static DamageDef RK_ActiveOriginium;

        public static DamageDef RK_OriginiumCut;

        public static DamageDef RK_OriginiumBlast;
    }
}
