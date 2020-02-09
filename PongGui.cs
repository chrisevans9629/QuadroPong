using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

namespace MyGame
{
    public class PongGui
    {
        private CheckBox debugginCheckBox;
        private CheckBox runningCheckBox;
        private CheckBox soundCheckBox;

        public PongGui()
        {
            Screen = new Screen()
            {
                Content = new StackPanel()
                {
                    Margin = new Thickness(10),
                    Items =
                    {
                        new CheckBox()
                        {
                            Name = "Running",
                            Content = "Start",
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
                }
            };

            runningCheckBox = Screen.FindControl<CheckBox>("Running");
            soundCheckBox = Screen.FindControl<CheckBox>("Sound");
            debugginCheckBox = Screen.FindControl<CheckBox>("Debug");

            soundCheckBox.IsChecked = true;
            runningCheckBox.IsChecked = true;
        }

        public Screen Screen { get; set; }
        public bool IsDebugging => debugginCheckBox.IsChecked;
        public bool SoundOn => soundCheckBox.IsChecked;
        public bool IsRunning => runningCheckBox.IsChecked;
    }
}