using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConnectorCore.Models;

namespace ConnectorCenter.Views.FirstStart
{
    public class FirstStartViewModel : PageModel
    {
        public FirstStartViewModel(bool isFirstStart)
        {
            IsFirstStart = isFirstStart;
        }

        public bool IsFirstStart { get; set; }
        public AppUser FirstAppUser { get; set; }
    }
}
