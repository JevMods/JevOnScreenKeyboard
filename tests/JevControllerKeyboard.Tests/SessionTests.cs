using System;
using System.Collections.Generic;
using System.Linq;
using JevControllerKeyboard.Features;
using JevControllerKeyboard.Infrastructure;
using static JevControllerKeyboard.Tests.Check;

namespace JevControllerKeyboard.Tests
{
    public static class SessionTests
    {
        public static void Run()
        {
            Sessions();
        }

        private static void Sessions()
        {
            var catalog = new LanguageLayouts(EmbeddedText.Read);

            var layout = new KeyLayout();
            layout.Reset(catalog.For("French"));
            var editor = new Editor();
            var session = new KeyboardSession(layout, editor);
            Focus(layout, 'e');
            session.ToggleChoices();
            That("accents open", true, session.AccentsOpen);
            That("accents focused", true, layout.ChoicesFocused);
            That("accents on top row", 0, layout.Row);
            session.Press(layout.Selected);
            That("accent typed", "é", editor.Text);
            That("accents closed", false, session.AccentsOpen);
            That("selection returned", 'e', layout.Selected.Character);

            Focus(layout, 'a');
            session.ToggleChoices();
            session.ToggleChoices();
            That("toggle cancels accents", false, session.AccentsOpen);
            That("toggle returns selection", 'a', layout.Selected.Character);

            Focus(layout, 'a');
            session.ToggleChoices();
            layout.MoveVertical(1);
            session.DismissAccentsOnNavigation();
            That("moving away dismisses accents", false, layout.ChoicesShown);

            Focus(layout, '1');
            session.ToggleChoices();
            That("digits have no variants", false, layout.ChoicesShown);

            layout.Reset(catalog.For("Chinese"));
            editor = new Editor();
            session = new KeyboardSession(layout, editor);
            Focus(layout, 'n');
            session.Press(layout.Selected);
            Focus(layout, 'i');
            session.Press(layout.Selected);
            That("candidates shown", true, layout.ChoicesShown);
            That("candidate letters", "ni", editor.Text);
            session.ToggleChoices();
            That("toggle focuses candidates", true, layout.ChoicesFocused);
            session.Press(layout.Selected);
            That("candidate typed", "你", editor.Text);
            That("candidates cleared", false, layout.ChoicesShown);
            That("selection back on letter", 'i', layout.Selected.Character);
            Focus(layout, 'h');
            session.Press(layout.Selected);
            session.Backspace();
            That("backspace ends composition", "你", editor.Text);
            That("candidates gone", false, layout.ChoicesShown);

            layout.Reset(catalog.For("Korean"));
            editor = new Editor();
            session = new KeyboardSession(layout, editor);
            Focus(layout, 'ㅎ');
            session.Press(layout.Selected);
            Focus(layout, 'ㅏ');
            session.Press(layout.Selected);
            That("hangul keeps digits", false, layout.ChoicesShown);
            That("hangul syllable", "하", editor.Text);
            var choice = new Key(KeyAction.Choice, 'a', 0f, 1f);
            var letter = new Key(KeyAction.Character, 'a', 0f, 1f);
            That("choice keys do not repeat", false, KeyboardSession.Repeats(choice));
            That("letters repeat", true, KeyboardSession.Repeats(letter));
        }

        private static void Focus(KeyLayout layout, char character)
        {
            for (var step = 0; step < 200 && layout.Selected.Character != character; step++)
            {
                layout.MoveHorizontal(1);
                if (step % 20 == 19)
                {
                    layout.MoveVertical(1);
                }
            }

            That("focus " + character, character, layout.Selected.Character);
        }
    }
}
