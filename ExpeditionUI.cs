﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

using System.Linq;

namespace Expeditions
{
    class ExpeditionUI : UIState
    {
        private static Color backgroundColour = new Color(63, 65, 151, 200);
        private static Color borderColour = new Color(18, 18, 31, 200);

        public UIPanel coinCounterPanel;
        public UIMoneyDisplay moneyDiplay;
        public ItemSlot itemslot;
        public static bool visible = false;

        public override void OnInitialize()
        {
            coinCounterPanel = new UIPanel();
            coinCounterPanel.SetPadding(0);
            coinCounterPanel.Left.Set(400f, 0f);
            coinCounterPanel.Top.Set(100f, 0f);
            coinCounterPanel.Width.Set(170f, 0f);
            coinCounterPanel.Height.Set(70f, 0f);
            coinCounterPanel.BackgroundColor = backgroundColour;
            coinCounterPanel.BorderColor = borderColour;

            coinCounterPanel.OnMouseDown += new UIElement.MouseEvent(DragStart);
            coinCounterPanel.OnMouseUp += new UIElement.MouseEvent(DragEnd);

            Texture2D buttonPlayTexture = ModLoader.GetTexture("Terraria/UI/ButtonPlay");
            UIImageButton playButton = new UIImageButton(buttonPlayTexture);
            playButton.Left.Set(110, 0f);
            playButton.Top.Set(10, 0f);
            playButton.Width.Set(22, 0f);
            playButton.Height.Set(22, 0f);
            playButton.OnClick += new MouseEvent(PlayButtonClicked);
            coinCounterPanel.Append(playButton);

            Texture2D buttonDeleteTexture = ModLoader.GetTexture("Terraria/UI/ButtonDelete");
            UIImageButton closeButton = new UIImageButton(buttonDeleteTexture);
            closeButton.Left.Set(140, 0f);
            closeButton.Top.Set(10, 0f);
            closeButton.Width.Set(22, 0f);
            closeButton.Height.Set(22, 0f);
            closeButton.OnClick += new MouseEvent(CloseButtonClicked);
            coinCounterPanel.Append(closeButton);

            moneyDiplay = new UIMoneyDisplay();
            moneyDiplay.Left.Set(15, 0f);
            moneyDiplay.Top.Set(20, 0f);
            moneyDiplay.Width.Set(100f, 0f);
            moneyDiplay.Height.Set(0, 1f);
            coinCounterPanel.Append(moneyDiplay);

            base.Append(coinCounterPanel);
        }

        private void PlayButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.PlaySound(10, -1, -1, 1);
            moneyDiplay.ResetCoins();
        }

