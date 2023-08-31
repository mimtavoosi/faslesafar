using FasleSafar.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RefereeManager.Components
{
    public class FooterMenuComponent : ViewComponent
    {
        private IContentRep _contentRep;

        public FooterMenuComponent(IContentRep contentRep)
        {
            _contentRep = contentRep;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("/Views/Components/FooterMenuComponent.cshtml", _contentRep.GetContentsForMenu());
        }
    }
}
