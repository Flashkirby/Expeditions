

using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Expeditions.Common.Configs
{
    public class ExpeditionsConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(true)]
        public bool newQuestPopup;

        [DefaultValue(true)]
        public bool chatTrackEnabled;

        [DefaultValue(true)]
        public bool trackerEnabled;

        [Range(0, 255)]
        [DefaultValue(128)]
        public byte trackerAlphaByte;

        [DefaultValue(true)]
        public bool trackerDescriptions;

        [Range(0.5f, 1f)]
        [DefaultValue(1f)]
        [Increment(0.05f)]
        public float trackerScale;

        [DefaultValue(true)]
        public bool autoShowEnabled;

        [Range(0, 300)]
        [DefaultValue(120)]
        public int autoShowHoldTime;
    }
}