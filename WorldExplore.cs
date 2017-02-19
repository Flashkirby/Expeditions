using System;
using System.Collections.Generic;

using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Expeditions
{
    public class WorldExplore : ModWorld
    {
        internal static Expedition syncedDailyExpedition = null;
        private static int expeditionIndex = -1;
        internal static int dailyExpIndex { get { return expeditionIndex; } }
        
        public override void Initialize()
        {
            syncedDailyExpedition = null;
        }

        public override void PostUpdate()
        {
            if(Main.dayTime && Main.time == 0.0)
            {
                List<Expedition> dailys = new List<Expedition>();
                foreach (ModExpedition me in Expeditions.GetExpeditionsList())
                {
                    if (me.expedition.CheckDailyAssigned()) dailys.Add(me.expedition);
                }

                if (dailys.Count > 0)
                {
                    // Try for one that we didn't have last time
                    Expedition previousDaily = syncedDailyExpedition;
                    expeditionIndex = -1;
                    for (int i = 0; i < 100; i++)
                    {
                        expeditionIndex = Main.rand.Next(dailys.Count);
                        if (previousDaily != dailys[expeditionIndex]) break;
                    }

                    if (Expeditions.DEBUG) Main.NewText("dailys = " + dailys.Count + ", picked " + expeditionIndex);
                    NetSyncDaily(dailys[expeditionIndex]);
                    if (Main.netMode == 2)
                    {
                        Expeditions.SendNet_NewDaily(mod);
                    }
                }
                else
                {
                    if(Main.netMode != 1) syncedDailyExpedition = null;
                }

            }
        }

        internal static void NetSyncDaily(Expedition expedition)
        {
            syncedDailyExpedition = expedition;
            syncedDailyExpedition.ResetProgress(true);
            if (Expeditions.DEBUG)
            {
                if (syncedDailyExpedition != null)
                {
                    Expeditions.DisplayUnlockedExpedition(expedition, "Daily Expedition: ");
                    Main.NewText("Daily EXP = " + syncedDailyExpedition.name);
                }
            }
        }

        public static bool IsCurrentDaily(Expedition expedition)
        {
            if (syncedDailyExpedition == null) return false;
            return Expedition.CompareExpeditions(expedition, syncedDailyExpedition);
        }


        #region SaveLoad overrides

        public override TagCompound Save()
        {
            int dQuestID = 0;
            if (syncedDailyExpedition != null) dQuestID = Expedition.GetHashID(syncedDailyExpedition);
            return new TagCompound
            {
                { "dailyQuestID", dQuestID }
            };
        }

        public override void Load(TagCompound tag)
        {
            syncedDailyExpedition = Expeditions.FindExpedition(tag.GetInt("dailyQuestID"));
        }
        #endregion
    }
}
