using FasleSafar.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RefereeManager.Components
{
    public class FooterSiteInfoComponent : ViewComponent
    {
        private IContentRep _contentRep;

        public FooterSiteInfoComponent(IContentRep contentRep)
        {
            _contentRep = contentRep;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("/Views/Components/FooterSiteInfoComponent.cshtml", _contentRep.GetSiteInfo());
        }
    }
}
