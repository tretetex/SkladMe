using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using SkladMe.Common.Model;
using SkladMe.Infrastructure.Collections;
using SkladMe.Model;
using SkladMe.Models;

namespace SkladMe.ViewModel
{
    [Serializable]
    public class FiltersVM : BaseVM
    {
        #region fields
        private bool _prefixOpen = true;
        private bool _prefixActive = true;
        private bool _prefixCompleted = true;
        private bool _prefixAvailable = true;

        private bool _organizerExistYes;
        private bool _organizerExistNoMatter = true;
        private bool _organizerExistNo;

        private bool _feeSoonYes;
        private bool _feeSoonNoMatter = true;
        private bool _feeSoonNo;

        private Filters _model;
        private string _title = "Новый";
        private bool _isSaved;

        #endregion

        #region constructors

        public FiltersVM()
        {
            Model = new Filters();
            SetKeywordCollModel();
        }

        public FiltersVM(bool initCollection) : this()
        {
            if (initCollection)
            {
                CollectionsInit();
            }
        }

        private void SetKeywordCollModel()
        {
            TitleKeywordIncluded.Model = Model.TitleKeywordIncluded;
            TitleKeywordExcluded.Model = Model.TitleKeywordExcluded;
            TagsIncluded.Model = Model.TagsIncluded;
            TagsExcluded.Model = Model.TagsExcluded;
        }
        #endregion

        #region public properties

        public Filters Model
        {
            get { return _model; }
            set
            {
                _model = value;
                SetKeywordCollModel();
                OnPropertyChanged(String.Empty);
            }
        }

        public KeywordCollection TitleKeywordIncluded { get; } = new KeywordCollection();
        public KeywordCollection TitleKeywordExcluded { get; } = new KeywordCollection();
        public KeywordCollection TagsIncluded { get; } = new KeywordCollection();
        public KeywordCollection TagsExcluded { get; } = new KeywordCollection();

        public ChapterCollection AuthorsChapters { get; set; } = new ChapterCollection();
        public ChapterCollection TranslationsChapters { get; set; } = new ChapterCollection();
        public ChapterCollection CommonChapters { get; set; } = new ChapterCollection();

        [XmlIgnore]
        public string Path { get; set; }

        [XmlIgnore]
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public bool IsSaved
        {
            get { return _isSaved; }
            set
            {
                _isSaved = value;
                OnPropertyChanged();
            }
        }

        public bool AllWordsContains
        {
            get { return Model.AllWordsContains; }
            set
            {
                Model.AllWordsContains = value;
                OnPropertyChanged();
            }
        }

        public bool AllTagsContains
        {
            get { return Model.AllTagsContains; }
            set
            {
                Model.AllTagsContains = value;
                OnPropertyChanged();
            }
        }

        public bool PrefixOpen
        {
            get { return _prefixOpen; }
            set
            {
                _prefixOpen = value;
                Model.PrefixOpen = _prefixOpen;
                OnPropertyChanged();
            }
        }

        public bool PrefixActive
        {
            get { return _prefixActive; }
            set
            {
                _prefixActive = value;
                Model.PrefixActive = _prefixActive;
                OnPropertyChanged();
            }
        }

        public bool PrefixCompleted
        {
            get { return _prefixCompleted; }
            set
            {
                _prefixCompleted = value;
                Model.PrefixCompleted = _prefixCompleted;
                OnPropertyChanged();
            }
        }

        public bool PrefixAvailable
        {
            get { return _prefixAvailable; }
            set
            {
                _prefixAvailable = value;
                Model.PrefixAvailable = _prefixAvailable;
                OnPropertyChanged();
            }
        }

        public int? PriceFrom
        {
            get { return Model.Price.From; }
            set
            {
                Model.Price = Range.New(value, PriceTo);
                OnPropertyChanged(nameof(PriceTo));
                OnPropertyChanged(nameof(PriceFrom));
            }
        }

