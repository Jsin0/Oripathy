﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Originium
{
    [DefOf]
    public static class DamageDefOf
    {
        static DamageDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DamageDefOf));
        }

        public static DamageDef RM_OriginiumDust;

        public static DamageDef RM_OriginiumCut;

        //public static DamageDef RM_OriginiumBomb;
    }
}
