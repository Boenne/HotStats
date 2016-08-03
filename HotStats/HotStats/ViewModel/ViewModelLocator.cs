using HotStats.ViewModels.Interfaces;

namespace HotStats.ViewModel
{
    public class ViewModelLocator
    {
        public IMainViewModel Main => IoCContainer.Resolve<IMainViewModel>();
    }
}