        public int? PriceTo
        {
            get { return Model.Price.To; }
            set
            {
                Model.Price = Range.New(PriceFrom, value);
                OnPropertyChanged(nameof(PriceTo));
                OnPropertyChanged(nameof(PriceFrom));
            }
        }

        public int? FeeFrom
        {
            get { return Model.Fee.From; }
            set
            {
                Model.Fee = Range.New(value, FeeTo);
                OnPropertyChanged(nameof(FeeTo));
                OnPropertyChanged(nameof(FeeFrom));
            }
        }

        public int? FeeTo
        {
            get { return Model.Fee.To; }
            set
            {
                Model.Fee = Range.New(FeeFrom, value);
                OnPropertyChanged(nameof(FeeTo));
                OnPropertyChanged(nameof(FeeFrom));
            }
        }

        public double? FeeToPriceFrom
        {
            get { return Model.FeeToPrice.From; }
            set {
                Model.FeeToPrice = Range.New(value, FeeToPriceTo);
                OnPropertyChanged(nameof(FeeToPriceTo));
                OnPropertyChanged(nameof(FeeToPriceFrom));
            }
        }

        public double? FeeToPriceTo
        {
            get { return Model.FeeToPrice.To; }
            set {
                Model.FeeToPrice = Range.New(FeeToPriceFrom, value);
                OnPropertyChanged(nameof(FeeToPriceTo));
                OnPropertyChanged(nameof(FeeToPriceFrom));
            }
        }

        public int? RealFeeFrom
        {
            get { return Model.RealFee.From; }
            set
            {
                Model.RealFee = Range.New(value, RealFeeTo);
                OnPropertyChanged(nameof(RealFeeTo));
                OnPropertyChanged(nameof(RealFeeFrom));
            }
        }

        public int? RealFeeTo
        {
            get { return Model.RealFee.To; }
            set
            {
                Model.RealFee = Range.New(RealFeeFrom, value);
                OnPropertyChanged(nameof(RealFeeTo));
                OnPropertyChanged(nameof(RealFeeFrom));
            }
        }

        public DateTime? DateOfCreationFrom
        {
            get { return Model.DateOfCreation.From; }
            set
            {
                Model.DateOfCreation = Range.New(value, DateOfCreationTo);
                OnPropertyChanged(nameof(DateOfCreationTo));
                OnPropertyChanged(nameof(DateOfCreationFrom));
            }
        }

        public DateTime? DateOfCreationTo
        {
            get { return Model.DateOfCreation.To; }
            set
            {
                Model.DateOfCreation = Range.New(DateOfCreationFrom, value);
                OnPropertyChanged(nameof(DateOfCreationTo));
                OnPropertyChanged(nameof(DateOfCreationFrom));
            }
        }

        public string TopicStarter
        {
            get { return Model.TopicStarter; }
            set
            {
                Model.TopicStarter = value;
                OnPropertyChanged();
            }
        }

        public bool OrganizerExistYes
        {
            get { return _organizerExistYes; }
            set
            {
                _organizerExistYes = value;
                if (_organizerExistYes)
                {
                    Model.OrganizerExist = true;
                }
                OnPropertyChanged();
            }
        }

        public bool OrganizerExistNoMatter
        {
            get { return _organizerExistNoMatter; }
            set
            {
                _organizerExistNoMatter = value;
                if (_organizerExistNoMatter)
                {
                    Model.OrganizerExist = null;
                }
                OnPropertyChanged();
            }
        }

        public bool OrganizerExistNo
        {
            get { return _organizerExistNo; }
            set
            {
                _organizerExistNo = value;
                if (_organizerExistNo)
                {
                    PrefixCompleted = false;
                    PrefixActive = false;
                    PrefixAvailable = false;
                    Model.OrganizerExist = false;
                }
                OnPropertyChanged();
            }
        }

        public string OrganizerNickname
        {
            get { return Model.OrganizerNickname; }
            set
            {
                Model.OrganizerNickname = value;
                OnPropertyChanged();
            }
        }

