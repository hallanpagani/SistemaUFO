using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Web;
using System.Web.Security;

namespace BaseModelo.model.generico
{
    public partial class BaseID : IBase
    {
        [Required]
        [Column("id_conta")]
        public long IdConta { get; set; }

        public BaseID()
        {
            var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
            try
            {
                IdConta =  Convert.ToInt64(claimsIdentity.FindFirst(ClaimTypes.GroupSid).Value ?? "0");
            }
            catch (Exception e)
            {
                IdConta = 0;
            }
        }

    }


}
