using CommunityToolkit.Mvvm.ComponentModel;

namespace TaoBD10.Model
{
    public class WebContentModel : ObservableObject
    {
        public string Key { get; set; }
        public string Code { get; set; }
        public string AddressSend { get; set; }
        public string AddressReiceive { get; set; }
        public string BuuCucPhat { get; set; }
        public string BuuCucGui { get; set; }
        public string MaBuuCuc { get; set; }
        public string NguoiGui { get; set; }
        public KiemTraModel KiemTraWeb { get; set; }
    }
}