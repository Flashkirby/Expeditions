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
    /// <summary>
    /// See the API class. Do not use unless you know precisely what you're doing.
    /// </summary>
    public class ExpeditionUI : UIState
    {
        public const int viewMode_NPC = 0;
        public const int viewMode_Tile = 1;
        public const int viewMode_Menu = 2;

        //First Panel
        private const int _navPanelWidth = 400;
        public static bool visible = false;
        public static int viewMode = 0;
        private UIPanel _navigationPanel;
        private UIValueBar _scrollBar;
        private UITextWrap _indexText;
        private List<UIToggleImage> _categoryButtons = new List<UIToggleImage>();

        // Second Panel
        private const int _expPanelWidth = 460;
        private UIPanel _expeditionPanel;
        private UITextWrap _titleHeader;
        private UIImage _headImage;
        private UITextWrap _description;
        private UITextWrap _conditionHeader;
        private UITextWrap _conditionsDesc;
        private UIItemSlots _deliverableSlots;
        private UITextWrap _rewardHeader;
        private UIItemSlots _rewardSlots;
        private List<ItemSlot> _deliveryItems = new List<ItemSlot>();
        private List<ItemSlot> _rewardItems = new List<ItemSlot>();
        private UITextButton _trackButton;
        private UITextButton _completeButton;

        private ModExpedition currentME
        {
            get { return sortedList[_scrollBar.Value - 1]; }
        }

        // Data
        public List<ModExpedition> filterList;
        public List<ModExpedition> sortedList;

        private bool previewMode { get { return viewMode == viewMode_Menu; } }

        public override void OnInitialize()
        {
            filterList = new List<ModExpedition>(Expeditions.GetExpeditionsList());
            sortedList = new List<ModExpedition>(filterList);

            _navigationPanel = new UIPanel();
            _navigationPanel.SetPadding(0);
            _navigationPanel.Left.Set(30 + (_expPanelWidth - _navPanelWidth) / 2, 0);
            _navigationPanel.Top.Set(120, 0);
            _navigationPanel.Width.Set(_navPanelWidth, 0);
            _navigationPanel.Height.Set(90, 0);
            _navigationPanel.BackgroundColor = UIColour.backgroundColour;
            _navigationPanel.BorderColor = UIColour.borderColour;

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
            AppendCategoryButtonsLine1(_navPanelWidth - 150, 10);
            AppendCategoryButtonsLine2(_navPanelWidth - 150, 46);
            _indexText = AppendText("000/000", 156, 16, Color.White, true);
            base.Append(_navigationPanel);

            //#########################################################################
            _expeditionPanel = new UIPanel();
            _expeditionPanel.SetPadding(0);
            _expeditionPanel.Left.Set(30, 0);
            _expeditionPanel.Top.Set(_navigationPanel.Top.Pixels + _navigationPanel.Height.Pixels + 4, 0);
            _expeditionPanel.Width.Set(_expPanelWidth, 0);
            _expeditionPanel.Height.Set(300, 0);
            _expeditionPanel.BackgroundColor = UIColour.backgroundColour;
            _expeditionPanel.BorderColor = UIColour.borderColour;

            _deliverableSlots = new UIItemSlots(7);
            _deliverableSlots.Left.Set(14, 0);
            _deliverableSlots.Top.Set(8, 0);
            _deliverableSlots.Width.Set(_expPanelWidth - 20, 0);

            _rewardSlots = new UIItemSlots(15);
            _rewardSlots.Left.Set(14, 0);
            _rewardSlots.Top.Set(8, 0);
            _rewardSlots.Width.Set(_expPanelWidth - 20, 0);
            
            _headImage = new UIImage(Main.npcHeadTexture[0]);
            _headImage.Left.Set(_expPanelWidth - 38, 0);
            _headImage.Top.Set(6, 0);

            Color invis = new Color(0, 0, 0, 0);
            _titleHeader = AppendTextPan2("Title", _expPanelWidth/2, 16, Color.White, Color.Black, true);
            _expeditionPanel.Append(_headImage);
            _description = AppendTextPan2("The character '_' fills a large amount of space, eg. ___________________________________________. Cool!",
                16, 16, Color.White, invis);
            _conditionHeader = AppendTextPan2("Goals", _expPanelWidth / 2, 16, Color.White, Color.Black, true);
            _conditionsDesc = AppendTextPan2("Be the amazing person you already are. ", 16, 16, Color.White, invis);
            _expeditionPanel.Append(_deliverableSlots);
            _rewardHeader = AppendTextPan2("Bounty", _expPanelWidth / 2, 16, Color.White, Color.Black, true);
            _expeditionPanel.Append(_rewardSlots);
            _trackButton = AppendTextButtonPan2("Un/track", 20, 0, new MouseEvent(ToggleTrackedClicked));
            _completeButton = AppendTextButtonPan2("Complete", 120, 0, new MouseEvent(CompleteClicked));
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
        private UITextButton AppendTextButtonPan2(string text, float x, float y, MouseEvent evt)
        {
            UITextButton textButton = new UITextButton(text, 1, false);
            textButton.Left.Set(x, 0f);
            textButton.Top.Set(y, 0f);
            textButton.OnMouseDown += evt;
            _expeditionPanel.Append(textButton);
            return textButton;
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
            UITextWrap textWrap = new UITextWrap(text, _expPanelWidth - 32, colour, border, centre);
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
                if (j == 2)
                {
                    uIToggleImage.OnClick += new UIElement.MouseEvent(this.SortAlphabetical);
                }
                else if(j ==3)
                {
                    uIToggleImage.OnClick += new UIElement.MouseEvent(this.SortDifficulty);
                }
                else
                {
                    uIToggleImage.OnClick += new UIElement.MouseEvent(this.FilterList);
                }
                if(j != 3)
                {
                    // Set include completed, repeat only, and sort by alphabet as false
                    uIToggleImage.SetState(false);
                }
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
                Context, new Vector2(x, y), default(Color));
        }

        private void IncrementIndexClick(UIMouseEvent evt, UIElement listeningElement)
        {
            _scrollBar.Value++;
            //UpdateIndex();
        }
        private void DecrementIndexClick(UIMouseEvent evt, UIElement listeningElement)
        {
            _scrollBar.Value--;
            //UpdateIndex();
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

        private void SortAlphabetical(UIMouseEvent evt, UIElement listeningElement)
        {
            if (this._categoryButtons[6].IsOn)
            { this._categoryButtons[7].SetState(false); }
            else
            { this._categoryButtons[7].SetState(true); }

            ListRecalculate();
        }

        private void SortDifficulty(UIMouseEvent evt, UIElement listeningElement)
        {
            if (this._categoryButtons[7].IsOn)
            { this._categoryButtons[6].SetState(false); }
            else
            { this._categoryButtons[6].SetState(true); }

            ListRecalculate();
        }

        private void ToggleTrackedClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            if (currentME != null)
            {
                currentME.expedition.ToggleTrackingActive();
                Main.PlaySound(12, -1, -1, 1);
            }
            //UpdateIndex();
        }

        private void CompleteClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            if(currentME != null && !previewMode &&
                (currentME.expedition.ConditionsMet()))
            {
                currentME.expedition.CompleteExpedition(false);
                Main.PlaySound(12, -1, -1, 1);

                if(!currentME.expedition.partyShare || Main.netMode != 1) ListRecalculate();
            }
            else
            {
                Main.PlaySound(22, -1, -1, 1);
                ListRecalculate();
            }
        }

        private string StrTick(bool ticked)
        {
            return ticked ? "[x] " : "[ ] ";
        }
        /// <summary>
        /// Update the Index Text
        /// </summary>
        public void UpdateIndex()
        {
            _scrollBar.Value = _scrollBar.Value; //this will update to include floor/ceiling
            _indexText.SetText(_scrollBar.Value + "/" + _scrollBar.MaxValue);

            if (_scrollBar.Value > 0)
            {
                float yBottom = 0;
                _titleHeader.SetText(currentME.expedition.name + (currentME.expedition.completed ? " (Completed)" : "") + (Expeditions.DEBUG?" #"+Expedition.GetHashID(currentME.expedition).ToString("X"):""));
                yBottom += _titleHeader.TextHeight + 10;

                try
                {
                    if (currentME.expedition.npcHead > 0)
                    {
                        _headImage.SetImage(Main.npcHeadTexture[currentME.expedition.npcHead]);
                    }
                    else
                    {
                        _headImage.SetImage(Expeditions.bountyBoardTexture);
                    }
                }
                catch (Exception e) // On a fail, we'll just default to unknown
                {
                    _headImage.SetImage(Expeditions.bountyBoardTexture);
                }

                _description.SetText(currentME.expedition.GetDescription());
                _description.Top.Set(yBottom, 0f);
                yBottom += _description.TextHeight;

                _conditionHeader.SetText("Goals");
                _conditionHeader.Top.Set(yBottom, 0f);
                yBottom += _conditionHeader.TextHeight;

                // build conditionals strings
                string conditionals = "";
                string firstLine = "";
                if (currentME.expedition.conditionDescription1 != "")
                {
                    conditionals += StrTick(currentME.expedition.condition1Met) + 
                        currentME.expedition.conditionDescription1;
                    firstLine = "\n";
                }
                if (currentME.expedition.conditionDescription2 != "")
                {
                    conditionals += firstLine + StrTick(currentME.expedition.condition2Met) +
                          currentME.expedition.conditionDescription2;
                    firstLine = "\n";
                }
                if (currentME.expedition.conditionDescription3 != "")
                {
                    conditionals += firstLine + StrTick(currentME.expedition.condition3Met) +
                          currentME.expedition.conditionDescription3;
                    firstLine = "\n";
                }
                if (currentME.expedition.conditionDescriptionCountable != "")
                {
                    conditionals += firstLine + "[" + currentME.expedition.conditionCounted
                        + (currentME.expedition.conditionCountedMax > 0 ? "/" + currentME.expedition.conditionCountedMax : "")
                        + "] " + currentME.expedition.conditionDescriptionCountable;
                    firstLine = "\n";
                }
                _conditionsDesc.SetText(conditionals);
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

                if (currentME.expedition.completed && !currentME.expedition.repeatable)
                {
                    _trackButton.SetText("");
                    _completeButton.SetText("");
                }
                else
                {
                    _trackButton.SetText((currentME.expedition.trackingActive ? "Untrack" : "Track"));
                    _trackButton.Top.Set(yBottom + 10f, 0f);
                    if (!previewMode)
                    {
                        _completeButton.SetText((currentME.expedition.ConditionsMet() ? "Complete" : "In Progress"));
                    }else
                    {
                        _completeButton.SetText("");
                    }
                    _completeButton.Top.Set(yBottom + 10f, 0f);
                    yBottom += 10;
                }

                _expeditionPanel.Height.Set(32 + yBottom, 0);
            }
            else
            {
                _titleHeader.SetText("No Expeditions Posted");
                _headImage.SetImage(Expeditions.bountyBoardTexture);
                _description.SetText("");
                _conditionHeader.SetText("");
                _conditionsDesc.SetText("");
                _deliverableSlots.Items = null;
                _rewardHeader.SetText("");
                _rewardSlots.Items = null;
                _trackButton.SetText("");
                _completeButton.SetText("");

                _expeditionPanel.Height.Set(32 + _titleHeader.TextHeight, 0);
            }

            // move the elements to new set position
            this.Recalculate();
        }
        public void UpdateIndex(UIMouseEvent evt, UIElement listeningElement)
        {
            UpdateIndex();
        }

        /// <summary>
        /// Recalculate and sort the list again
        /// </summary>
        public void ListRecalculate()
        {
            if (Expeditions.DEBUG) Main.NewText("#Recalculating");
            // get a new list
            filterList.Clear();
            sortedList.Clear();

            int anyMatch = 0;
            foreach (ModExpedition current in Expeditions.GetExpeditionsList())
            {
                Expedition e = current.expedition;
                anyMatch = 0;
                // line 1
                if (e.ctgSlay && this._categoryButtons[0].IsOn) { anyMatch++; }
                if (e.ctgCollect && this._categoryButtons[1].IsOn) { anyMatch++; }
                if (e.ctgExplore && this._categoryButtons[2].IsOn) { anyMatch++; }
                if (e.ctgImportant && this._categoryButtons[3].IsOn) { anyMatch++; }
                if (anyMatch == 0) continue;
                // line 2
                if (e.completed && !this._categoryButtons[4].IsOn) { continue; }
                if (!e.repeatable && this._categoryButtons[5].IsOn) { continue; }
                if (!e.PrerequisitesMet()) continue;
                filterList.Add(current);
            }

            // sort by these
            int sortMode = 0;
            if (this._categoryButtons[6].IsOn) sortMode = 0;
            if (this._categoryButtons[7].IsOn) sortMode = 1;

            switch(sortMode)
            {
                case 1:
                    sortedList = filterList.OrderBy(me => me.expedition.difficulty).ToList();
                    break;
                default:
                    sortedList = filterList.OrderBy(me => me.expedition.name).ToList();
                    break;
            }

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
            // set scrollbar blip colours
            Color[] colours = new Color[sortedList.Count];
            for(int i = 0; i < colours.Length; i++)
            {
                colours[i] = UIColour.GetColourFromRarity(sortedList[i].expedition.difficulty);
            }
            _scrollBar.SetBlipColours(colours);
            
            //Main.NewText("Re-sorted List: " + sortedList.Count);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
			if (sortedList.Count > 0)
			{
                int tier = currentME.expedition.difficulty;
                if (currentME.expedition.ctgImportant && !currentME.expedition.completed) tier = -12;
                _titleHeader.SetColour(UIColour.GetColourFromRarity(tier));
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
            // constantly check while open
            UpdateIndex();

            // draw category mouse text
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
                            text = "Show Completed";
                            break;
                        case 5:
                            text = "Only Repeatables";
                            break;
                        case 6:
                            text = "Sort by A-Z";
                            break;
                        case 7:
                            text = "Sort by Tier";
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
}
