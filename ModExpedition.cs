using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Expeditions
{
    public class ModExpedition
    {
        public Expedition expedition
        {
            get;
            internal set;
        }
        public Mod mod
        {
            get;
            internal set;
        }
        public ModExpedition()
        {
            expedition = new Expedition();

        }

        #region Virtual Methods
        /// <summary>
        /// The initialisation method for mods using this. Use it to set the title, rewards etc.
        /// </summary>
        public virtual void SetDefaults()
        {

        }

        /// <summary>
        /// Put in any checks here to determine whether the expedition is complete, sans deliverables
        /// </summary>
        /// <returns>True if conditions are met</returns>
        public virtual bool CheckConditions()
        {
            return true;
        }

        /// <summary>
        /// Put in any checks here to determine whether the expedition is visible yet
        /// </summary>
        /// <returns>True if prerequisites are met</returns>
        public virtual bool CheckPrerequisites()
        {
            return true;
        }
        #endregion

    }
}