        public bool FeeSoonYes
        {
            get { return _feeSoonYes; }
            set
            {
                _feeSoonYes = value;
                if (_feeSoonYes)
                {
                    Model.FeeSoon = true;
                    PrefixCompleted = false;
                    PrefixActive = false;
                }
                OnPropertyChanged();
            }
        }

        public bool FeeSoonNoMatter
        {
            get { return _feeSoonNoMatter; }
            set
            {
                _feeSoonNoMatter = value;
                if (_feeSoonNoMatter)
                {
                    Model.FeeSoon = null;
                }
                OnPropertyChanged();
            }
        }

        public bool FeeSoonNo
        {
            get { return _feeSoonNo; }
            set
            {
                _feeSoonNo = value;
                if (_feeSoonNo)
                {
                    Model.FeeSoon = false;
                }
                OnPropertyChanged();
            }
        }

        public DateTime? FeeDateFrom
        {
            get { return Model.FeeDate.From; }
            set
            {
                Model.FeeDate = Range.New(value, FeeDateTo);
                OnPropertyChanged(nameof(FeeDateTo));
                OnPropertyChanged(nameof(FeeDateFrom));
            }
        }

        public DateTime? FeeDateTo
        {
            get { return Model.FeeDate.To; }
            set
            {
                Model.FeeDate = Range.New(FeeDateFrom, value);
                OnPropertyChanged(nameof(FeeDateTo));
                OnPropertyChanged(nameof(FeeDateFrom));
            }
        }

        public int? PeopleMainCountFrom
        {
            get { return Model.PeopleMainCount.From; }
            set
            {
                Model.PeopleMainCount = Range.New(value, PeopleMainCountTo);
                OnPropertyChanged(nameof(PeopleMainCountTo));
                OnPropertyChanged(nameof(PeopleMainCountFrom));
            }
        }

        public int? PeopleMainCountTo
        {
            get { return Model.PeopleMainCount.To; }
            set
            {
                Model.PeopleMainCount = Range.New(PeopleMainCountFrom, value);
                OnPropertyChanged(nameof(PeopleMainCountTo));
                OnPropertyChanged(nameof(PeopleMainCountFrom));
            }
        }

        public int? PeopleReserveCountFrom
        {
            get { return Model.PeopleReserveCount.From; }
            set
            {
                Model.PeopleReserveCount = Range.New(value, PeopleReserveCountTo);
                OnPropertyChanged(nameof(PeopleReserveCountTo));
                OnPropertyChanged(nameof(PeopleReserveCountFrom));
            }
        }

        public int? PeopleReserveCountTo
        {
            get { return Model.PeopleReserveCount.To; }
            set
            {
                Model.PeopleReserveCount = Range.New(PeopleReserveCountFrom, value);
                OnPropertyChanged(nameof(PeopleReserveCountTo));
                OnPropertyChanged(nameof(PeopleReserveCountFrom));
            }
        }

        public int? PeopleCountForMinimalFrom
        {
            get { return Model.PeopleCountForMinimal.From; }
            set
            {
                Model.PeopleCountForMinimal = Range.New(value, PeopleCountForMinimalTo);
                OnPropertyChanged(nameof(PeopleCountForMinimalTo));
                OnPropertyChanged(nameof(PeopleCountForMinimalFrom));
            }
        }

        public int? PeopleCountForMinimalTo
        {
            get { return Model.PeopleCountForMinimal.To; }
            set
            {
                Model.PeopleCountForMinimal = Range.New(PeopleCountForMinimalFrom, value);
                OnPropertyChanged(nameof(PeopleCountForMinimalTo));
                OnPropertyChanged(nameof(PeopleCountForMinimalFrom));
            }
        }

        public int? ReviewCountFrom
        {
            get { return Model.ReviewCount.From; }
            set
            {
                Model.ReviewCount = Range.New(value, ReviewCountTo);
                OnPropertyChanged(nameof(ReviewCountTo));
                OnPropertyChanged(nameof(ReviewCountFrom));
            }
        }

