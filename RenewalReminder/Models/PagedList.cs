using System;
using System.Collections.Generic;
using System.Linq;

namespace KvsProject.Models
{
    public class PagedList<T> : IList<T>, IEnumerable<T>, ICollection<T>
    {
        protected readonly List<T> InnerList = new List<T>();

        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }

        public PagedList() : base()
        {
        }

        public PagedList(IEnumerable<T> data, int totalCount)
        {
            InnerList = data.ToList();
            TotalCount = totalCount;
            if (PageSize > 0)
            {
                TotalPage = (int)Math.Ceiling(1M * TotalCount / PageSize);
            }
        }
        public PagedList(IEnumerable<T> data, int totalCount, int page, int pageSize)
        {
            InnerList = data.ToList();
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
            if (PageSize > 0)
            {
                TotalPage = (int)Math.Ceiling(1M * TotalCount / PageSize);
            }
        }

        public T this[int index] { get => InnerList[index]; set => InnerList[index] = value; }

        public int Count => InnerList.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            InnerList.Add(item);
        }

        public void Clear()
        {
            InnerList.Clear();
        }

        public bool Contains(T item)
        {
            return InnerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            InnerList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return InnerList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            InnerList.Insert(index, item);
        }

        public bool Remove(T item)
        {
            return InnerList.Remove(item);
        }

        public void RemoveAt(int index)
        {
            InnerList.RemoveAt(index);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }
    }
}
