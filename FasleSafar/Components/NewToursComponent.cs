using FasleSafar.Data.Repositories;
using FasleSafar.Models;
using FasleSafar.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RefereeManager.Components
{
    public class NewToursComponent : ViewComponent
    {
        private ITourRep _tourRep;

        public NewToursComponent(ITourRep tourRep)
        {
            _tourRep = tourRep;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<TourVM> tourVMs = _tourRep.GetOpenTours().Take(3).ToList().Take(3).ToList().Select(t => new TourVM()
            {
                AvgScore = t.AvgScore,
                Capacity = t.Capacity,
                DaysCount = t.DaysCount,
                DestinationId = t.DestinationId,
                EndDate = t.EndDate,
                OpenState = t.OpenState,
                Price = _tourRep.GetFirstPriceOfTour(t.TourId),
                ScoreCount = t.ScoreCount,
                StartDate = t.StartDate,
                TotalScore = t.TotalScore,
                TourId = t.TourId,
                TourTitle = t.TourTitle,
                TourDescription = t.TourDescription,
                TourType = t.TourType,
                TransportType = t.TransportType,
                Destination = t.Destination,
                Vehicle = t.Vehicle,
                BigImage = t.BigImage,
                SmallImage = t.SmallImage,
                Attractions = t.Attractions,
                ExcludeCosts = t.ExcludeCosts,
                Facilities = t.Facilities,
                ImagesAlbum = t.ImagesAlbum,
                IncludeCosts = t.IncludeCosts,
                ReachTime = t.ReachTime,
                ReturnTime = t.ReturnTime,
                GeoCoordinates = t.GeoCoordinates,
                FrameCase = 1,
            }).ToList();
            return View("/Views/Components/NewToursComponent.cshtml", tourVMs);
        }
    }
}
