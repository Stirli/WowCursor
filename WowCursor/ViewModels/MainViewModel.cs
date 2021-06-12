using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WowCursor.MVVM;

namespace WowCursor.ViewModels
{
    class MainViewModel:ViewModelBase
    {
        public SamplesBlocksViewModel SamplesData{ get; private set; }
        public MainViewModel()
        {
            SamplesData = new SamplesBlocksViewModel();
            StartRecodCommand = new RelayCommand(button1_Click, (_) => IsWriteReady);
            StopRecodCommand = new RelayCommand(button2_Click, (_) => !IsWriteReady);
            RecordDevices = new ObservableCollection<string>(EnumerateRecordDevices());
            IsWriteReady = true;
        }
        public ICommand StartRecodCommand { get; set; }
        public ICommand StopRecodCommand { get; set; }
        public bool IsWriteReady { get => isWriting; set => Set(nameof(IsWriteReady), ref isWriting, value); }

        public ObservableCollection<string> RecordDevices { get; private set; }
        public int SelectedRecordDiviceIndex { get; set; }

        private IEnumerable<string> EnumerateRecordDevices()
        {
            int waveInDevices = WaveIn.DeviceCount;
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                yield return $"Device {waveInDevice}: {deviceInfo.ProductName}, {deviceInfo.Channels} channels";
            }
        }

        // WaveIn - поток для записи
        WaveIn waveIn;
        //Класс для записи в файл
        WaveFileWriter writer;
        //Имя файла для записи
        string outputFilename = "имя_файла.wav";
        private bool isWriting;

        //Получение данных из входного буфера 
        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            //Записываем данные из буфера в файл
            writer.Write(e.Buffer, 0, e.BytesRecorded);
        }
        //Завершаем запись
        void StopRecording()
        {
            MessageBox.Show("StopRecording");
            waveIn.StopRecording();
            IsWriteReady = true;
        }
        //Окончание записи
        private void waveIn_RecordingStopped(object sender, EventArgs e)
        {
            {
                waveIn.Dispose();
                waveIn = null;
                writer.Close();
                writer = null;
            }
        }
        //Начинаем запись - обработчик нажатия кнопки
        private void button1_Click(object parameter)
        {
            try
            {
                MessageBox.Show("Start Recording");
                waveIn = new WaveIn();
                //Дефолтное устройство для записи (если оно имеется)
                //встроенный микрофон ноутбука имеет номер 0
                waveIn.DeviceNumber = SelectedRecordDiviceIndex;
                //Прикрепляем к событию DataAvailable обработчик, возникающий при наличии записываемых данных
                waveIn.DataAvailable += waveIn_DataAvailable;
                //Прикрепляем обработчик завершения записи
                waveIn.RecordingStopped += waveIn_RecordingStopped;
                //Формат wav-файла - принимает параметры - частоту дискретизации и количество каналов(здесь mono)
                waveIn.WaveFormat = new WaveFormat(8000, 1);
                //Инициализируем объект WaveFileWriter
                writer = new WaveFileWriter(outputFilename, waveIn.WaveFormat);
                //Начало записи
                waveIn.StartRecording();
                IsWriteReady = false;
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
        }
        //Прерываем запись - обработчик нажатия второй кнопки
        private void button2_Click(object parameter)
        {
            if (waveIn != null)
            {
                StopRecording();
            }
        }
    }
}