        public int? ReviewCountTo
        {
            get { return Model.ReviewCount.To; }
            set
            {
                Model.ReviewCount = Range.New(ReviewCountFrom, value);
                OnPropertyChanged(nameof(ReviewCountTo));
                OnPropertyChanged(nameof(ReviewCountFrom));
            }
        }

        public double? ProductRaitingFrom
        {
            get { return Model.ProductRaiting.From; }
            set
            {
                if (value > 5)
                {
                    value = 5;
                }
                Model.ProductRaiting = Range.New(value, ProductRaitingTo);
                OnPropertyChanged(nameof(ProductRaitingTo));
                OnPropertyChanged(nameof(ProductRaitingFrom));
            }
        }

        public double? ProductRaitingTo
        {
            get { return Model.ProductRaiting.To; }
            set
            {
                if (value > 5)
                {
                    value = 5;
                }
                Model.ProductRaiting = Range.New(ProductRaitingFrom, value);
                OnPropertyChanged(nameof(ProductRaitingTo));
                OnPropertyChanged(nameof(ProductRaitingFrom));
            }
        }

        public double? PopularityFrom
        {
            get { return Model.Popularity.From; }
            set
            {
                Model.Popularity = Range.New(value, PopularityTo);
                OnPropertyChanged(nameof(PopularityTo));
                OnPropertyChanged(nameof(PopularityFrom));
            }
        }

        public double? PopularityTo
        {
            get { return Model.Popularity.To; }
            set
            {
                Model.Popularity = Range.New(PopularityFrom, value);
                OnPropertyChanged(nameof(PopularityTo));
                OnPropertyChanged(nameof(PopularityFrom));
            }
        }

        public double? OrgRaitingFrom
        {
            get { return Model.OrgRaiting.From; }
            set
            {
                Model.OrgRaiting = Range.New(value, OrgRaitingTo);
                OnPropertyChanged(nameof(OrgRaitingTo));
                OnPropertyChanged(nameof(OrgRaitingFrom));
            }
        }

        public double? OrgRaitingTo
        {
            get { return Model.OrgRaiting.To; }
            set
            {
                Model.OrgRaiting = Range.New(OrgRaitingFrom, value);
                OnPropertyChanged(nameof(OrgRaitingTo));
                OnPropertyChanged(nameof(OrgRaitingFrom));
            }
        }

        public double? ClubMemberRaitingFrom
        {
            get { return Model.ClubMemberRaiting.From; }
            set
            {
                Model.ClubMemberRaiting = Range.New(value, ClubMemberRaitingTo);
                OnPropertyChanged(nameof(ClubMemberRaitingTo));
                OnPropertyChanged(nameof(ClubMemberRaitingFrom));
            }
        }

        public double? ClubMemberRaitingTo
        {
            get { return Model.ClubMemberRaiting.To; }
            set
            {
                Model.ClubMemberRaiting = Range.New(ClubMemberRaitingFrom, value);
                OnPropertyChanged(nameof(ClubMemberRaitingTo));
                OnPropertyChanged(nameof(ClubMemberRaitingFrom));
            }
        }


        public double? InvolvementFrom
        {
            get { return Model.Involvement.From; }
            set
            {
                Model.Involvement = Range.New(value, InvolvementTo);
                OnPropertyChanged(nameof(InvolvementTo));
                OnPropertyChanged(nameof(InvolvementFrom));
            }
        }

        public double? InvolvementTo
        {
            get { return Model.Involvement.To; }
            set
            {
                Model.Involvement = Range.New(InvolvementFrom, value);
                OnPropertyChanged(nameof(InvolvementTo));
                OnPropertyChanged(nameof(InvolvementFrom));
            }
        }
        #endregion

        #region private methods

        protected override void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (!string.IsNullOrWhiteSpace(propertyName) && propertyName != nameof(IsSaved) && IsSaved)
            {
                IsSaved = false;
            }
        }

