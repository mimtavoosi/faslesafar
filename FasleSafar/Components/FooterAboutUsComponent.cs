using FasleSafar.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RefereeManager.Components
{
    public class FooterAboutUsComponent : ViewComponent
    {
        private IContentRep _contentRep;

        public FooterAboutUsComponent(IContentRep contentRep)
        {
            _contentRep = contentRep;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("/Views/Components/FooterAboutUsComponent.cshtml", _contentRep.GetAllContents().SingleOrDefault(c=> c.ContentTitle == "درباره ما"));
        }
    }
}
