using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConnectorCore.Models.VisualModels.Interfaces;
using md=ConnectorCore.Models.VisualModels;

namespace ConnectorCenter.Views.VisualScheme
{
    public class GenerateCssModel : PageModel
    {
        public GenerateCssModel(md.VisualScheme scheme)
        {
            Scheme = scheme ?? md.VisualScheme.GetDefaultVisualScheme();
        }

        public md.VisualScheme Scheme { get; set; }
    }
}
