using FasleSafar.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RefereeManager.Components
{
    public class HeaderSiteInfoComponent : ViewComponent
    {
        private IContentRep _contentRep;

        public HeaderSiteInfoComponent(IContentRep contentRep)
        {
            _contentRep = contentRep;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("/Views/Components/HeaderSiteInfoComponent.cshtml", _contentRep.GetSiteInfo());
        }
    }
}
