﻿using MyGame.Levels;

namespace MyGame
{
    public interface IPongGame
    {
        IContentManager ContentManager { get; }
        int Width { get; set; }
        int Height { get; set; }
        void StartGameTeams();
        void StartGame4Player();
        void StartGameClassic();
        void ShowMainMenu();
        void ShowSettings();
        void Exit();
        bool IsInGame { get; }
        void ResumeGame();
        void HostGame();
        void JoinGame();
    }
}