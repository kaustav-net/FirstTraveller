using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace First_Traveller.OtherClass
{
    class ShareText
    {
        private string sharedText;

        public ShareText()
        {
            formatSharedText();
        }

        private void formatSharedText()
        {
            sharedText = "Check out \"First Traveller\" for Windows Phone http://goo.gl/Gqc9al";
        }

        public string GetSharedText()
        {
            return sharedText;
        }
    }
}
