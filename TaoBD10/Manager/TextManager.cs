using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Manager
{
    public static class TextManager
    {
        public const string SCRIPT_LOGIN = @"
Element.prototype.remove = function() {
    this.parentElement.removeChild(this);
}
NodeList.prototype.remove = HTMLCollection.prototype.remove = function() {
    for(var i = this.length - 1; i >= 0; i--) {
        if(this[i] && this[i].parentElement) {
            this[i].parentElement.removeChild(this[i]);
        }
    }
}
document.getElementById(""vnpNav"").remove();
document.getElementsByClassName("".mainHomepageTitle"").remove();
document.getElementsByClassName("".mainHomepageUserLogin"").remove();
document.getElementsByClassName("".footer"").remove();

                                    var textbox = document.getElementById(""MainContent_imgCaptcha"");
    textbox.focus();
                    textbox.scrollIntoView();
                    ";

    }
}
