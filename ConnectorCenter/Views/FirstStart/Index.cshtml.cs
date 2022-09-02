using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConnectorCore.Models;
using ConnectorCenter.Data;

namespace ConnectorCenter.Views.FirstStart
{
    public class IndexViewModel : PageModel
    {
        public IndexViewModel(DataBaseContext context)
        {
            IsFirstStart = !context.Users.Any();
        }
        public IndexViewModel(bool isFirstStart)
        {
            IsFirstStart = isFirstStart;
        }

        public bool IsFirstStart { get; set; }
    }
}
