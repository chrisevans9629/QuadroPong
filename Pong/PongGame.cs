﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using Akavache;
using DryIoc;
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
using PongGame.States;

namespace MyGame
{
    public class PongGame : Game, IPongGame
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private GuiSystem _guiSystem;
        private IGui _gui;
        //private MainMenu mainMenu;
        private readonly ISettings _settings;
        private readonly FrameCounter _frameCounter;
        Level? level;
        private readonly Container container;
        public IContentManager ContentManager => new ContentManagerWrapper(Content);
        public PongGame()
        {
            Akavache.Registrations.Start("PongGame");
            container = new Container();

            container.Register<IGameStateManager, GameStateManager>(Reuse.Singleton);
            container.Register<ISettings, Settings>(Reuse.Singleton);
            container.Register<FrameCounter>();
            container.RegisterInstance<IPongGame>(this);
            container.Register<MainMenuGui>();
            container.Register<PongGui>();
            container.Register<SettingsGui>();

            container.Register<RegularPongLevel>(setup: Setup.With(allowDisposableTransient: true));
            container.Register<FourPlayerLevel>(setup: Setup.With(allowDisposableTransient: true));


            _frameCounter = container.Resolve<FrameCounter>();

            _settings = container.Resolve<ISettings>();

            _graphics = new GraphicsDeviceManager(this)
            {
                PreferMultiSampling = true,
                PreferredBackBufferHeight = 1000,
                PreferredBackBufferWidth = 1000,
                IsFullScreen = _settings.IsFullScreen,
            };

            this.Window.AllowUserResizing = true;
            Window.ClientSizeChanged += WindowOnClientSizeChanged;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void OnExiting(object sender, EventArgs args)
        {

            level?.SaveGame();
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
            // _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            // _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            // _graphics.ApplyChanges();
            level?.WindowResized();
        }


        public int Width
        {
            get => GraphicsDevice.Viewport.Width;
            set
            {
                _graphics.PreferredBackBufferWidth = value;
                _graphics.ApplyChanges();
            }
        }

        public int Height
        {
            get => GraphicsDevice.Viewport.Height;
            set
            {
                _graphics.PreferredBackBufferHeight = value;
                _graphics.ApplyChanges();
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var font = Content.Load<SpriteFont>("arial");
            var font1 = Content.Load<BitmapFont>("Sensation");
            music = Content.Load<SoundEffect>("retromusic");

            this._frameCounter.Load(font);
            LoadMusic();
            level?.LoadContent(new ContentManagerWrapper(Content), new Point(Width, Height));
            LoadGui(font1);
        }




        private void LoadGui(BitmapFont font1)
        {
            var viewportAdapter = new DefaultViewportAdapter(GraphicsDevice);
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, () => Matrix.Identity);
            BitmapFont.UseKernings = false;
            Skin.CreateDefault(font1);

            _gui = container.Resolve<MainMenuGui>();
            _guiSystem = new GuiSystem(viewportAdapter, guiRenderer)
            {
                ActiveScreen = _gui.Screen //gui.Screen,
            };
        }

        public void StartGameTeams()
        {
            _settings.IsPaused = false;
            _gui = container.Resolve<PongGui>();
            _guiSystem.ActiveScreen = _gui.Screen;
            IsInGame = true;
            var lvl = container.Resolve<RegularPongLevel>();
            lvl.GameMode = GameMode.Teams;
            this.level = lvl;//new RegularPongLevel(true);

            level.Initialize();
            level.LoadContent(new ContentManagerWrapper(Content), new Point(Width, Height));
        }
        public void StartGameClassic()
        {
            _settings.IsPaused = false;
            _gui = container.Resolve<PongGui>();
            _guiSystem.ActiveScreen = _gui.Screen;
            IsInGame = true;
            this.level = container.Resolve<RegularPongLevel>();
            level.GameMode = GameMode.Classic;
            level.Initialize();
            level.LoadContent(new ContentManagerWrapper(Content), new Point(Width, Height));
        }
        public void StartGame4Player()
        {
            _settings.IsPaused = false;
            _gui = container.Resolve<PongGui>();
            _guiSystem.ActiveScreen = _gui.Screen;
            IsInGame = true;
            this.level = container.Resolve<FourPlayerLevel>();
            level.GameMode = GameMode.PlayerVs;
            level.Initialize();
            level.LoadContent(new ContentManagerWrapper(Content), new Point(Width, Height));
        }
        public void ShowMainMenu()
        {
            BackToMainMenu();
        }

        public void ShowSettings()
        {
            _gui = container.Resolve<SettingsGui>();
            _guiSystem.ActiveScreen = _gui.Screen;
        }

        private void BackToMainMenu()
        {
            _gui = container.Resolve<MainMenuGui>();
            _guiSystem.ActiveScreen = _gui.Screen;
            _settings.IsPaused = true;
        }



        private void LoadMusic()
        {
            var backSong = music.CreateInstance();
            backSong.IsLooped = true;
            if (_settings.IsSoundOn)
                backSong.Volume = 0;
            else
            {
                backSong.Volume = defaultVolume;
            }
            backSong.Play();
            musicSoundEffect = backSong;

        }
        private SoundEffect music;

        public SoundEffectInstance musicSoundEffect { get; set; }
        private float defaultVolume = 0.2f;
        public bool IsInGame { get; set; }
        public void ResumeGame()
        {
            if (!IsInGame)
            {
                var gameState = container.Resolve<IGameStateManager>();
                var state = gameState.LoadGame();
                IsInGame = true;
                if (state.GameMode == GameMode.PlayerVs)
                {
                    level = container.Resolve<FourPlayerLevel>();
                }
                else
                {
                    level = container.Resolve<RegularPongLevel>();
                }
                level.Initialize();
                //level.LoadContent(new ContentManagerWrapper(Content), new Point(Width, Height) );
                level.LoadSavedGame(new ContentManagerWrapper(Content), state);
            }
            _settings.IsPaused = false;
            _gui = container.Resolve<PongGui>();
            _guiSystem.ActiveScreen = _gui.Screen;
        }

        public void HostGame()
        {
            _settings.IsPaused = false;
            _gui = container.Resolve<PongGui>();
            _guiSystem.ActiveScreen = _gui.Screen;
            IsInGame = true;
            this.level = container.Resolve<FourPlayerLevel>();
            level.GameMode = GameMode.PlayerVs;
            level.GameHosting = GameHosting.Host;
            level.Initialize();
            level.LoadContent(new ContentManagerWrapper(Content), new Point(Width, Height));
        }

        public void JoinGame()
        {
            _settings.IsPaused = false;
            _gui = container.Resolve<PongGui>();
            _guiSystem.ActiveScreen = _gui.Screen;
            IsInGame = true;
            this.level = container.Resolve<FourPlayerLevel>();
            level.GameMode = GameMode.PlayerVs;
            level.GameHosting = GameHosting.Client;
            level.Initialize();
            level.LoadContent(new ContentManagerWrapper(Content), new Point(Width, Height));
        }

        protected override void Update(GameTime gameTime)
        {
            //frameCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            _guiSystem.Update(gameTime);

            _gui.Update();
            if (_graphics.IsFullScreen != _settings.IsFullScreen)
            {
                _graphics.IsFullScreen = _settings.IsFullScreen;
                _graphics.ApplyChanges();
            }

            if (!_settings.IsSoundOn)
            {
                musicSoundEffect.Volume = 0;
            }
            else
            {
                musicSoundEffect.Volume = defaultVolume;
            }

            if (!IsInGame)
                return;

            if (_settings.IsPaused)
                return;


            level?.Update(gameTime, new GameState() { Width = Width, Height = Height, IsDebug = _settings.IsDebugging, ViewPort = GraphicsDevice.Viewport.Bounds, IsSoundOn = _settings.IsSoundOn });


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
