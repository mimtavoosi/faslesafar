using FasleSafar.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RefereeManager.Components
{
    public class SloganComponent : ViewComponent
    {
        private IContentRep _contentRep;

        public SloganComponent(IContentRep contentRep)
        {
            _contentRep = contentRep;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("/Views/Components/SloganComponent.cshtml", _contentRep.GetContentById(1030));
        }
    }
}
