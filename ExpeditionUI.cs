using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Expeditions.UI;

using System.Linq;

namespace Expeditions
{
    class ExpeditionUI : UIState
    {
        //First Panel
        public static bool visible = false;
        private UIPanel _navigationPanel;
        private UIValueBar _scrollBar;
        private UITextWrap _indexText;
        private List<UIToggleImage> _categoryButtons = new List<UIToggleImage>();

        // Second Panel
        private UIPanel _expeditionPanel;
        private UITextWrap _title;
        private UITextWrap _description;
        private UITextWrap _condition;
        private UITextWrap _conditionsDesc;
        //private List<ItemSlot>

        // Data
        public List<ModExpedition> filterList;
        public List<ModExpedition> sortedList;

        // TODO: DELET dis
        public UIMoneyDisplay moneyDiplay;



        public override void OnInitialize()
        {
            filterList = new List<ModExpedition>(Expeditions.GetExpeditionsList());
            sortedList = new List<ModExpedition>(filterList);

            _navigationPanel = new UIPanel();
            _navigationPanel.SetPadding(0);
            _navigationPanel.Left.Set(400, 0);
            _navigationPanel.Top.Set(100, 0);
            _navigationPanel.Width.Set(400, 0);
            _navigationPanel.Height.Set(90, 0);
            _navigationPanel.BackgroundColor = UIColour.backgroundColour;
            _navigationPanel.BorderColor = UIColour.borderColour;

            Texture2D buttonPlayTexture = ModLoader.GetTexture("Terraria/UI/ButtonPlay");
            UIImageButton playButton = new UIImageButton(buttonPlayTexture);
            playButton.Left.Set(510, 0f);
            playButton.Top.Set(10, 0f);
            playButton.Width.Set(22, 0f);
            playButton.Height.Set(22, 0f);
            playButton.OnClick += new MouseEvent(IncrementIndexClick);
            _navigationPanel.Append(playButton);

            Texture2D buttonDeleteTexture = ModLoader.GetTexture("Terraria/UI/ButtonDelete");
            UIImageButton closeButton = new UIImageButton(buttonDeleteTexture);
            closeButton.Left.Set(540, 0f);
            closeButton.Top.Set(10, 0f);
            closeButton.Width.Set(22, 0f);
            closeButton.Height.Set(22, 0f);
            closeButton.OnClick += new MouseEvent(CloseButtonClicked);
            _navigationPanel.Append(closeButton);

            // Bar
            _scrollBar = new UIValueBar(0, sortedList.Count);
            _scrollBar.Left.Set(40, 0f);
            _scrollBar.Top.Set(50, 0f);
            _scrollBar.OnMouseUp += new MouseEvent(UpdateIndex);

            // Append (in reverse order)
            AppendTextButton("Close", 16, 16, new MouseEvent(CloseButtonClicked));
            AppendTextButton("Next", 200, 16, new MouseEvent(IncrementIndexClick));
            AppendTextButton("Prev", 80, 16, new MouseEvent(DecrementIndexClick));
            _navigationPanel.Append(_scrollBar);
            AppendCategoryButtonsLine2(250, 46);
            AppendCategoryButtonsLine1(250, 10);
            _indexText = AppendText("000/000", 156, 16, Color.White, true);
            base.Append(_navigationPanel);

            //#########################################################################
            float yOffset = _navigationPanel.Top.Pixels + _navigationPanel.Height.Pixels;
            _expeditionPanel = new UIPanel();
            _expeditionPanel.SetPadding(0);
            _expeditionPanel.Left.Set(400, 0);
            _expeditionPanel.Top.Set(yOffset + 8, 0);
            _expeditionPanel.Width.Set(400, 0);
            _expeditionPanel.Height.Set(120, 0);
            _expeditionPanel.BackgroundColor = UIColour.backgroundColour;
            _expeditionPanel.BorderColor = UIColour.borderColour;

            base.Append(_expeditionPanel);
        }

        private void AppendTextButton(string text, float x, float y, MouseEvent evt)
        {
            UITextButton textButton = new UITextButton(text, 1, false);
            textButton.Left.Set(x, 0f);
            textButton.Top.Set(y, 0f);
            textButton.OnMouseDown += evt;
            _navigationPanel.Append(textButton);
        }

        private UITextWrap AppendText(string text, float x, float y, Color colour, bool centre = false)
        {
            UITextWrap textWrap = new UITextWrap(text, Color.White, Color.Black, centre);
            textWrap.Left.Set(x, 0f);
            textWrap.Top.Set(y - 3f, 0f);
            _navigationPanel.Append(textWrap);
            return textWrap;
        }

        private void AppendCategoryButtonsLine1(float x, float y)
        {
            UIElement uIElement = new UIElement();
            uIElement.Width.Set(0f, 1f);
            uIElement.Height.Set(32f, 0f);
            uIElement.Top.Set(y, 0f);
            Texture2D texture = ModLoader.GetTexture("Terraria/UI/Achievement_Categories");
            for (int j = 0; j < 4; j++)
            {
                UIToggleImage uIToggleImage = new UIToggleImage(texture, 32, 32, new Point(34 * j, 0), new Point(34 * j, 34));
                uIToggleImage.Left.Set((float)(j * 36 + x), 0f);
                uIToggleImage.SetState(true);
                uIToggleImage.OnClick += new UIElement.MouseEvent(this.FilterList);
                this._categoryButtons.Add(uIToggleImage);
                uIElement.Append(uIToggleImage);
            }
            _navigationPanel.Append(uIElement);
        }
        private void AppendCategoryButtonsLine2(float x, float y)
        {
            UIElement uIElement = new UIElement();
            uIElement.Width.Set(0f, 1f);
            uIElement.Height.Set(32f, 0f);
            uIElement.Top.Set(y, 0f);
            Texture2D texture = Expeditions.sortingTexture;
            for (int j = 0; j < 4; j++)
            {
                UIToggleImage uIToggleImage = new UIToggleImage(texture, 32, 32, new Point(34 * j, 0), new Point(34 * j, 34));
                uIToggleImage.Left.Set((float)(j * 36 + x), 0f);
                uIToggleImage.SetState(true);
                uIToggleImage.OnClick += new UIElement.MouseEvent(this.FilterList);
                this._categoryButtons.Add(uIToggleImage);
                uIElement.Append(uIToggleImage);
            }
            _navigationPanel.Append(uIElement);
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

        private void IncrementIndexClick(UIMouseEvent evt, UIElement listeningElement)
        {
            _scrollBar.Value++;
            UpdateIndex();
        }
        private void DecrementIndexClick(UIMouseEvent evt, UIElement listeningElement)
        {
            _scrollBar.Value--;
            UpdateIndex();
        }

        private void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.PlaySound(11, -1, -1, 1);
            visible = false;
        }

