using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Expeditions
{
    public class ModExpedition
    {
        public readonly Mod mod;

        public ModExpedition(Mod mod)
        {
            this.mod = mod;
        }

        public virtual bool Autoload(ref string name)
        {
            return this.mod.Properties.Autoload;
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
