using FasleSafar.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RefereeManager.Components
{
    public class WhatsAppIconComponent : ViewComponent
    {
        private IContentRep _contentRep;

        public WhatsAppIconComponent(IContentRep contentRep)
        {
            _contentRep = contentRep;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("/Views/Components/WhatsAppIconComponent.cshtml", _contentRep.GetContentById(16));
        }
    }
}
