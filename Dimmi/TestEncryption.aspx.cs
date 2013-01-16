using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using Keyczar;
using Dimmi.Encryption;
using System.Collections;

namespace Dimmi
{
    public partial class TestEncryption : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Label1.Text = Decrypt(Encrypter());

            string[] encryptedstuff = Crypto.Encrypter("091-73938623482736428:cmduden@gmail.com");

            Label1.Text = Crypto.Decrypt(encryptedstuff).ToString();
        }

    }
}