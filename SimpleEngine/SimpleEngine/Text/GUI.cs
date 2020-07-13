using System.Collections.Generic;
using System.Linq;
using SimpleEngine.GameScene;
using OpenTK;

namespace SimpleEngine.Text
{

    /// <summary>
    /// Represents a window, containing textboxes
    /// Each box has a gamestate assigned to it - GameState which the game is set to on a mouse clicknig the given textbox
    /// </summary>
    public class GUI
    {
        public Dictionary<TextBox, GameStates> TextBoxes { set; get; } 
            = new Dictionary<TextBox, GameStates>();

        public GUI(Dictionary<TextBox, GameStates> textBoxes)
        {
            TextBoxes = textBoxes;
        }

        /// <param name="mousePos">mouse cursor position RELATIVE to window's MIDDLE </param>
        public GameStates OnMouseClick(Vector2 mousePos)
        {
            foreach (var pair in TextBoxes)
            {
                if (pair.Key.HasHitbox && pair.Key.IsColliding(mousePos))
                {
                    return pair.Value;
                }
            }
            return GameStates.None;
        }

        /// <param name="mousePos">mouse cursor position RELATIVE to window MIDDLE </param>
        public void Draw(Vector2 mousePos, int modelUniform = 0, int colorUniform = 2, int textureBinding = 0)
        {
            TextBoxes.Keys.ToList().ForEach(textBox => textBox.Draw(mousePos, modelUniform, colorUniform, textureBinding));
        }
    }
}
