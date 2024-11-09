using ModelTrain.Model.Pieces;
using System.Collections.ObjectModel;

namespace ModelTrain.Model
{
    public interface ILocalDatabase
    {
        public ObservableCollection<PieceBase> Hotbar { get; set; }
        
        public bool SaveToFile();
        public bool LoadFromFile();
        public bool SetUser(string email);

        public T GetSetting<T>(string setting);
        public void SetSetting<T>(string setting, T value);
    }
}