        private void CollectionsInit()
        {
            //TODO: Нужно ли перенести в бд?
            AuthorsChapters = new ChapterCollection()
            {
                new ChapterVM() {Name = "Бизнес и свое дело", Id = 107},
                new ChapterVM() {Name = "Дизайн и креатив", Id = 108},
                new ChapterVM() {Name = "Здоровье и быт", Id = 109},
                new ChapterVM() {Name = "Психология и отношения", Id = 110},
                new ChapterVM() {Name = "Хобби и увлечения", Id = 111}
            };

            TranslationsChapters = new ChapterCollection()
            {
                new ChapterVM() {Name = "Программирование", Id = 112},
                new ChapterVM() {Name = "Бизнес и свое дело", Id = 113},
                new ChapterVM() {Name = "Дизайн и креатив", Id = 114},
                new ChapterVM() {Name = "Здоровье и быт", Id = 115},
                new ChapterVM() {Name = "Психология и отношения", Id = 116},
                new ChapterVM() {Name = "Хобби и увлечения", Id = 117}
            };

            CommonChapters = new ChapterCollection()
            {
                new ChapterVM() {Name = "Курсы по программированию", Id = 21},
                new ChapterVM() {Name = "Курсы по администрированию", Id = 16},
                new ChapterVM() {Name = "Курсы по бизнесу", Id = 24},
                new ChapterVM() {Name = "Бухгалтерия и финансы", Id = 103},
                new ChapterVM() {Name = "Курсы по SEO и SMM", Id = 26},
                new ChapterVM() {Name = "Курсы по дизайну", Id = 19},
                new ChapterVM() {Name = "Курсы по фото и видео", Id = 78},
                new ChapterVM() {Name = "Курсы по музыке", Id = 60},
                new ChapterVM() {Name = "Электронные книги", Id = 30},
                new ChapterVM() {Name = "Курсы по здоровью", Id = 71},
                new ChapterVM() {Name = "Курсы по самообороне", Id = 118},
                new ChapterVM() {Name = "Отдых и путешествия", Id = 97},
                new ChapterVM() {Name = "Курсы по психологии", Id = 38},
                new ChapterVM() {Name = "Курсы по эзотерике", Id = 98},
                new ChapterVM() {Name = "Курсы по соблазнению", Id = 59},
                new ChapterVM() {Name = "Имидж и стиль", Id = 102},
                new ChapterVM() {Name = "Дети и родители", Id = 95},
                new ChapterVM() {Name = "Школа и репетиторство", Id = 104},
                new ChapterVM() {Name = "Хобби и рукоделие", Id = 99},
                new ChapterVM() {Name = "Строительство и ремонт", Id = 94},
                new ChapterVM() {Name = "Сад и огород", Id = 101},
                new ChapterVM() {Name = "Авто-мото", Id = 100},
                new ChapterVM() {Name = "Скрипты и программы", Id = 32},
                new ChapterVM() {Name = "Шаблоны и темы", Id = 82},
                new ChapterVM() {Name = "Базы и каталоги", Id = 58},
                new ChapterVM() {Name = "Покер, ставки, казино", Id = 36},
                new ChapterVM() {Name = "Спортивные события", Id = 69},
                new ChapterVM() {Name = "Форекс и инвестиции", Id = 37},
                new ChapterVM() {Name = "Доступ к платным ресурсам", Id = 31},
                new ChapterVM() {Name = "Иностранные языки", Id = 83},
                new ChapterVM() {Name = "Разные аудио и видеокурсы", Id = 28}
            };

            AuthorsChapters.IsSelectedAll = true;
            TranslationsChapters.IsSelectedAll = true;
            CommonChapters.IsSelectedAll = true;
        }

        public List<ChapterVM> GetEnabledSubchapter()
        {
            var enabledSubchapters = AuthorsChapters.Where(c => c.IsSelected).ToList();
            enabledSubchapters.AddRange(TranslationsChapters.Where(c => c.IsSelected).ToList());
            enabledSubchapters.AddRange(CommonChapters.Where(c => c.IsSelected).ToList());
            return enabledSubchapters;
        }

        #endregion
    }
}