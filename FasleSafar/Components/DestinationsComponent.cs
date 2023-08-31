using FasleSafar.Data.Repositories;
using FasleSafar.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RefereeManager.Components
{
    public class DestinationsComponent : ViewComponent
    {
        private IDestinationRep _destinationRep;
        private ITourRep _tourRep;

        public DestinationsComponent(IDestinationRep destinationRep, ITourRep tourRep)
        {
            _destinationRep = destinationRep;
            _tourRep = tourRep;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<DestinationVM> destinations = _destinationRep.GetAllDestinations().Select(d => new DestinationVM()
            {
                DestinationDescription= d.DestinationDescription,
                DestinationName= d.DestinationName,
                DestinationId= d.DestinationId,
                ToursCount = _tourRep.GetToursOfDestination(d.DestinationId).Where(t=> t.OpenState == "فعال").Count(),
                BigImage = d.BigImage,
                City = d.City,
                Country = d.Country,
                Province = d.Province,
                GeoCoordinates = d.GeoCoordinates,
                ImagesAlbum = d.ImagesAlbum,
                IsAttraction = d.IsAttraction,
                OnVitrin = d.OnVitrin
            }).Where(t => t.ToursCount > 0 && t.OnVitrin).ToList();
            return View("/Views/Components/DestinationsComponent.cshtml", destinations);
        }
    }
}
