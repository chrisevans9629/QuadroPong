using System;
using Akavache;

namespace MyGame
{
    public class Settings : ISettings
    {
        private IBlobCache blobCache;
        private bool _isSoundOn;
        private bool _isDebugging;

        public Settings()
        {
            blobCache = BlobCache.LocalMachine;

            blobCache.GetOrCreateObject(nameof(IsDebugging), () => false).Subscribe(b => _isDebugging = b);
            blobCache.GetOrCreateObject(nameof(IsSoundOn), () => false).Subscribe(b => _isSoundOn = b);
        }
        public bool IsPaused { get; set; }

        public bool IsDebugging
        {
            get => _isDebugging;
            set
            {
                if(_isDebugging == value)
                    return;
                _isDebugging = value;
                blobCache.InsertObject(nameof(IsDebugging), value);
            }
        }

        public bool IsSoundOn
        {
            get => _isSoundOn;
            set
            {
                if(_isSoundOn == value)
                    return;
                _isSoundOn = value;
                blobCache.InsertObject(nameof(IsSoundOn), value);
            }
        }
    }
}