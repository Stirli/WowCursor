using NAudio.Wave;
using System;
using System.Windows;
using System.Windows.Input;
using WowCursor.MVVM;

namespace WowCursor.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private AudioDeviceViewModel _selectedDivice;
        private bool _isWriting;
        public MainViewModel()
        {
            SamplesData = new SamplesBlocksViewModel();
            RecordDevices = new RecordDevicesListViewModel();
            RecordDevices.ListChanged += (_, e) =>
            {
                Application.Current.Dispatcher.Invoke(() => CommandManager.InvalidateRequerySuggested());
            };
            StartRecodCommand = new RelayCommand(_ => StartRecording(), _ => IsWriteReady && RecordDevices.IsListReady);
            StopRecodCommand = new RelayCommand(_ => StopRecording(), _ => !IsWriteReady);
            IsWriteReady = true;
        }
        public SamplesBlocksViewModel SamplesData { get; private set; }
        public RecordDevicesListViewModel RecordDevices { get; private set; }

        public string Message { get; set; }
        public bool IsWriteReady { get => _isWriting; set => Set(nameof(IsWriteReady), ref _isWriting, value); }
        public AudioDeviceViewModel SelectedDevice { get => _selectedDivice; set => Set(nameof(SelectedDevice), ref _selectedDivice, value); }
        public RelayCommand StartRecodCommand { get; set; }
        public RelayCommand StopRecodCommand { get; set; }


        // WaveIn - поток для записи
        private WaveIn _waveIn;

        //Класс для записи в файл
        private WaveFileWriter _writer;

        //Имя файла для записи
        private string _outputFilename = "имя_файла.wav";

        //Получение данных из входного буфера
        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            //Записываем данные из буфера в файл
            _writer.Write(e.Buffer, 0, e.BytesRecorded);
        }

        //Завершаем запись
        private void StopRecording()
        {
            _waveIn.StopRecording();
            IsWriteReady = true;
        }

        //Окончание записи
        private void WaveIn_RecordingStopped(object sender, EventArgs e)
        {
            _waveIn.Dispose();
            _waveIn = null;
            _writer.Close();
            _writer = null;
        }

        private void StartRecording()
        {
            try
            {
                _waveIn = new WaveIn
                {
                    //Дефолтное устройство для записи (если оно имеется)
                    DeviceNumber = SelectedDevice.Number
                };
                //Прикрепляем к событию DataAvailable обработчик, возникающий при наличии записываемых данных
                _waveIn.DataAvailable += WaveIn_DataAvailable;
                //Прикрепляем обработчик завершения записи
                _waveIn.RecordingStopped += WaveIn_RecordingStopped;
                //Формат wav-файла - принимает параметры - частоту дискретизации и количество каналов(здесь mono)
                _waveIn.WaveFormat = new WaveFormat(8000, 1);
                //Инициализируем объект WaveFileWriter
                _writer = new WaveFileWriter(_outputFilename, _waveIn.WaveFormat);
                //Начало записи
                _waveIn.StartRecording();
                IsWriteReady = false;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }
    }
}
