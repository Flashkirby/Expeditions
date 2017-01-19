using System;
using System.Collections.Generic;

using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Expeditions
{
    public class WorldExplore : ModWorld
    {
        private static Expedition syncedDailyExpedition = null;
        
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

                if(dailys.Count > 0)
                {
                    int random = Main.rand.Next(dailys.Count);
                    if (Expeditions.DEBUG) Main.NewText("dailys = " + dailys.Count + ", picked " + random);

                    NetSyncDaily(dailys[random]);
                    if (Main.netMode == 2)
                    {
                        Expeditions.SendNet_NewDaily(mod, syncedDailyExpedition);
                    }
                }
                else
                {
                    syncedDailyExpedition = null;
                }

            }
        }

        internal static void NetSyncDaily(Expedition expedition)
        {
            syncedDailyExpedition = expedition;
            syncedDailyExpedition.ResetProgress(true);
            if (Expeditions.DEBUG)
            {
                if (syncedDailyExpedition != null) Expeditions.DisplayUnlockedExpedition(expedition, "Daily Expedition: ");
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
