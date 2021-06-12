using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WowCursor.MVVM;

namespace WowCursor.ViewModels
{
    public class RecordDevicesListViewModel : ViewModelBase
    {

        public bool IsListReady => !updating;

        private List<string> devices;
        private bool updating;

        public event EventHandler ListChanged;
        public RecordDevicesListViewModel()
        {
            UpdateRecordDevicesAsync();
        }
        public IEnumerable<string> Devices => devices;
        private void UpdateRecordDevicesAsync()
        {
            updating = true;
            int waveInDevices = WaveIn.DeviceCount;
            _ = Task.Run(() =>
              {
                  var list = new List<string>(waveInDevices);
                  for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
                  {
                      WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                      Thread.Sleep(1000);
                      list.Add($"Device {waveInDevice}: {deviceInfo.ProductName}, {deviceInfo.Channels} channels");
                  }

                  devices = list;
                  updating = false;
                  OnPropertyChanged(nameof(Devices));
                  OnPropertyChanged(nameof(IsListReady));
                  ListChanged?.Invoke(this, new EventArgs());
              });
        }
    }
}
