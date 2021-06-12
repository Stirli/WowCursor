using WowCursor.MVVM;

namespace WowCursor.ViewModels
{
    class SamplesBlocksViewModel : ViewModelBase
    {
        private int hight = 60;
        private int middle = 30;
        private int low = 80;

        public int Hight { get => hight; set => Set(nameof(Hight), ref hight, value); }
        public int Middle { get => middle; set => Set(nameof(Middle), ref middle, value); }
        public int Low { get => low; set => Set(nameof(Low), ref low, value); }
        
    }
}
