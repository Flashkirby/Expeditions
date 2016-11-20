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

        public static bool visible = false;
        public UIPanel navigationPanel;
        public UIValueBar scrollBar;
        public UITextWrap indexText;

        private List<UIToggleImage> _categoryButtons = new List<UIToggleImage>();
        public List<ModExpedition> filterList;
        public List<ModExpedition> sortedList;

        // TODO: DELET dis
        public UIMoneyDisplay moneyDiplay;
        public ItemSlot itemslot;



        public override void OnInitialize()
        {
            filterList = new List<ModExpedition>(Expeditions.GetExpeditionsList());
            sortedList = new List<ModExpedition>(filterList);

            navigationPanel = new UIPanel();
            navigationPanel.SetPadding(0);
            navigationPanel.Left.Set(400, 0);
            navigationPanel.Top.Set(100, 0);
            navigationPanel.Width.Set(400, 0);
            navigationPanel.Height.Set(90, 0);
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
            playButton.OnClick += new MouseEvent(IncrementIndexClick);
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

            // Close button
            AppendTextButton("Close", 16, 16, new MouseEvent(CloseButtonClicked));
            // Prev button
            AppendTextButton("Prev", 80, 16, new MouseEvent(DecrementIndexClick));
            // Next button
            AppendTextButton("Next", 200, 16, new MouseEvent(IncrementIndexClick));

            // Bar
            scrollBar = new UIValueBar(0, sortedList.Count);
            scrollBar.Left.Set(40, 0f);
            scrollBar.Top.Set(50, 0f);
            scrollBar.OnMouseUp += new MouseEvent(UpdateIndex);
            navigationPanel.Append(scrollBar);

            // Category Filter Buttons
            AppendCategoryButtonsLine1(250, 10);
            AppendCategoryButtonsLine2(250, 46);

            // Counter text
            indexText = AppendText("000/000", 156, 16, Color.White, true);


            base.Append(navigationPanel);
        }

        private void AppendTextButton(string text, float x, float y, MouseEvent evt)
        {
            UITextButton textButton = new UITextButton(text, 1, false);
            textButton.Left.Set(x, 0f);
            textButton.Top.Set(y, 0f);
            textButton.OnMouseDown += evt;
            navigationPanel.Append(textButton);
        }

        private UITextWrap AppendText(string text, float x, float y, Color colour, bool centre = false)
        {
            UITextWrap textWrap = new UITextWrap(text, Color.White, Color.Black, centre);
            textWrap.Left.Set(x, 0f);
            textWrap.Top.Set(y - 3f, 0f);
            navigationPanel.Append(textWrap);
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
            navigationPanel.Append(uIElement);
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
            navigationPanel.Append(uIElement);
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
            scrollBar.Value++;
            UpdateIndex();
        }
        private void DecrementIndexClick(UIMouseEvent evt, UIElement listeningElement)
        {
            scrollBar.Value--;
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
            scrollBar.Value = scrollBar.Value; //this will update to include floor/ceiling
            indexText.SetText(scrollBar.Value + "/" + scrollBar.MaxValue);
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
                scrollBar.MinValue = 1;
                scrollBar.MaxValue = sortedList.Count;
            }
            else
            {
                scrollBar.MinValue = 0;
                scrollBar.MaxValue = 0;
            }
            UpdateIndex();
            Main.NewText("Re-sorted List: " + sortedList.Count);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
            if (navigationPanel.ContainsPoint(MousePosition))
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

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Main.PlaySound(12, -1, -1, 1);
            Recalculate();
            _isOn = true;
        }
        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Main.PlaySound(12, -1, -1, 1);
            Recalculate();
            _isOn = false;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = base.GetDimensions();
            Vector2 pos = dimensions.Position();
            Vector2 size = Main.fontMouseText.MeasureString(_text);

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
                Utils.DrawBorderStringBig(spriteBatch, this._text, pos + scaleCentre, textColor, textScale, scaleCentre.X, scaleCentre.Y, -1);
                return;
            }
            Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, this._text, pos.X + scaleCentre.X, pos.Y + scaleCentre.Y,
                textColor, Color.Black, scaleCentre, textScale);
        }
    }
    internal class UITextWrap : UIElement
    {
        private string _text = "";
        private int _maxLines = 10;
        private int _maxWidth = 460;
        private int _textHeight = 0;
        private bool _centred = false;
        private Color _colour;
        private Color _borderColour;
        public int TextHeight
        {
            get { return _textHeight; }
        }
        public UITextWrap(string text, Color color, Color borderColour, bool centre)
        {
            this.SetText(text);
            _centred = centre;
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
            float offsetX = 0;
            if (_centred) offsetX = Main.fontMouseText.MeasureString(_text).X * 0.5f;

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
            _textHeight = 30 * noOfLines;
            for (int i = 0; i < noOfLines; i++)
            {
                Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, array[i], pos.X - offsetX, (float)(pos.Y + i * 30), textColor, _borderColour, Vector2.Zero, 1f);
            }
        }
    }
    internal class UIValueBar : UIElement
    {
        private static Color barColour = new Color(43, 56, 101, 200);

        Texture2D bar = Main.colorBarTexture;
        Texture2D blip = Main.colorBlipTexture;
        Texture2D slider = Main.colorSliderTexture; //80 range with offset 3

        private int _minValue = 0;
        private int _maxValue = 4;
        private int _lastIndex = 0;
        private int _index = 0;
        private int _dragVal = 0;
        private float _widthRange = 160f;
        private bool _dragging = false;
        public int Value
        {
            get { return _index; }
            set
            {
                if (value > _maxValue) value = _maxValue;
                if (value < _minValue) value = _minValue;
                _index = value;
                //Main.NewText("Scroll Value: " + _minValue + " < " + value + " > " + _maxValue);
            }
        }
        public int MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }
        public int MaxValue
        {
            get { return _maxValue; }
            set
            {
                if (value < _minValue) value = _minValue;
                _maxValue = value;
            }
        }
        public UIValueBar(int minValue, int maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            _widthRange = 160f;
            Width.Set(178f, 0f);
            Height.Set(25f, 0f);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            _dragging = true;
        }
        public override void MouseUp(UIMouseEvent evt)
        {
            base.MouseUp(evt);
            _dragging = false;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = base.GetDimensions();
            Vector2 pos = dimensions.Position();

            // Draw bar
            spriteBatch.Draw(bar, pos + new Vector2(0f, 4f), bar.Bounds, barColour);

            // Draw blips on bar
            int range = _maxValue - _minValue;
            for (int i = 0; i < range + 1; i++)
            {
                spriteBatch.Draw(blip, pos + new Vector2(8f + (160f / range) * i, 8f), blip.Bounds, Color.LightSlateGray);
            }


            // set to mouse
            if (_dragging) _dragVal = (int)(Main.mouseX - pos.X - 10);

            // limit slider to bar
            _dragVal = (int)Math.Max(Math.Min(_widthRange, _dragVal), 0f);

            //Set index to rounded position of dragVal in relation to range and distance across width
            _lastIndex = _index;
            if (_dragging) _index = (int)((0.5f + _minValue) + range * _dragVal / _widthRange);

            // Draw Slider
            spriteBatch.Draw(slider, pos + new Vector2(3f + _dragVal, 0f), slider.Bounds, Color.White);
            _dragVal = (int)(_widthRange / range * (_index - _minValue));
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
