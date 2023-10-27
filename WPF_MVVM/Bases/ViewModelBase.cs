using CommunityToolkit.Mvvm.ComponentModel;
using WPF_MVVM.Interfaces;

namespace WPF_MVVM.Bases
{
    internal class ViewModelBase : ObservableObject, INavigationAware, IViewModelBase
    {
        public virtual void OnNavigated(object? sender, object? e)
        {
        }
        public virtual void OnNavigating(object? sender, object? e)
        {
        }
    }
}
