using Kola.Infrastructure;
using Kola.Infrastructure.EventAggregator;
using Kola.Infrastructure.Extensions;
using Kola.Infrastructure.Helper;
using Kola.Infrastructure.Hepler;
using Kola.Infrastructure.Navigation;
using Kola.Infrastructure.Setting.Contract;
using Kola.Infrastructure.UserAuthentication.Contracts;
using Kola.Starting.Models;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using reewire.core.services;
using reewire.core.services.contracts.application;
using reewire.core.services.contracts.catalog;
using reewire.core.services.contracts.platform;
using reewire.core.services.contracts.services;
using reewire.core.services.models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Kola.Starting.ViewModels
{
    public class SiginUpViewModel : ViewModelBase
    {
        private readonly IGenders _genderservice;
        private readonly IIdentificationTypes _idservice;
        private readonly IDocumentTypes _doctypeservice;
        private readonly ICountries _countriesService;
        private readonly IFeature _customerservice;
        private readonly IAppInfo _appInfo;
        private IApplications _applicationsService;

        private readonly IAuthentication _authentication;


        #region View properties
        //Step 2
        public Field<string> Gender { get; set; } = new Field<string>();
        public Field<string> FirstName { get; set; } = new Field<string>();
        public Field<string> LastName { get; set; } = new Field<string>();
        public Field<string> Email { get; set; } = new Field<string>();
        //Step 3
        public Field<string> Country { get; set; } = new Field<string>(Constants.CountryId.ToString(), Constants.CountryName, true);
        public Field<string> CountryIdentityDocument { get; set; } = new Field<string>(Constants.CountryId.ToString(), Constants.CountryName, true);
        public Field<string> IdentityDocumentType { get; set; } = new Field<string>();
        public Field<string> IdentityDocumentNumber { get; set; } = new Field<string>();
        public Field<DateTime> StartingDateIdentityDocument { get; set; } = new Field<DateTime>(DateTime.Now);
        public Field<DateTime> ExpireDateIdentityDocument { get; set; } = new Field<DateTime>(DateTime.Now);
        //Step 4
        public Field<string> Counties { get; set; } = new Field<string>();
        public Field<string> District { get; set; } = new Field<string>();
        public Field<string> CitiesAndTowns { get; set; } = new Field<string>();
        public Field<string> Communities { get; set; } = new Field<string>();
        //Step 5
        public Field<string> Address { get; set; } = new Field<string>();
        public Field<string> PhoneNumber { get; set; } = new Field<string>();
        public Field<string> SecurityQuestion { get; set; } = new Field<string>();
        public Field<string> Answer { get; set; } = new Field<string>();
        public Field<string> Password { get; set; } = new Field<string>();
        public Field<string> ConfirmPassword { get; set; } = new Field<string>();
        public Field<int> PasswordMaxLength { get; set; } = new Field<int>(Constants.MaxLenthPinCode);
        public Field<int> PhoneNumberMaxLength { get; set; } = new Field<int>(Constants.MaxLengthPhoneNumber);
        #endregion

        public Field<int> TotalStep { get; set; } = new Field<int>(6);
        public Field<int> CurrentStep { get; set; } = new Field<int>(1);

        public Field<bool> IsTakePicture { get; set; } = new Field<bool>(false);

        public Field<bool> FirstStep { get; set; } = new Field<bool>(true);

        public Field<bool> SecondStep { get; set; } = new Field<bool>(false);

        public Field<bool> ThirdStep { get; set; } = new Field<bool>(false);

        public Field<bool> FourthStep { get; set; } = new Field<bool>(false);

        public Field<bool> FifthStep { get; set; } = new Field<bool>(false);

        public Field<bool> SixthStep { get; set; } = new Field<bool>(false);

        public Field<string> PreviousText { get; set; } = new Field<string>();
        public Field<string> NextText { get; set; } = new Field<string>();

        private ImageSource _userImage;
        public ImageSource UserImage
        {
            get => _userImage;
            set => SetProperty(ref _userImage, value);
        }


        private IEventAggregator _ea;
        public ICommand DisplayBurger => new DelegateCommand(OnDisplayBurger);
        public ICommand GotoPageCommand => new DelegateCommand<string>(GotoPage);
        public ICommand TakePictureCommand => new DelegateCommand(TakePicture);
        public new ICommand GotoCancelCommand => new DelegateCommand(GotoCancel);

        public new ICommand GotoPreviousCommand => new DelegateCommand(GotoPrevious);
        public ICommand GotoNextCommand => new DelegateCommand(GotoNext);
        public ICommand OnOpenGenderPopupCommand => new DelegateCommand(OpenGenderPopupAsync);
        public ICommand OnOpenIdentityDocumentPopupCommand => new DelegateCommand(OpenIdentityDocumentPopupAsync);
        public ICommand OnOpenCountryPopupCommand => new DelegateCommand(OpenCountryPopupAsync);
        public ICommand OnOpenIdTypePopupCommand => new DelegateCommand(OpenIdTypePopupAsync);
        public ICommand OnOpenCountiesPopupCommand => new DelegateCommand(OpenCountiesPopupAsync);
        public ICommand OnOpenDistrictPopupCommand => new DelegateCommand(OpenDistrictPopupAsync);
        public ICommand OnOpenSecurityQuestionsPopupCommand => new DelegateCommand(OpenSecurityQuestionPopupAsync);
        public ICommand OnOpenCitiesAndTownsPopupCommand => new DelegateCommand(OpenCitiesAndTownsPopupAsync);
        //public ICommand OnOpenCommunitiesPopupCommand => new DelegateCommand(OpenCommunitiesPopupAsync);
        private readonly IPageDialogService _dialogService;

        public SiginUpViewModel(INavigationService navigationService,
                                IEventAggregator ea,
                                IPageDialogService dialogService,
                                IGenders genderservice,
                                IIdentificationTypes idservice,
                                IDocumentTypes doctypeservice,
                                IAuthentication authentication,
                                IFeature customerservice,
                                 IAppInfo appInfo,
                                 IApplications applicationsService,
                                ICountries countriesService) : base(navigationService, false)
        {
            _ea = ea;
            _dialogService = dialogService;

            _genderservice = genderservice;
            _idservice = idservice;
            _doctypeservice = doctypeservice;
            _customerservice = customerservice;

            _countriesService = countriesService ?? throw new ArgumentNullException(nameof(countriesService));
            _appInfo = appInfo ?? throw new System.ArgumentNullException(nameof(appInfo));
            _applicationsService = applicationsService ?? throw new System.ArgumentNullException(nameof(applicationsService));
            _authentication = authentication;


            PreviousText.Value = TranslateManagerHelper.Convert("cancel").ToUpperInvariant();
            NextText.Value = TranslateManagerHelper.Convert("next").ToUpperInvariant();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var popupId = parameters.GetValue<string>(Constants.PopupId);
            var selectedData = parameters.GetValue<PikerItem>(Constants.PopupResponseData);
            switch (popupId)
            {
                case "TITLES_POPUP":
                    Gender.Id = selectedData.Id;
                    Gender.Value = selectedData.Name_1;
                    break;
                case "COUNTRY_POPUP":
                    Country.Id = selectedData.Id;
                    Country.Value = selectedData.Name_1;
                    Counties.Id = "";
                    Counties.Value = "";
                    District.Id = "";
                    District.Value = "";
                    CitiesAndTowns.Id = "";
                    CitiesAndTowns.Value = "";
                    Communities.Id = "";
                    Communities.Value = "";
                    break;
                case "IDENTITY_DOCUMENT_POPUP":
                    CountryIdentityDocument.Id = selectedData.Id;
                    CountryIdentityDocument.Value = selectedData.Name_1;
                    break;
                case "IDENTITY_DOCUMENT_TYPE_POPUP":
                    IdentityDocumentType.Id = selectedData.Id;
                    IdentityDocumentType.Value = selectedData.Name_1;
                    break;
                case "COUNTIES_POPUP":
                    Counties.Id = selectedData.Id;
                    Counties.Value = selectedData.Name_1;
                    District.Id = "";
                    District.Value = "";
                    CitiesAndTowns.Id = "";
                    CitiesAndTowns.Value = "";
                    Communities.Id = "";
                    Communities.Value = "";
                    break;
                case "DISTRICT_POPUP":
                    District.Id = selectedData.Id;
                    District.Value = selectedData.Name_1;
                    //CitiesAndTowns.Id = "";
                    //CitiesAndTowns.Value = "";
                    //Communities.Id = "";
                    //Communities.Value = "";
                    break;
                case "CITIES_AND_TOWNS_POPUP":
                    CitiesAndTowns.Id = selectedData.Id;
                    CitiesAndTowns.Value = selectedData.Name_1;
                    //Communities.Id = "";
                    //Communities.Value = "";
                    break;
                case "COMMUNITIES_POPUP":
                    Communities.Id = selectedData.Id;
                    Communities.Value = selectedData.Name_1;
                    break;
                case "SECURITY_QUESTION_POPUP":
                    SecurityQuestion.Id = selectedData.Id;
                    SecurityQuestion.Value = selectedData.Name_1;
                    break;
                default:
                    break;
            }
        }

        protected async void GotoPage(string p)
        {
            await NavigationService.NavigateAsync($"{p}");
        }

        private async void OpenGenderPopupAsync()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            // appel service pour recuperer la liste des genders, attention, le service devra gerer le cache de sorte
            // a ne pas rappeller a cahque fois le service referential

            ObservableCollection<PikerItem> exampleGendersList = new ObservableCollection<PikerItem>();

            var listg = await _genderservice.GetList("");
            if (listg?.isuccess == true)
            {
                exampleGendersList = listg.data.Select(x => new PikerItem { Id = x.id.ToString(), Name_1 = x.name }).ToObservableCollection();
            }

            var parameters = new NavigationParameters
                {
                    { Constants.PopupId, "TITLES_POPUP" },
                    { Constants.PopupTitle, TranslateManagerHelper.Convert("choose_title").ToUpperInvariant() },
                   // {Constants.PopupIcon, "icon-add-user"},
                    {Constants.PopupCurrentData, exampleGendersList.FirstOrDefault(x => x.Id == Gender.Id)},
                    { Constants.PopupResquestData, exampleGendersList }
                };

            await NavigationService.NavigateAsync(PopupName.PikerPopup, parameters).ConfigureAwait(false);

            IsLoading = false;
        }
        private async void OpenCountryPopupAsync()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            // appel service pour recuperer la liste des genders, attention, le service devra gerer le cache de sorte
            // a ne pas rappeller a cahque fois le service referential

            var exampleContriesList = new ObservableCollection<PikerItem>();

            var countriesList = await _countriesService.GetList(_authentication.GetToken());
            if (countriesList?.isuccess == true)
            {
                exampleContriesList = countriesList.data.Select(x => new PikerItem { Id = x.Id.ToString(), Name_1 = x.IsoCode }).ToObservableCollection();
            }

            var parameters = new NavigationParameters
                {
                    { Constants.PopupId, "COUNTRY_POPUP" },
                    { Constants.PopupTitle, TranslateManagerHelper.Convert("select_country").ToUpperInvariant() },
                   // {Constants.PopupIcon, "icon-add-user"},
                    {Constants.PopupCurrentData, exampleContriesList.FirstOrDefault(x => x.Id == Country.Id)},
                    { Constants.PopupResquestData, exampleContriesList }
                };

            await NavigationService.NavigateAsync(PopupName.PikerPopup, parameters).ConfigureAwait(false);

            IsLoading = false;
        }
        private async void OpenIdentityDocumentPopupAsync()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            // appel service pour recuperer la liste des type document, attention, le service devra gerer le cache de sorte
            // a ne pas rappeller a cahque fois le service referential

            ObservableCollection<PikerItem> list = new ObservableCollection<PikerItem>();

            var listids = await _idservice.GetList("");
            if (listids?.isuccess == true)
            {
                list = listids.data.Select(x => new PikerItem { Id = x.id.ToString(), Name_1 = x.name }).ToObservableCollection();
            }

            var parameters = new NavigationParameters
                {
                    { Constants.PopupId, "IDENTITY_DOCUMENT_POPUP" },
                    { Constants.PopupTitle, TranslateManagerHelper.Convert("select_identity_document").ToUpperInvariant() },
                   // {Constants.PopupIcon, "icon-add-user"},
                    {Constants.PopupCurrentData, list.FirstOrDefault(x => x.Id == CountryIdentityDocument.Id) },
                    { Constants.PopupResquestData, list }
                };

            await NavigationService.NavigateAsync(PopupName.PikerPopup, parameters).ConfigureAwait(false);

            IsLoading = false;
        }
        private async void OpenIdTypePopupAsync()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            // appel service pour recuperer la liste des identity type, attention, le service devra gerer le cache de sorte
            // a ne pas rappeller a cahque fois le service referential

            ObservableCollection<PikerItem> list = new ObservableCollection<PikerItem>();

            var listids = await _doctypeservice.GetList("");
            if (listids?.isuccess == true)
            {
                list = listids.data.Select(x => new PikerItem { Id = x.id.ToString(), Name_1 = x.name }).ToObservableCollection();
            }


            var parameters = new NavigationParameters
                {
                    { Constants.PopupId, "IDENTITY_DOCUMENT_TYPE_POPUP" },
                    { Constants.PopupTitle, TranslateManagerHelper.Convert("select_identity_type").ToUpperInvariant() },
                   // {Constants.PopupIcon, "icon-add-user"},
                    {Constants.PopupCurrentData, list.FirstOrDefault(x => x.Id == IdentityDocumentType.Id)},
                    { Constants.PopupResquestData, list }
                };

            await NavigationService.NavigateAsync(PopupName.PikerPopup, parameters).ConfigureAwait(false);

            IsLoading = false;
        }
        private async void OpenCountiesPopupAsync()
        {
            Country.HasError = false;
            Counties.HasError = false;
            if (string.IsNullOrEmpty(Country.Id))
            {
                Country.ErrorText = TranslateManagerHelper.Convert("country_error");
                Country.HasError = true;
                IsLoading = false;
                return;
            }

            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            // appel service pour recuperer la liste des regions, attention, le service devra gerer le cache de sorte
            // a ne pas rappeller a cahque fois le service referential


            ObservableCollection<PikerItem> list = new ObservableCollection<PikerItem>();

            var CountryId = long.Parse(Country.Id);
            var countiesList = await _countriesService.GetRegions(_authentication.GetToken(), CountryId);
            if (countiesList?.isuccess == true)
            {
                list = countiesList.data.Select(x => new PikerItem { Id = x.id.ToString(), Name_1 = x.name }).ToObservableCollection();
            }

            var parameters = new NavigationParameters
                {
                    { Constants.PopupId, "COUNTIES_POPUP" },
                    { Constants.PopupTitle, TranslateManagerHelper.Convert("select_counties").ToUpperInvariant()},
                   // {Constants.PopupIcon, "icon-add-user"},
                    {Constants.PopupCurrentData, list.FirstOrDefault(x => x.Id  == Counties.Id)},
                    { Constants.PopupResquestData, list }
                };

            await NavigationService.NavigateAsync(PopupName.PikerPopup, parameters).ConfigureAwait(false);

            IsLoading = false;
        }
        private async void OpenDistrictPopupAsync()
        {
            District.HasError = false;
            Counties.HasError = false;
            if (string.IsNullOrEmpty(Counties.Id))
            {
                Counties.ErrorText = TranslateManagerHelper.Convert("counties_error");
                Counties.HasError = true;
                IsLoading = false;
                return;
            }

            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            // appel service pour recuperer la liste des Departements, attention, le service devra gerer le cache de sorte
            // a ne pas rappeller a cahque fois le service referential

            ObservableCollection<PikerItem> list = new ObservableCollection<PikerItem>();

            var CountieId = long.Parse(Counties.Id);
            var districtsList = await _countriesService.GetDistricts(_authentication.GetToken(), long.Parse(Country.Id), CountieId);
            if (districtsList?.isuccess == true)
            {
                list = districtsList.data.Select(x => new PikerItem { Id = x.id.ToString(), Name_1 = x.name }).ToObservableCollection();
            }

            var parameters = new NavigationParameters
                {
                    { Constants.PopupId, "DISTRICT_POPUP" },
                    { Constants.PopupTitle, TranslateManagerHelper.Convert("select_district").ToUpperInvariant() },
                   // {Constants.PopupIcon, "icon-add-user"},
                    {Constants.PopupCurrentData, list.FirstOrDefault(x => x.Id == District.Id) },
                    { Constants.PopupResquestData, list }
                };

            await NavigationService.NavigateAsync(PopupName.PikerPopup, parameters).ConfigureAwait(false);

            IsLoading = false;
        }
        private async void OpenCitiesAndTownsPopupAsync()
        {
            //District.HasError = false;
            //CitiesAndTowns.HasError = false;
            //if (string.IsNullOrEmpty(District.Id))
            //{
            //    District.ErrorText = TranslateManagerHelper.Convert("district_error");
            //    District.HasError = true;
            //    IsLoading = false;
            //    return;
            //}
            District.HasError = false;
            Counties.HasError = false;
            if (string.IsNullOrEmpty(Counties.Id))
            {
                Counties.ErrorText = TranslateManagerHelper.Convert("counties_error");
                Counties.HasError = true;
                IsLoading = false;
                return;
            }


            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            // appel service pour recuperer la liste des Departements, attention, le service devra gerer le cache de sorte
            // a ne pas rappeller a cahque fois le service referential

            ObservableCollection<PikerItem> list = new ObservableCollection<PikerItem>();

            var CountieId = long.Parse(Counties.Id);
            var districtsList = await _countriesService.GetDepartments(_authentication.GetToken(), long.Parse(Country.Id), CountieId);
            if (districtsList?.isuccess == true)
            {
                list = districtsList.data.Select(x => new PikerItem { Id = x.id.ToString(), Name_1 = x.name }).ToObservableCollection();
            }

            var parameters = new NavigationParameters
                {
                    { Constants.PopupId, "CITIES_AND_TOWNS_POPUP" },
                    { Constants.PopupTitle, TranslateManagerHelper.Convert("select_cities_and_towns").ToUpperInvariant() },
                   // {Constants.PopupIcon, "icon-add-user"},
                    {Constants.PopupCurrentData, list.FirstOrDefault(x => x.Id == CitiesAndTowns.Id)},
                    { Constants.PopupResquestData, list }
                };

            await NavigationService.NavigateAsync(PopupName.PikerPopup, parameters).ConfigureAwait(false);

            IsLoading = false;
        }
        //private async void OpenCommunitiesPopupAsync()
        //{
        //    if (IsBusy || IsLoading)
        //    {
        //        //Message Toast
        //        ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
        //        return;
        //    }
        //    IsLoading = true;

        //    // appel service pour recuperer la liste des Neighborhoods, attention, le service devra gerer le cache de sorte
        //    // a ne pas rappeller a cahque fois le service referential

        //    var list = new ObservableCollection<PikerItem>() {
        //        new PikerItem { Id = "1", Name_1 = "Example Communities 1"} ,
        //        new PikerItem { Id="2", Name_1="Example Communities 2"},
        //        new PikerItem { Id="3", Name_1="Example Communities 3"}
        //        };

        //    var parameters = new NavigationParameters
        //        {
        //            { Constants.PopupId, "COMMUNITIES_POPUP" },
        //            { Constants.PopupTitle, TranslateManagerHelper.Convert("select_communities").ToUpperInvariant() },
        //           // {Constants.PopupIcon, "icon-add-user"},
        //            {Constants.PopupCurrentData, list.FirstOrDefault(x => x.Id == Communities.Id) },
        //            { Constants.PopupResquestData, list }
        //        };

        //    await NavigationService.NavigateAsync(PopupName.PikerPopup, parameters).ConfigureAwait(false);

        //    IsLoading = false;
        //}
        private async void OpenSecurityQuestionPopupAsync()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            // appel service pour recuperer la liste des Neighborhoods, attention, le service devra gerer le cache de sorte
            // a ne pas rappeller a cahque fois le service referential
            ObservableCollection<PikerItem> list = new ObservableCollection<PikerItem>();
            var listr = await _applicationsService.GetApplicationQuestions(_authentication.GetToken(), _appInfo.App.Id);
            if (listr?.isuccess == true)
            {
                list = listr.data.Select(x => new PikerItem { Id = x.Id.ToString(), Name_1 = x.Name }).ToObservableCollection();
            }

            var parameters = new NavigationParameters
                {
                    { Constants.PopupId, "SECURITY_QUESTION_POPUP" },
                    { Constants.PopupTitle, TranslateManagerHelper.Convert("select_security_question").ToUpperInvariant() },
                   // {Constants.PopupIcon, "icon-add-user"},
                    {Constants.PopupCurrentData, list.FirstOrDefault(x => x.Id == SecurityQuestion.Id) },
                    { Constants.PopupResquestData, list }
                };

            await NavigationService.NavigateAsync(PopupName.PikerPopup, parameters).ConfigureAwait(false);

            IsLoading = false;
        }
        private async void TakePicture()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                var parameters = new NavigationParameters
                                    {
                                        { Constants.PopupIsSucces, false },
                                        { Constants.PopupMessage, TranslateManagerHelper.Convert("camera_not_available") },
                                        { Constants.PopupIsBeforeHome, true },
                                        {Constants.PopupNextPage, ""}
                                    };
                await NavigationService.NavigateAsync(PopupName.SuccessfullPopup, parameters).ConfigureAwait(false);

                IsLoading = false;
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "KolaDirectory",
                Name = "kolakash_auto_enroll_picture.jpg",
                DefaultCamera = CameraDevice.Front,
                PhotoSize = PhotoSize.Medium,
                CustomPhotoSize = 10,
                CompressionQuality = 12,
            }).ConfigureAwait(false);

            if (file == null)
            {
                IsLoading = false;
                return;
            }

            UserImage = ImageSource.FromStream(() =>
            {
                var stream = file.GetStreamWithImageRotatedForExternalStorage();
                return stream;
            });
            IsTakePicture.Value = true;
            IsLoading = false;
        }

        private async void GotoCancel()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            await NavigationService.GoBackAsync().ConfigureAwait(false);
            IsLoading = false;
        }

        private async void GotoNext()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            switch (CurrentStep.Value)
            {
                case 1:
                    {
                        FirstStep.Value = false;
                        SecondStep.Value = true;
                        PreviousText.Value = TranslateManagerHelper.Convert("previous").ToUpperInvariant();
                        CurrentStep.Value++;
                        break;
                    }
                case 2:
                    {
                        Gender.HasError = FirstName.HasError = LastName.HasError = Email.HasError = false;

                        if (string.IsNullOrEmpty(Gender.Value))
                        {
                            Gender.ErrorText = TranslateManagerHelper.Convert("gender_error");
                            Gender.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        if (string.IsNullOrEmpty(FirstName.Value))
                        {
                            FirstName.ErrorText = TranslateManagerHelper.Convert("firstname_error");
                            FirstName.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        if (string.IsNullOrEmpty(LastName.Value))
                        {
                            LastName.ErrorText = TranslateManagerHelper.Convert("lastname_error");
                            LastName.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        if (!string.IsNullOrEmpty(Email.Value))
                        {
                            if (!ValidEmailHelper.IsValidEmail(Email.Value))
                            {
                                Email.ErrorText = TranslateManagerHelper.Convert("email_error");
                                Email.HasError = true;
                                IsLoading = false;
                                return;
                            }
                        }
                        SecondStep.Value = false;
                        ThirdStep.Value = true;
                        CurrentStep.Value++;
                        break;
                    }
                case 3:
                    {
                        Country.HasError = CountryIdentityDocument.HasError = IdentityDocumentType.HasError = IdentityDocumentNumber.HasError = false;
                        StartingDateIdentityDocument.HasError = ExpireDateIdentityDocument.HasError = false;

                        if (string.IsNullOrEmpty(Country.Value))
                        {
                            Country.ErrorText = TranslateManagerHelper.Convert("country_error");
                            Country.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        if (string.IsNullOrEmpty(CountryIdentityDocument.Value))
                        {
                            CountryIdentityDocument.ErrorText = TranslateManagerHelper.Convert("country_identity_document_error");
                            CountryIdentityDocument.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        if (string.IsNullOrEmpty(IdentityDocumentType.Value))
                        {
                            IdentityDocumentType.ErrorText = TranslateManagerHelper.Convert("identity_document_type_error");
                            IdentityDocumentType.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        if (string.IsNullOrEmpty(IdentityDocumentNumber.Value))
                        {
                            IdentityDocumentNumber.ErrorText = TranslateManagerHelper.Convert("identity_document_number_error");
                            IdentityDocumentNumber.HasError = true;
                            IsLoading = false;
                            return;
                        }


                        var today = DateTime.Now;

                        if (StartingDateIdentityDocument.Value.CompareDateGreaterThan(today) && !StartingDateIdentityDocument.Value.CompareDateEqualThan(today))
                        {
                            StartingDateIdentityDocument.ErrorText = TranslateManagerHelper.Convert("issue_date_error");
                            StartingDateIdentityDocument.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        if (ExpireDateIdentityDocument.Value.CompareDateLessThan(today) || ExpireDateIdentityDocument.Value.CompareDateEqualThan(today))
                        {
                            ExpireDateIdentityDocument.ErrorText = TranslateManagerHelper.Convert("expiration_date_error");
                            ExpireDateIdentityDocument.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        if (ExpireDateIdentityDocument.Value.CompareDateLessThan(StartingDateIdentityDocument.Value) ||
                            ExpireDateIdentityDocument.Value.CompareDateEqualThan(StartingDateIdentityDocument.Value))
                        {
                            ExpireDateIdentityDocument.ErrorText = TranslateManagerHelper.Convert("expiration_issue_date_error");
                            ExpireDateIdentityDocument.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        ThirdStep.Value = false;
                        FourthStep.Value = true;
                        CurrentStep.Value++;
                        break;
                    }
                case 4:
                    {
                        //Counties.HasError = District.HasError = CitiesAndTowns.HasError = Communities.HasError = false;

                        //if (string.IsNullOrEmpty(Counties.Value))
                        //{
                        //    Counties.ErrorText = TranslateManagerHelper.Convert("counties_error");
                        //    Counties.HasError = true;
                        //    IsLoading = false;
                        //    return;
                        //}

                        //if (string.IsNullOrEmpty(District.Value))
                        //{
                        //    District.ErrorText = TranslateManagerHelper.Convert("district_error");
                        //    District.HasError = true;
                        //    IsLoading = false;
                        //    return;
                        //}

                        //if (string.IsNullOrEmpty(CitiesAndTowns.Value))
                        //{
                        //    CitiesAndTowns.ErrorText = TranslateManagerHelper.Convert("cities_and_towns_error");
                        //    CitiesAndTowns.HasError = true;
                        //    IsLoading = false;
                        //    return;
                        //}

                        //if (string.IsNullOrEmpty(Communities.Value))
                        //{
                        //    Communities.ErrorText = TranslateManagerHelper.Convert("communities_error");
                        //    Communities.HasError = true;
                        //    IsLoading = false;
                        //    return;
                        //}

                        FourthStep.Value = false;
                        FifthStep.Value = true;
                        CurrentStep.Value++;
                        break;
                    }
                case 5:
                    {
                        Address.HasError = PhoneNumber.HasError = SecurityQuestion.HasError = Answer.HasError = Password.HasError = ConfirmPassword.HasError = false;

                        if (string.IsNullOrEmpty(Address.Value))
                        {
                            Address.ErrorText = TranslateManagerHelper.Convert("address_error");
                            Address.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        if (string.IsNullOrEmpty(PhoneNumber.Value))
                        {
                            PhoneNumber.ErrorText = TranslateManagerHelper.Convert("phone_number_error");
                            PhoneNumber.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        if (PhoneNumber.Value.Length != PhoneNumberMaxLength.Value)
                        {
                            PhoneNumber.ErrorText = TranslateManagerHelper.Convert("phone_number_is_short");
                            PhoneNumber.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        if (string.IsNullOrEmpty(SecurityQuestion.Value))
                        {
                            SecurityQuestion.ErrorText = TranslateManagerHelper.Convert("question_securuty_error");
                            SecurityQuestion.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        if (string.IsNullOrEmpty(Answer.Value))
                        {
                            Answer.ErrorText = TranslateManagerHelper.Convert("answer_error");
                            Answer.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        if (string.IsNullOrEmpty(Answer.Value))
                        {
                            Answer.ErrorText = TranslateManagerHelper.Convert("answer_error");
                            Answer.HasError = true;
                            IsLoading = false;
                            return;
                        }
                        if (string.IsNullOrEmpty(Password.Value))
                        {
                            Password.ErrorText = TranslateManagerHelper.Convert("pincode_error");
                            Password.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        if (Password.Value.Length != PasswordMaxLength.Value)
                        {
                            Password.ErrorText = TranslateManagerHelper.Convert("pincode_is_short");
                            Password.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        if (string.IsNullOrEmpty(ConfirmPassword.Value))
                        {
                            ConfirmPassword.ErrorText = TranslateManagerHelper.Convert("pincode_error");
                            ConfirmPassword.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        if (ConfirmPassword.Value.Length != PasswordMaxLength.Value)
                        {
                            ConfirmPassword.ErrorText = TranslateManagerHelper.Convert("pincode_is_short");
                            ConfirmPassword.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        if (Password.Value != ConfirmPassword.Value)
                        {
                            ConfirmPassword.ErrorText = TranslateManagerHelper.Convert("pincode_no_match");
                            ConfirmPassword.HasError = true;
                            IsLoading = false;
                            return;
                        }

                        FifthStep.Value = false;
                        SixthStep.Value = true;
                        NextText.Value = TranslateManagerHelper.Convert("validate").ToUpperInvariant();
                        CurrentStep.Value++;
                        break;
                    }
                case 6:
                    // appel du service pour le signup et affichage du popup succes ou failed en fonction du retour service

                    CustomerEnrollmentRequest cust = new CustomerEnrollmentRequest
                    {
                        Fname = FirstName.Value,
                        Lname = LastName.Value,
                        Address = Address.Value,
                        GenderId = long.Parse(Gender.Id),
                        Email = Email.Value,
                        Phone = ApiConstants.CountryPrefix + PhoneNumber.Value,
                        LanguageId = 1,
                        CountryId = long.Parse(Country.Id),
                        upassword = Password.Value,
                        AnswerSQ = Answer.Value,
                        IdCountry = long.Parse(CountryIdentityDocument.Id),
                        ContactPhone = PhoneNumber.Value,
                        IdIssueDate = StartingDateIdentityDocument.Value,
                        IdExpirationDate = ExpireDateIdentityDocument.Value,
                        SecurityQuestionId = long.Parse(SecurityQuestion.Id),
                        IdNumber = IdentityDocumentNumber.Value,
                    };

                    // ajout des champs non obligatoires
                    if (!string.IsNullOrEmpty(Counties.Id))
                        cust.RegionId = long.Parse(Counties.Id);

                    if (!string.IsNullOrEmpty(District.Id))
                        cust.DepartmentId = long.Parse(District.Id);

                    if (!string.IsNullOrEmpty(CitiesAndTowns.Id))
                        cust.CityId = long.Parse(CitiesAndTowns.Id);

                    if (!string.IsNullOrEmpty(Communities.Id))
                        cust.NeighborhoodId = long.Parse(Communities.Id);


                    featureDTO f = new featureDTO
                    {
                        FeatureName = "CUSTOMER_ENROLLMENT",
                        Amount = 0,
                        SenderId = "",
                        SenderPassword = "",
                        TargetId = "",
                        jsonObjString = JsonConvert.SerializeObject(cust)
                    };

                    string objstring = f.jsonObjString;
                    var result = await _customerservice.Execute<featureDTO, CustomerEnrollmentResponse>(_authentication.GetToken(), f, ApiConstants.SystemId);

                    NavigationParameters parameters = null;
                    string message = "";
                    bool isSuccess = false;
                    if (result != null)
                    {
                        if (result.isuccess)
                        {
                            message = !string.IsNullOrEmpty(result.msg) ? result.msg : TranslateManagerHelper.Convert("enrollment_successfully");
                            isSuccess = true;
                        }
                        else
                        {
                            switch (result.errcode)
                            {
                                case "EXCEPTION":
                                    message = TranslateManagerHelper.Convert("operation_failed");
                                    break;
                                default:
                                    message = !string.IsNullOrEmpty(result.msg) ? result.msg : TranslateManagerHelper.Convert("enrollment_failed");
                                    break;
                            }

                            isSuccess = false;
                        }

                    }
                    else
                    {
                        // TODO : what message do we send ?
                        isSuccess = false;
                        message = TranslateManagerHelper.Convert("error_contact_support");
                    }

                    parameters = new NavigationParameters
                    {
                        { Constants.PopupIsSucces, isSuccess },
                        { Constants.PopupIsBeforeHome, true },
                        { Constants.PopupMessage, message },
                        {Constants.PopupNextPage, FunctionName.Login }
                    };


                    await NavigationService.NavigateAsync(PopupName.SuccessfullPopup, parameters);
                    break;
                default:
                    break;
            }
            IsLoading = false;
        }

        private async void GotoPrevious()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            switch (CurrentStep.Value)
            {
                case 6:
                    {
                        SixthStep.Value = false;
                        FifthStep.Value = true;
                        NextText.Value = TranslateManagerHelper.Convert("next").ToUpperInvariant();
                        CurrentStep.Value--;
                        break;
                    }
                case 5:
                    {
                        FifthStep.Value = false;
                        FourthStep.Value = true;
                        CurrentStep.Value--;
                        break;
                    }
                case 4:
                    {
                        FourthStep.Value = false;
                        ThirdStep.Value = true;
                        CurrentStep.Value--;
                        break;
                    }
                case 3:
                    {
                        ThirdStep.Value = false;
                        SecondStep.Value = true;
                        CurrentStep.Value--;
                        break;
                    }
                case 2:
                    {
                        SecondStep.Value = false;
                        FirstStep.Value = true;
                        PreviousText.Value = TranslateManagerHelper.Convert("cancel").ToUpperInvariant();
                        CurrentStep.Value--;
                        break;
                    }
                default:
                    await NavigationService.GoBackAsync().ConfigureAwait(false);
                    break;
            }
            IsLoading = false;
        }

        private void OnDisplayBurger()
        {
            _ea.GetEvent<BurgerEvent>().Publish();
        }
    }
}
