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
    }

    public class SettingsGui : IGui
    {
        readonly Thickness _padding = new Thickness(50, 20);
        readonly Thickness _margin = new Thickness(10);
        public SettingsGui(IPongGame pongGame)
        {
            Screen = new Screen()
            {
                Content = new StackPanel()
                {
                    Items =
                    {
                        Button("Main Menu", pongGame.ShowMainMenu)
                    }
                }
            };
        }
        public Screen Screen { get; set; }


        public List<Action> Updates { get; set; } = new List<Action>();


        public Button Button(string text, Action action, Action? update = null)
        {
            var t = new Button() { Content = text, Name = text, Padding = _padding, Margin = _margin };
            t.Style();
            t.Clicked += (sender, args) => action();
            if (update != null)
            {
                Updates.Add(update);
            }
            return t;
        }
        public void Update()
        {
            foreach (var update in Updates)
            {
                update();
            }
        }
    }
}