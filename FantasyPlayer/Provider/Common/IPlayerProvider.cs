using System.Threading.Tasks;
using FantasyPlayer.Interface;
using FantasyPlayer.Interfaces;

namespace FantasyPlayer.Provider.Common
{
    public interface IPlayerProvider
    {
        public PlayerStateStruct PlayerState { get; set; }

        public Task<IPlayerProvider> Initialize(IPlugin plugin);
        public void Update();
        public void Dispose();

        public void StartAuth();
        public void RetryAuth();
        public void ReAuth();

        public void SwapRepeatState();
        public void SetPauseOrPlay(bool play);
        public void SetSkip(bool forward);
        public void SetShuffle(bool value);
        public void SetVolume(int volume);
    }
}