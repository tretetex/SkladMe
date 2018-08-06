using System;
using System.Collections.Generic;
using System.Linq;
using SkDAL.Model;
using SkladMe.API.Methods;
using SkladMe.Common.Model;

namespace SkladMe.Model
{
    [Serializable]
    public class Filters
    {
        private static Product _product;
        private static Filters _filters;

        #region Properties

        public Range<int?> Price { get; set; }  
        public Range<int?> Fee { get; set; }
        public Range<double?> FeeToPrice { get; set; }
        public Range<int?> RealFee { get; set; }
        public bool? OrganizerExist { get; set; }
        public bool? FeeSoon { get; set; }
        public Range<int?> PeopleMainCount { get; set; }
        public Range<int?> PeopleReserveCount { get; set; }
        public Range<int?> PeopleCountForMinimal { get; set; }
        public Range<double?> ProductRaiting { get; set; }
        public Range<int?> ReviewCount { get; set; }
        public Range<double?> Popularity { get; set; }
        public Range<double?> OrgRaiting { get; set; }
        public Range<double?> ClubMemberRaiting { get; set; }
        public Range<double?> Involvement { get; set; }
        public Range<DateTime?> DateOfCreation { get; set; }
        public Range<DateTime?> FeeDate { get; set; }
        public bool PrefixOpen { get; set; } = true;
        public bool PrefixActive { get; set; } = true;
        public bool PrefixCompleted { get; set; } = true;
        public bool PrefixAvailable { get; set; } = true;
        public bool AllWordsContains { get; set; }
        public bool AllTagsContains { get; set; } = true;
        public string TopicStarter { get; set; }
        public string OrganizerNickname { get; set; }
        public List<string> TitleKeywordIncluded { get; set; } = new List<string>();
        public List<string> TitleKeywordExcluded { get; set; } = new List<string>();
        public List<string> TagsIncluded { get; set; } = new List<string>();
        public List<string> TagsExcluded { get; set; } = new List<string>();
        
        #endregion Properties

        #region methods

        public static bool IsGoodProduct(Product product, Filters filters, bool isPartialInfo = false)
        {
            if (product == null || filters == null)
            {
                return false;
            }
            _product = product;
            _filters = filters;

            if (!IsCreatorSatisfies()) return false;
            if (!IsPrefixSatisfies()) return false;
            if (!IsFeeSatisfies()) return false;
            if (!IsDateOfCreationSatisfies()) return false;
            if (!IsDateOfFeeSatisfies()) return false;
            if (!IsProductRatingSatisfies()) return false;
            if (!IsTitleIncludeKeywordsSatisfies()) return false;
            if (!IsTitleExcludeKeywordsSatisfies()) return false;

            if (isPartialInfo)
            {
                ClearStaticResources();
                return true;
            }

            if(!IsPopularitySatisfies()) return false;
            if(!IsClubMemberRatingSatisfies()) return false;
            if(!IsOrganizerRatingSatisfies()) return false;
            if(!IsInvolvementSatisfies()) return false;
            if (!IsFeeToPriceSatisfies()) return false;
            if (!IsPriceSatisfies()) return false;
            if (!IsRealFeeSatisfies()) return false;
            if (!IsOrganizerSatisfies()) return false;
            if (!IsPeopleCountSatisfies()) return false;
            if (!IsReviewCountSatisfies()) return false;
            if (!IsTagsSatisfied()) return false;

            ClearStaticResources();

            return true;
        }

        private static void ClearStaticResources()
        {
            _product = null;
            _filters = null;
        }

