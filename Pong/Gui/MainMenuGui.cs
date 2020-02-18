using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using PongGame.States;

namespace MyGame
{
    public class MainMenuGui : IGui
    {
        private readonly IPongGame _pongGame;

        public Button Button(string text, Action action, bool visible = true)
        {
            var t = new Button(){Content = text, Name = text, Padding = _padding, Margin = _margin, IsVisible = visible};
            t.Style();
            Buttons.Add(t);
            t.Clicked += (sender, args) => action();
            return t;
        }

        readonly Thickness _padding = new Thickness(50, 20);
        readonly Thickness _margin = new Thickness(10);
        public MainMenuGui(
            IPongGame pongGame,
            IGameStateManager gameStateManager)
        {
            _pongGame = pongGame;
            var resume = Button("Resume", pongGame.ResumeGame, pongGame.IsInGame || gameStateManager.HasSavedGame());
            var start = Button("Start 4 Player", pongGame.StartGame4Player);
            var start2 = Button("Start 2 Player", pongGame.StartGameClassic);
            var startTeams = Button("Start 4 Player Teams", pongGame.StartGameTeams);
            var settings = Button("Settings", pongGame.ShowSettings);
            var quit = Button("Quit",pongGame.Exit);


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
                        resume,
                        start,
                        start2,
                        startTeams,
                        settings,
                        quit,
                    },
                    HorizontalAlignment = HorizontalAlignment.Centre,
                    VerticalAlignment = VerticalAlignment.Centre
                }
            };
            label = src.FindControl<Label>("winner");
            start.IsPressed = true;
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
                    if (selectedButton > 0 && Buttons[selectedButton-1].IsVisible)
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
                    if (selectedButton < Buttons.Count-1 && Buttons[selectedButton+1].IsVisible)
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
        public Screen Screen { get; set; }
    }
}