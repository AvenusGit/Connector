using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.Connections
{
    public class EditModel : PageModel
    {
        public EditModel(Connection connection)
        {
            Connection = connection;
        }
        public Connection Connection { get; set; }
    }
}
