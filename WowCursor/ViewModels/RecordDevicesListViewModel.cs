using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WowCursor.MVVM;

namespace WowCursor.ViewModels
{
    public class RecordDevicesListViewModel : ViewModelBase
    {

        private IEnumerable<AudioDeviceViewModel> _devices;
        private bool _isListReady;

        public RecordDevicesListViewModel()
        {
            UpdateCommand = new RelayCommand((_) => BeginUpdateDevicesList(), (_) => _isListReady);
            BeginUpdateDevicesList();
        }


        public event EventHandler ListChanged;

        public IEnumerable<AudioDeviceViewModel> Devices => _devices;
        public bool IsListReady { get => _isListReady; set => Set(nameof(IsListReady), ref _isListReady, value); }
        public RelayCommand UpdateCommand { get; set; }

        private void BeginUpdateDevicesList()
        {
            _isListReady = false;
            int waveInDevices = WaveIn.DeviceCount;
            _ = Task.Run(() =>
              {
                  using (MMDeviceEnumerator enumerator = new MMDeviceEnumerator())
                  {
                      MMDeviceCollection devList = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
                      _devices = devList.Select(d => new AudioDeviceViewModel(d)).ToList();
                  }

                  _isListReady = true;
                  OnPropertyChanged(nameof(Devices));
                  OnPropertyChanged(nameof(IsListReady));
                  ListChanged?.Invoke(this, new EventArgs());
              });
        }
    }
}
