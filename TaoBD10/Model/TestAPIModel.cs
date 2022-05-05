using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class TestAPIModel
    {
        public int Index { get; set; }
        public string Text { get; set; }
        public string ClassName { get; set; }
        public IntPtr Handle { get; set; }
        public TestAPIModel()
        {

        }
        public TestAPIModel(int index,string text,string className,IntPtr handle)
        {
            this.Index = index;
            this.Text = text;
            this.ClassName = className;
            this.Handle = handle;
        }
    }
}
