﻿using System;
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
        private UITextWrap _titleHeader;
        private UITextWrap _description;
        private UITextWrap _conditionHeader;
        private UITextWrap _conditionsDesc;
        private UIItemSlots _deliverableSlots;
        private UITextWrap _rewardHeader;
        private UIItemSlots _rewardSlots;
        private List<ItemSlot> _deliveryItems = new List<ItemSlot>();
        private List<ItemSlot> _rewardItems = new List<ItemSlot>();

        private ModExpedition currentME
        {
            get { return sortedList[_scrollBar.Value - 1]; }
        }

        // Data
        public List<ModExpedition> filterList;
        public List<ModExpedition> sortedList;



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
            AppendCategoryButtonsLine1(250, 10);
            AppendCategoryButtonsLine2(250, 46);
            _indexText = AppendText("000/000", 156, 16, Color.White, true);
            base.Append(_navigationPanel);

            //#########################################################################
            _expeditionPanel = new UIPanel();
            _expeditionPanel.SetPadding(0);
            _expeditionPanel.Left.Set(400, 0);
            _expeditionPanel.Top.Set(_navigationPanel.Top.Pixels + _navigationPanel.Height.Pixels + 8, 0);
            _expeditionPanel.Width.Set(400, 0);
            _expeditionPanel.Height.Set(300, 0);
            _expeditionPanel.BackgroundColor = UIColour.backgroundColour;
            _expeditionPanel.BorderColor = UIColour.borderColour;

            _deliverableSlots = new UIItemSlots(7);
            _deliverableSlots.Left.Set(14, 0);
            _deliverableSlots.Top.Set(8, 0);
            _deliverableSlots.Width.Set(380, 0);

            _rewardSlots = new UIItemSlots(15);
            _rewardSlots.Left.Set(14, 0);
            _rewardSlots.Top.Set(8, 0);
            _rewardSlots.Width.Set(380, 0);

            Color invis = new Color(0, 0, 0, 0);
            _titleHeader = AppendTextPan2("Title", 200, 16, Color.White, Color.Black, true);
            _description = AppendTextPan2("The character '_' fills a large amount of space, eg. ___________________________________________. Cool!",
                16, 16, Color.White, invis);
            _conditionHeader = AppendTextPan2("Goals", 200, 16, Color.White, Color.Black, true);
            _conditionsDesc = AppendTextPan2("Be the amazing person you already are. ", 16, 16, Color.White, invis);
            _expeditionPanel.Append(_deliverableSlots);
            _rewardHeader = AppendTextPan2("Bounty", 200, 16, Color.White, Color.Black, true);
            _expeditionPanel.Append(_rewardSlots);
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
        private void AppendTextButtonPan2(string text, float x, float y, MouseEvent evt)
        {
            UITextButton textButton = new UITextButton(text, 1, false);
            textButton.Left.Set(x, 0f);
            textButton.Top.Set(y, 0f);
            textButton.OnMouseDown += evt;
            _expeditionPanel.Append(textButton);
        }

        private UITextWrap AppendText(string text, float x, float y, Color colour, bool centre = false)
        {
            UITextWrap textWrap = new UITextWrap(text, 368, colour, Color.Black, centre);
            textWrap.Left.Set(x, 0f);
            textWrap.Top.Set(y - 3f, 0f);
            _navigationPanel.Append(textWrap);
            return textWrap;
        }
        private UITextWrap AppendTextPan2(string text, float x, float y, Color colour, Color border, bool centre = false)
        {
            UITextWrap textWrap = new UITextWrap(text, 368, colour, border, centre);
            textWrap.Left.Set(x, 0f);
            textWrap.Top.Set(y - 3f, 0f);
            _expeditionPanel.Append(textWrap);
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

            if (_scrollBar.Value > 0)
            {
                float yBottom = 0;
                _titleHeader.SetText(currentME.expedition.title);
                yBottom += _titleHeader.TextHeight + 10;

                _description.SetText(currentME.expedition.description);
                _description.Top.Set(yBottom, 0f);
                yBottom += _description.TextHeight;

                _conditionHeader.SetText("Goals");
                _conditionHeader.Top.Set(yBottom, 0f);
                yBottom += _conditionHeader.TextHeight;

                _conditionsDesc.SetText(currentME.expedition.conditionDescription);
                _conditionsDesc.Top.Set(yBottom, 0f);
                yBottom += _conditionsDesc.TextHeight;

                _deliverableSlots.Items = currentME.expedition.GetDeliverablesArray();
                _deliverableSlots.Top.Set(yBottom, 0f);
                yBottom += _deliverableSlots.SlotHeight;

                _rewardHeader.SetText("Bounty");
                _rewardHeader.Top.Set(yBottom, 0f);
                yBottom += _rewardHeader.TextHeight;

                _rewardSlots.Items = currentME.expedition.GetRewardsArray();
                _rewardSlots.Top.Set(yBottom, 0f);
                yBottom += _rewardSlots.SlotHeight;

                _expeditionPanel.Height.Set(32 + yBottom, 0);
            }
            else
            {
                _titleHeader.SetText("No Expeditions Posted");
                _description.SetText("");
                _conditionHeader.SetText("");
                _conditionsDesc.SetText("");
                _deliverableSlots.Items = null;
                _rewardHeader.SetText("");
                _rewardSlots.Items = null;

                _expeditionPanel.Height.Set(32 + _titleHeader.TextHeight, 0);
            }
            this.Recalculate();
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
			if (sortedList.Count > 0)
			{
				_titleHeader.SetColour(UIColour.GetColourFromRarity(currentME.expedition.difficulty));
			}
			else
			{
				_titleHeader.SetColour(UIColour.Grey);
			}
				
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
    
    public class UIItemSlots : UIElement
    {
        public const int itemSlotSize = (int)(50 * 0.6f + 4); //scale being used

        private int _context = 7;

        public Item[] Items;
        public int SlotHeight
        {
            get
            {
                if(Items == null || Items.Length <= 0) return 0;
                return itemSlotSize * (int)(1 + (Items.Length * itemSlotSize) / Width.Pixels);
            }
        }

        public UIItemSlots(int context = 7)
        {
            _context = context;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (Items == null) return;
            CalculatedStyle dimensions = base.GetDimensions();
            Rectangle bounds = dimensions.ToRectangle();

            if (Items.Length > 0)
            {
                int noOfLines = (Items.Length * itemSlotSize) / bounds.Width;
                //Main.NewText(deliverables.Length + " / " + (bounds.Width / itemSlotSize));
                //Main.NewText(deliverables.Length + " items cover " + noOfLines + "lines");
                int i = 0;
                noOfLines++;
                for (int y = 0; y < noOfLines; y++)
                {
                    for (int x = 0; x < (bounds.Width / itemSlotSize); x++)
                    {
                        ExpeditionUI.DrawItemSlot(spriteBatch, Items[i],
                            bounds.Left + x * itemSlotSize, bounds.Y + y * itemSlotSize, _context);
                        i++;
                        if (i >= Items.Length) break;
                    }
                    if (i >= Items.Length) break;
                }
            }
        }
    }
}