        private static bool IsCreatorSatisfies()
        {
            if (!string.IsNullOrEmpty(_filters.TopicStarter))
            {
                if (_filters.TopicStarter.Trim() != _product.Creator.Nickname)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsPrefixSatisfies()
        {
            if (_product.Prefix == null)
            {
                return false;
            }
            var productPrefix = Products.Prefixes[_product.Prefix.Title.ToLower()];
            switch (productPrefix)
            {
                case Products.PrefixId.Open:
                    if (!_filters.PrefixOpen) return false;
                    break;
                case Products.PrefixId.Active:
                    if (!_filters.PrefixActive) return false;
                    break;
                case Products.PrefixId.Available:
                    if (!_filters.PrefixAvailable) return false;
                    break;
                case Products.PrefixId.Completed:
                    if (!_filters.PrefixCompleted) return false;
                    break;
            }
            return true;
        }

        private static bool IsFeeSatisfies()
        {
            if (!Range.IsInclude(_filters.Fee, _product.Fee))
            {
                return false;
            }
            return true;
        }

        private static bool IsDateOfCreationSatisfies()
        {
            if (!Range.IsInclude(_filters.DateOfCreation, _product.DateOfCreation))
            {
                return false;
            }
            return true;
        }

        private static bool IsDateOfFeeSatisfies()
        {
            if (_filters.OrganizerExist == true)
            {
                if (_filters.FeeSoon == true)
                {
                    if (_product.DateFee == null)
                    {
                        return false;
                    }
                    if (!Range.IsInclude(_filters.FeeDate, _product.DateFee))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool IsProductRatingSatisfies()
        {
            if (!Range.IsInclude(_filters.ProductRaiting, _product.Rating))
            {
                return false;
            }
            return true;
        }

        private static bool IsTitleIncludeKeywordsSatisfies()
        {
            if (_filters.TitleKeywordIncluded.Count > 0)
            {
                if (_filters.AllWordsContains)
                {
                    if (!IsValueIncludeKeywords(_product.Title.ToLower(), _filters.TitleKeywordIncluded))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!StringContainsAny(_product.Title.ToLower(), _filters.TitleKeywordIncluded))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool StringContainsAny(string mainStr, List<string> checkList)
        {
            foreach (var word in checkList)
            {
                if (mainStr.Contains(word.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsTitleExcludeKeywordsSatisfies()
        {
            if (_filters.TitleKeywordExcluded.Count > 0)
            {
                if (!IsValueExcludeKeywords(_product.Title.ToLower(), _filters.TitleKeywordExcluded))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsValueIncludeKeywords(string value, List<string> keywords)
        {
            return !keywords.Where((t, i) => value.Contains(keywords.ElementAt(i).ToLower()) == false).Any();
        }

        private static bool IsValueExcludeKeywords(string value, List<string> keywords)
        {
            return !keywords.Where((t, i) => value.Contains(keywords.ElementAt(i).ToLower())).Any();
        }

        private static bool IsPriceSatisfies()
        {
            if (!Range.IsInclude(_filters.Price, _product.Price))
            {
                return false;
            }
            return true;
        }

        private static bool IsFeeToPriceSatisfies()
        {
            if (!_product.Fee.HasValue || !_product.Price.HasValue ) return false;
            double? productFeeToPrice = 0;
            if (_product.Price != 0)
            {
                productFeeToPrice = (double) _product.Fee / _product.Price;
            }

            if (!Range.IsInclude(_filters.FeeToPrice, productFeeToPrice))
            {
                return false;
            }
            return true;
        }

        private static bool IsInvolvementSatisfies()
        {
            double? involvement = 0;

            if (_product.ViewCount != 0)
            {
                involvement = (double)(_product.MembersAsMainCount + _product.MembersAsReserveCount) / _product.ViewCount * 100;
            }
            if (!Range.IsInclude(_filters.Involvement, involvement))
            {
                return false;
            }
            return true;
        }
            
        private static bool IsPopularitySatisfies()
        {
            if (!Range.IsInclude(_filters.Popularity, _product.Popularity))
            {
                return false;
            }
            return true;
        }

        private static bool IsOrganizerRatingSatisfies()
        {
            if (!Range.IsInclude(_filters.OrgRaiting, _product.OrganizerRating))
            {
                return false;
            }
            return true;
        }

        private static bool IsClubMemberRatingSatisfies()
        {
            if (!Range.IsInclude(_filters.ClubMemberRaiting, _product.ClubMemberRating))
            {
                return false;
            }
            return true;
        }

        private static bool IsRealFeeSatisfies()
        {
            if (!Range.IsInclude(_filters.RealFee, _product.RealFee))
            {
                return false;
            }
            return true;
        }

        private static bool IsOrganizerSatisfies()
        {
            if (_filters.OrganizerExist != null &&
                _filters.OrganizerExist == string.IsNullOrEmpty(_product.Organizer?.Nickname))
            {
                return false;
            }

            if (_filters.OrganizerExist == true && !string.IsNullOrEmpty(_filters.OrganizerNickname))
                //Проверка ника орга
            {
                if (_filters.OrganizerNickname.Trim() != _product.Organizer?.Nickname)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsPeopleCountSatisfies()
        {
            if (!Range.IsInclude(_filters.PeopleMainCount, _product.MembersAsMainCount))
            {
                return false;
            }
            if (!Range.IsInclude(_filters.PeopleReserveCount, _product.MembersAsReserveCount))
            {
                return false;
            }
            return true;
        }

        private static bool IsReviewCountSatisfies()
        {
            if (!Range.IsInclude(_filters.ReviewCount, _product.ReviewCount))
            {
                return false;
            }
            return true;
        }

        private static bool IsListsIntersect(List<string> maintList, List<string> checkList)
        {
            if (maintList == null || checkList == null)
            {
                return false;
            }
            var mainLower = maintList.Select(s => s.ToLower()).ToList();
            var checkLower = checkList.Select(s => s.ToLower()).ToList();
            return mainLower.Intersect(checkLower).Any();
        }

        private static bool IsTagsSatisfied()
        {
            var tagsList = _product.Tags?.Select(t => t.Title.ToLower()).ToList();

            if (!IsTagsIncludedSatisfies(tagsList)) return false;
            if (!IsTagsExcludedSatisfies(tagsList)) return false;
            return true;
        }

        private static bool IsTagsExcludedSatisfies(List<string> tagsList)
        {
            if (_filters.TagsExcluded.Count > 0)
            {
                if (tagsList is null)
                {
                    return false;
                }

                if (IsListsIntersect(tagsList, _filters.TagsExcluded))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsTagsIncludedSatisfies(List<string> tagsList)
        {
            if (_filters.TagsIncluded.Count > 0)
            {
                if (tagsList is null)
                {
                    return false;
                }

                if (!_filters.AllTagsContains)
                {
                    if (!IsListsIntersect(tagsList, _filters.TagsIncluded))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!IsListsIntersectFull(tagsList, _filters.TagsIncluded))
                    {
                        return false;
                    }
                }
                
            }
            return true;
        }

        private static bool IsListsIntersectFull(List<string> main, List<string> check)
        {
            foreach (string s in check)
            {
                if (!main.Contains(s))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion methods
    }
}