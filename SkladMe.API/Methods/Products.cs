using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using SkDAL.Model;

namespace SkladMe.API.Methods
{
    public class Products
    {
        public enum PrefixId
        {
            Open = 1,
            Active = 2,
            Available = 3,
            Completed = 4
        }

        public static Dictionary<string, PrefixId> Prefixes { get; }
        public static Dictionary<string, string> XPaths { get; set; }

        const int MinFee = 40;

        #region private fields

        private SkladchikApiClient _skladchikApiClient;

        private static readonly Dictionary<string, int> _monthWords = new Dictionary<string, int>()
        {
            ["янв"] = 1,
            ["фев"] = 2,
            ["мар"] = 3,
            ["апр"] = 4,
            ["май"] = 5,
            ["июн"] = 6,
            ["июл"] = 7,
            ["авг"] = 8,
            ["сен"] = 9,
            ["окт"] = 10,
            ["ноя"] = 11,
            ["дек"] = 12,
        };


        private static readonly Dictionary<string, int> _chapters = new Dictionary<string, int>()
        {
            ["Авторские складчины"] = 105,
            ["Переводы"] = 106,
            ["Складчины"] = 15
        };

        private static readonly Dictionary<string, int> _subchaptersTranslater = new Dictionary<string, int>()
        {
            ["Программирование"] = 112,
            ["Бизнес и свое дело"] = 113,
            ["Дизайн и креатив"] = 114,
            ["Здоровье и быт"] = 115,
            ["Психология и отношения"] = 116,
            ["Хобби и увлечения"] = 117
        };

        private static readonly Dictionary<string, int> _subchaptersAuthor = new Dictionary<string, int>()
        {
            ["Бизнес и свое дело"] = 107,
            ["Дизайн и креатив"] = 108,
            ["Здоровье и быт"] = 109,
            ["Психология и отношения"] = 110,
            ["Хобби и увлечения"] = 111
        };

        private static readonly Dictionary<string, int> _subchaptersOther = new Dictionary<string, int>()
        {
            ["Курсы по программированию"] = 21,
            ["Курсы по администрированию"] = 16,
            ["Курсы по бизнесу"] = 24,
            ["Бухгалтерия и финансы"] = 103,
            ["Курсы по SEO и SMM"] = 26,
            ["Курсы по дизайну"] = 19,
            ["Курсы по фото и видео"] = 78,
            ["Курсы по музыке"] = 60,
            ["Электронные книги"] = 30,
            ["Курсы по здоровью"] = 71,
            ["Курсы по самообороне"] = 118,
            ["Отдых и путешествия"] = 97,
            ["Курсы по психологии"] = 38,
            ["Курсы по эзотерике"] = 98,
            ["Курсы по соблазнению"] = 59,
            ["Имидж и стиль"] = 102,
            ["Дети и родители"] = 95,
            ["Школа и репетиторство"] = 104,
            ["Хобби и рукоделие"] = 99,
            ["Строительство и ремонт"] = 94,
            ["Сад и огород"] = 101,
            ["Авто-мото"] = 100,
            ["Скрипты и программы"] = 32,
            ["Шаблоны и темы"] = 82,
            ["Базы и каталоги"] = 58,
            ["Покер, ставки, казино"] = 36,
            ["Спортивные события"] = 69,
            ["Форекс и инвестиции"] = 37,
            ["Доступ к платным ресурсам"] = 31,
            ["Иностранные языки"] = 83,
            ["Разные аудио и видеокурсы"] = 28
        };

        #endregion

        static Products()
        {
            Prefixes = new Dictionary<string, PrefixId>()
            {
                {"открыто", PrefixId.Open},
                {"активно", PrefixId.Active},
                {"доступно", PrefixId.Available},
                {"закрыто", PrefixId.Completed}
            };
        }

        public Products(SkladchikApiClient skladchikApiClient)
        {
            _skladchikApiClient = skladchikApiClient;
        }

        #region private methods

        private static string GetNodeValue(HtmlDocument doc, string key, string attr = "")
        {
            var xpath = XPaths[key];
            var node = doc.DocumentNode.SelectSingleNode(xpath);
            var value = node?.GetAttributeValue(attr, node.InnerText);
            return value;
        }

