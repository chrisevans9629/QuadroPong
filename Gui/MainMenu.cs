using System;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

namespace MyGame
{
    public class MainMenu
    {
        public MainMenu()
        {
            var src = new Screen()
            {
                Content = new StackPanel()
                {
                    Margin = new Thickness(10),
                    Items =
                    {
                        new Label()
                        {
                            Name = "winner"
                        },
                        new Button()
                        {
                            Name = "start",
                            Content = "Start",
                            Padding = new Thickness(50,20),
                            Margin = new Thickness(10)
                        },
                        new Button()
                        {
                            Name = "settings",
                            Content = "Settings",
                            Padding = new Thickness(50,20),
                            Margin = new Thickness(10)
                        }
                    },
                    HorizontalAlignment = HorizontalAlignment.Centre,
                    VerticalAlignment = VerticalAlignment.Centre
                }
            };

            src.FindControl<Button>("start").Clicked += (sender, args) => Start?.Invoke();
            src.FindControl<Button>("settings").Clicked += (sender, args) => Settings?.Invoke();
            label = src.FindControl<Label>("winner");
            Screen = src;
        }

        private Label label;
        public string? Winner { get => label.Content?.ToString(); set => label.Content = value; }
        public Action Start { get; set; }
        public Action Settings { get; set; }
        public Screen Screen { get; set; }

    }
}