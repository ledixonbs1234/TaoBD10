using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class OptionAllViewModel : ObservableObject
    {
        private OptionModel _Option;

        public OptionModel Option
        {
            get { return _Option; }
            set { SetProperty(ref _Option, value); }
        }
        private ObservableCollection<OptionInfoModel> _Options;

        public ObservableCollection<OptionInfoModel> Options
        {
            get { return _Options; }
            set { SetProperty(ref _Options, value); }
        }
        public ICommand SaveCommand { get; }

        void Save()
        {
            Option.AccountDinhVi = Options[0].Content1;
            Option.PWDinhVi = Options[0].Content2;
            Option.AccountMPS = Options[1].Content1;
            Option.PWMPS = Options[1].Content2;
            Option.AccountPNS = Options[2].Content1;
            Option.PWPNS = Options[2].Content2;
            Option.MaKhaiThac = Options[3].Content1;
            Option.MaBuuCucPhat = Options[4].Content1;
            Option.CodeBCMPSKT = Options[5].Content1;
            Option.StateBCMPSKT = Options[6].Content1;
            Option.CodeBCMPSBCP = Options[7].Content1;
            Option.StateBCMPSBCP = Options[8].Content1;
            Option.GoFastBD10Di = Options[9].Content1;
            Option.GoFastBD10Den = Options[10].Content1;
            Option.GoFastKTCTKT = Options[11].Content1;
            Option.GoFastKTCTBCP = Options[12].Content1;
            Option.GoFastQLCTCDKT = Options[13].Content1;
            Option.GoFastQLCTCDBCP = Options[14].Content1;

            FileManager.SaveOptionOffline(Option);
        }

        public ICommand LayDuLieuCommand { get; }

        

        void LayDuLieu()
        {
           Option = FileManager.GetOptionAll();
        }

        public ICommand PublishCloudCommand { get; }

     

        void PublishCloud()
        {
            FileManager.SaveOptionAll(Option);
        }






        public OptionAllViewModel()
        {
            PublishCloudCommand = new RelayCommand(PublishCloud);
            LayDuLieuCommand = new RelayCommand(LayDuLieu); 
            Option = new OptionModel();
            GetOptionData();
            Options = new ObservableCollection<OptionInfoModel>();
            Options.Add(new OptionInfoModel("Tài khoản định vị", Option.AccountDinhVi, Option.PWDinhVi));
            Options.Add(new OptionInfoModel("Tài khoản MPS", Option.AccountMPS, Option.PWMPS));
            Options.Add(new OptionInfoModel("Tài khoản PNS", Option.AccountPNS, Option.PWPNS));
            Options.Add(new OptionInfoModel("Mã Khai Thác", Option.MaKhaiThac));
            Options.Add(new OptionInfoModel("Mã Bưu Cục Phát", Option.MaBuuCucPhat));
            Options.Add(new OptionInfoModel("Mã MPS KT", Option.CodeBCMPSKT));
            Options.Add(new OptionInfoModel("Mã Trạng Thái MPS KT", Option.StateBCMPSKT));
            Options.Add(new OptionInfoModel("Mã MPS BCP", Option.CodeBCMPSBCP));
            Options.Add(new OptionInfoModel("Mã Trạng Thái MPS BCP", Option.CodeBCMPSBCP));
            Options.Add(new OptionInfoModel("BĐ 10 Đi", Option.GoFastBD10Di));
            Options.Add(new OptionInfoModel("BĐ 10 Đến", Option.GoFastBD10Den));
            Options.Add(new OptionInfoModel("Khởi Tạo Chuyến Thư KT", Option.GoFastKTCTKT));
            Options.Add(new OptionInfoModel("Khởi Tạo Chuyến Thư BCP", Option.GoFastKTCTBCP));
            Options.Add(new OptionInfoModel("Quản lý chuyến thư chiều đến KT", Option.GoFastQLCTCDKT));
            Options.Add(new OptionInfoModel("Quản lý chuyến thư chiều đến BCP", Option.GoFastQLCTCDBCP));

            SaveCommand = new RelayCommand(Save);

        }

        private void GetOptionData()
        {
            Option = FileManager.GetOptionOffline();

        }
    }
}
