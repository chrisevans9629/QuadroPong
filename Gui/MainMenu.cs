using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

namespace MyGame
{
    public interface IGui
    {
        Screen Screen { get; }
        void Update();
    }

   
    public interface IPongGame
    {
        int Width { get; }
        int Height { get; }
        void StartGameTeams();
        void StartGame4Player();
        void StartGameClassic();
        void ShowMainMenu();
        void ShowSettings();
        void Exit();
    }
    
    public class MainMenu : IGui
    {
        private readonly IPongGame _pongGame;

        public Button Button(string text)
        {
            var t = new Button(){Content = text, Name = text, Padding = _padding, Margin = _margin};
            Buttons.Add(t);
            return t;
        }

        readonly Thickness _padding = new Thickness(50, 20);
        readonly Thickness _margin = new Thickness(10);
        public MainMenu(
            IPongGame pongGame)
        {
            _pongGame = pongGame;
            var start = Button("Start 4 Player");
            var start2 = Button("Start 2 Player");
            var startTeams = Button("Start 4 Player Teams");
            startTeams.Clicked += (sender, args) => pongGame.StartGameTeams();
            var settings = Button("Settings");
            var quit = Button("Quit");


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
            start2.Clicked += (sender, args) => pongGame.StartGameClassic();
            start.Clicked += (sender, args) => pongGame.StartGame4Player();
            settings.Clicked += (sender, args) => pongGame.ShowSettings();
            label = src.FindControl<Label>("winner");

            quit.Clicked += (sender, args) => pongGame.Exit();

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
        public Screen Screen { get; set; }
    }
}