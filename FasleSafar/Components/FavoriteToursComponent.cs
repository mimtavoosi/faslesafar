using FasleSafar.Data.Repositories;
using FasleSafar.Models;
using FasleSafar.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RefereeManager.Components
{
    public class FavoriteToursComponent : ViewComponent
    {
        private ITourRep _tourRep;

        public FavoriteToursComponent(ITourRep tourRep)
        {
            _tourRep = tourRep;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<TourVM> tourVMs = _tourRep.GetFavoriteTours().Take(3).ToList().Select(t => new TourVM()
            {
                AvgScore = t.AvgScore,
                Capacity = t.Capacity,
                DaysCount = t.DaysCount,
                DestinationId = t.DestinationId,
                EndDate = t.EndDate,
                OpenState = t.OpenState,
                Price = t.Price.Value.FixPrice(),
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
                ExcludeCosts = t.ExcludeCosts,
                Facilities = t.Facilities,
                ImagesAlbum = t.ImagesAlbum,
                IncludeCosts = t.IncludeCosts,
                ReachTime = t.ReachTime,
                ReturnTime = t.ReturnTime,
                Attractions = t.Attractions,
                GeoCoordinates = t.GeoCoordinates,
            }).ToList();
            return View("/Views/Components/FavoriteToursComponent.cshtml", tourVMs);
        }
    }
}
