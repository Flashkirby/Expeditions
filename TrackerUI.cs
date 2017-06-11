using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using ReLogic.Graphics;

namespace Expeditions
{
    public class TrackerUI : UIState
    {
        internal static bool visible = false;
        internal static bool allowUpdateVisible = true;
        internal static bool showDescription = true;
        public static bool Visible
        {
            get { return visible || (allowUpdateVisible && recentChangeTick > 0); }
            set { visible = value; }
        }
        public static bool VisibleWithAlpha
        {
            get { return visible || (allowUpdateVisible && recentChangeTick > 0) || textAlpha > 0; }
            set { visible = value; }
        }
        internal static int ChangeTickMax = 60;

        internal static int recentChangeTick = 0;
        internal static float _xPos = 30f;
        internal static float _yPos = 110f;
        internal static byte permaVisAlpha = 255;
        internal static byte textAlpha = 255;
        internal static float textScale = 0.6f;

        private UIText uiText;
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                textAlpha = permaVisAlpha;
            }
            else
            {
                if (textAlpha > 0) textAlpha--;
            }

            float colMod = (Main.mouseTextColor / 255f);
            Color mainColour = new Color(255, 255, 255, 255) * colMod;
            Color bordColour = new Color(0, 0, 0, 255) * colMod;
            mainColour.A = textAlpha;
            bordColour.A = textAlpha;

            try
            {
                DrawTrackedText(spriteBatch, mainColour, bordColour, colMod);
            }
            catch (Exception e) { Main.NewTextMultiline(e.ToString()); }
        }

        private static void DrawTrackedText(SpriteBatch spriteBatch, Color mainC, Color bordC, float colMod)
        {
            float startY = _yPos;
            List<ModExpedition> mes = API.GetExpeditionsList();
            foreach (ModExpedition me in mes)
            {
                if (!me.expedition.trackingActive) continue;
                byte state = 255;
                if(Expeditions.checkedState.TryGetValue(me.expedition.GetHashID(), out state))
                {
                    if (state == 0 || state == 2 || state == 255) continue;
                }

                // Draw the Title
                string title = me.expedition.name;
                if (title.Length > 0)
                {
                    Color titleColour = UI.UIColour.GetColourFromRarity(me.expedition.difficulty) * colMod;
                    titleColour.A = textAlpha;
                    Utils.DrawBorderStringFourWay(spriteBatch,
                        Main.fontMouseText,
                        title,
                        _xPos, startY, titleColour * (textAlpha / 255f), bordC,
                        Vector2.Zero, textScale);
                    startY += Main.fontMouseText.MeasureString(title).Y * textScale;
                }

                // Draw the Description
                if (showDescription)
                {
                    string description = "";
                    string collecItems = me.expedition.hasDeliverables ? "Collect items" : "";
                    AddCondition(ref description, me.expedition.conditionDescription1, me.expedition.condition1Met);
                    AddCondition(ref description, me.expedition.conditionDescription2, me.expedition.condition2Met);
                    AddCondition(ref description, me.expedition.conditionDescription3, me.expedition.condition3Met);
                    AddCondition(ref description, collecItems, me.expedition.CheckRequiredItems(false));
                    AddCondition(ref description, me.expedition.conditionDescriptionCountable, me.expedition.conditionCounted, me.expedition.conditionCountedMax);
                    if (description.Length > 0)
                    {
                        Utils.DrawBorderStringFourWay(spriteBatch,
                            Main.fontMouseText,
                            description,
                            _xPos, startY, mainC * (textAlpha / 255f), bordC * (textAlpha / 255f),
                            Vector2.Zero, textScale);
                        startY += Main.fontMouseText.MeasureString(
                            description.Substring(0, description.Length - 2) // Remove last \n character
                            ).Y * textScale;
                    }
                }
            }
        }

        internal static void AddCondition(ref string text, string description, bool condition)
        {
            if(description != "")
            {
                text += "  " + ExpeditionUI.StrTick(condition) + description + "\n";
            }
        }
        internal static void AddCondition(ref string text, string description, int count, int countMax)
        {
            if (description != "")
            {
                text += "  " + string.Concat(
                    "[", count,
                    (countMax > 0 ? "/" + countMax : "")
                    , "] ", description,
                    "\n");
            }
        }
    }
}
