using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConnectorCore.Models.VisualModels.Interfaces;
using md=ConnectorCore.Models.VisualModels;

namespace ConnectorCenter.Views.Js
{
    public class JsModel : PageModel
    {
        public JsModel(md.VisualScheme scheme)
        {
            Scheme = scheme ?? md.VisualScheme.GetDefaultVisualScheme();
        }

        public md.VisualScheme Scheme { get; set; }
    }
}
