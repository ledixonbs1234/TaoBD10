using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class OptionModel:ObservableObject
    {
        private string _MaBuuCuc;

        public string MaBuuCuc
        {
            get { return _MaBuuCuc; }
            set { SetProperty(ref _MaBuuCuc, value); }
        }


        private string _GoFastKhoiTaoCT;

        public string GoFastKhoiTaoCT
        {
            get { return _GoFastKhoiTaoCT; }
            set { SetProperty(ref _GoFastKhoiTaoCT, value); }
        }


        private string _GoFastBD10Di;

        public string GoFastBD10Di
        {
            get { return _GoFastBD10Di; }
            set { SetProperty(ref _GoFastBD10Di, value); }
        }


        private string _GoFastBD10Den;

        public string GoFastBD10Den
        {
            get { return _GoFastBD10Den; }
            set { SetProperty(ref _GoFastBD10Den, value); }
        }


        private string _NameBCMPS1;

        public string NameBCMPS1
        {
            get { return _NameBCMPS1; }
            set { SetProperty(ref _NameBCMPS1, value); }
        }


        private string _CodeBCMPS1;

        public string CodeBCMPS1
        {
            get { return _CodeBCMPS1; }
            set { SetProperty(ref _CodeBCMPS1, value); }
        }
        private string _StateBCMPS1;

        public string StateBCMPS1
        {
            get { return _StateBCMPS1; }
            set { SetProperty(ref _StateBCMPS1, value); }
        }



        private string _NameBCMPS2;

        public string NameBCMPS2
        {
            get { return _NameBCMPS2; }
            set { SetProperty(ref _NameBCMPS2, value); }
        }


        private string _CodeBCMPS2;

        public string CodeBCMPS2
        {
            get { return _CodeBCMPS2; }
            set { SetProperty(ref _CodeBCMPS2, value); }
        }


        private string _StateBCMPS2;

        public string StateBCMPS2
        {
            get { return _StateBCMPS2; }
            set { SetProperty(ref _StateBCMPS2, value); }
        }


        private string _MaBuuCucXN;

        public string MaBuuCucXN
        {
            get { return _MaBuuCucXN; }
            set { SetProperty(ref _MaBuuCucXN, value); }
        }


        private string _AcountDinhVi;

        public string AcountDinhVi
        {
            get { return _AcountDinhVi; }
            set { SetProperty(ref _AcountDinhVi, value); }
        }


        private string _PasswordDinhVi;

        public string PasswordDinhVi
        {
            get { return _PasswordDinhVi; }
            set { SetProperty(ref _PasswordDinhVi, value); }
        }


        private string _AcountMPS;

        public string AcountMPS
        {
            get { return _AcountMPS; }
            set { SetProperty(ref _AcountMPS, value); }
        }


        private string _PasswordMPS;

        public string PasswordMPS
        {
            get { return _PasswordMPS; }
            set { SetProperty(ref _PasswordMPS, value); }
        }


        private string _AcountPNS;

        public string AcountPNS
        {
            get { return _AcountPNS; }
            set { SetProperty(ref _AcountPNS, value); }
        }


        private string _PasswordPNS;

        public string PasswordPNS
        {
            get { return _PasswordPNS; }
            set { SetProperty(ref _PasswordPNS, value); }
        }


        public OptionModel()
        {

        }

    }
}
