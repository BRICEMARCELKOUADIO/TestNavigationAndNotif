using Kola.Infrastructure;
using Kola.Infrastructure.Extensions;
using Kola.Infrastructure.Helper;
using Kola.Infrastructure.Navigation;
using Kola.Starting.Models;
using Prism.Commands;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Kola.Starting.Dialogs
{
    public class PikerPopupViewModel : ViewModelBase
    {
        private readonly int _maxItems = Constants.ItemsCount_To_Show_Search_Input; 
        private string _id;
        private PikerItem _currentItem;

        public Field<string> CancelText { get; set; } = new Field<string>("cancel");

        private ObservableCollection<PikerItem> _items;
        public ObservableCollection<PikerItem> Items
        {
            get { return _items; }
            set
            {
                SetProperty(ref _items, value);
            }
        }

        private ObservableCollection<PikerItem> _allItems;

        public Field<string> Search { get; set; } = new Field<string>("", "", false);

        public new ICommand OnSelectedCommand => new DelegateCommand<string>(OnSelected);
        public new ICommand GotoCancelCommand => new DelegateCommand(GotoCancel);
        //public new ICommand OnSearchTappedCommand => new DelegateCommand<string>(SearchTapped);

        public PikerPopupViewModel(INavigationService navigationService) : base(navigationService)
        {
            _items = new ObservableCollection<PikerItem>();
            _allItems = new ObservableCollection<PikerItem>();
            _currentItem = new PikerItem();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            IsLoading = true;
            _id = parameters.GetValue<string>(Constants.PopupId);
            Title = parameters.GetValue<string>(Constants.PopupTitle);
            Items = _allItems = parameters.GetValue<ObservableCollection<PikerItem>>(Constants.PopupResquestData);
            Search.IsVisible = Items.Count > _maxItems;
            _currentItem = parameters.GetValue<PikerItem>(Constants.PopupCurrentData);
            if (_currentItem != null)
            {
                var item = Items.FirstOrDefault(x => x.Id == _currentItem.Id);
                if (item != null)
                {
                    item.IsSelected.Value = true;
                }
            }
            IsEmpty = Items.Count == 0;
            IsLoading = false;
        }

        private async void OnSelected(string id)
        {
            IsLoading = true;

            _currentItem = _items.FirstOrDefault(x => x.Id == id);
            if (_currentItem != null)
            {
                var parameters = new NavigationParameters
                {
                    { Constants.PopupId, _id },
                    {Constants.PopupResponseData, _currentItem},
                };
                await NavigationService.GoBackAsync(parameters);
            }

            IsLoading = false;
        }

        private async void GotoCancel()
        {
            await NavigationService.GoBackAsync();
        }

        public void SearchTapped(string value)
        {
            if (_allItems.Count <= _maxItems)
            {
                return;
            }

            var str = value.Replace(" ", "");
            if (string.IsNullOrEmpty(str))
            {
                Items = _allItems;
                return;
            }

            if (IsBusy || IsLoading)
            {
                // Message Tosat
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            var list = _allItems.Where(x => x.Name_1.ToLowerInvariant().Contains(value.ToLowerInvariant()));

            Items = (list == null || list?.Count() == 0) ? new ObservableCollection<PikerItem>() : list.ToObservableCollection();
            IsEmpty = Items.Count == 0;

            IsLoading = false;
        }
    }
}
