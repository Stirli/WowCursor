using NAudio.CoreAudioApi;
using System.Threading;
using System.Threading.Tasks;
using WowCursor.MVVM;

namespace WowCursor.ViewModels
{
    public class AudioDeviceViewModel : ViewModelBase
    {
        private readonly Task _updateTask;
        private readonly MMDevice _device;

        private int _masterPeak;


        public AudioDeviceViewModel(MMDevice device)
        {
            _device = device;
            FriendlyName = device.FriendlyName;
            _updateTask = Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(100);
                    MasterPeak =  (int)(_device.AudioMeterInformation.MasterPeakValue * 100);
                }
            });
        }

        public string FriendlyName { get; }
        public int Number { get;}
        public int MasterPeak { get => _masterPeak; set => Set(nameof(MasterPeak), ref _masterPeak, value); }
    }
}