        private static void SetUserInfo(HtmlNode node, ref User user)
        {
            user.Nickname = node.InnerText;
            var idString = node.GetAttributeValue("href", "");
            idString = Regex.Match(idString, @"(?<=\.)\d*(?=/)").Value;
            int.TryParse(idString, out int id);
            user.Id = id;

            int groupId;
            var isGroupIdExist = int.TryParse(node.GetAttributeValue("data-usergroupid", null), out groupId);

            if (isGroupIdExist)
            {
                user.UserGroupId = groupId;
            }
        }

        private static List<User> GetUsers(HtmlDocument doc, string key)
        {
            var nodes = new List<User>();
            try
            {
                string xpath = XPaths[key];
                foreach (var node in doc.DocumentNode.SelectNodes(xpath))
                {
                    var user = new User();
                    SetUserInfo(node, ref user);
                    nodes.Add(user);
                }
                return nodes;
            }
            catch
            {
                return null;
            }
        }

        public static DateTime ConvertDate(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return new DateTime();
            }
            string strDay = Regex.Match(date, @"\d{1}.*?(?=\ )").Value;
            string strMonth = Regex.Match(date, @"[а-я]{3}").Value;
            string strYear = Regex.Match(date, @"\d{4}").Value;
            string strHour = Regex.Match(date, @"\d+(?=:\d+$)").Value;
            string strMinute = Regex.Match(date, @"(?<=:)\d+$").Value;

            int.TryParse(strDay, out int day);

            int month = 1;
            if (!string.IsNullOrWhiteSpace(strMonth))
            {
                month = _monthWords[strMonth];
            }

            int.TryParse(strYear, out int year);
            int.TryParse(strHour, out int hour);
            int.TryParse(strMinute, out int minute);

            return new DateTime(year, month, day, hour, minute, 0);
        }

        #endregion

        public async Task<Product> GetByUrlAsync(string url)
        {
            var productId = ParseId(url);
            var product = await GetByIdAsync(productId).ConfigureAwait(false);

            return product;
        }   

        public static int ParseId(string source)
        {   
            string id =  Regex.Match(source, @"((?<=\.)\d{4,})|(^\d{4,}$)").Value;
            return int.Parse(id);
        }

        /// <summary>
        /// Получение складчины.
        /// </summary>
        /// <param name="id">Id складчины.</param>
        /// <param name="dateUpdate">Дата последнего события.</param>
        /// <param name="viewCount">Количество просмотров.</param>
        /// <param name="important">Закреплена ли в разделе.</param>
        public async Task<Product> GetByIdAsync(int id, DateTime dateUpdate = default(DateTime), int viewCount = 0,
            bool important = false)
        {
            var product = new Product
            {
                Id = id,
                ViewCount = viewCount,
                Important = important,
                DateUpdate = dateUpdate
            };
            var source = await _skladchikApiClient.CallAsync($"{SkladchikApiClient.Domain}threads/{product.Id}").ConfigureAwait(false);
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            SetProductFields(source, product);

            return product;
        }

        private static void SetProductFields(string source, Product product)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(source);

