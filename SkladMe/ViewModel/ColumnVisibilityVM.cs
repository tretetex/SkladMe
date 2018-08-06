using System;
using SkladMe.Infrastructure;

namespace SkladMe.ViewModel
{
    public class ColumnVisibilityVM : BaseVM
    {
        #region fileds

        private static string _columnVisibilityTempPath = Environment.CurrentDirectory + "\\columnsvisibility.temp";

        private bool _note = true;
        private bool _viewCount = true;
        private bool _rating = true;
        private bool _reviewCount = true;
        private bool _clubMemberRating = true;
        private bool _organizerRating = true;
        private bool _popularity = true;
        private bool _dateUpdate = true;
        private bool _dateCreate = true;
        private bool _creator = true;
        private bool _peopleForMin = true;
        private bool _reserveList = true;
        private bool _mainList = true;
        private bool _users = true;
        private bool _organizer = true;
        private bool _isRepeat = true;
        private bool _fee = true;
        private bool _price = true;
        private bool _title = true;
        private bool _subchapter = true;
        private bool _chapter = true;
        private bool _prefix = true;
        private bool _id = true;
        private bool _dateFee = true;
        private bool _feeToPrice = true;
        private bool _involvement = true;

        #endregion fileds

        #region properties

        public bool Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public bool Prefix
        {
            get => _prefix;
            set
            {
                _prefix = value;
                OnPropertyChanged();
            }
        }

        public bool Chapter
        {
            get => _chapter;
            set
            {
                _chapter = value;
                OnPropertyChanged();
            }
        }

        public bool Subchapter
        {
            get => _subchapter;
            set
            {
                _subchapter = value;
                OnPropertyChanged();
            }
        }

        public bool Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public bool Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
            }
        }

        public bool Fee
        {
            get => _fee;
            set
            {
                _fee = value;
                OnPropertyChanged();
            }
        }

        public bool FeeToPrice
        {
            get => _feeToPrice;
            set
            {
                _feeToPrice = value; 
                OnPropertyChanged();
            }
        }

        public bool IsRepeat
        {
            get => _isRepeat;
            set
            {
                _isRepeat = value;
                OnPropertyChanged();
            }
        }

        public bool Organizer
        {
            get => _organizer;
            set
            {
                _organizer = value;
                OnPropertyChanged();
            }
        }

        public bool Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        public bool MainList
        {
            get => _mainList;
            set
            {
                _mainList = value;
                OnPropertyChanged();
            }
        }

        public bool ReserveList
        {
            get => _reserveList;
            set
            {
                _reserveList = value;
                OnPropertyChanged();
            }
        }

        public bool PeopleForMin
        {
            get => _peopleForMin;
            set
            {
                _peopleForMin = value;
                OnPropertyChanged();
            }
        }

        public bool Creator
        {
            get => _creator;
            set
            {
                _creator = value;
                OnPropertyChanged();
            }
        }

        public bool DateCreate
        {
            get => _dateCreate;
            set
            {
                _dateCreate = value;
                OnPropertyChanged();
            }
        }

        public bool DateFee
        {
            get => _dateFee;
            set
            {
                _dateFee = value;
                OnPropertyChanged();
            }
        }

        public bool DateUpdate
        {
            get => _dateUpdate;
            set
            {
                _dateUpdate = value;
                OnPropertyChanged();
            }
        }

        public bool Popularity
        {
            get => _popularity;
            set
            {
                _popularity = value;
                OnPropertyChanged();
            }
        }

        public bool Involvement
        {
            get => _involvement;
            set
            {
                _involvement = value;
                OnPropertyChanged();
            }
        }

        public bool OrganizerRating
        {
            get => _organizerRating;
            set
            {
                _organizerRating = value;
                OnPropertyChanged();
            }
        }

        public bool ClubMemberRating
        {
            get => _clubMemberRating;
            set
            {
                _clubMemberRating = value;
                OnPropertyChanged();
            }
        }

        public bool ReviewCount
        {
            get => _reviewCount;
            set
            {
                _reviewCount = value;
                OnPropertyChanged();
            }
        }

        public bool Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                OnPropertyChanged();
            }
        }

        public bool ViewCount
        {
            get => _viewCount;
            set
            {
                _viewCount = value;
                OnPropertyChanged();
            }
        }

        public bool Note
        {
            get => _note;
            set
            {
                _note = value;
                OnPropertyChanged();
            }
        }

        #endregion properties

        public bool SaveTempVisibility()
        {
            try
            {
                Serialization.SerializeToXml(_columnVisibilityTempPath, this);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static ColumnVisibilityVM LoadingVisibility()
        {
            return (ColumnVisibilityVM)
                Serialization.DeserializeFromXml(_columnVisibilityTempPath, typeof(ColumnVisibilityVM));
        }
    }
}