        private void FilterList(UIMouseEvent evt, UIElement listeningElement)
        {
            ListRecalculate();
        }

        public void UpdateIndex() { UpdateIndex(null, null); }
        /// <summary>
        /// Update the Index Text
        /// </summary>
        public void UpdateIndex(UIMouseEvent evt, UIElement listeningElement)
        {
            _scrollBar.Value = _scrollBar.Value; //this will update to include floor/ceiling
            _indexText.SetText(_scrollBar.Value + "/" + _scrollBar.MaxValue);
        }

        /// <summary>
        /// Recalculate and sort the list again
        /// </summary>
        public void ListRecalculate()
        {
            // get a new list
            filterList.Clear();
            sortedList.Clear();

            int anyMatch = 0;
            foreach (ModExpedition current in Expeditions.GetExpeditionsList())
            {
                Expedition e = current.expedition;
                anyMatch = 0;
                // line 1
                if (e.defeat && this._categoryButtons[0].IsOn) { anyMatch++; }
                if (e.deliver && this._categoryButtons[1].IsOn) { anyMatch++; }
                if (e.explore && this._categoryButtons[2].IsOn) { anyMatch++; }
                if (e.important && this._categoryButtons[3].IsOn) { anyMatch++; }
                if (anyMatch == 0) continue;
                // line 2
                if (e.completed && !this._categoryButtons[5].IsOn) { continue; }
                if (e.repeatable && !this._categoryButtons[6].IsOn) { continue; }
                filterList.Add(current);
            }


            // to be sorted later
            sortedList.AddRange(filterList);

            // set scrollbar
            if(sortedList.Count > 0)
            {
                _scrollBar.MinValue = 1;
                _scrollBar.MaxValue = sortedList.Count;
            }
            else
            {
                _scrollBar.MinValue = 0;
                _scrollBar.MaxValue = 0;
            }
            UpdateIndex();
            Main.NewText("Re-sorted List: " + sortedList.Count);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
            if (_navigationPanel.ContainsPoint(MousePosition) ||
                _expeditionPanel.ContainsPoint(MousePosition))
            {
                Main.player[Main.myPlayer].mouseInterface = true;
            }

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            CategoryButtonMouseText(spriteBatch);
        }

        /// <summary>
        /// Draw the text of image buttons
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void CategoryButtonMouseText(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < this._categoryButtons.Count; i++)
            {
                if (this._categoryButtons[i].IsMouseHovering)
                {
                    string text;
                    switch (i)
                    {
                        case -1:
                            text = "None";
                            break;
                        case 0:
                            text = "Slayer";
                            break;
                        case 1:
                            text = "Collector";
                            break;
                        case 2:
                            text = "Explorer";
                            break;
                        case 3:
                            text = "Challenger";
                            break;
                        case 4:
                            text = "Completed";
                            break;
                        case 5:
                            text = "Repeatable";
                            break;
                        case 6:
                            text = "Sort Alphabetically";
                            break;
                        case 7:
                            text = "Sort by Difficulty";
                            break;
                        default:
                            text = "None";
                            break;
                    }
                    float x = Main.fontMouseText.MeasureString(text).X;
                    Vector2 vector = new Vector2((float)Main.mouseX, (float)Main.mouseY) + new Vector2(16f);
                    if (vector.Y > (float)(Main.screenHeight - 30))
                    {
                        vector.Y = (float)(Main.screenHeight - 30);
                    }
                    if (vector.X > (float)Main.screenWidth - x)
                    {
                        vector.X = (float)(Main.screenWidth - 460);
                    }
                    Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, text, vector.X, vector.Y, new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor), Color.Black, Vector2.Zero, 1f);
                    return;
                }
            }
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




            List<ModExpedition> expeditionList = Expeditions.GetExpeditionsList();
            Item theItem = expeditionList[0].expedition.GetDeliverablesArray()[0];
            ExpeditionUI.DrawItemSlot(spriteBatch, theItem, 200, 280, 7);
            theItem = expeditionList[0].expedition.GetRewardsArray()[0];
            ExpeditionUI.DrawItemSlot(spriteBatch, theItem, 200, 310, 15);
            theItem = expeditionList[0].expedition.GetRewardsArray()[1];
            ExpeditionUI.DrawItemSlot(spriteBatch, theItem, 230, 310, 15);

            //TODO: Remove
            //test show all quests
            string listOfEs = "";
            foreach (ModExpedition me in expeditionList)
            {
                listOfEs += me.expedition.title + "\n";
            }
            Main.spriteBatch.DrawString(
            Main.fontMouseText, listOfEs,
            new Vector2(200, 200), Color.Magenta, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
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
}
