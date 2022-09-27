namespace TaoBD10.Model
{
    public class TuiThuModel
    {
        public string Ma { get; set; }

        public TuiThuModel(string ma, string content)
        {
            Content = content;
            Ma = ma;
        }

        public string Content { get; set; }
    }
}