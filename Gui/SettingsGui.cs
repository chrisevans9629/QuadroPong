using System;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

namespace MyGame
{
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

        public Button Button(string text, Action action)
        {
            var t = new Button() { Content = text, Name = text, Padding = _padding, Margin = _margin };
            t.Clicked += (sender, args) => action();
            return t;
        }
        public void Update()
        {
        }
    }
}