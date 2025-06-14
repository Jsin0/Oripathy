﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace RimKnights.Utilities
{
    [StaticConstructorOnStartup]
    static class StringPatcher
    {
        static StringPatcher()
        {
            if((CoreMod.orifuel || CoreMod.orundum ) && LanguageDatabase.activeLanguage.folderName == "English")
            {
                PatchText();
            }
        }
        public static void PatchText()
        {
            IEnumerable<Type> allDefTypes = typeof(Def).AllSubclassesNonAbstract();
            List<(string, string)> pairs = GenerateStringPairs();
            foreach (Type type in allDefTypes)
            {
                if (type.IsAbstract || type == typeof(BackstoryDef)) continue;
                foreach(Def def in GenDefDatabase.GetAllDefsInDatabaseForDef(type))
                {
                    bool flag = false;
                    if(def?.label != null)
                    {
                        foreach((string,string) pair in pairs)
                        {
                            if (def.label.Contains(pair.Item1))
                            {
                                //Log.Message("replacing label");
                                def.label = ReplaceWord(def.label, pair.Item1, pair.Item2);
                                flag = true;
                            }
                        }
                    }
                    if(def?.description != null)
                    {
                        foreach ((string, string) pair in pairs)
                        {
                            if (def.description.Contains(pair.Item1))
                            {
                                //Log.Message("replacing description");
                                def.description = ReplaceWord(def.description, pair.Item1, pair.Item2);
                                flag = true;
                            }

                        }
                    }

                    if(type == typeof(RecipeDef))
                    {
                        //Log.Message("recipedef");
                        RecipeDef recipeDef = def as RecipeDef;
                        if (recipeDef?.jobString != null)
                        {
                            foreach ((string, string) pair in pairs)
                            {
                                if (recipeDef.jobString.Contains(pair.Item1))
                                {
                                    //Log.Message("replacing description");
                                    recipeDef.jobString = ReplaceWord(recipeDef.jobString, pair.Item1, pair.Item2);
                                    flag = true;
                                }

                            }
                        }
                    }
                    //if (flag) Log.Message(def.label + ": " + def.description);
                }
            }
        }

        static List<(string,string)> GenerateStringPairs()
        {
            List<(string,string)> pairs = new List<(string,string)> ();
            if (CoreMod.orifuel)
            {
                pairs.Add((RimWorld.ThingDefOf.Chemfuel.label, "orifuel"));
                if(ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData mod) => mod.Name =="Vanilla Chemfuel Expanded" || mod.PackageId == "vanillaexpanded.vchemfuele"))
                {
                    pairs.Add(("deepchem", "orisludge"));
                }
            }
            return pairs;
        }

        private static string ReplaceWord(string input, string word, string replacement)
        {
            string pattern = $@"\b{word}\b";
            return Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase);
            
        }
    }
}
