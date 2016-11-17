using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Expeditions
{
	class Expeditions : Mod
	{
        public static bool expeditionMenu = false;

        public Expeditions()
		{
			Properties = new ModProperties()
			{
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (expeditionMenu)
            {
                if (Main.playerInventory ||
                    Main.player[Main.myPlayer].chest != -1 ||
                    Main.npcShop != 0 ||
                    Main.player[Main.myPlayer].talkNPC > 0 ||
                    Main.InReforgeMenu ||
                    Main.InGuideCraftMenu ||
                    Main.gameMenu
                    )
                {
                    expeditionMenu = false;
                    return;
                }

                //to look at drawning like achievement window?
                /*
                Main.spriteBatch.DrawString(
                    Main.fontMouseText, "Test", 
                    new Vector2(100, 100), Color.Magenta, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                    */
                string mainText = "Expedition Log";

                //calculate wordwrap of text
                int noOfLines = 0;
                string[] array = Utils.WordwrapString(mainText, Main.fontMouseText, 460, 10, out noOfLines);
                noOfLines++;
                Color color = new Microsoft.Xna.Framework.Color(200, 200, 200, 200);
                int textVal = (int)((Main.mouseTextColor * 2 + 255) / 3);
                Color textColor = new Microsoft.Xna.Framework.Color(textVal, textVal, textVal, textVal);

                //draw the main textbox
                Main.spriteBatch.Draw(Main.chatBackTexture, new Vector2((float)(Main.screenWidth / 2 - Main.chatBackTexture.Width / 2), 100f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, Main.chatBackTexture.Width, (noOfLines + 1) * 30)), color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                //draw the bottom of the box
                Main.spriteBatch.Draw(Main.chatBackTexture, new Vector2((float)(Main.screenWidth / 2 - Main.chatBackTexture.Width / 2), (float)(100 + (noOfLines + 1) * 30)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, Main.chatBackTexture.Height - 30, Main.chatBackTexture.Width, 30)), color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                //draw each line of text
                for (int i = 0; i < noOfLines; i++)
                {
                    Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, array[i], (float)(170 + (Main.screenWidth - 800) / 2), (float)(120 + i * 30), textColor, Microsoft.Xna.Framework.Color.Black, Vector2.Zero, 1f);
                }

                //Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(Main.screenWidth / 2 - Main.chatBackTexture.Width / 2, 100, Main.chatBackTexture.Width, (noOfLines + 2) * 30);


                string text = "Close";
                int num18 = 180 + (Main.screenWidth - 800) / 2;
                int num19 = 130 + noOfLines * 30;
                float scale = 0.9f;
                if (Main.mouseX > num18 && (float)Main.mouseX < (float)num18 + Main.fontMouseText.MeasureString(text).X && Main.mouseY > num19 && (float)Main.mouseY < (float)num19 + Main.fontMouseText.MeasureString(text).Y)
                {
                    Main.player[Main.myPlayer].mouseInterface = true;
                    scale = 1.1f;
                    if (!Main.npcChatFocus2)
                    {
                        Main.PlaySound(12, -1, -1, 1);
                    }
                    Main.npcChatFocus2 = true;
                    Main.player[Main.myPlayer].releaseUseItem = false;
                }
                else
                {
                    if (Main.npcChatFocus2)
                    {
                        Main.PlaySound(12, -1, -1, 1);
                    }
                    Main.npcChatFocus2 = false;
                }
                Vector2 vector2 = Main.fontMouseText.MeasureString(text) * 0.5f;
                Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, text, (float)num18 + vector2.X, (float)num19 + vector2.Y, textColor, Microsoft.Xna.Framework.Color.Black, vector2, scale);

            }
        }

        public static void OpenExpeditionMenu()
        {
            Player player = Main.player[Main.myPlayer];
            

            Main.playerInventory = false;
            Main.npcShop = 0;
            if (player.talkNPC > 0) player.talkNPC = 0;
            player.sign = -1;
            Main.npcChatText = "";

            Main.PlaySound(10, -1, -1, 1); //open menu
            Expeditions.expeditionMenu = true;
        }

        public static void CloseExpeditionMenu()
        {
            Main.npcChatText = "";

            Main.PlaySound(11, -1, -1, 1); //close menu
            Expeditions.expeditionMenu = false;
        }



    }
}
