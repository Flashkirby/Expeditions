using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace Expeditions.UI
{
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
}
