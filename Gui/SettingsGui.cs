using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

namespace MyGame
{
    public static class Styles
    {
        public static Button Style(this Button btn)
        {
            btn.BackgroundColor = Color.Black;
            btn.BorderColor = Color.White;
            btn.TextColor = Color.White;
            btn.BorderThickness = 2;
            return btn;
        }
        public static StackPanel Orientation(this StackPanel stack, Orientation orientation)
        {
            stack.Orientation = orientation;
            return stack;
        }
    }

    public class SettingsGui : IGui
    {
        readonly Thickness _padding = new Thickness(50, 20);
        readonly Thickness _margin = new Thickness(10);
        public SettingsGui(IPongGame pongGame, ISettings settings)
        {
            Screen = new Screen()
            {
                Content = new StackPanel()
                {
                    HorizontalAlignment = HorizontalAlignment.Centre,
                    VerticalAlignment = VerticalAlignment.Centre,
                    Items =
                    {
                        Button("Main Menu", pongGame.ShowMainMenu),
                        CheckBox("Has Astroids", settings.HasAstroids, p => settings.HasAstroids = p.IsChecked),
                        Stack(
                            Button("-", () => settings.MasterVolume--),
                            Label(settings.MasterVolume.ToString(), p => ((Label)p).Content = settings.MasterVolume),
                            Button("+", () => settings.MasterVolume++)
                            ).Orientation(Orientation.Horizontal)
                    }
                }
            };
        }
        public Screen Screen { get; set; }

        class GuiUpdate
        {
            public Action<Control> action { get; set; }
            public Control control { get; set; }
        }
        private List<GuiUpdate> Updates { get; set; } = new List<GuiUpdate>();
        void Add(Control control, Action<Control>? update)
        {
            if (update != null)
            {
                Updates.Add(new GuiUpdate { control = control, action = update });
            }
        }
        public Label Label(string text, Action<Control>? update = null)
        {
            var lbl = new Label(text);
            lbl.VerticalAlignment = VerticalAlignment.Centre;
            Add(lbl, update);
            return lbl;
        }

        public StackPanel Stack(params Control[] controls)
        {
            var stack = new StackPanel();
            foreach (var control in controls)
            {
                stack.Items.Add(control);
            }
            return stack;
        }

        public CheckBox CheckBox(string text, bool isChecked, Action<CheckBox>? update = null)
        {
            var t = new CheckBox()
            {
                Content = text,
                Name = text,
                Padding = _padding,
                Margin = _margin,
                IsChecked = isChecked,
            };
            Add(t, control => update?.Invoke(control as CheckBox));
            return t;
        }
        public Button Button(string text, Action action, Action<Control>? update = null)
        {
            var t = new Button() { Content = text, Name = text, Padding = _padding, Margin = _margin };
            t.Style();
            t.Clicked += (sender, args) => action();
            Add(t, update);
            return t;
        }
        public void Update()
        {
            foreach (var update in Updates)
            {
                update.action(update.control);
            }
        }
    }
}