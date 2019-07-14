using AutoMapper;
using CoffeeRun.Models;
using CoffeeRun.ViewModels;

namespace CoffeeRun.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Friend, FriendViewModel>();
            CreateMap<FriendViewModel, Friend>();

            CreateMap<Run, RunViewModel>();
            CreateMap<Run, DashboardViewModel>();
            CreateMap<RunViewModel, Run>();

            CreateMap<Order, OrderViewModel>();
            CreateMap<OrderViewModel, Order>();
        }
    }
}