        private void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.PlaySound(10, -1, -1, 1);
            visible = false;
        }

        Vector2 offset;
        public bool dragging = false;
        private void DragStart(UIMouseEvent evt, UIElement listeningElement)
        {
            offset = new Vector2(evt.MousePosition.X - coinCounterPanel.Left.Pixels, evt.MousePosition.Y - coinCounterPanel.Top.Pixels);
            dragging = true;
        }

        private void DragEnd(UIMouseEvent evt, UIElement listeningElement)
        {
            Vector2 end = evt.MousePosition;
            dragging = false;

            coinCounterPanel.Left.Set(end.X - offset.X, 0f);
            coinCounterPanel.Top.Set(end.Y - offset.Y, 0f);

            Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
            if (coinCounterPanel.ContainsPoint(MousePosition))
            {
                Main.player[Main.myPlayer].mouseInterface = true;
            }
            if (dragging)
            {
                coinCounterPanel.Left.Set(MousePosition.X - offset.X, 0f);
                coinCounterPanel.Top.Set(MousePosition.Y - offset.Y, 0f);
                Recalculate();
            }

        }

        public static void DrawItemSlot(SpriteBatch spriteBatch, Item item, float x, float y, int Context)
        {
            if (Main.mouseX >= x && (float)Main.mouseX <= (float)x + (float)Main.inventoryBackTexture.Width * Main.inventoryScale && Main.mouseY >= y && (float)Main.mouseY <= (float)y + (float)Main.inventoryBackTexture.Height * Main.inventoryScale)
            {
                Main.player[Main.myPlayer].mouseInterface = true;
                ItemSlot.MouseHover(ref item, Context);
            }
            Main.inventoryScale = 0.6f;
            ItemSlot.Draw(Main.spriteBatch, ref item,
                Context, new Vector2(x, y), default(Microsoft.Xna.Framework.Color));
        }

        public void updateValue(int pickedUp)
        {
            moneyDiplay.coins += pickedUp;
            moneyDiplay.addCPM(pickedUp);
        }
    }

    public class UIMoneyDisplay : UIElement
    {
        public long coins;

        public UIMoneyDisplay()
        {
            Width.Set(100, 0f);
            Height.Set(40, 0f);

            for (int i = 0; i < 60; i++)
            {
                coinBins[i] = -1;
            }
        }

        DateTime dpsEnd;
        DateTime dpsStart;
        int dpsDamage;
        public bool dpsStarted;
        public DateTime dpsLastHit;

        // Array of ints 60 long.
        // "length" = seconds since reset
        // reset on button or 20 seconds of inactibvity?
        // pointer to index so on new you can clear previous
        int[] coinBins = new int[60];
        int coinBinsIndex;

        public void addCPM(int coins)
        {
            int second = DateTime.Now.Second;
            if (second != coinBinsIndex)
            {
                coinBinsIndex = second;
                coinBins[coinBinsIndex] = 0;
            }
            coinBins[coinBinsIndex] += coins;
        }

        public int getCPM()
        {
            int second = DateTime.Now.Second;
            if (second != coinBinsIndex)
            {
                coinBinsIndex = second;
                coinBins[coinBinsIndex] = 0;
            }

            long sum = coinBins.Sum(a => a > -1 ? a : 0);
            int count = coinBins.Count(a => a > -1);
            if (count == 0)
            {
                return 0;
            }
            return (int)((sum * 60f) / count);
        }

        /// <summary>
        /// It gets draw here.
        /// </summary>
        /// <param name="spriteBatch"></param>
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle innerDimensions = base.GetInnerDimensions();
            Vector2 drawPos = new Vector2(innerDimensions.X + 5f, innerDimensions.Y + 30f);

            float shopx = innerDimensions.X;
            float shopy = innerDimensions.Y;

            int[] coinsArray = Utils.CoinsSplit(coins);
            for (int j = 0; j < 4; j++)
            {
                int num = (j == 0 && coinsArray[3 - j] > 99) ? -6 : 0;
                spriteBatch.Draw(Main.itemTexture[74 - j], new Vector2(shopx + 11f + (float)(24 * j), shopy /*+ 75f*/), null, Color.White, 0f, Main.itemTexture[74 - j].Size() / 2f, 1f, SpriteEffects.None, 0f);
                Utils.DrawBorderStringFourWay(spriteBatch, Main.fontItemStack, coinsArray[3 - j].ToString(), shopx + (float)(24 * j) + (float)num, shopy/* + 75f*/, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
            }

            coinsArray = Utils.CoinsSplit(getCPM());
            for (int j = 0; j < 4; j++)
            {
                int num = (j == 0 && coinsArray[3 - j] > 99) ? -6 : 0;
                spriteBatch.Draw(Main.itemTexture[74 - j], new Vector2(shopx + 11f + (float)(24 * j), shopy + 25f), null, Color.White, 0f, Main.itemTexture[74 - j].Size() / 2f, 1f, SpriteEffects.None, 0f);
                Utils.DrawBorderStringFourWay(spriteBatch, Main.fontItemStack, coinsArray[3 - j].ToString(), shopx + (float)(24 * j) + (float)num, shopy + 25f, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
            }
            Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, "CPM", shopx + (float)(24 * 4), shopy + 25f, Color.White, Color.Black, new Vector2(0.3f), 0.75f);





            Item theItem = Expeditions.expeditionList[0].expedition.GetDeliverablesArray()[0];
            ExpeditionUI.DrawItemSlot(spriteBatch, theItem, 200, 280, 7);
            theItem = Expeditions.expeditionList[0].expedition.GetRewardsArray()[0];
            ExpeditionUI.DrawItemSlot(spriteBatch, theItem, 200, 310, 15);
            theItem = Expeditions.expeditionList[0].expedition.GetRewardsArray()[1];
            ExpeditionUI.DrawItemSlot(spriteBatch, theItem, 230, 310, 15);
        }

        internal void ResetCoins()
        {
            coins = 0;
            for (int i = 0; i < 60; i++)
            {
                coinBins[i] = -1;
            }
        }
    }

    public class MoneyCounterGlobalItem : GlobalItem
    {
        public override bool OnPickup(Item item, Player player)
        {
            if (item.type == ItemID.CopperCoin)
            {
                (mod as Expeditions).expeditionUI.updateValue(item.stack);
            }
            else if (item.type == ItemID.SilverCoin)
            {
                (mod as Expeditions).expeditionUI.updateValue(item.stack * 100);
            }
            else if (item.type == ItemID.GoldCoin)
            {
                (mod as Expeditions).expeditionUI.updateValue(item.stack * 10000);
            }
            else if (item.type == ItemID.PlatinumCoin)
            {
                (mod as Expeditions).expeditionUI.updateValue(item.stack * 1000000);
            }
            return base.OnPickup(item, player);
        }
    }
}
