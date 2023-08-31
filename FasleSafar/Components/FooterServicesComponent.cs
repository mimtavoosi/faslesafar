using FasleSafar.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RefereeManager.Components
{
    public class FooterServicesComponent : ViewComponent
    {
        private IContentRep _contentRep;

        public FooterServicesComponent(IContentRep contentRep)
        {
            _contentRep = contentRep;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("/Views/Components/FooterServicesComponent.cshtml", _contentRep.GetServices());
        }
    }
}
