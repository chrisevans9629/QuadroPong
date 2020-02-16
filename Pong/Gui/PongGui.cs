using System.Collections.Generic;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

namespace MyGame
{
    public class PongGui : IGui
    {
        private readonly IPongGame _pongGame;
        private readonly ISettings _settings;
        private CheckBox debugginCheckBox;
        private CheckBox runningCheckBox;
        private CheckBox soundCheckBox;

        public PongGui(IPongGame pongGame, ISettings settings)
        {
            _pongGame = pongGame;
            _settings = settings;
            Screen = new Screen()
            {
                Content = new StackPanel()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(20),
                    Items =
                    {
                        new Button()
                        {
                            Name = "main",
                            Content = "Main Menu",
                            Width = 100
                        },
                        new CheckBox()
                        {
                            Name = "Running",
                            Content = "Paused",
                        },
                        new CheckBox()
                        {
                            Name = "Sound",
                            Content = "Sound",
                        },
                        new CheckBox()
                        {
                            Name = "Debug",
                            Content = "Debug",
                        },

                    }
                },

            };
            var btn = Screen.FindControl<Button>("main");
            btn.Clicked += (sender, args) => pongGame.ShowMainMenu();
            btn.Style();
            runningCheckBox = Screen.FindControl<CheckBox>("Running");
            soundCheckBox = Screen.FindControl<CheckBox>("Sound");
            debugginCheckBox = Screen.FindControl<CheckBox>("Debug");

            debugginCheckBox.IsChecked = settings.IsDebugging;
            soundCheckBox.IsChecked = settings.IsSoundOn;
            runningCheckBox.IsChecked = settings.IsPaused;
        }


        public void Update()
        {
            _settings.IsDebugging = IsDebugging;
            _settings.IsSoundOn = SoundOn;
            _settings.IsPaused = IsRunning;
        }
        public Screen Screen { get; set; }
        public bool IsDebugging => debugginCheckBox.IsChecked;
        public bool SoundOn => soundCheckBox.IsChecked;
        public bool IsRunning => runningCheckBox.IsChecked;
    }
}