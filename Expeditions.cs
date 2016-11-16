using Terraria.ModLoader;

namespace Expeditions
{
	class Expeditions : Mod
	{
		public Expeditions()
		{
			Properties = new ModProperties()
			{
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}
	}
}
