using HuntLog.Services;

namespace HuntLog.Factories
{
    public interface IHunterFactory
    {

    }

    public class HunterFactory : IHunterFactory
    {
        private readonly IHunterService _hunterService;
        private readonly INavigator _navigator;

        public HunterFactory(IHunterService hunterService, INavigator navigator)
        {
            _hunterService = hunterService;
            _navigator = navigator;
        }
    }
}
