﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using Akavache;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.ViewportAdapters;
using MyGame.Levels;
using PongGame;
using PongGame.Sprites;

namespace MyGame
{
    public class PongGame : Game, IPongGame
    {
        private FrameCounter frameCounter = new FrameCounter();
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Settings _settings;
        private GuiSystem _guiSystem;
        private IGui gui;
        //private MainMenu mainMenu;

        Level? level;

        public PongGame()
        {
            Akavache.Registrations.Start("PongGame");
            _settings = new Settings();
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferMultiSampling = true;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.PreferredBackBufferWidth = 1000;
            this.Window.AllowUserResizing = true;
            Window.ClientSizeChanged += WindowOnClientSizeChanged;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            BlobCache.Shutdown().Wait();
            base.OnExiting(sender, args);
        }

        protected override void Initialize()
        {
            level?.Initialize();
            //fourPlayer.BackToMenu = o => BackToMainMenu();
            base.Initialize();
        }

        private void WindowOnClientSizeChanged(object sender, EventArgs e)
        {
            _guiSystem.ClientSizeChanged();
            _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            _graphics.ApplyChanges();
            level?.WindowResized();
        }
       

        public int Width => GraphicsDevice.Viewport.Width;
        public int Height => GraphicsDevice.Viewport.Height;
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var font = Content.Load<SpriteFont>("arial");
            var font1 = Content.Load<BitmapFont>("Sensation");
            music = Content.Load<SoundEffect>("retromusic");

            this.frameCounter.Load(font);
            LoadMusic();
            level?.LoadContent(Content, new Point(Width, Height));
            LoadGui(font1);
        }

        
       

        private void LoadGui(BitmapFont font1)
        {
            var viewportAdapter = new DefaultViewportAdapter(GraphicsDevice);
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, () => Matrix.Identity);
            BitmapFont.UseKernings = false;
            Skin.CreateDefault(font1);
            //gui = new PongGui();
            //mainMenu = new MainMenu
            //{
            //    Start = StartGame,
            //    Quit = Exit, 
            //    Start2 = StartGame2, 
            //    StartTeamsAction = StartGameTeams
            //};
            gui = new MainMenu(this);
            _guiSystem = new GuiSystem(viewportAdapter, guiRenderer)
            {
                ActiveScreen = gui.Screen //gui.Screen,
            };
        }

        public void StartGameTeams()
        {
            gui = new PongGui(this, _settings);
            _guiSystem.ActiveScreen = gui.Screen;
            IsInGame = true;
            //level?.Dispose();
            this.level = new RegularPongLevel(true);
            level.Initialize();
            level.LoadContent(Content, new Point(Width, Height));
            level.BackToMenu = o => BackToMainMenu();
        }
        public void StartGameClassic()
        {
            gui = new PongGui(this, _settings);
            _guiSystem.ActiveScreen = gui.Screen;
            IsInGame = true;
            //level?.Dispose();
            this.level = new RegularPongLevel();
            level.Initialize();
            level.LoadContent(Content, new Point(Width, Height));
            level.BackToMenu = o => BackToMainMenu();
        }

        public void ShowMainMenu()
        {
            BackToMainMenu();
        }

        public void ShowSettings()
        {
            gui = new SettingsGui(this);
            _guiSystem.ActiveScreen = gui.Screen;
        }

        private void BackToMainMenu()
        {
            gui = new MainMenu(this);
            _guiSystem.ActiveScreen = gui.Screen;
            IsInGame = false;
        }

      
        public void StartGame4Player()
        {
            gui = new PongGui(this, _settings);
            _guiSystem.ActiveScreen = gui.Screen;
            IsInGame = true;
            //level?.Dispose();
            this.level = new FourPlayerLevel();
            level.Initialize();
            level.LoadContent(Content, new Point(Width, Height));
            level.BackToMenu = o => BackToMainMenu();
        }
        private void LoadMusic()
        {
            var backSong = music.CreateInstance();
            backSong.IsLooped = true;
            backSong.Volume = defaultVolume;
            backSong.Play();
            musicSoundEffect = backSong;

        }
        private SoundEffect music;

        public SoundEffectInstance musicSoundEffect { get; set; }
        private float defaultVolume = 0.2f;
        public bool IsInGame { get; set; }
        protected override void Update(GameTime gameTime)
        {
            //frameCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            _guiSystem.Update(gameTime);
            
            gui.Update();

            if (!IsInGame)
                return;

            if (_settings.IsPaused)
                return;
            if (!_settings.IsSoundOn)
            {
                musicSoundEffect.Volume = 0;
            }
            else
            {
                musicSoundEffect.Volume = defaultVolume;
            }
            
            level?.Update(gameTime, new GameState(){Width = Width, Height = Height, IsDebug = _settings.IsDebugging, ViewPort = GraphicsDevice.Viewport.Bounds, IsSoundOn = _settings.IsSoundOn});
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);


            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            level?.Draw(_spriteBatch, gameTime, new Point(Width, Height));
            
            _guiSystem.Draw(gameTime);

            //frameCounter.Draw(_spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
