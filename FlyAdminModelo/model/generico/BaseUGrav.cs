using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Web;
using System.Web.Security;

namespace BaseModelo.model.generico
{
    public partial class BaseUGrav : BaseID
    {
        [Display(Name = "Usuário")]
        [Column("ds_cliente")]
        public string Cliente { get; set; }

        public BaseUGrav()
        {
            FormsIdentity ident = HttpContext.Current.User.Identity as FormsIdentity;
            if (ident != null)
            {
                FormsAuthenticationTicket ticket = ident.Ticket;
                string userDataString = ticket.UserData;
                // Split on the |
                string[] userDataPieces = userDataString.Split("|".ToCharArray());
                Cliente = userDataPieces[0];
            }

        }

    }


}
