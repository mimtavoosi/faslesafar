using FasleSafar.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RefereeManager.Components
{
    public class HeaderServicesComponent : ViewComponent
    {
        private IContentRep _contentRep;

        public HeaderServicesComponent(IContentRep contentRep)
        {
            _contentRep = contentRep;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("/Views/Components/HeaderServicesComponent.cshtml", _contentRep.GetServices());
        }
    }
}