            SetTitle(doc, product);
            SetChapters(doc, product);
            SetPrefix(doc, product);
            SetIsRepeat(product);
            SetDateCreate(doc, product);
            SetTags(doc, product);
            SetRaiting(doc, product);
            SetReviewCount(doc, product);
            SetPrice(doc, product);
            SetFee(doc, product);
            SetDateFee(doc, product);
            SetOrganizer(doc, product);
            SetMembersAsMain(doc, product);
            SetMembersAsReserve(doc, product);
            SetUsersTotalCount(product);
            SetUsersAsAuthor(doc, product);
            SetCreator(doc, product);
            SetPopularity(product);
            SetStatusRatings(product);
            SetRealFee(product);
            SetPeopleForMinFee(product);
        }

        private static void SetPopularity(Product p)
        {
            var oldDate = p.DateOfCreation;
            var newDate = DateTime.Now;

            // Difference in days, hours, and minutes.
            var ts = newDate - oldDate;

            // Difference in days.
            int differenceInDays = ts.Days;
            if (differenceInDays == 0)
            {
                differenceInDays = 1;
            }
            var popularity = Convert.ToDouble(p.UsersTotalCount) / differenceInDays;
            p.Popularity = Math.Round(popularity, 2);
        }

        private static void SetStatusRatings(Product p)
        {
            double A = p.Popularity; //A - популярность складчины
            double B = Convert.ToDouble(p.Price); //B - цена товара

            double C = 0;
            if (p.MembersAsMain != null)
            {
                C = p.MembersAsMain.Count;
            }
            ; //C - количество участников в основном списке

            double D = 0;
            if (p.MembersAsReserve != null)
            {
                D = p.MembersAsReserve.Count;
            } //D - количество участников в резервном списке

            if (C == 0)
            {
                C = 1;
            }
            var clubMemberRating = A * (100000 - B) * ((100000 - B) / ((540 + B * 102 / 100 + C * 102 / 10) / (C))) *
                                   ((C) / ((5800 + B * 102 / 10) / 298)) / 100000000;
            var organizerRating = A * (100000 - B) * ((100000 - B) / ((540 + B * 102 / 100 + C * 102 / 10) / (C))) *
                                  ((C + D * 10) / ((5800 + B * 102 / 10) / 298)) / 100000000;

            p.ClubMemberRating = Math.Round(clubMemberRating, 3);
            p.OrganizerRating = Math.Round(organizerRating, 3);
        }

        private static void SetRealFee(Product p)
        {
            if (p.Organizer == null && p.ChapterId == 15)
            {
                int A = Convert.ToInt32(p.Price); //A = цена товара
                int B = 0;
                if (p.MembersAsMain != null)
                {
                    B = p.MembersAsMain.Count;
                } //B = количество участников
                if (B == 0)
                {
                    B = 1;
                }
                int C = (540 + A * 102 / 100 + B * 102 / 10) / B;

                p.RealFee = C < MinFee ? MinFee : C;
                if (p.RealFee < p.Fee)
                {
                    p.RealFee = Convert.ToInt32(p.Fee);
                }
            }
            else
            {
                p.RealFee = Convert.ToInt32(p.Fee);
            }
        }

        private static void SetPeopleForMinFee(Product p)
        {
            int A = Convert.ToInt32(p.Price); //A = цена товара
            int peopleForMin = (5800 + A * 102 / 10) / 298;
            if (p.MembersAsMain != null)
            {
                if (peopleForMin <= p.MembersAsMain.Count)
                {
                    p.RealFee = 40;
                    p.PeopleForMin = 0;
                }
                else
                {
                    p.PeopleForMin = Math.Abs(p.MembersAsMain.Count - peopleForMin);
                }
            }
            else
            {
                p.PeopleForMin = peopleForMin;
            }
        }

        private static void SetChapters(HtmlDocument doc, Product p)
        {
            var chapterName = GetNodeValue(doc, "Chapter");
            p.ChapterId = _chapters[chapterName];
            p.Chapter = new Chapter() {Title = chapterName, Id = Convert.ToInt32(p.ChapterId)};

            var subchapterName = GetNodeValue(doc, "Subchapter");

            switch (p.ChapterId)
            {
                case 105:
                    p.SubchapterId = _subchaptersAuthor[subchapterName];
                    p.Subсhapter = new Subсhapter() {Id = p.SubchapterId, Title = subchapterName};
                    break;
                case 106:
                    p.SubchapterId = _subchaptersTranslater[subchapterName];
                    p.Subсhapter = new Subсhapter() {Id = p.SubchapterId, Title = subchapterName};
                    break;
                default:
                    p.SubchapterId = _subchaptersOther[subchapterName];
                    p.Subсhapter = new Subсhapter() {Id = p.SubchapterId, Title = subchapterName};
                    break;
            }
        }

        private static void SetPrefix(HtmlDocument doc, Product p)
        {
            p.Prefix = new Prefix();
            string prefix = GetNodeValue(doc, "Prefix");
            if (!string.IsNullOrEmpty(prefix))
            {
                p.PrefixId = (int) Prefixes[prefix.ToLower()];
                p.Prefix.Title = prefix;
                p.Prefix.Id = p.PrefixId;
            }
        }

        private static void SetIsRepeat(Product p)
        {
            p.IsRepeat = p.Title.ToUpper().Contains("ПОВТОР");
        }

        private static void SetTitle(HtmlDocument doc, Product p)
            => p.Title = HttpUtility.HtmlDecode(GetNodeValue(doc, "Title"));

        private static void SetDateCreate(HtmlDocument doc, Product p)
        {
            string date = GetNodeValue(doc, "DateCreate", "title") ?? GetNodeValue(doc, "DateCreateNew");
            p.DateOfCreation = ConvertDate(date);
        }

        private static void SetTags(HtmlDocument doc, Product p)
        {
            string xpath = XPaths["Tags"];
            var nodes = doc.DocumentNode.SelectNodes(xpath);
            if (nodes == null)
            {
                return;
            }

            p.Tags = new List<Tag>();

            foreach (var node in nodes)
            {
                p.Tags.Add(new Tag() {Title = node.InnerText});
            }
        }

        private static void SetRaiting(HtmlDocument doc, Product p)
        {
            double a;
            if (double.TryParse(GetNodeValue(doc, "Raiting", "title"), out a))
            {
                p.Rating = a;
            }
            else
            {
                p.Rating = null;
            }
        }

        private static void SetReviewCount(HtmlDocument doc, Product p)
        {
            string s = GetNodeValue(doc, "ReviewCount");
            if (string.IsNullOrEmpty(s))
            {
                return;
            }
            s = Regex.Match(s, @"\d+").Value;
            int.TryParse(s, out int rc);
            p.ReviewCount = rc;
        }

        private static void SetPrice(HtmlDocument doc, Product p)
        {
            string s = GetNodeValue(doc, "Price");
            if (String.IsNullOrEmpty(s))
            {
                return;
            }

            s = Regex.Match(s, @"\d+").Value;
            int.TryParse(s, out int price);
            p.Price = price;
        }

        private static void SetFee(HtmlDocument doc, Product p)
        {
            string s = GetNodeValue(doc, "Fee");
            if (string.IsNullOrEmpty(s))
            {
                return;
            }
            s = Regex.Match(s, @"\d+").Value;
            int.TryParse(s, out int fee);
            p.Fee = fee;
        }

        private static void SetDateFee(HtmlDocument doc, Product p)
        {
            string s = GetNodeValue(doc, "DateFee");
            if (string.IsNullOrEmpty(s))
            {
                return;
            }

            s = Regex.Match(s.Trim(), @"\d+\ [a-zA-Zа-яА-Я]+\ \d+$").Value;
            p.DateFee = ConvertDate(s);
        }

        private static void SetOrganizer(HtmlDocument doc, Product p)
        {
            p.Organizer = new User {Nickname = GetNodeValue(doc, "Organizer")};

            if (p.Organizer.Nickname == null)
            {
                p.Organizer = null;
                return;
            }

            var idString = Regex.Match(GetNodeValue(doc, "Organizer", "href"), @"(?<=\.)\d*(?=/)").Value;
            int.TryParse(idString, out int id);
            p.Organizer.Id = id;
        }

        private static void SetMembersAsMain(HtmlDocument doc, Product p)
        {
            p.MembersAsMain = GetUsers(doc, "MembersAsMain");
            p.MembersAsMainCount = p.MembersAsMain?.Count ?? 0;
        }

        private static void SetMembersAsReserve(HtmlDocument doc, Product p)
        {
            p.MembersAsReserve = GetUsers(doc, "MembersAsReserve");
            p.MembersAsReserveCount = p.MembersAsReserve?.Count ?? 0;
        }

        private static void SetUsersTotalCount(Product p)
        {
            if (p.MembersAsMain != null)
            {
                p.UsersTotalCount += p.MembersAsMain.Count;
            }
            if (p.MembersAsReserve != null)
            {
                p.UsersTotalCount += p.MembersAsReserve.Count;
            }
        }

        private static void SetUsersAsAuthor(HtmlDocument doc, Product p)
        {
            if (p.ChapterId != 15)
            {
                p.UsersAsAuthor = GetUsers(doc, "UsersAsAuthor");
            }
            p.UsersAsAuthorCount = p.UsersAsAuthor?.Count ?? 0;
        }

        private static void SetCreator(HtmlDocument doc, Product p)
        {
            p.Creator = new User();
            p.Creator.Nickname = GetNodeValue(doc, "Creator");
            string idString = Regex.Match(GetNodeValue(doc, "Creator", "href"), @"(?<=\.)\d*(?=/)").Value;
            int.TryParse(idString, out int id);
            p.Creator.Id = id;
        }

        public static string GetProductLink(Product p)
        {
            return SkladchikApiClient.ThreadsUrl + p.Id;
        }
    }
}