using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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

             start = src.FindControl<Button>("start");
            start.Clicked += (sender, args) => Start?.Invoke();
             settings = src.FindControl<Button>("settings");
            settings.Clicked += (sender, args) => Settings?.Invoke();
            settings.IsPressed = true;
            label = src.FindControl<Label>("winner");
            
            
            
            Screen = src;
        }

        private Button start;
        private Button settings;
        private Button? selectedButton;
        public void Update()
        {
            if(!Screen.IsVisible)
                return;
            var capabilities = GamePad.GetCapabilities(PlayerIndex.One);
            if (capabilities.IsConnected && capabilities.HasLeftYThumbStick && capabilities.HasAButton)
            {
                
                var state = GamePad.GetState(PlayerIndex.One);
                if (state.Buttons.A == ButtonState.Pressed)
                {
                    selectedButton?.Click();
                }
                var left = state.ThumbSticks.Left;
                if(left == Vector2.Zero)
                    return;
                if (left.Y > 0.5f)
                {
                    start.IsPressed = true;
                    selectedButton = start;
                    settings.IsPressed = false;
                }
                else if (left.Y < -0.5f)
                {
                    settings.IsPressed = true;
                    selectedButton = settings;
                    start.IsPressed = false;
                }
            }
        }

        private Label label;
        public string? Winner { get => label.Content?.ToString(); set => label.Content = value; }
        public Action Start { get; set; }
        public Action Settings { get; set; }
        public Screen Screen { get; set; }

    }
}