using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria;
using Terraria.UI;

namespace Expeditions.UI
{
    public class UITextButton : UIElement
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
            DynamicSpriteFont spriteFont = large ? Main.fontDeathText : Main.fontMouseText;
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
}
