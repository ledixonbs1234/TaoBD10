using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace TaoBD10.Model
{
    public class TestAPIModel : ObservableObject
    {
        private int _Index;

        public int Index
        {
            get { return _Index; }
            set { SetProperty(ref _Index, value); }
        }
        private string _Text;

        public string Text
        {
            get { return _Text; }
            set { SetProperty(ref _Text, value); }
        }

        private string _ClassName;

        public string ClassName
        {
            get { return _ClassName; }
            set { SetProperty(ref _ClassName, value); }
        }


        private IntPtr _Handle;

        public IntPtr Handle
        {
            get { return _Handle; }
            set { SetProperty(ref _Handle, value); }
        }


        public TestAPIModel()
        {
        }

        public TestAPIModel(int index, string text, string className, IntPtr handle)
        {
            this.Index = index;
            this.Text = text;
            this.ClassName = className;
            this.Handle = handle;
        }
    }
}