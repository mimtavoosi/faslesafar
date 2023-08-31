using FasleSafar.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RefereeManager.Components
{
    public class HeaderMenuComponent : ViewComponent
    {
        private IContentRep _contentRep;

        public HeaderMenuComponent(IContentRep contentRep)
        {
            _contentRep = contentRep;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("/Views/Components/HeaderMenuComponent.cshtml", _contentRep.GetContentsForMenu());
        }
    }
}
