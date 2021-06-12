using NAudio.Wave;
using System;
using System.Windows;
using System.Windows.Input;
using WowCursor.MVVM;

namespace WowCursor.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public SamplesBlocksViewModel SamplesData { get; private set; }
        public RecordDevicesListViewModel RecordDevices { get; private set; }
        public MainViewModel()
        {
            SamplesData = new SamplesBlocksViewModel();
            RecordDevices = new RecordDevicesListViewModel();
            RecordDevices.ListChanged += (_, e) =>
            Application.Current.Dispatcher.Invoke(() => CommandManager.InvalidateRequerySuggested());
            StartRecodCommand = new RelayCommand(button1_Click, (_) => IsWriteReady && RecordDevices.IsListReady);
            StopRecodCommand = new RelayCommand(button2_Click, (_) => !IsWriteReady);
            IsWriteReady = true;
        }
        public RelayCommand StartRecodCommand { get; set; }
        public RelayCommand StopRecodCommand { get; set; }
        public bool IsWriteReady { get => isWriting; set => Set(nameof(IsWriteReady), ref isWriting, value); }

        public int SelectedRecordDiviceIndex { get; set; }



        // WaveIn - поток для записи
        private WaveIn waveIn;

        //Класс для записи в файл
        private WaveFileWriter writer;

        //Имя файла для записи
        private string outputFilename = "имя_файла.wav";
        private bool isWriting;

        //Получение данных из входного буфера 
        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            //Записываем данные из буфера в файл
            writer.Write(e.Buffer, 0, e.BytesRecorded);
        }

        //Завершаем запись
        private void StopRecording()
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
