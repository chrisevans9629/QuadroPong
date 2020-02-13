using System;
using System.Collections.Generic;
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
            var padding = new Thickness(50, 20);
            var margin = new Thickness(10);

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
                            Padding = padding,
                            Margin = margin
                        },
                        new Button()
                        {
                            Name = "settings",
                            Content = "Settings",
                            Padding = padding,
                            Margin = margin
                        },
                        new Button()
                        {
                            Name = "quit",
                            Content = "Quit",
                            Padding = padding,
                            Margin = margin
                        },
                    },
                    HorizontalAlignment = HorizontalAlignment.Centre,
                    VerticalAlignment = VerticalAlignment.Centre
                }
            };

            var start = src.FindControl<Button>("start");
            start.Clicked += (sender, args) => Start?.Invoke();
            var settings = src.FindControl<Button>("settings");
            settings.Clicked += (sender, args) => Settings?.Invoke();
            label = src.FindControl<Label>("winner");

            var quitButton = src.FindControl<Button>("quit");
            quitButton.Clicked += (sender, args) => Quit?.Invoke();

            start.IsPressed = true;

            Buttons.Add(start);
            Buttons.Add(settings);
            Buttons.Add(quitButton);

            Screen = src;
        }

        public List<Button> Buttons { get; set; } = new List<Button>();

        private int selectedButton;
        public void Update()
        {
            if (!Screen.IsVisible)
                return;
            var capabilities = GamePad.GetCapabilities(PlayerIndex.One);
            if (capabilities.IsConnected && capabilities.HasLeftYThumbStick && capabilities.HasAButton)
            {

                var state = GamePad.GetState(PlayerIndex.One);
                if (state.Buttons.A == ButtonState.Pressed)
                {
                    Buttons[selectedButton]?.Click();
                }
                var left = state.ThumbSticks.Left;
                if (left == Vector2.Zero)
                    return;
                if (left.Y > 0.5f)
                {
                    if(wasUp)
                        return;
                    if (selectedButton > 0)
                    {
                        selectedButton--;
                    }
                    foreach (var button in Buttons)
                    {
                        if (button == Buttons[selectedButton])
                        {
                            button.IsPressed = true;
                        }
                        else
                        {
                            button.IsPressed = false;
                        }
                    }

                    wasUp = true;
                    //start.IsPressed = true;
                    //selectedButton = start;
                    //settings.IsPressed = false;
                }
                else if (left.Y < -0.5f)
                {
                    if(wasDown)
                        return;
                    if (selectedButton < Buttons.Count-1)
                    {
                        selectedButton++;
                    }
                    foreach (var button in Buttons)
                    {
                        if (button == Buttons[selectedButton])
                        {
                            button.IsPressed = true;
                        }
                        else
                        {
                            button.IsPressed = false;
                        }
                    }

                    //settings.IsPressed = true;
                    //selectedButton = settings;
                    //start.IsPressed = false;
                    wasDown = true;
                }
                else
                {
                    wasUp = false;
                    wasDown = false;
                }
            }
        }

        private bool wasUp;
        private bool wasDown;
        private Label label;
        public string? Winner { get => label.Content?.ToString(); set => label.Content = value; }
        public Action Start { get; set; }
        public Action Settings { get; set; }
        public Action Quit { get; set; }
        public Screen Screen { get; set; }



    }
}