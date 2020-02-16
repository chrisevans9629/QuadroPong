using System;
using System.Runtime.CompilerServices;
using Akavache;

namespace MyGame
{
    public class Settings : ISettings
    {
        private IBlobCache blobCache;
        private bool _isSoundOn;
        private bool _isDebugging;
        private bool _hasAstroids;
        private float _masterVolume;
        private bool _isFullScreen;

        public Settings()
        {
            blobCache = BlobCache.LocalMachine;

            blobCache.GetOrCreateObject(nameof(IsDebugging), () => false).Subscribe(b => _isDebugging = b);
            blobCache.GetOrCreateObject(nameof(IsSoundOn), () => false).Subscribe(b => _isSoundOn = b);
            blobCache.GetOrCreateObject(nameof(HasAstroids), () => true).Subscribe(b => _hasAstroids = b);
            blobCache.GetOrCreateObject(nameof(MasterVolume), () => 10f).Subscribe(b => _masterVolume = b);
            blobCache.GetOrCreateObject(nameof(IsFullScreen), () => false).Subscribe(b => _isFullScreen = b);
        }
        public bool IsPaused { get; set; }

        
        public bool IsDebugging
        {
            get => _isDebugging;
            set => Set(ref _isDebugging, value);
        }

        public bool IsSoundOn
        {
            get => _isSoundOn;
            set => Set(ref _isSoundOn, value);
        }

        public bool HasAstroids
        {
            get => _hasAstroids;
            set => Set(ref _hasAstroids, value);
        }

        public float MasterVolume
        {
            get => _masterVolume;
            set => Set(ref _masterVolume, value);
        }

        public bool IsFullScreen
        {
            get => _isFullScreen;
            set => Set(ref _isFullScreen,value);
        }

        void Set<T>(ref T prop, T value, [CallerMemberName] string? property = null)
        {
            if (prop?.Equals(value) == true)
                return;
            prop = value;
            blobCache.InsertObject(property, value);
        }
    }
}