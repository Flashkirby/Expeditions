using System;
using System.Collections.Generic;
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

        public UIPanel navigationPanel;
        public UIMoneyDisplay moneyDiplay;
        public ItemSlot itemslot;
        public static bool visible = false;


        private List<UIToggleImage> _categoryButtons = new List<UIToggleImage>();

        public override void OnInitialize()
        {
            navigationPanel = new UIPanel();
            navigationPanel.SetPadding(0);
            navigationPanel.Left.Set(400, 0);
            navigationPanel.Top.Set(100, 0);
            navigationPanel.Width.Set(400, 0);
            navigationPanel.Height.Set(80, 0);
            navigationPanel.BackgroundColor = backgroundColour;
            navigationPanel.BorderColor = borderColour;

            //navigationPanel.OnMouseDown += new UIElement.MouseEvent(DragStart);
            //navigationPanel.OnMouseUp += new UIElement.MouseEvent(DragEnd);

            Texture2D buttonPlayTexture = ModLoader.GetTexture("Terraria/UI/ButtonPlay");
            UIImageButton playButton = new UIImageButton(buttonPlayTexture);
            playButton.Left.Set(510, 0f);
            playButton.Top.Set(10, 0f);
            playButton.Width.Set(22, 0f);
            playButton.Height.Set(22, 0f);
            playButton.OnClick += new MouseEvent(PlayButtonClicked);
            navigationPanel.Append(playButton);

            Texture2D buttonDeleteTexture = ModLoader.GetTexture("Terraria/UI/ButtonDelete");
            UIImageButton closeButton = new UIImageButton(buttonDeleteTexture);
            closeButton.Left.Set(540, 0f);
            closeButton.Top.Set(10, 0f);
            closeButton.Width.Set(22, 0f);
            closeButton.Height.Set(22, 0f);
            closeButton.OnClick += new MouseEvent(CloseButtonClicked);
            navigationPanel.Append(closeButton);

            moneyDiplay = new UIMoneyDisplay();
            moneyDiplay.Left.Set(15, 0f);
            moneyDiplay.Top.Set(420, 0f);
            moneyDiplay.Width.Set(100f, 0f);
            moneyDiplay.Height.Set(0, 1f);
            navigationPanel.Append(moneyDiplay);

            //#########################################################################

            //close button
            AppendTextButton("Close", 50, 30, new MouseEvent(CloseButtonClicked));
            //prev button
            AppendTextButton("Prev", 120, 30, new MouseEvent(PlayButtonClicked));

            //AppendText("000/000", 160, 10, Color.White);
            UITextWrap textWrap = new UITextWrap("000/000", Color.White, Color.Black);
            textWrap.Left.Set(160, 0f);
            textWrap.Top.Set(10, 0f);
            navigationPanel.Append(textWrap);

            //next button
            AppendTextButton("Next", 220, 30, new MouseEvent(PlayButtonClicked));

            // Category Filter Buttons, Line 1
            AppendCategoryButtonsLine1(250, 10);

            base.Append(navigationPanel);
        }

        private void AppendTextButton(string text, float x, float y, MouseEvent evt)
        {
            UITextButton textButton = new UITextButton(text, 1, false);
            textButton.Left.Set(x, 0f);
            textButton.Top.Set(y, 0f);
            textButton.OnClick += evt;
            navigationPanel.Append(textButton);
        }

        private void AppendText(string text, float x, float y, Color colour)
        {
            UITextWrap textWrap = new UITextWrap(text, Color.White, Color.Black);
            textWrap.Left.Set(x, 0f);
            textWrap.Top.Set(y, 0f);
            navigationPanel.Append(textWrap);
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
            navigationPanel.Append(uIElement);
        }

        private void PlayButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.PlaySound(10, -1, -1, 1);
            moneyDiplay.ResetCoins();
        }

        private void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.PlaySound(11, -1, -1, 1);
            visible = false;
        }

        private void FilterList(UIMouseEvent evt, UIElement listeningElement)
        {
            /*
            this._achievementsList.Clear();
            foreach (UIAchievementListItem current in this._achievementElements)
            {
                if (this._categoryButtons[(int)current.GetAchievement().Category].IsOn)
                {
                    this._achievementsList.Add(current);
                }
            }
            */
            this.Recalculate();
        }

        Vector2 offset;
        public bool dragging = false;
        private void DragStart(UIMouseEvent evt, UIElement listeningElement)
        {
            offset = new Vector2(evt.MousePosition.X - navigationPanel.Left.Pixels, evt.MousePosition.Y - navigationPanel.Top.Pixels);
            dragging = true;
        }

        private void DragEnd(UIMouseEvent evt, UIElement listeningElement)
        {
            Vector2 end = evt.MousePosition;
            dragging = false;

            navigationPanel.Left.Set(end.X - offset.X, 0f);
            navigationPanel.Top.Set(end.Y - offset.Y, 0f);

            Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
            if (navigationPanel.ContainsPoint(MousePosition))
            {
                Main.player[Main.myPlayer].mouseInterface = true;
            }
            if (dragging)
            {
                navigationPanel.Left.Set(MousePosition.X - offset.X, 0f);
                navigationPanel.Top.Set(MousePosition.Y - offset.Y, 0f);
                Recalculate();
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            CategoryButtonMouseText(spriteBatch);
        }

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
                            text = "Repeatable";
                            break;
                        case 5:
                            text = "Party Share";
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
    }
    
    internal class UITextButton : UIElement
    {
        private string _text = "";
        private float _textScale = 1f;
        private Vector2 _textSize = Vector2.Zero;
        private bool _isLarge;
        private bool _isOn;
        public bool IsOn
        {
            get
            {
                return this._isOn;
            }
        }

        public UITextButton(string text, float textScale = 1f, bool large = false)
        {
            this.SetText(text, textScale, large);
        }
        public override void Recalculate()
        {
            this.SetText(this._text, this._textScale, this._isLarge);
            base.Recalculate();
        }

        public void SetText(string text)
        {
            this.SetText(text, this._textScale, this._isLarge);
        }
        public void SetText(string text, float textScale, bool large)
        {
            SpriteFont spriteFont = large ? Main.fontDeathText : Main.fontMouseText;
            Vector2 vector = new Vector2(spriteFont.MeasureString(text).X, large ? 32f : 16f) * textScale;
            this._text = text;
            this._textScale = textScale;
            this._textSize = vector;
            this._isLarge = large;
            this.MinWidth.Set(vector.X + this.PaddingLeft + this.PaddingRight, 0f);
            this.MinHeight.Set(vector.Y + this.PaddingTop + this.PaddingBottom, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = base.GetDimensions();
            Vector2 pos = dimensions.Position();
            Vector2 size = Main.fontMouseText.MeasureString(_text);


            if (Main.mouseX > pos.X - size.X * 0.5f && (float)Main.mouseX < (float)pos.X + size.X * 0.5f && 
                Main.mouseY > pos.Y - size.Y * 0.5f && (float)Main.mouseY < (float)pos.Y + size.Y * 0.5f) //yes normal text buttons extend down like this
            {
                Main.player[Main.myPlayer].mouseInterface = true;
                if (!_isOn)
                {
                    Main.PlaySound(12, -1, -1, 1);
                    Recalculate();
                }
                _isOn = true;
                Main.player[Main.myPlayer].releaseUseItem = false;
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Click(new UIMouseEvent(this, new Vector2(Main.mouseX, Main.mouseY)));
                }
            }
            else
            {
                if (_isOn)
                {
                    Main.PlaySound(12, -1, -1, 1);
                    Recalculate();
                }
                _isOn = false;
            }


            if (this._isLarge)
            {
                pos.Y -= 10f * this._textScale;
            }
            else
            {
                pos.Y -= 2f * this._textScale;
            }
            pos.X += (dimensions.Width - this._textSize.X) * 0.5f;
            float textScale = _textScale * (_isOn ? 1.1f : 1f);
            Vector2 scaleCentre = size * 0.5f;
            Color textColor = new Color(Main.mouseTextColor, (int)((double)Main.mouseTextColor / 1.1), Main.mouseTextColor / 2, Main.mouseTextColor);
            if (this._isLarge)
            {
                Utils.DrawBorderStringBig(spriteBatch, this._text, pos, textColor, textScale, scaleCentre.X, scaleCentre.Y, -1);
                return;
            }
            Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, this._text, pos.X, pos.Y,
                textColor, Color.Black, scaleCentre, textScale);
        }
    }
    internal class UITextWrap : UIElement
    {
        private string _text = "";
        private int _maxLines = 10;
        private int _maxWidth = 460;
        private Color _colour;
        private Color _borderColour;
        public UITextWrap(string text, Color color, Color borderColour)
        {
            this.SetText(text);
            _colour = color;
            _borderColour = borderColour;
        }

        public override void Recalculate()
        {
            this.SetText(this._text);
            base.Recalculate();
        }
        public void SetText(string text)
        {
            Vector2 vector = new Vector2(Main.fontMouseText.MeasureString(text).X, 16f);
            this._text = text;
            this.MinWidth.Set(vector.X + this.PaddingLeft + this.PaddingRight, 0f);
            this.MinHeight.Set(vector.Y + this.PaddingTop + this.PaddingBottom, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = base.GetDimensions();
            Vector2 pos = dimensions.Position();

            //calculate wordwrap of text
            int noOfLines = 0;
            string[] array = Utils.WordwrapString(_text, Main.fontMouseText, _maxWidth, _maxLines, out noOfLines);
            noOfLines++;
            int textValR = (int)((Main.mouseTextColor * 2 + _colour.R) / 3);
            int textValG = (int)((Main.mouseTextColor * 2 + _colour.G) / 3);
            int textValB = (int)((Main.mouseTextColor * 2 + _colour.B) / 3);
            int textValA = (int)((Main.mouseTextColor * 2 + _colour.A) / 3);
            Color textColor = new Color(textValR, textValG, textValB, textValA);
            
            //draw each line of text
            for (int i = 0; i < noOfLines; i++)
            {
                Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, array[i], pos.X, (float)(pos.Y + i * 30), textColor, _borderColour, Vector2.Zero, 1f);
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
}
