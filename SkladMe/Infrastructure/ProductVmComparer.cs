using System.Collections.Generic;

namespace SkladMe.ViewModel
{
    class ProductVmComparer : IEqualityComparer<ProductVM>
    {
        public bool Equals(ProductVM x, ProductVM y)
        {
            return x.Model.Id == y.Model.Id;
        }

        public int GetHashCode(ProductVM obj)
        {
            return obj.Model.Id;
        }
    }
}
