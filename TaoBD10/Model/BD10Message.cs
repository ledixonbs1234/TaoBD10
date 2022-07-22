using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System.Collections.Generic;

namespace TaoBD10.Model
{
    public class BD10Message : ValueChangedMessage<List<BD10InfoModel>>
    {
        public BD10Message(List<BD10InfoModel> list) : base(list)
        {
        }
    }

    public class TuiHangHoaMessage : ValueChangedMessage<List<HangHoaDetailModel>>
    {
        public TuiHangHoaMessage(List<HangHoaDetailModel> list) : base(list)
        {
        }
    }

    public class TamQuansMessage : ValueChangedMessage<List<TamQuanModel>>
    {
        public TamQuansMessage(List<TamQuanModel> list) : base(list)
        {
        }
    }

    public class HangTonMessage : ValueChangedMessage<List<HangTonModel>>
    {
        public HangTonMessage(List<HangTonModel> list) : base(list)
        {
        }
    }

    public class ContentMessage : ValueChangedMessage<ContentModel>
    {
        public ContentMessage(ContentModel content) : base(content)
        {
        }
    }

    public class ChiTietTuiMessage : ValueChangedMessage<ChiTietTuiInfo>
    {
        public ChiTietTuiMessage(ChiTietTuiInfo content) : base(content)
        {
        }
    }

    public class PNSNameMessage : ValueChangedMessage<List<PNSNameModel>>
    {
        public PNSNameMessage(List<PNSNameModel> content) : base(content)
        {
        }
    }

    public class SHTuiMessage : ValueChangedMessage<SHTuiCodeModel>
    {
        public SHTuiMessage(SHTuiCodeModel content) : base(content)
        {
        }
    }

    public class WebMessage : ValueChangedMessage<WebContentModel>
    {
        public WebMessage(WebContentModel content) : base(content)
        {
        }
    }

    public class KiemTraMessage : ValueChangedMessage<KiemTraModel>
    {
        public KiemTraMessage(KiemTraModel content) : base(content)
        {
        }
    }
}